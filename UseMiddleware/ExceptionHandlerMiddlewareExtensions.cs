namespace UseMiddleware;

public static class ExceptionHandlerMiddlewareExtensions
{
    public static void UseExceptionHandler(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandler>();
    }
}