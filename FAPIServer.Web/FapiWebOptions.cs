namespace FAPIServer.Web;

public class FapiWebOptions
{
    public string ConsentPath { get; set; } = "/consent";
    public string ReturnUrlParamName { get; set; } = "returnUrl";
}
