using FAPIServer.Extensions;
using FAPIServer.Storage.Stores;
using FAPIServer.Storage.ValueObjects;
using FAPIServer.Validation.Contexts;
using FAPIServer.Validation.Results;
using Json.Pointer;
using Json.Schema;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FAPIServer.Validation.Default;

public class ResourceValidator : IResourceValidator
{
    private readonly IAuthorizationDetailSchemaStore _authorizationDetailSchemaStore;
    private readonly IClaimsStore _claimsStore;

    public ResourceValidator(IAuthorizationDetailSchemaStore authorizationDetailSchemaStore, IClaimsStore claimsStore)
    {
        _authorizationDetailSchemaStore = authorizationDetailSchemaStore;
        _claimsStore = claimsStore;
    }

    private readonly static EvaluationOptions EvaluationOptions = new EvaluationOptions
    {
        OutputFormat = OutputFormat.List
    };

    public async Task<ResourceValidationResult> ValidateAsync(ResourceValidationContext context, CancellationToken cancellationToken = default)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var authorizationDetailsValidationResult = await ValidateAuthorizationDetails(context, cancellationToken);
        if (!authorizationDetailsValidationResult.IsValid)
            return authorizationDetailsValidationResult;

        var claimsValidationResult = await ValidateClaims(context, cancellationToken);
        if (!claimsValidationResult.IsValid)
            return claimsValidationResult;

        return new(authorizationDetailsValidationResult.AuthorizationDetails,
            authorizationDetailsValidationResult.InvolvedSchemas,
            claimsValidationResult.Claims);
    }

    private async Task<ResourceValidationResult> ValidateAuthorizationDetails(ResourceValidationContext context, CancellationToken cancellationToken)
    {
        IEnumerable<AuthorizationDetail> authorizationDetails = Array.Empty<AuthorizationDetail>();
        if (!context.RequestedAuthorizationDetails.IsNullOrEmpty())
        {
            try
            {
                authorizationDetails = AuthorizationDetailExtensions.ReadFromJson(context.RequestedAuthorizationDetails, out int mismatchesCount);
                if (mismatchesCount > 0)
                    return InvalidAuthorizationDetails(
                        "The 'authorization_details' must be a single JSON object or array of objects. " +
                        "Every object must have not null 'type', 'actions', 'locations' property. The 'actions' must be object and have properties with objects as values");
            }
            catch
            {
                return InvalidAuthorizationDetails("The 'authorization_details' is not well-formed JSON object or array");
            }

            if (authorizationDetails.DistinctBy(p => p.Type).Count() != authorizationDetails.Count())
                return InvalidAuthorizationDetails("The 'authorization_details' array cannot contain multiple entries of the same type");
        }

        if (!authorizationDetails.Any())
            return new(null, null, null);

        if (authorizationDetails.Any(p => p.Type == Constants.BuiltInAuthorizationDetails.OpenId.Type) && context.GrantType == Constants.GrantTypes.ClientCredentials)
            return new(Error.InvalidAuthorizationDetails, "The 'openid' type cannot be requested using 'client_credentials' grant");

        // Get all to avoid calling it in loop
        var schemas = await _authorizationDetailSchemaStore.FindEnabledByTypesAsync(authorizationDetails.Select(p => p.Type), cancellationToken);

        foreach (var authorizationDetail in authorizationDetails)
        {
            var schema = schemas.SingleOrDefault(p => p.Type == authorizationDetail.Type && p.Enabled);
            if (schema == null)
                return InvalidAuthorizationDetails($"The '{authorizationDetail.Type}' is not supported authorization detail type");

            if (!context.Client.AllowedSchemas.Any(p => p.SchemaType == schema.Type))
                return InvalidAuthorizationDetails($"The '{schema.Type}' is not allowed for this client");

            if (!authorizationDetail.Locations.All(p =>
                    Uri.IsWellFormedUriString(p, UriKind.Absolute) && schema.SupportedLocations.Contains(new Uri(p))))
                return InvalidAuthorizationDetails($"The requested locations are not valid for '{authorizationDetail.Type}' authorization detail type");

            if (schema.ExtensionsSchema != null)
            {
                var schemaValidationResult = schema.ExtensionsSchema.Evaluate(JsonNode.Parse(JsonSerializer.Serialize(authorizationDetail.Extensions)),
                    EvaluationOptions);
                if (!schemaValidationResult.IsValid)
                {
                    var errors = GetSchemaValidationErrors(schemaValidationResult);
                    if (ShouldFailWhenSchemaValidationFails(context, authorizationDetail, errors))
                        return InvalidAuthorizationDetails(CreateFailureMessage(errors));
                }
            }
            else if (authorizationDetail.Extensions is not null && authorizationDetail.Extensions.Any())
                return InvalidAuthorizationDetails($"The '{authorizationDetail.Type}' authorization detail does not allow extension fields");

            foreach (var action in authorizationDetail.Actions)
            {
                var actionSchema = schema.SupportedActions.SingleOrDefault(p => p.Name == action.Key);
                if (actionSchema == null)
                    return InvalidAuthorizationDetails($"The '{schema.Type}' does not support '{action.Key}' action");

                if (!context.Client.AllowedSchemas.Where(p => p.SchemaType == schema.Type).Any(p => p.AllowedActions.Contains(action.Key)))
                    return InvalidAuthorizationDetails($"This client is not allowed to use '{action.Key}' action in '{schema.Type}' authorization detail type");

                JsonSchema? jsonSchema = actionSchema.UseDefaultSchema ? schema.DefaultActionSchema : actionSchema.ActionSchema;
                if (jsonSchema != null)
                {
                    var schemaValidationResult = jsonSchema.Evaluate(JsonNode.Parse(JsonSerializer.Serialize(action.Value)), EvaluationOptions);
                    if (!schemaValidationResult.IsValid)
                    {
                        var errors = GetSchemaValidationErrors(schemaValidationResult);
                        if (ShouldFailWhenSchemaValidationFails(context, authorizationDetail, errors, action.Key))
                        {
                            var failureMessage = $"The validation error occurred in '{schema.Type}' in '{action.Key}' action. {CreateFailureMessage(errors)}";
                            return InvalidAuthorizationDetails(failureMessage);
                        }
                    }
                }
                else if (action.Value is JsonElement jsonElm && jsonElm.EnumerateObject().Any())
                    return InvalidAuthorizationDetails($"The '{action.Key}' action in '{schema.Type}' authorization detail type must be empty JSON object");
            }
        }

        return new(authorizationDetails, schemas);
    }

    private async Task<ResourceValidationResult> ValidateClaims(ResourceValidationContext context, CancellationToken cancellationToken)
    {
        var requestedClaims = context.RequestedClaims?.FromSpaceDelimitedString();
        if (context.GrantManagementAction != Constants.GrantManagementActions.Merge
            && (context.GrantType == Constants.GrantTypes.AuthorizationCode || context.GrantType == Constants.GrantTypes.Ciba)
            && (requestedClaims is null || !requestedClaims.Contains(Constants.BuiltInClaims.Subject)))
            return new(Error.InvalidClaims, $"The '{Constants.BuiltInClaims.Subject}' must be requested");

        if (requestedClaims is not null)
        {
            var claims = await _claimsStore.FindEnabledByTypesAsync(requestedClaims, cancellationToken);
            if (requestedClaims.Count() != claims.Count() || claims.Any(p => !p.Enabled))
                return new(Error.InvalidClaims, "The requested claims are not supported");

            if (!requestedClaims.All(context.Client.AllowedClaims.Contains))
                return new(Error.InvalidClaims, "The requested claims are not allowed for this client");
        }

        return new(requestedClaims);
    }

    private static IEnumerable<SchemaValidationError> GetSchemaValidationErrors(EvaluationResults results)
    {
        return results.Details.Where(p => !p.IsValid && p.HasErrors).Select(p => new SchemaValidationError
        {
            Path = p.InstanceLocation.ToString(JsonPointerStyle.UriEncoded),
            ErrorMessage = p.Errors!.Values.First(),
            Types = p.Errors!.Keys
        });
    }

    private static bool ShouldFailWhenSchemaValidationFails(ResourceValidationContext context, AuthorizationDetail current, IEnumerable<SchemaValidationError> errors,
        string? action = null)
    {
        // If merge, it means that this authorization details will merge another. So required by schema fields don't need
        // and even shouldn't be present to avoid unwanted value overrides
        // So when only errors are about that required fields are not present, we can say it's valid when merging
        // But there is exception. In case when action is merge and action or whole authorization detail type is not already granted then we require all properties
        // It prevents attack where client creates empty authorization detail and then merge it with incomplete authorization detail
        var grantedAuthorizationDetail = context.RequestedGrant?.AuthorizationDetails.SingleOrDefault(p => p.Type == current.Type);
        return grantedAuthorizationDetail is null
            || (!action.IsNullOrEmpty() && !grantedAuthorizationDetail.Actions.ContainsKey(action))
            || context.GrantManagementAction != Constants.GrantManagementActions.Merge
            || !errors.SelectMany(p => p.Types).All(p => p == "required");
    }

    private static string CreateFailureMessage(IEnumerable<SchemaValidationError> errors)
    {
        // Error desc, because it must be string, contains all schema validation errors separated by dot
        return string.Join(". ", errors.Select(p => $"{p.ErrorMessage} at {p.Path}"));
    }

    private static ResourceValidationResult InvalidAuthorizationDetails(string? failureMessage = null)
        => new(Error.InvalidAuthorizationDetails, failureMessage);

    private sealed class SchemaValidationError
    {
        public string Path { get; set; }
        public string ErrorMessage { get; set; }
        public IEnumerable<string> Types { get; set; }
    }
}
