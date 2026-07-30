using UserManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Registrar los controladores
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

// --- CONFIGURACIÓN DE MIDDLEWARES EN ORDEN CORPORATIVO ---

// 1. Error-Handling Primero (Para capturar errores de cualquier nivel inferior)
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2. Authentication Segundo (Para denegar accesos inválidos de inmediato sin gastar procesamiento)
app.UseMiddleware<AuthenticationMiddleware>();

// 3. Logging Al final de la cadena de control (Para capturar la auditoría real de la petición)
app.UseMiddleware<LoggingMiddleware>();

// Mapear rutas de controladores
app.MapControllers();

app.Run();