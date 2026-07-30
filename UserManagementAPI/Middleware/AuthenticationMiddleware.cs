namespace UserManagementAPI.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private const string TargetToken = "TechHiveSecret2026"; // Token estático para la simulación

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Si el cliente intenta ingresar con un token incorrecto o vacío
        if (!context.Request.Headers.TryGetValue("X-Auth-Token", out var extractedToken) || extractedToken != TargetToken)
        {
            context.Response.StatusCode = 401; // Unauthorized
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "Acceso denegado. Token inválido o ausente." });
            return; // Bloquea la petición aquí, no pasa al controlador
        }

        await _next(context); // Token correcto, continúa al siguiente paso
    }
}