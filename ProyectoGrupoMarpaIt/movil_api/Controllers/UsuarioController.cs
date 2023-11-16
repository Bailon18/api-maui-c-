using Microsoft.AspNetCore.Mvc;
using movil_api.Datos;
using movil_api.Modelo;


namespace movil_api.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController: ControllerBase
    {
        [HttpGet]
        public async Task <ActionResult<List<UsuarioModel>>> getUsuarios()
        {
            var funcion = new UsuarioD();

            var listaUsuarios = await funcion.listarUsuarios();
            return listaUsuarios;
        }

        [HttpPost("validar")]
        public async Task<ActionResult<UsuarioModel>> ValidarCredenciales([FromBody] CredencialesModel credenciales)
        {
            var funcion = new UsuarioD();

            var usuarioValidado = await funcion.ValidarCredenciales(credenciales.correo, credenciales.contrasena);

            if (usuarioValidado != null)
            {
                return usuarioValidado;
            }

            return NoContent();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioModel>> BuscarPorId(int id)
        {
            var funcion = new UsuarioD();

            var usuario = await funcion.BuscarPorId(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> InsertarUsuario([FromBody] UsuarioModel usuario)
        {
            try
            {
                var funcion = new UsuarioD();
                var exito = await funcion.InsertarUsuario(usuario);
                return Ok(exito);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar usuario: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> ActualizarUsuario(int id, [FromBody] UsuarioModel usuario)
        {

            var funcion = new UsuarioD();

            if (id != usuario.usuario_id)
            {
                return BadRequest("El ID del usuario no coincide");
            }

          
            
            var exito = await funcion.ActualizarUsuario(usuario);
            return Ok(exito);
            
           
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> EliminarUsuario(int id)
        {
            try
            {
                var funcion = new UsuarioD();

                var exito = await funcion.EliminarUsuario(id);
                return Ok(exito);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar usuario: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }


        [HttpGet("rol/{nombreRol}")]
        public async Task<ActionResult<int>> ObtenerRolIdPorNombre(string nombreRol)
        {
            try
            {
                var funcion = new UsuarioD();
                int rolId = await funcion.ObtenerRolIdPorNombre(nombreRol);
                return Ok(rolId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el ID del rol por nombre: {ex.Message}");
            }
        }

        [HttpGet("departamento/{nombreDepartamento}")]
        public async Task<ActionResult<int>> ObtenerDepartamentoIdPorNombre(string nombreDepartamento)
        {
            try
            {
                var funcion = new UsuarioD();
                int departamentoId = await funcion.ObtenerDepartamentoIdPorNombre(nombreDepartamento);
                return Ok(departamentoId);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el ID del departamento por nombre: {ex.Message}");
            }
        }
    }

}
