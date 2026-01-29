using Microsoft.AspNetCore.Builder;

namespace WasmAI.AutoGenerator.Auth;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAutoAuthWasm(this IApplicationBuilder app)
    {
        var options = AutoAuthConfig.Options;
        var apiOptions = options.ApiOptions;
        var apiOAuth = options.ApiOAuth;

        var environment = AutoAuthConfig.CurrentEnvironment;
        Console.WriteLine($"Configuring middleware for environment: {environment}");

        if (apiOptions.EnableSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(uiOptions =>
            {
               // uiOptions.SwaggerEndpoint("/swagger/v1/swagger.json", $"{apiOptions.Title} {apiOptions.Version}");
                uiOptions.OAuthClientId(apiOAuth.ClientId);

                if (!string.IsNullOrEmpty(apiOAuth.ClientSecret))
                    uiOptions.OAuthClientSecret(apiOAuth.ClientSecret);

                uiOptions.OAuthAppName(apiOAuth.AppName);

                if (apiOAuth.UsePkce)
                    uiOptions.OAuthUsePkce();

                uiOptions.OAuthScopeSeparator(apiOAuth.ScopeSeparator);

                if (environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
                {
                    uiOptions.DisplayRequestDuration();
                    uiOptions.EnableDeepLinking();
                    uiOptions.EnableFilter();
                    uiOptions.ShowExtensions();
                    uiOptions.DefaultModelsExpandDepth(-1);
                }
            });
        }

        if (apiOptions.EnableCors && apiOptions.AllowedOrigins.Any())
        {
            app.UseCors(builder => builder
                .WithOrigins(apiOptions.AllowedOrigins)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        }
        else if (environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        }

        app.UseAuthentication();
        app.UseAuthorization();

        if (apiOptions.RateLimit?.Enabled == true && !environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            app.UseRateLimiter();
        }

        if (environment.Equals("Development", StringComparison.OrdinalIgnoreCase))
        {
            app.UseDeveloperExceptionPage();
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
                await next();
            });
        }

        return app;
    }
}