using System.Text.Json;

namespace WasmAI.AutoGenerator.Auth;

public static class AutoAuthConfig
{
    private static AutoAuthWasmOptions _options;
    private static string _currentEnvironment;
    public static string PathToAutoauthapisettings = "autoauthapisettings.json";

    public static AutoAuthWasmOptions Options => _options ?? LoadConfigForEnvironment();

    public static string CurrentEnvironment => _currentEnvironment ?? GetEnvironmentName();

    private static string GetEnvironmentName()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Production";
    }

    private static AutoAuthWasmOptions LoadConfigForEnvironment()
    {
        var environment = GetEnvironmentName();
        _currentEnvironment = environment;

        return environment.ToLower() switch
        {
            "development" => LoadDevelopmentConfig(),
            _ => LoadDefaultConfig()
        };
    }

    private static AutoAuthWasmOptions LoadDefaultConfig()
    {
        return LoadFromFile(PathToAutoauthapisettings);
    }

    private static AutoAuthWasmOptions LoadDevelopmentConfig()
    {
        var devFile = PathToAutoauthapisettings;

        if (File.Exists(devFile))
        {
            try
            {
                return LoadFromFile(devFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Failed to load development config: {ex.Message}");
                Console.WriteLine("Falling back to default config...");
            }
        }

        return LoadDefaultConfig();
    }

    private static AutoAuthWasmOptions LoadFromFile(string configFile)
    {
        if (!File.Exists(configFile))
        {
            Console.WriteLine($"Config file not found: {configFile}");
            return new AutoAuthWasmOptions();
        }

        try
        {
            // **التصحيح هنا: قراءة محتوى الملف أولاً**
            string jsonContent = File.ReadAllText(configFile);

            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                Console.WriteLine($"Config file is empty: {configFile}");
                return new AutoAuthWasmOptions();
            }

            // التحقق من صحة JSON
            try
            {
                using JsonDocument doc = JsonDocument.Parse(jsonContent);
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"Invalid JSON in {configFile}: {jsonEx.Message}");
                return new AutoAuthWasmOptions();
            }

            // **التصحيح: استخدام jsonContent بدلاً من configFile**
            var configRoot = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                jsonContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (configRoot == null || !configRoot.ContainsKey("autoAuthWasmOptions"))
            {
                Console.WriteLine($"AutoAuthWasmOptions section not found in {configFile}");
                return new AutoAuthWasmOptions();
            }

            var optionsJson = configRoot["autoAuthWasmOptions"].GetRawText();
            _options = JsonSerializer.Deserialize<AutoAuthWasmOptions>(
                optionsJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new AutoAuthWasmOptions();

            Console.WriteLine($"✅ Loaded config from: {configFile} (Environment: {CurrentEnvironment})");
            return _options;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading config from {configFile}: {ex.Message}");
            return new AutoAuthWasmOptions();
        }
    }

    // نسخة أبسط من LoadFromFile بدون Dictionary<string, JsonElement>
    private static AutoAuthWasmOptions LoadFromFileSimple(string configFile)
    {
        if (!File.Exists(configFile))
        {
            Console.WriteLine($"Config file not found: {configFile}");
            return new AutoAuthWasmOptions();
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
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error loading config: {ex.Message}");
            return new AutoAuthWasmOptions();
        }
    }

    public static void Reload()
    {
        _options = null;
        LoadConfigForEnvironment();
    }

    public static void ForceReload(string configFile)
    {
        _options = null;
        _currentEnvironment = null;
        LoadFromFile(configFile);
    }

    public static AutoAuthWasmOptions GetConfigForEnvironment(string environment)
    {
        if (environment.Equals(CurrentEnvironment, StringComparison.OrdinalIgnoreCase))
            return Options;

        return environment.ToLower() switch
        {
            "development" => LoadDevelopmentConfig(),
            _ => LoadDefaultConfig()
        };
    }

    public static void PrintCurrentConfig()
    {
        var options = Options;
        Console.WriteLine($"=== Current Configuration (Environment: {CurrentEnvironment}) ===");
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
        Console.WriteLine("==========================================");
    }

    // دالة لإنشاء ملف إعدادات فارغ إذا لم يوجد
    public static void EnsureConfigFileExists(string filePath = null)
    {
        filePath ??= PathToAutoauthapisettings;

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Creating default config file: {filePath}");
            CreateDefaultConfigFile(filePath);
        }

        // التحقق من ملف Development إذا كنا في بيئة التطوير
        if (GetEnvironmentName().Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            string devFile = "autoauthapisettings.Development.json";
            if (!File.Exists(devFile))
            {
                Console.WriteLine($"Creating development config file: {devFile}");
                CreateDevelopmentConfigFile(devFile);
            }
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

    private static void CreateDevelopmentConfigFile(string filePath)
    {
        var devConfig = new
        {
            AutoAuthWasmOptions = new
            {
                ApiOptions = new
                {
                    Title = "API - Development",
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
                    RateLimit = new
                    {
                        Enabled = false,
                        Limit = 1000,
                        PeriodMinutes = 1
                    }
                },
                ApiSalla = new
                {
                    BaseUrl = "https://salla.dev.lahjai.net",
                    ApiVersion = "v2",
                    TimeoutSeconds = 60,
                    RetryCount = 5
                },
                ApiOAuth = new
                {
                    ClientId = "dev_client_id",
                    ClientSecret = "dev_client_secret",
                    AppName = "lahja-dev",
                    UsePkce = true,
                    ScopeSeparator = " ",
                    IdentityServerBaseUrl = "https://auth.dev.lahjai.net",
                    ApiScope = "lahja_api_app",
                    AllowScopes = new[] { "salla_api_app", "offline_access" }
                },
                JwtSettings = new
                {
                    KeyId = "DEV_KEY_ID",
                    Modulus = "DEV_MODULUS",
                    Exponent = "DEV_EXPONENT",
                    Issuer = "https://auth.dev.lahjai.net",
                    Audience = "lahja_api_app",
                    ValidateLifetime = true,
                    ClockSkewMinutes = 10
                }
            }
        };

        var json = JsonSerializer.Serialize(devConfig, new JsonSerializerOptions
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