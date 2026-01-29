using System.Text.Json;
using System.Text.Json.Serialization;

namespace WasmAI.AutoGenerator.Auth;

public static class CreateConfigFile
{
   
    public static void GenerateEmptyWasmConfig(string outputPath = "autoauthapisettings.json")
    {
        var config = new AutoAuthWasmOptions
        {
            ApiOptions = new ApiOptions
            {
                Title = "Name API - ",
                Version = "v1",
                EnableSwagger = true,
                EnableCors = true,
                AllowedOrigins = new[]
                {
                    "http://localhost:3000",
                    "http://localhost:4200",
                    "http://localhost:5000",
                    "https://localhost:5001"
                },
                RateLimit = new RateLimitConfig
                {
                    Enabled = false,
                    Limit = 1000,
                    PeriodMinutes = 1
                }
            },
            ApiSalla = new ApiSallaOptions
            {
                BaseUrl = "https://salla.lahjai.net",
                ApiVersion = "v2",
                TimeoutSeconds = 60,
                RetryCount = 5
            },
            ApiOAuth = new ApiOAuthOptions
            {
                ClientId = "dev_client_id",
                ClientSecret = "dev_client_secret",
                AppName = "dev",
                UsePkce = true,
                ScopeSeparator = " ",
                IdentityServerBaseUrl = "https://auth.dev.lahjai.net",
                ApiScope = "API_SCOPE",
                AllowScopes = new[] {  "offline_access", "openid", "profile", "email" }
            },
            JwtSettings = new JwtOptions
            {
                KeyId = "D5966B9792FE28029F484D649F2F5EA1",
                Modulus = "sV2RkdlvlzG9WqD-SmYUqVocQV_FiFWFEVVMB_c_-5S_miKur-3-_-XeI4x3BFY96m_gyy5guvL5gW-x1Oa84txrWD7NdtbGNC9CCMyHkSe4XMivg3AOHfCylmeQAHL91FtjtXiUp3I39waPtYTrVPpYRmM1I8lT81TPh8hy-jpBayrdjuvjOjrfT6E0aJPq0twhfDNjhWYd5UszVjwhPY4LLutzLXbHOMHkFyMUMrXiF_n3kI0RTOPUABJvfp79L9T0dcK9kLsMpc3f381gqf6-2J5Kvq6ZtdMcMXt2k5x20_cExpA1VP5p1uKQY_tcsPCG0lRmydK8I_2KzsbwzQ",
                Exponent = "AQAB",
                Issuer = "https://auth.lahjai.net",
                Audience = "API_SCOPE",
                ValidateLifetime = true,
                ClockSkewMinutes = 5
            }
        };

        SaveConfig(config, outputPath);
        Console.WriteLine($"Generated: {outputPath}");
    }

    public static void GenerateEmptyConfig(string outputPath = "autoauthapisettings.json")
    {
        var config = new AutoAuthWasmOptions
        {
            ApiOptions = new ApiOptions
            {
                Title = "",
                Version = "v1",
                EnableSwagger = true,
                EnableCors = true,
                AllowedOrigins = Array.Empty<string>(),
                RateLimit = new RateLimitConfig
                {
                    Enabled = true,
                    Limit = 100,
                    PeriodMinutes = 1
                }
            },
            ApiSalla = new ApiSallaOptions
            {
                BaseUrl = "",
                ApiVersion = "v2",
                TimeoutSeconds = 30,
                RetryCount = 3
            },
            ApiOAuth = new ApiOAuthOptions
            {
                ClientId = "",
                ClientSecret = "",
                AppName = "",
                UsePkce = true,
                ScopeSeparator = " ",
                IdentityServerBaseUrl = "",
                ApiScope = "",
                AllowScopes = Array.Empty<string>()
            },
            JwtSettings = new JwtOptions
            {
                KeyId = "",
                Modulus = "",
                Exponent = "",
                Issuer = "",
                Audience = "",
                ValidateLifetime = true,
                ClockSkewMinutes = 5
            }
        };

        SaveConfig(config, outputPath);
        Console.WriteLine($"Generated empty: {outputPath}");
    }

    public static void GenerateBothConfigs(string directory = ".")
    {
        GenerateEmptyWasmConfig(Path.Combine(directory, "autoauthapisettings.json"));
       // GenerateDevelopmentConfig(Path.Combine(directory, "autoauthapisettings.Development.json"));
        Console.WriteLine("Generated both config files successfully!");
    }

    private static void SaveConfig(AutoAuthWasmOptions config, string outputPath)
    {
        var wrapper = new { AutoAuthWasmOptions = config };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(wrapper, options);

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(outputPath, json);
    }
}