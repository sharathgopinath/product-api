namespace Product.Api.Authentication;

public class BasicAuthenticationOptions
{
    public const string SectionName = "BasicAuthentication";
    public const string SchemeName = "Basic";

    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}
