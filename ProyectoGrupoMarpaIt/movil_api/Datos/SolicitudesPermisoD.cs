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
                using (var cmd = new SqlCommand("SELECT * FROM Solicitudes_Permiso ORDER BY fecha_solicitud DESC", sql))
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
                    using (var cmd = new SqlCommand("SELECT SP.solicitud_id, SP.motivo, SP.estado_aprobacion, SP.cuenta_vacaciones, SP.cuenta_dias_laborales, SP.goce_sueldo, SP.tipo_permiso, SP.fecha_solicitud, U.usuario_id " +
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
                                    fecha_solicitud = reader.GetDateTime(7),
                                    usuario_id = reader.GetInt32(8)
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
                using (var cmd = new SqlCommand("INSERT INTO Detalles_Solicitud_Dias (solicitud_id, fecha_inicio, fecha_fin, dias_permiso) VALUES (@solicitud_id, @fecha_inicio, @fecha_fin, @dias_permiso)", sql))
                {
                    await sql.OpenAsync();

                    cmd.Parameters.AddWithValue("@solicitud_id", detallesDias.SolicitudId);
                    cmd.Parameters.AddWithValue("@fecha_inicio", detallesDias.FechaInicio);
                    cmd.Parameters.AddWithValue("@fecha_fin", detallesDias.FechaFin);
                    cmd.Parameters.AddWithValue("@dias_permiso", detallesDias.DiasPermisos);

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
                using (var cmd = new SqlCommand("INSERT INTO Detalles_Solicitud_Horas (solicitud_id, hora_inicio, hora_fin, fecha_permiso) VALUES (@solicitud_id, @hora_inicio, @hora_fin, @fecha_permiso)", sql))
                {
                    await sql.OpenAsync();

                    cmd.Parameters.AddWithValue("@solicitud_id", detallesHoras.SolicitudId);
                    cmd.Parameters.AddWithValue("@hora_inicio", detallesHoras.HoraInicio);
                    cmd.Parameters.AddWithValue("@hora_fin", detallesHoras.HoraFin);
                    cmd.Parameters.AddWithValue("@fecha_permiso", detallesHoras.FechaPermiso);

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


        public async Task<DetallesSolicitudDiasModel> ObtenerDetallesPorSolicitudId(int solicitudId)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                {
                    await sql.OpenAsync();

                    using (var cmd = new SqlCommand("SELECT * FROM Detalles_Solicitud_Dias WHERE solicitud_id = @solicitudId", sql))
                    {
                        cmd.Parameters.AddWithValue("@solicitudId", solicitudId);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var detallesSolicitud = new DetallesSolicitudDiasModel
                                {
                                    SolicitudId = reader.GetInt32(reader.GetOrdinal("solicitud_id")),
                                    FechaInicio = reader.GetDateTime(reader.GetOrdinal("fecha_inicio")),
                                    FechaFin = reader.GetDateTime(reader.GetOrdinal("fecha_fin")),
                                    DiasPermisos = reader.GetInt32(reader.GetOrdinal("dias_permiso"))
                                };

                                return detallesSolicitud;
                            }
                            else
                            {
                                Console.WriteLine("No se encontraron detalles para la solicitud especificada.");
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalles por solicitud_id: {ex.Message}");
                throw;
            }
        }


        public async Task<DetallesSolicitudHorasModel> ObtenerDetallesHorasPorSolicitudId(int solicitudId)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                {
                    await sql.OpenAsync();

                    using (var sqlCommand = new SqlCommand("SELECT * FROM Detalles_Solicitud_Horas WHERE solicitud_id = @solicitudId", sql))
                    {
                        sqlCommand.Parameters.AddWithValue("@solicitudId", solicitudId);

                        using (var reader = await sqlCommand.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var detallesSolicitud = new DetallesSolicitudHorasModel
                                {
                                    SolicitudId = reader.GetInt32(reader.GetOrdinal("solicitud_id")),
                                    HoraInicio = reader.GetTimeSpan(reader.GetOrdinal("hora_inicio")),
                                    HoraFin = reader.GetTimeSpan(reader.GetOrdinal("hora_fin")),
                                    FechaPermiso = reader.GetDateTime(reader.GetOrdinal("fecha_permiso"))
                                };

                                return detallesSolicitud;
                            }
                            else
                            {
                                Console.WriteLine("No se encontraron detalles para la solicitud especificada.");
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener detalles por solicitud_id: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ActualizarSolicitudCompleta(SolicitudCompletaModel solicitudCompletaModel)
        {
            try
            {
                SolicitudesPermisoModel solicitud = solicitudCompletaModel.Solicitud;

                bool solicitudActualizada = await ActualizarSolicitudPermiso(solicitud);

                if (solicitudActualizada)
                {
                    if (solicitud.tipo_permiso == "Dias")
                    {
                        DetallesSolicitudDiasModel detallesDias = solicitudCompletaModel.DetallesDias;
                        detallesDias.SolicitudId = solicitud.solicitud_id;

                        return await ActualizarDetallesSolicitudDias(detallesDias);
                    }
                    else if (solicitud.tipo_permiso == "Horas")
                    {
                        DetallesSolicitudHorasModel detallesHoras = solicitudCompletaModel.DetallesHoras;
                        detallesHoras.SolicitudId = solicitud.solicitud_id;

                        return await ActualizarDetallesSolicitudHoras(detallesHoras);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar solicitud completa: {ex.Message}");
                throw;
            }

            return false;
        }

        public async Task<bool> ActualizarSolicitudPermiso(SolicitudesPermisoModel solicitud)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("UPDATE Solicitudes_Permiso SET usuario_id = @usuario_id, motivo = @motivo, cuenta_vacaciones = @cuenta_vacaciones, cuenta_dias_laborales = @cuenta_dias_laborales, goce_sueldo = @goce_sueldo, tipo_permiso = @tipo_permiso WHERE solicitud_id = @solicitud_id", sql))
                {
                    await sql.OpenAsync();

                    cmd.Parameters.AddWithValue("@solicitud_id", solicitud.solicitud_id);
                    cmd.Parameters.AddWithValue("@usuario_id", solicitud.usuario_id);
                    cmd.Parameters.AddWithValue("@motivo", solicitud.motivo);
                    cmd.Parameters.AddWithValue("@cuenta_vacaciones", solicitud.cuenta_vacaciones);
                    cmd.Parameters.AddWithValue("@cuenta_dias_laborales", solicitud.cuenta_dias_laborales);
                    cmd.Parameters.AddWithValue("@goce_sueldo", solicitud.goce_sueldo);
                    cmd.Parameters.AddWithValue("@tipo_permiso", solicitud.tipo_permiso);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar solicitud de permiso: {ex.Message}");
                throw;
            }
        }

        private async Task<bool> ActualizarDetallesSolicitudDias(DetallesSolicitudDiasModel detallesDias)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("UPDATE Detalles_Solicitud_Dias SET fecha_inicio = @fecha_inicio, fecha_fin = @fecha_fin, dias_permiso = @dias_permiso WHERE solicitud_id = @solicitud_id", sql))
                {
                    await sql.OpenAsync();

                    cmd.Parameters.AddWithValue("@solicitud_id", detallesDias.SolicitudId);
                    cmd.Parameters.AddWithValue("@fecha_inicio", detallesDias.FechaInicio);
                    cmd.Parameters.AddWithValue("@fecha_fin", detallesDias.FechaFin);
                    cmd.Parameters.AddWithValue("@dias_permiso", detallesDias.DiasPermisos);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar detalles por días: {ex.Message}");
                throw;
            }
        }

        private async Task<bool> ActualizarDetallesSolicitudHoras(DetallesSolicitudHorasModel detallesHoras)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("UPDATE Detalles_Solicitud_Horas SET hora_inicio = @hora_inicio, hora_fin = @hora_fin, fecha_permiso = @fecha_permiso WHERE solicitud_id = @solicitud_id", sql))
                {
                    await sql.OpenAsync();

                    cmd.Parameters.AddWithValue("@solicitud_id", detallesHoras.SolicitudId);
                    cmd.Parameters.AddWithValue("@hora_inicio", detallesHoras.HoraInicio);
                    cmd.Parameters.AddWithValue("@hora_fin", detallesHoras.HoraFin);
                    cmd.Parameters.AddWithValue("@fecha_permiso", detallesHoras.FechaPermiso);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar detalles por horas: {ex.Message}");
                throw;
            }
        }

        public async Task<List<SolicitudesPermisoModel>> BuscarSolicitudesPorNombreOApellido(string nombreOApellido)
        {
            var lista = new List<SolicitudesPermisoModel>();

            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                {
                    await sql.OpenAsync();

                    using (var cmd = new SqlCommand("SELECT SP.solicitud_id, SP.usuario_id, SP.motivo, SP.estado_aprobacion, SP.cuenta_vacaciones, SP.cuenta_dias_laborales, SP.goce_sueldo, SP.tipo_permiso, SP.fecha_solicitud " +
                                                    "FROM Solicitudes_Permiso SP " +
                                                    "INNER JOIN Usuarios U ON SP.usuario_id = U.usuario_id " +
                                                    "WHERE U.nombre LIKE @nombreOApellido OR U.apellidos LIKE @nombreOApellido", sql))
                    {
                        cmd.Parameters.AddWithValue("@nombreOApellido", "%" + nombreOApellido + "%");

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var solicitudPermiso = new SolicitudesPermisoModel
                                {
                                    solicitud_id = reader.GetInt32(0),
                                    usuario_id = reader.GetInt32(1),
                                    motivo = reader.GetString(2),
                                    estado_aprobacion = reader.GetString(3),
                                    cuenta_vacaciones = reader.GetBoolean(4),
                                    cuenta_dias_laborales = reader.GetBoolean(5),
                                    goce_sueldo = reader.GetBoolean(6),
                                    tipo_permiso = reader.GetString(7),
                                    fecha_solicitud = reader.GetDateTime(8)
                                };

                                lista.Add(solicitudPermiso);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción según sea necesario
                Console.WriteLine($"Error al buscar solicitudes por nombre o apellido: {ex.Message}");
                throw;
            }

            return lista;
        }


        public async Task<bool> aprobacion(int solicitudId, string nuevoEstado, bool cuenta_vacaciones, bool goce_sueldo)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("UPDATE Solicitudes_Permiso SET estado_aprobacion = @nuevoEstado, cuenta_vacaciones = @cuenta_vacaciones, goce_sueldo = @goce_sueldo  WHERE solicitud_id = @solicitudId", sql))
                {
                    await sql.OpenAsync();

                    cmd.Parameters.AddWithValue("@solicitudId", solicitudId);
                    cmd.Parameters.AddWithValue("@nuevoEstado", nuevoEstado);
                    cmd.Parameters.AddWithValue("@cuenta_vacaciones", cuenta_vacaciones);
                    cmd.Parameters.AddWithValue("@goce_sueldo", goce_sueldo);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar el estado de la solicitud: {ex.Message}");
                throw;
            }
        }

        public async Task<CalendarizacionModel> ObtenerInformacionCalendarizacion(int usuarioId)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                {
                    await sql.OpenAsync();

                    using (var cmd = new SqlCommand("SELECT * FROM Calendarizacion WHERE usuario_id = @usuarioId", sql))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var calendarizacion = new CalendarizacionModel
                                {
                                    usuario_id = reader.GetInt32(reader.GetOrdinal("usuario_id")),
                                    dias_totales = reader.GetInt32(reader.GetOrdinal("dias_totales")),
                                    dias_tomados = reader.GetInt32(reader.GetOrdinal("dias_tomados")),
                                    dias_restantes = reader.GetInt32(reader.GetOrdinal("dias_restantes"))
                                };

                                return calendarizacion;
                            }
                            else
                            {
                                Console.WriteLine("No se encontró información en la tabla Calendarizacion para este usuario.");
                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener información de Calendarizacion por usuario_id: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ActualizarCalendarizacion(int usuarioId, int diasTotales, int diasTomados, int diasRestantes)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("UPDATE Calendarizacion SET dias_totales = @diasTotales, dias_tomados = @diasTomados, dias_restantes = @diasRestantes WHERE usuario_id = @usuarioId", sql))
                {
                    await sql.OpenAsync();

                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                    cmd.Parameters.AddWithValue("@diasTotales", diasTotales);
                    cmd.Parameters.AddWithValue("@diasTomados", diasTomados);
                    cmd.Parameters.AddWithValue("@diasRestantes", diasRestantes);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la tabla Calendarizacion: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> InsertarEnCalendarizacion(int usuarioId, int diasTotales, int diasTomados, int diasRestantes)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("INSERT INTO Calendarizacion (usuario_id, dias_totales, dias_tomados, dias_restantes) VALUES (@usuarioId, @diasTotales, @diasTomados, @diasRestantes)", sql))
                {
                    await sql.OpenAsync();

                    cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                    cmd.Parameters.AddWithValue("@diasTotales", diasTotales);
                    cmd.Parameters.AddWithValue("@diasTomados", diasTomados);
                    cmd.Parameters.AddWithValue("@diasRestantes", diasRestantes);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar en la tabla Calendarizacion: {ex.Message}");
                throw;
            }
        }

    }

}
