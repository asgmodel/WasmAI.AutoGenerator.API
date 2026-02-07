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

        Console.WriteLine("Initializing AutoAuthWasm...");

        // التحقق من القيم الأساسية
        if (string.IsNullOrEmpty(apiOAuth.IdentityServerBaseUrl))
            Console.WriteLine("⚠️ Warning: IdentityServerBaseUrl is not configured");

        if (string.IsNullOrEmpty(jwtOptions.Modulus) || string.IsNullOrEmpty(jwtOptions.Exponent))
            Console.WriteLine("⚠️ Warning: JWT RSA keys are not configured");

        // إضافة Swagger
        if (apiOptions.EnableSwagger)
        {
            AddSwaggerConfiguration(services, apiOptions, apiOAuth);
        }

        // إضافة مصادقة JWT
        try
        {
            AddJwtAuthentication(services, apiOAuth, jwtOptions);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to configure JWT authentication: {ex.Message}", ex);
        }

        // إضافة خدمات HTTP
        AddHttpServices(services, apiSalla, apiOAuth);

        // إضافة الخيارات كخدمة singleton
        services.AddSingleton(options);

        Console.WriteLine("✅ AutoAuthWasm initialized successfully");
        return services;
    }

    private static void AddSwaggerConfiguration(IServiceCollection services, ApiOptions apiOptions, ApiOAuthOptions apiOAuth)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = apiOptions.Title,
                Version = apiOptions.Version
            });

            var scopesDict = new Dictionary<string, string>();

            // إضافة النطاق الرئيسي
            if (!string.IsNullOrEmpty(apiOAuth.ApiScope))
            {
                scopesDict[apiOAuth.ApiScope] = GetScopeDescription(apiOAuth.ApiScope);
            }

            // إضافة النطاقات المسموح بها
            if (apiOAuth.AllowScopes != null)
            {
                foreach (var scope in apiOAuth.AllowScopes)
                {
                    if (!string.IsNullOrEmpty(scope) && !scopesDict.ContainsKey(scope))
                    {
                        scopesDict[scope] = GetScopeDescription(scope);
                    }
                }
            }

            // إضافة تعريف الأمان OAuth2 إذا كان هناك عنوان Identity Server
            if (!string.IsNullOrEmpty(apiOAuth.IdentityServerBaseUrl) && scopesDict.Any())
            {
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

                // جمع كل النطاقات
                var apiScopes = new List<string>();
                if (!string.IsNullOrEmpty(apiOAuth.ApiScope))
                {
                    apiScopes.Add(apiOAuth.ApiScope);
                }
                if (apiOAuth.AllowScopes != null)
                {
                    apiScopes.AddRange(apiOAuth.AllowScopes.Where(s => !string.IsNullOrEmpty(s)));
                }

                // إضافة متطلبات الأمان
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
                        apiScopes.Distinct().ToList()
                    }
                });
            }
        });

        services.AddEndpointsApiExplorer();
    }

    private static void AddJwtAuthentication(IServiceCollection services, ApiOAuthOptions apiOAuth, JwtOptions jwtOptions)
    {
        // التحقق من وجود مفاتيح RSA
        if (string.IsNullOrEmpty(jwtOptions.Modulus) || string.IsNullOrEmpty(jwtOptions.Exponent))
        {
            Console.WriteLine("⚠️ JWT RSA keys not configured - using development test keys");
            UseTestKeysForDevelopment(services, apiOAuth, jwtOptions);
            return;
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
                        ValidIssuer = !string.IsNullOrEmpty(jwtOptions.Issuer)
                            ? jwtOptions.Issuer
                            : apiOAuth.IdentityServerBaseUrl,
                        ValidateAudience = true,
                        ValidAudience = !string.IsNullOrEmpty(jwtOptions.Audience)
                            ? jwtOptions.Audience
                            : apiOAuth.ApiScope,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateLifetime = jwtOptions.ValidateLifetime,
                        ClockSkew = jwtOptions.ClockSkew
                    };

                    // إضافة معالجات الأحداث للتصحيح
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
                });

            Console.WriteLine("✅ JWT Authentication configured");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error configuring JWT: {ex.Message}");
            throw;
        }
    }

    private static void UseTestKeysForDevelopment(IServiceCollection services, ApiOAuthOptions apiOAuth, JwtOptions jwtOptions)
    {
        // مفاتيح RSA تجريبية للتطوير فقط
        var rsa = RSA.Create();
        var parameters = rsa.ExportParameters(false);

        var key = new RsaSecurityKey(rsa)
        {
            KeyId = "TestKey_" + Guid.NewGuid().ToString("N").Substring(0, 8)
        };

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = false // تعطيل التحقق من الصلاحية للتطوير
                };

                Console.WriteLine("⚠️ Using development/test JWT configuration");
            });
    }

    private static void AddHttpServices(IServiceCollection services, ApiSallaOptions apiSalla, ApiOAuthOptions apiOAuth)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient();

        // إضافة HttpClient مخصص لـ Salla API إذا تم تكوين BaseUrl
        if (!string.IsNullOrEmpty(apiSalla.BaseUrl))
        {
            services.AddHttpClient("SallaApi", client =>
            {
                client.BaseAddress = new Uri(apiSalla.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(apiSalla.TimeoutSeconds);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                if (!string.IsNullOrEmpty(apiOAuth.AppName))
                {
                    client.DefaultRequestHeaders.Add("User-Agent", apiOAuth.AppName);
                }
            });

            Console.WriteLine($"✅ Salla API HttpClient configured with base URL: {apiSalla.BaseUrl}");
        }
        else
        {
            Console.WriteLine("⚠️ Salla API BaseUrl not configured - SallaApi HttpClient not registered");
        }
    }

    private static string GetScopeDescription(string scope)
    {
        return scope.ToLower() switch
        {
            "lahja_api_app" => "Access Lahja API",
            "salla_api_app" => "Access Salla API",
            "offline_access" => "Offline access with refresh tokens",
            "openid" => "OpenID Connect authentication",
            "profile" => "User profile information",
            "email" => "Email address",
            _ => $"Access to {scope}"
        };
    }
}