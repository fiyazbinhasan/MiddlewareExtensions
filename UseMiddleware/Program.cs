using System.Globalization;
using System.Net;
using System.Text.Json;
using UseMiddleware;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.UseExceptionHandler();

var loggerFactory = LoggerFactory.Create(loggingBuilder =>
{
    loggingBuilder.AddFilter(level => level == LogLevel.Warning);
    loggingBuilder.AddConsole();
});

var logger = loggerFactory.CreateLogger<Program>();

app.Use(async (context, next) =>
{
    var cultureQuery = context.Request.Query["culture"];
    if (!string.IsNullOrWhiteSpace(cultureQuery))
    {
        var culture = new CultureInfo(cultureQuery);

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    // Call the next delegate/middleware in the pipeline
    await next(context);
});

app.Run(async (context) =>
{
    await context.Response.WriteAsync(
        $"Hello {Equals(c, context)}");
});

app.Use(async (context, requestDelegate) =>
{
    try
    {
        await requestDelegate(context);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex.Message);
        var result = JsonSerializer.Serialize(new ErrorEnvelope(ex.Message, (int)HttpStatusCode.InternalServerError));
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(result);
    }
});

app.Use(async (context, requestDelegate) =>
{
    try
    {
        var previousContextResponse = context.Response;
        await requestDelegate(context);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex.Message);
        var result = JsonSerializer.Serialize(new ErrorEnvelope(ex.Message, (int)HttpStatusCode.InternalServerError));
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(result);
    }
});

app.Use(async (context, func) =>
{
    try
    {
        await func();
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex.Message);
        var result = JsonSerializer.Serialize(new ErrorEnvelope(ex.Message, (int)HttpStatusCode.InternalServerError));
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(result);
    }
});

app.MapGet("/", () => new Item("Test"));

app.Run();