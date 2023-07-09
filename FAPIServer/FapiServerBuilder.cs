using FAPIServer.InMemory;
using FAPIServer.Services;
using FAPIServer.Storage.Models;
using FAPIServer.Storage.Stores;
using FAPIServer.Storage.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace FAPIServer;

public class FapiServerBuilder
{
    private readonly IServiceCollection _services;
    public IServiceCollection Services => _services;

    public FapiServerBuilder(IServiceCollection services)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
    }

    public FapiServerBuilder UseInMemorySecretCredentialsStore(Ed25519KeyPair? ed25519KeyPair = null)
    {
        Ed25519KeyPair keyPair;
        if (ed25519KeyPair is not null)
        {
            keyPair = ed25519KeyPair;
        }
        else
        {
            Ed25519KeyPairGenerator generator = new Ed25519KeyPairGenerator();
            generator.Init(new Ed25519KeyGenerationParameters(new SecureRandom()));

            AsymmetricCipherKeyPair asymmetricKeyPair = generator.GenerateKeyPair();

            keyPair = new Ed25519KeyPair(
                new Ed25519Key(((Ed25519PrivateKeyParameters)asymmetricKeyPair.Private).GetEncoded()),
                new Ed25519Key(((Ed25519PublicKeyParameters)asymmetricKeyPair.Public).GetEncoded()));
        }

        _services.TryAddSingleton<ISecretCredentialsStore>(new InMemorySecretCredentialsStore(keyPair));

        return this;
    }

    public FapiServerBuilder AddUserService<TStore>()
        where TStore : class, IUserService
    {
        _services.TryAddScoped<IUserService, TStore>();
        return this;
    }

    public FapiServerBuilder AddCibaUserNotificationService<TService>()
        where TService : class, ICibaUserNotificationService
    {
        _services.TryAddScoped<ICibaUserNotificationService, TService>();
        return this;
    }
}
