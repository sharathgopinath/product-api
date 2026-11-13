using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Product.Api.Authentication;

/// <summary>
/// Authenticates requests using HTTP Basic credentials checked against <see cref="BasicAuthenticationOptions"/> in fixed time;
/// challenges return a 401 ProblemDetails.
/// </summary>
/// <param name="schemeOptions">Options for the authentication scheme.</param>
/// <param name="loggerFactory">Factory for the handler's logger.</param>
/// <param name="encoder">URL encoder used by the base handler.</param>
/// <param name="credentials">Expected username and password.</param>
public class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> schemeOptions,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder,
    IOptions<BasicAuthenticationOptions> credentials)
    : AuthenticationHandler<AuthenticationSchemeOptions>(schemeOptions, loggerFactory, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? header = Request.Headers.Authorization;
        if (string.IsNullOrEmpty(header) || !AuthenticationHeaderValue.TryParse(header, out var value)
            || !string.Equals(value.Scheme, BasicAuthenticationOptions.SchemeName, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        string decoded;
        try
        {
            decoded = Encoding.UTF8.GetString(Convert.FromBase64String(value.Parameter ?? ""));
        }
        catch (FormatException)
        {
            Logger.LogWarning("Invalid Basic Authorization header for {Path}", Request.Path);
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header"));
        }

        var separator = decoded.IndexOf(':');
        if (separator < 0)
        {
            Logger.LogWarning("Invalid Basic Authorization header for {Path}", Request.Path);
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header"));
        }

        var username = decoded[..separator];
        var password = decoded[(separator + 1)..];
        var expected = credentials.Value;

        // Evaluate both comparisons so timing doesn't reveal which part was wrong
        var usernameMatches = FixedTimeEquals(username, expected.Username);
        var passwordMatches = FixedTimeEquals(password, expected.Password);
        if (!usernameMatches || !passwordMatches || expected.Username.Length == 0)
        {
            Logger.LogWarning("Invalid credentials for {Path}", Request.Path);
            return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));
        }

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, username)], Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers.WWWAuthenticate = "Basic realm=\"Product API\", charset=\"UTF-8\"";
        await Results.Problem(
                detail: "Valid Basic authentication credentials are required.",
                statusCode: StatusCodes.Status401Unauthorized,
                extensions: new Dictionary<string, object?> { ["traceId"] = Context.TraceIdentifier })
            .ExecuteAsync(Context);
    }

    private static bool FixedTimeEquals(string actual, string expected) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(actual), Encoding.UTF8.GetBytes(expected));
}
