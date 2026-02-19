namespace ProductsMicroservice.API.Middlewares;

// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext httpContext, RequestDelegate next)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{ex.GetType().ToString()}:{ex.Message}");

            if (ex.InnerException is not null)
            {
                _logger.LogError($"{ex.InnerException.GetType().ToString()}:{ex.InnerException.Message}");
            }

            httpContext.Response.StatusCode = 500;

            await httpContext.Response
                             .WriteAsJsonAsync(new
                             {
                                 Message = ex.Message,
                                 Type = ex.GetType().ToString()
                             });
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
