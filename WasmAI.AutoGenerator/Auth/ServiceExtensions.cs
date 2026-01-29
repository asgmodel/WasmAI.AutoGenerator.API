using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace WasmAI.AutoGenerator.Auth;

public static class ServiceExtensions
{
    public static IServiceCollection AddAutoAuthWasm(this IServiceCollection services)
    {
        var options = AutoAuthConfig.Options;
        var apiOptions = options.ApiOptions;
        var apiSalla = options.ApiSalla;
        var apiOAuth = options.ApiOAuth;
        var jwtOptions = options.JwtSettings;

        Console.WriteLine($"Initializing AutoAuthWam for environment: {AutoAuthConfig.CurrentEnvironment}");

        if (apiOptions.EnableSwagger)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = apiOptions.Title,
                    Version = apiOptions.Version
                });

                var scopesDict = new Dictionary<string, string>();
                scopesDict[apiOAuth.ApiScope] = GetScopeDescription(apiOAuth.ApiScope);
                if (apiOAuth.AllowScopes != null)
                {
                    foreach (var scope in apiOAuth.AllowScopes)
                    {
                        scopesDict[scope] = GetScopeDescription(scope);
                    }
                }

                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{apiOAuth.IdentityServerBaseUrl}/connect/authorize"),
                            TokenUrl = new Uri($"{apiOAuth.IdentityServerBaseUrl}/connect/token"),
                            Scopes = scopesDict
                        }
                    }
                });
                var apiScopes =new List<string>() { apiOAuth.ApiScope };
                if (apiOAuth.AllowScopes != null)
                {
                    apiScopes.AddRange(apiOAuth.AllowScopes);
                }
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            }
                        },

                         apiScopes
                    }
                });
            });

            services.AddEndpointsApiExplorer();
        }

        try
        {
            var rsa = RSA.Create();
            rsa.ImportParameters(new RSAParameters
            {
                Modulus = Base64UrlEncoder.DecodeBytes(jwtOptions.Modulus),
                Exponent = Base64UrlEncoder.DecodeBytes(jwtOptions.Exponent)
            });

            var key = new RsaSecurityKey(rsa) { KeyId = jwtOptions.KeyId };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = apiOAuth.IdentityServerBaseUrl,
                        ValidateAudience = true,
                        ValidAudience = apiOAuth.ApiScope,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateLifetime = jwtOptions.ValidateLifetime,
                        ClockSkew = jwtOptions.ClockSkew
                    };

                    if (AutoAuthConfig.CurrentEnvironment.Equals("Development", StringComparison.OrdinalIgnoreCase))
                    {
                        options.Events = new JwtBearerEvents
                        {
                            OnAuthenticationFailed = context =>
                            {
                                Console.WriteLine($"JWT Authentication Failed: {context.Exception.Message}");
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                Console.WriteLine($"JWT Token Validated for: {context.Principal?.Identity?.Name}");
                                return Task.CompletedTask;
                            }
                        };
                    }
                });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to configure JWT authentication: {ex.Message}");
        }

        services.AddHttpContextAccessor();
        services.AddHttpClient();

        services.AddHttpClient("SallaApi", client =>
        {
            client.BaseAddress = new Uri(apiSalla.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(apiSalla.TimeoutSeconds);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", apiOAuth.AppName);
        });

        services.AddSingleton(options);

        if (AutoAuthConfig.CurrentEnvironment.Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Development mode enabled - additional logging and features active");
        }

        return services;
    }

    private static string GetScopeDescription(string scope)
    {
        return scope switch
        {
            "lahja_api_app" => "Access Lahja API",
            "salla_api_app" => "Access Salla API",
            "offline_access" => "Offline access with refresh tokens",
            _ => scope
        };
    }
}