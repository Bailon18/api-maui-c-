using movil_api.Conexion;
using movil_api.Modelo;
using System.Data.SqlClient;
using System.Data;

namespace movil_api.Datos
{
    public class SolicitudesPermisoD
    {
        ConexionBd cn = new ConexionBd();

        public async Task<List<SolicitudesPermisoModel>> listarSolicitudesPermiso()
        {
            var lista = new List<SolicitudesPermisoModel>();

            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("SELECT * FROM Solicitudes_Permiso", sql))
                {
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var solicitudPermiso = new SolicitudesPermisoModel
                            {
                                solicitud_id = reader.GetInt32(reader.GetOrdinal("solicitud_id")),
                                usuario_id = reader.GetInt32(reader.GetOrdinal("usuario_id")),
                                motivo = reader.GetString(reader.GetOrdinal("motivo")),
                                estado_aprobacion = reader.GetString(reader.GetOrdinal("estado_aprobacion")),
                                cuenta_vacaciones = reader.GetBoolean(reader.GetOrdinal("cuenta_vacaciones")),
                                cuenta_dias_laborales = reader.GetBoolean(reader.GetOrdinal("cuenta_dias_laborales")),
                                goce_sueldo = reader.GetBoolean(reader.GetOrdinal("goce_sueldo")),
                                tipo_permiso = reader.GetString(reader.GetOrdinal("tipo_permiso")),
                                fecha_solicitud = reader.GetDateTime(reader.GetOrdinal("fecha_solicitud"))
                            };

                            lista.Add(solicitudPermiso);
                        }
                    }
                }
            }

            return lista;
        }

        public async Task<List<SolicitudesPermisoModel>> listarSolicitudesPorUsuario(int usuarioId)
        {
            var listaCompleta = await listarSolicitudesPermiso();

            var listaFiltrada = listaCompleta.Where(solicitud => solicitud.usuario_id == usuarioId).ToList();

            return listaFiltrada;
        }

        public async Task<List<SolicitudesPermisoModel>> listarSolicitudesPorEstado(string estado)
        {
            var listaCompleta = await listarSolicitudesPermiso();

            var listaFiltrada = listaCompleta.Where(solicitud => solicitud.estado_aprobacion == estado).ToList();

            return listaFiltrada;
        }

        public async Task<List<SolicitudesPermisoModel>> ObtenerSolicitudesPorDepartamento(int departamentoId)
        {
            var lista = new List<SolicitudesPermisoModel>();

            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new SqlCommand("SELECT SP.solicitud_id, SP.motivo, SP.estado_aprobacion, SP.cuenta_vacaciones, SP.cuenta_dias_laborales, SP.goce_sueldo, SP.tipo_permiso, SP.fecha_solicitud " +
                                                    "FROM Solicitudes_Permiso SP " +
                                                    "INNER JOIN Usuarios U ON SP.usuario_id = U.usuario_id " +
                                                    "WHERE U.departamento_id = @departamentoId", sql))
                    {
                        cmd.Parameters.AddWithValue("@departamentoId", departamentoId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var solicitud = new SolicitudesPermisoModel
                                {
                                    solicitud_id = reader.GetInt32(0),
                                    motivo = reader.GetString(1),
                                    estado_aprobacion = reader.GetString(2),
                                    cuenta_vacaciones = reader.GetBoolean(3),
                                    cuenta_dias_laborales = reader.GetBoolean(4),
                                    goce_sueldo = reader.GetBoolean(5),
                                    tipo_permiso = reader.GetString(6),
                                    fecha_solicitud = reader.GetDateTime(7)
                                };

                                lista.Add(solicitud);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción según sea necesario
            }

            return lista;
        }

        public async Task<bool> insertarSolicitudCompleta(SolicitudCompletaModel solicitudCompletaModel)
        {
            try
            {
                SolicitudesPermisoModel solicitudesPermisoModel = solicitudCompletaModel.Solicitud;

                int solicitudId = await InsertarSolicitudPermiso(solicitudesPermisoModel);

                if (solicitudId > 0)
                {
                    if (solicitudesPermisoModel.tipo_permiso == "Dias")
                    {
                        DetallesSolicitudDiasModel detallesSolicitudDiasModel = solicitudCompletaModel.DetallesDias;
                        detallesSolicitudDiasModel.SolicitudId = solicitudId;

                        bool detallesInsertados = await InsertarDetallesSolicitudDias(detallesSolicitudDiasModel);

            
                        return detallesInsertados;
                    }
                    else if (solicitudesPermisoModel.tipo_permiso == "Horas")
                    {
                        DetallesSolicitudHorasModel detallesSolicitudHorasModel = solicitudCompletaModel.DetallesHoras;
                        detallesSolicitudHorasModel.SolicitudId = solicitudId;

                  
                        bool detallesInsertados = await InsertarDetallesSolicitudHoras(detallesSolicitudHorasModel);

        
                        return detallesInsertados;
                    }
                }
            }
            catch (Exception ex) { 
            
                return false;
            }

            return false;
        }


        public async Task<int> InsertarSolicitudPermiso(SolicitudesPermisoModel solicitud)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("INSERT INTO Solicitudes_Permiso (usuario_id, motivo, cuenta_vacaciones, cuenta_dias_laborales, goce_sueldo, tipo_permiso) VALUES (@usuario_id, @motivo, @cuenta_vacaciones, @cuenta_dias_laborales, @goce_sueldo, @tipo_permiso); SELECT SCOPE_IDENTITY()", sql))
                {
                    await sql.OpenAsync();

                    cmd.Parameters.AddWithValue("@usuario_id", solicitud.usuario_id);
                    cmd.Parameters.AddWithValue("@motivo", solicitud.motivo);
                    cmd.Parameters.AddWithValue("@cuenta_vacaciones", solicitud.cuenta_vacaciones);
                    cmd.Parameters.AddWithValue("@cuenta_dias_laborales", solicitud.cuenta_dias_laborales);
                    cmd.Parameters.AddWithValue("@goce_sueldo", solicitud.goce_sueldo);
                    cmd.Parameters.AddWithValue("@tipo_permiso", solicitud.tipo_permiso);

                    int solicitudId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                    return solicitudId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar solicitud de permiso: {ex.Message}");
                throw;
            }
        }

        private async Task<bool> InsertarDetallesSolicitudDias(DetallesSolicitudDiasModel detallesDias)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("INSERT INTO Detalles_Solicitud_Dias (solicitud_id, fecha_inicio, fecha_fin) VALUES (@solicitud_id, @fecha_inicio, @fecha_fin)", sql))
                {
                    await sql.OpenAsync();

                    cmd.Parameters.AddWithValue("@solicitud_id", detallesDias.SolicitudId);
                    cmd.Parameters.AddWithValue("@fecha_inicio", detallesDias.FechaInicio);
                    cmd.Parameters.AddWithValue("@fecha_fin", detallesDias.FechaFin);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar detalles por días: {ex.Message}");
                throw;
            }
        }

        private async Task<bool> InsertarDetallesSolicitudHoras(DetallesSolicitudHorasModel detallesHoras)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("INSERT INTO Detalles_Solicitud_Horas (solicitud_id, hora_inicio, hora_fin) VALUES (@solicitud_id, @hora_inicio, @hora_fin)", sql))
                {
                    await sql.OpenAsync();

                    cmd.Parameters.AddWithValue("@solicitud_id", detallesHoras.SolicitudId);
                    cmd.Parameters.AddWithValue("@hora_inicio", detallesHoras.HoraInicio);
                    cmd.Parameters.AddWithValue("@hora_fin", detallesHoras.HoraFin);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar detalles por horas: {ex.Message}");
                throw;
            }
        }


        public async Task<bool> EliminarSolicitudPermiso(int id)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("DELETE FROM Solicitudes_Permiso WHERE solicitud_id = @id", sql))
                {
                    await sql.OpenAsync();
                    cmd.Parameters.AddWithValue("@id", id);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar Solicitudes_Permiso: {ex.Message}");
                throw;
            }
        }

    }

}
