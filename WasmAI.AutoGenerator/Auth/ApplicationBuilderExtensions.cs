using Microsoft.AspNetCore.Builder;

namespace WasmAI.AutoGenerator.Auth;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAutoAuthWasm(this IApplicationBuilder app)
    {
        var options = AutoAuthConfig.Options;
        var apiOptions = options.ApiOptions;
        var apiOAuth = options.ApiOAuth;

        Console.WriteLine("Configuring AutoAuthWasm middleware...");

        // إعداد Swagger UI
        if (apiOptions.EnableSwagger)
        {
            ConfigureSwagger(app, apiOptions, apiOAuth);
        }

        // إعداد CORS
        ConfigureCors(app, apiOptions);

        // إعداد المصادقة والتفويض
        ConfigureAuthentication(app);

        // إعداد Rate Limiting
        ConfigureRateLimiting(app, apiOptions);

        // إعداد تطوير إضافي (اختياري)
        ConfigureDevelopmentFeatures(app);

        Console.WriteLine("✅ AutoAuthWasm middleware configured");
        return app;
    }

    private static void ConfigureSwagger(IApplicationBuilder app, ApiOptions apiOptions, ApiOAuthOptions apiOAuth)
    {
        app.UseSwagger();
        app.UseSwaggerUI(uiOptions =>
        {
            uiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", $"{apiOptions.Title} {apiOptions.Version}");

            // تكوين OAuth
            if (!string.IsNullOrEmpty(apiOAuth.ClientId))
            {
                uiOptions.OAuthClientId(apiOAuth.ClientId);
            }

            if (!string.IsNullOrEmpty(apiOAuth.ClientSecret))
            {
                uiOptions.OAuthClientSecret(apiOAuth.ClientSecret);
            }

            if (!string.IsNullOrEmpty(apiOAuth.AppName))
            {
                uiOptions.OAuthAppName(apiOAuth.AppName);
            }

            if (apiOAuth.UsePkce)
            {
                uiOptions.OAuthUsePkce();
            }

            uiOptions.OAuthScopeSeparator(apiOAuth.ScopeSeparator);

            // ميزات إضافية مفيدة
            uiOptions.DisplayRequestDuration();
            uiOptions.EnableFilter();
            uiOptions.EnableDeepLinking();
            uiOptions.ShowExtensions();
            uiOptions.DefaultModelsExpandDepth(-1);
          

            // تفعيل Try It Out افتراضيًا
            uiOptions.EnableTryItOutByDefault();
        });

        Console.WriteLine("✅ Swagger UI configured");
    }

    private static void ConfigureCors(IApplicationBuilder app, ApiOptions apiOptions)
    {
        if (apiOptions.EnableCors)
        {
            if (apiOptions.AllowedOrigins != null && apiOptions.AllowedOrigins.Any())
            {
                // CORS مقيد بالأصول المسموح بها
                app.UseCors(builder => builder
                    .WithOrigins(apiOptions.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());

                Console.WriteLine($"✅ CORS configured with {apiOptions.AllowedOrigins.Length} allowed origins");
            }
            else
            {
                // CORS مفتوح (للتنمية فقط - يجب تعديله للإنتاج)
                app.UseCors(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());

                Console.WriteLine("⚠️ CORS configured with AllowAnyOrigin (not recommended for production)");
            }
        }
        else
        {
            Console.WriteLine("ℹ️ CORS is disabled");
        }
    }

    private static void ConfigureAuthentication(IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        Console.WriteLine("✅ Authentication & Authorization middleware added");
    }

    private static void ConfigureRateLimiting(IApplicationBuilder app, ApiOptions apiOptions)
    {
        if (apiOptions.RateLimit?.Enabled == true)
        {
            try
            {
                app.UseRateLimiter();
                Console.WriteLine($"✅ Rate limiting enabled: {apiOptions.RateLimit.Limit} requests per {apiOptions.RateLimit.PeriodMinutes} minutes");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Rate limiting not available: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("ℹ️ Rate limiting is disabled");
        }
    }

    private static void ConfigureDevelopmentFeatures(IApplicationBuilder app)
    {
        // ميزات مفيدة للتنمية والإنتاج
        app.UseDeveloperExceptionPage();

        // إضافة middleware للتسجيل
        app.Use(async (context, next) =>
        {
            var startTime = DateTime.UtcNow;
            Console.WriteLine($"→ Request: {context.Request.Method} {context.Request.Path}");

            await next();

            var duration = DateTime.UtcNow - startTime;
            Console.WriteLine($"← Response: {context.Response.StatusCode} ({duration.TotalMilliseconds:F2}ms)");
        });

        // إضافة middleware لتحسين الأمان
        app.UseHsts();
        app.UseHttpsRedirection();

        Console.WriteLine("✅ Security and development features configured");
    }
}