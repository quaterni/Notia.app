using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Np.UsersService.Core.Authentication.Abstractions;
using Np.UsersService.Core.Authentication.Keycloak.Options;
using Np.UsersService.Core.Authentication.Options;

namespace Np.UsersService.Core.Authentication.Keycloak;

public static class DependencyInjection
{
    public static void AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var configurationOptions = new AuthenticationOptions();
                configuration.GetSection("Authentication").Bind(configurationOptions);
                options.Audience = configurationOptions.Audience;
                options.MetadataAddress = configurationOptions.MetadataAddress;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = configurationOptions.ValidIssuer
                };
            });

        services.Configure<IdentityClientOptions>(configuration.GetSection("Keycloak:IdentityClient"));

        services.AddTransient<KeycloakIdentityDelegatingHandler>();

        services.AddHttpClient<IIdentityService, KeycloakIdentityService>()
            .AddHttpMessageHandler<KeycloakIdentityDelegatingHandler>();
    }
}
