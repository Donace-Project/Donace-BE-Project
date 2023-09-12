using Donace_BE_Project.Middlewares;

namespace Donace_BE_Project.Extensions;

public static class ConfigureCustomMiddleware
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }
}
