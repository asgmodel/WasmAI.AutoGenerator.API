using System.Text.Json;

namespace WasmAI.AutoGenerator.Auth;

public static class AutoAuthConfig
{
    private static AutoAuthWasmOptions _options;
    public static readonly string ConfigFileName = "autoauthapisettings.json";

    public static AutoAuthWasmOptions Options => _options ?? LoadConfig();

    private static AutoAuthWasmOptions LoadConfig()
    {
        try
        {
            return LoadFromFile(ConfigFileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading config: {ex.Message}");
            return new AutoAuthWasmOptions();
        }
    }

    private static AutoAuthWasmOptions LoadFromFile(string configFile)
    {
        if (!File.Exists(configFile))
        {
            Console.WriteLine($"Config file not found: {configFile}");
            Console.WriteLine("Creating default config file...");
            CreateDefaultConfigFile(configFile);
        }

        try
        {
            string jsonContent = File.ReadAllText(configFile);

            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                Console.WriteLine($"Config file is empty: {configFile}");
                return new AutoAuthWasmOptions();
            }

            // طريقة مباشرة باستخدام فئة wrapper
            var wrapper = JsonSerializer.Deserialize<AutoAuthWasmOptionsWrapper>(
                jsonContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            _options = wrapper?.AutoAuthWasmOptions ?? new AutoAuthWasmOptions();

            Console.WriteLine($"✅ Loaded config from: {configFile}");
            return _options;
        }
        catch (JsonException jsonEx)
        {
            Console.WriteLine($"Invalid JSON in {configFile}: {jsonEx.Message}");
            return new AutoAuthWasmOptions();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading config from {configFile}: {ex.Message}");
            return new AutoAuthWasmOptions();
        }
    }

    public static void Reload()
    {
        _options = null;
        LoadConfig();
    }

    public static void PrintCurrentConfig()
    {
        var options = Options;
        Console.WriteLine($"=== Current Configuration ===");
        Console.WriteLine($"API Title: {options.ApiOptions.Title}");
        Console.WriteLine($"API Version: {options.ApiOptions.Version}");
        Console.WriteLine($"Swagger Enabled: {options.ApiOptions.EnableSwagger}");
        Console.WriteLine($"CORS Enabled: {options.ApiOptions.EnableCors}");
        Console.WriteLine($"Allowed Origins: {string.Join(", ", options.ApiOptions.AllowedOrigins)}");
        Console.WriteLine($"Salla Base URL: {options.ApiSalla.BaseUrl}");
        Console.WriteLine($"OAuth Client ID: {options.ApiOAuth.ClientId}");
        Console.WriteLine($"OAuth Scopes: {options.ApiOAuth.ScopesString}");
        Console.WriteLine($"JWT Issuer: {options.JwtSettings.Issuer}");
        Console.WriteLine($"JWT Audience: {options.JwtSettings.Audience}");
        Console.WriteLine("=============================");
    }

    public static void EnsureConfigFileExists()
    {
        if (!File.Exists(ConfigFileName))
        {
            Console.WriteLine($"Creating default config file: {ConfigFileName}");
            CreateDefaultConfigFile(ConfigFileName);
        }
    }

    private static void CreateDefaultConfigFile(string filePath)
    {
        var defaultConfig = new
        {
            AutoAuthWasmOptions = new
            {
                ApiOptions = new
                {
                    Title = "API",
                    Version = "v1",
                    EnableSwagger = true,
                    EnableCors = true,
                    AllowedOrigins = new string[] { },
                    RateLimit = new
                    {
                        Enabled = true,
                        Limit = 100,
                        PeriodMinutes = 1
                    }
                },
                ApiSalla = new
                {
                    BaseUrl = "",
                    ApiVersion = "v2",
                    TimeoutSeconds = 30,
                    RetryCount = 3
                },
                ApiOAuth = new
                {
                    ClientId = "",
                    ClientSecret = "",
                    AppName = "",
                    UsePkce = true,
                    ScopeSeparator = " ",
                    IdentityServerBaseUrl = "",
                    ApiScope = "",
                    AllowScopes = new string[] { }
                },
                JwtSettings = new
                {
                    KeyId = "",
                    Modulus = "",
                    Exponent = "",
                    Issuer = "",
                    Audience = "",
                    ValidateLifetime = true,
                    ClockSkewMinutes = 5
                }
            }
        };

        var json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        File.WriteAllText(filePath, json);
    }
}

// فئة wrapper للطريقة البسيطة
public class AutoAuthWasmOptionsWrapper
{
    public AutoAuthWasmOptions AutoAuthWasmOptions { get; set; } = new();
}

public class AutoAuthWasmOptions
{
    public ApiOptions ApiOptions { get; set; } = new();
    public ApiSallaOptions ApiSalla { get; set; } = new();
    public ApiOAuthOptions ApiOAuth { get; set; } = new();
    public JwtOptions JwtSettings { get; set; } = new();
}

public class ApiOptions
{
    public string Title { get; set; } = "API";
    public string Version { get; set; } = "v1";
    public bool EnableSwagger { get; set; } = true;
    public bool EnableCors { get; set; } = true;
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
    public RateLimitConfig RateLimit { get; set; } = new();
}

public class ApiSallaOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "v2";
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;

    public string FullApiUrl => $"{BaseUrl.TrimEnd('/')}/api/{ApiVersion}";
}

public class ApiOAuthOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string AppName { get; set; } = string.Empty;
    public bool UsePkce { get; set; } = true;
    public string ScopeSeparator { get; set; } = " ";
    public string IdentityServerBaseUrl { get; set; } = string.Empty;
    public string ApiScope { get; set; } = string.Empty;
    public string[] AllowScopes { get; set; } = Array.Empty<string>();

    public string ScopesString => string.Join(ScopeSeparator, AllowScopes);
}

public class JwtOptions
{
    public string KeyId { get; set; } = string.Empty;
    public string Modulus { get; set; } = string.Empty;
    public string Exponent { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public bool ValidateLifetime { get; set; } = true;
    public int ClockSkewMinutes { get; set; } = 5;

    public TimeSpan ClockSkew => TimeSpan.FromMinutes(ClockSkewMinutes);
}

public class RateLimitConfig
{
    public bool Enabled { get; set; } = true;
    public int Limit { get; set; } = 100;
    public int PeriodMinutes { get; set; } = 1;
}