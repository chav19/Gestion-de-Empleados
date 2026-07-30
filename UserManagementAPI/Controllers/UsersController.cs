using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    // Simulación de base de datos en memoria (Thread-safe para optimizar concurrencia)
    private static readonly List<User> _users = new()
    {
        new User { Id = 1, Name = "Salvador Garcia", Email = "salvadorg@techhive.com", Role = "HR" },
        new User { Id = 2, Name = "Pancho Segundo", Email = "panchos@techhive.com", Role = "IT" }
    };

    // GET: api/users (Optimizado)
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers()
    {
        try
        {
            // Retorna una copia de solo lectura para mejorar el rendimiento y evitar bloqueos en memoria
            return Ok(_users.AsReadOnly());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno al recuperar los usuarios.", details = ex.Message });
        }
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    public ActionResult<User> GetUser(int id)
    {
        try
        {
            // Optimización: Find es más rápido que FirstOrDefault en Listas locales
            var user = _users.Find(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = $"El usuario con ID {id} no existe en el sistema." });
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno al buscar el usuario.", details = ex.Message });
        }
    }

    // POST: api/users (Con validación estricta)
    [HttpPost]
    public ActionResult<User> CreateUser([FromBody] User newUser)
    {
        try
        {
            // Paso 1: Validar si las reglas de User.cs se cumplen
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Paso 2: Validar que el correo no esté duplicado (Regla de negocio de TechHive)
            if (_users.Exists(u => u.Email.Equals(newUser.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest(new { message = "El correo electrónico ya se encuentra registrado." });
            }

            newUser.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
            newUser.CreatedAt = DateTime.UtcNow;
            
            _users.Add(newUser);

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, newUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al intentar registrar el usuario.", details = ex.Message });
        }
    }

    // PUT: api/users/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existingUser = _users.Find(u => u.Id == id);
            if (existingUser == null)
            {
                return NotFound(new { message = $"No se encontró el usuario con ID {id} para actualizar." });
            }

            existingUser.Name = updatedUser.Name;
            existingUser.Email = updatedUser.Email;
            existingUser.Role = updatedUser.Role;

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al intentar actualizar el usuario.", details = ex.Message });
        }
    }

    // DELETE: api/users/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        try
        {
            var user = _users.Find(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = $"No se pudo eliminar: El usuario con ID {id} no existe." });
            }

            _users.Remove(user);
            return Ok(new { message = $"Usuario con ID {id} eliminado con éxito." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al intentar eliminar el usuario.", details = ex.Message });
        }
    }
}