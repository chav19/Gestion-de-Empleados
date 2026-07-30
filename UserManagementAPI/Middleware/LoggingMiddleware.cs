namespace UserManagementAPI.Middleware;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Registrar la entrada de la petición
        _logger.LogInformation($"[PETICIÓN INCOMING] Método: {context.Request.Method} | Ruta: {context.Request.Path}");

        await _next(context);

        // Registrar la salida de la respuesta
        _logger.LogInformation($"[RESPUESTA OUTGOING] Código de Estado: {context.Response.StatusCode} para {context.Request.Path}");
    }
}