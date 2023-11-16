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


        public async Task<bool> InsertarSolicitudPermiso(int usuarioId, string motivo, string tipoPermiso,
                                                         bool cuentaVacaciones, bool cuentaDiasLaborales,
                                                         bool goceSueldo)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                {
                    await sql.OpenAsync();
                    using (var cmd = new SqlCommand(@"INSERT INTO Solicitudes_Permiso 
                                (usuario_id, motivo, estado_aprobacion, cuenta_vacaciones, cuenta_dias_laborales, goce_sueldo, tipo_permiso, fecha_solicitud) 
                                VALUES 
                                (@usuarioId, @motivo, 'en_espera', @cuentaVacaciones, @cuentaDiasLaborales, @goceSueldo, @tipoPermiso, GETDATE())", sql))
                    {
                        cmd.Parameters.AddWithValue("@usuarioId", usuarioId);
                        cmd.Parameters.AddWithValue("@motivo", motivo);
                        cmd.Parameters.AddWithValue("@tipoPermiso", tipoPermiso);
                        cmd.Parameters.AddWithValue("@cuentaVacaciones", cuentaVacaciones);
                        cmd.Parameters.AddWithValue("@cuentaDiasLaborales", cuentaDiasLaborales);
                        cmd.Parameters.AddWithValue("@goceSueldo", goceSueldo);
                        cmd.Parameters.AddWithValue("@tipoPermiso", tipoPermiso);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }





    }

}
