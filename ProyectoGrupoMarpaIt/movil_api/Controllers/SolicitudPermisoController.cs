using Microsoft.AspNetCore.Mvc;
using movil_api.Datos;
using movil_api.Modelo;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace movil_api.Controllers
{
    [ApiController]
    [Route("api/solicitudes-permiso")]
    public class SolicitudPermisoController : ControllerBase
    {
        private readonly SolicitudesPermisoD _solicitudesPermisoD;

        public SolicitudPermisoController()
        {
            _solicitudesPermisoD = new SolicitudesPermisoD(); 
        }

        [HttpGet]
        public async Task<ActionResult<List<SolicitudesPermisoModel>>> ListarSolicitudesPermiso()
        {
            try
            {
                var listaSolicitudes = await _solicitudesPermisoD.listarSolicitudesPermiso();
                return Ok(listaSolicitudes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al listar las solicitudes de permiso: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }


        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<List<SolicitudesPermisoModel>>> ListarSolicitudesPorUsuario(int usuarioId)
        {
            try
            {
                var listaSolicitudesPorUsuario = await _solicitudesPermisoD.listarSolicitudesPorUsuario(usuarioId);
                return Ok(listaSolicitudesPorUsuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al listar las solicitudes de permiso por usuario: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }


        [HttpGet("estado/{estado}")]
        public async Task<ActionResult<List<SolicitudesPermisoModel>>> ListarSolicitudesPorEstado(string estado)
        {
            try
            {
                var listaSolicitudesPorEstado = await _solicitudesPermisoD.listarSolicitudesPorEstado(estado);
                return Ok(listaSolicitudesPorEstado);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al listar las solicitudes de permiso por estado: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }


        [HttpGet("departamento/{departamentoId}")]
        public async Task<ActionResult<List<SolicitudesPermisoModel>>> ObtenerSolicitudesPorDepartamento(int departamentoId)
        {
            try
            {
                var listaSolicitudesPorDepartamento = await _solicitudesPermisoD.ObtenerSolicitudesPorDepartamento(departamentoId);
                return Ok(listaSolicitudesPorDepartamento);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las solicitudes de permiso por departamento: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }


        [HttpPost("insertar-solicitud-completa")]
        public async Task<ActionResult<bool>> InsertarSolicitudCompleta([FromBody] SolicitudCompletaModel solicitudCompletaModel)
        {
            try
            {
                var exito = await _solicitudesPermisoD.insertarSolicitudCompleta(solicitudCompletaModel);

                if (exito)
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(500, "Error al insertar la solicitud completa");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar la solicitud completa: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> EliminarSolicitudPermiso(int id)
        {
            try
            {
                var exito = await _solicitudesPermisoD.EliminarSolicitudPermiso(id);
                return Ok(exito);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar solicitud: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }


        [HttpGet("detallesDias/{solicitudId}")]
        public async Task<ActionResult<DetallesSolicitudDiasModel>> ObtenerDetallesPorSolicitudId(int solicitudId)
        {
            try
            {
                var detallesSolicitud = await _solicitudesPermisoD.ObtenerDetallesPorSolicitudId(solicitudId);

                if (detallesSolicitud != null)
                {
                    return Ok(detallesSolicitud);
                }
                else
                {
                    return NotFound("No se encontraron detalles para la solicitud especificada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalles por solicitud_id: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }



        [HttpPut("actualizar-solicitud-completa")]
        public async Task<ActionResult<bool>> ActualizarSolicitudCompleta([FromBody] SolicitudCompletaModel solicitudCompletaModel)
        {
            try
            {

                var exito = await _solicitudesPermisoD.ActualizarSolicitudCompleta(solicitudCompletaModel);

                if (exito)
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(500, "Error al actualizar la solicitud completa");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la solicitud completa: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }



        [HttpGet("detallesHoras/{solicitudId}")] 
        public async Task<ActionResult<DetallesSolicitudHorasModel>> ObtenerDetallesHorasPorSolicitudId(int solicitudId)
        {
            try
            {
                var detallesSolicitudHoras = await _solicitudesPermisoD.ObtenerDetallesHorasPorSolicitudId(solicitudId);

                if (detallesSolicitudHoras != null)
                {
                    return Ok(detallesSolicitudHoras);
                }
                else
                {
                    return NotFound("No se encontraron detalles por horas para la solicitud especificada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalles por solicitud_id: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }



        [HttpGet("buscar")]
        public async Task<ActionResult<List<SolicitudesPermisoModel>>> BuscarSolicitudesPorNombreOApellido(string nombreOApellido)
        {
            try
            {
                var listaSolicitudes = await _solicitudesPermisoD.BuscarSolicitudesPorNombreOApellido(nombreOApellido);
                return Ok(listaSolicitudes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar las solicitudes por nombre o apellido: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }



        [HttpPut("cambiar-estado/{solicitudId}/{nuevoEstado}/{cuenta_vacaciones}/{goce_sueldo}")]
        public async Task<ActionResult<bool>> aprobacion(int solicitudId, string nuevoEstado, bool cuenta_vacaciones, bool goce_sueldo)
        {
            try
            {
                var cambioExitoso = await _solicitudesPermisoD.aprobacion(solicitudId, nuevoEstado,  cuenta_vacaciones,  goce_sueldo);

                if (cambioExitoso)
                {
                    return Ok(true);
                }
                else
                {
                    return StatusCode(500, "Error al cambiar el estado de la solicitud");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar el estado de la solicitud: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }



        [HttpGet("calendarizacion/{usuarioId}")]
        public async Task<IActionResult> ObtenerInformacion(int usuarioId)
        {
            try
            {
                var calendarizacion = await _calendarizacionService.ObtenerInformacionCalendarizacion(usuarioId);
                if (calendarizacion != null)
                {
                    return Ok(calendarizacion);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener información de Calendarizacion: {ex.Message}");
            }
        }


        [HttpPut("calendarizacion/{usuarioId}")]
        public async Task<IActionResult> Actualizar(int usuarioId, [FromBody] CalendarizacionModel calendarizacion)
        {
            try
            {
                bool actualizacionExitosa = await _calendarizacionService.ActualizarCalendarizacion(usuarioId, calendarizacion.dias_totales, calendarizacion.dias_tomados, calendarizacion.dias_restantes);
                if (actualizacionExitosa)
                {
                    return Ok("Actualización exitosa en Calendarizacion");
                }
                else
                {
                    return BadRequest("No se pudo actualizar la información en Calendarizacion");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar la tabla Calendarizacion: {ex.Message}");
            }
        }

        [HttpPost("calendarizacion")]
        public async Task<IActionResult> Insertar([FromBody] CalendarizacionModel calendarizacion)
        {
            try
            {
                bool insercionExitosa = await _calendarizacionService.InsertarEnCalendarizacion(calendarizacion.usuario_id, calendarizacion.dias_totales, calendarizacion.dias_tomados, calendarizacion.dias_restantes);
                if (insercionExitosa)
                {
                    return Ok("Inserción exitosa en Calendarizacion");
                }
                else
                {
                    return BadRequest("No se pudo insertar en la tabla Calendarizacion");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al insertar en la tabla Calendarizacion: {ex.Message}");
            }
        }

    }
}
