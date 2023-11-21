
using movil_api.Conexion;
using movil_api.Modelo;
using System.Data;
using System.Data.SqlClient;



namespace movil_api.Datos
{
    public class UsuarioD
    {



        ConexionBd cn = new ConexionBd();

        public async Task<List<UsuarioModel>> listarUsuarios() {

            var lista = new List<UsuarioModel>();

            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("ListarUsuarios", sql))
                {
                    await sql.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var item = await cmd.ExecuteReaderAsync())
                    {
                        while (await item.ReadAsync()) {

                            var usuariomodel = new UsuarioModel();

                            usuariomodel.usuario_id = (int)item["usuario_id"];
                            usuariomodel.nombre = (string)item["nombre"];
                            usuariomodel.apellidos = (string)item["apellidos"];
                            usuariomodel.codigoempleado = (string)item["codigoempleado"];
                            usuariomodel.correo = (string)item["correo"];
                            usuariomodel.fecha_ingreso = (DateTime)item["fecha_ingreso"];
                            usuariomodel.estado = (string)item["estado"];
                            usuariomodel.nombre_rol = (string)item["nombre_rol"];
                            usuariomodel.nombre_departamento = (string)item["nombre_departamento"];
                            lista.Add(usuariomodel);

                        }
                    }
                }
            }

            return lista;
        }


        public async Task<UsuarioModel> ValidarCredenciales(string correo, string contrasena)
        {
            UsuarioModel usuario = null;

            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("ValidarCredenciales", sql))
                {
                    await sql.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@correo", correo);
                    cmd.Parameters.AddWithValue("@contrasena", contrasena);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            usuario = new UsuarioModel
                            {
                                usuario_id = (int)reader["usuario_id"],
                                nombre = (string)reader["nombre"],
                                apellidos = (string)reader["apellidos"],
                                correo = (string)reader["correo"],
                                estado = (string)reader["estado"],
                                codigoempleado = (string)reader["codigoempleado"],
                                fecha_ingreso = (DateTime)reader["fecha_ingreso"],
                                nombre_rol = (string)reader["rol"],
                                nombre_departamento = (string)reader["departamento"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al validar credenciales: {ex.Message}");
                throw;
            }

            return usuario;
        }


        public async Task<UsuarioModel> BuscarPorId(int id)
        {
            UsuarioModel usuario = null;

            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("SELECT U.usuario_id, U.nombre, U.apellidos,  U.codigoempleado, U.contrasena, U.correo, U.estado, U.fecha_ingreso, R.nombre_rol AS rol, D.nombre_departamento AS departamento FROM Usuarios U INNER JOIN Roles R ON U.rol_id = R.rol_id INNER JOIN Departamentos D ON U.departamento_id = D.departamento_id WHERE U.usuario_id = @id", sql))
                {
                    await sql.OpenAsync();
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            usuario = new UsuarioModel
                            {
                                usuario_id = (int)reader["usuario_id"],
                                nombre = (string)reader["nombre"],
                                apellidos = (string)reader["apellidos"],
                                correo = (string)reader["correo"],
                                estado = (string)reader["estado"],
                                codigoempleado = (string)reader["codigoempleado"],
                                contrasena = (string)reader["contrasena"],
                                fecha_ingreso = (DateTime)reader["fecha_ingreso"],
                                nombre_rol = (string)reader["rol"],
                                nombre_departamento = (string)reader["departamento"]
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar usuario por ID: {ex.Message}");
                throw;
            }

            return usuario;
        }



        public async Task<bool> InsertarUsuario(UsuarioModel usuario)
        {
            try
            {
          
                int rolId = await ObtenerRolIdPorNombre(usuario.nombre_rol);
                int departamentoId = await ObtenerDepartamentoIdPorNombre(usuario.nombre_departamento);

                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("INSERT INTO Usuarios (correo, nombre, apellidos, contrasena, codigoempleado, estado, rol_id, departamento_id, fecha_ingreso) VALUES (@correo, @nombre, @apellidos, @contrasena, @codigoempleado, @estado, @rol_id, @departamento_id, @fecha_ingreso)", sql))
                {
                    await sql.OpenAsync();

                    cmd.Parameters.AddWithValue("@correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@apellidos", usuario.apellidos);
                    cmd.Parameters.AddWithValue("@contrasena", usuario.contrasena);
                    cmd.Parameters.AddWithValue("@codigoempleado", usuario.codigoempleado);
                    cmd.Parameters.AddWithValue("@estado", usuario.estado);
                    cmd.Parameters.AddWithValue("@rol_id", rolId);
                    cmd.Parameters.AddWithValue("@departamento_id", departamentoId);
                    cmd.Parameters.AddWithValue("@fecha_ingreso", usuario.fecha_ingreso);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar usuario: {ex.Message}");
                throw;
            }
        }


        public async Task<bool> ActualizarUsuario(UsuarioModel usuario)
        {
            try
            {
                // Obtener los IDs de rol y departamento antes de ejecutar la consulta
                int rolId = await ObtenerRolIdPorNombre(usuario.nombre_rol);
                int departamentoId = await ObtenerDepartamentoIdPorNombre(usuario.nombre_departamento);

                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("UPDATE Usuarios SET correo = @correo, nombre = @nombre, apellidos = @apellidos, contrasena = @contrasena, codigoempleado = @codigoempleado, estado = @estado, rol_id = @rol_id, departamento_id = @departamento_id, fecha_ingreso = @fecha_ingreso WHERE usuario_id = @id", sql))
                {
                    await sql.OpenAsync();
                    cmd.Parameters.AddWithValue("@id", usuario.usuario_id);
                    cmd.Parameters.AddWithValue("@correo", usuario.correo);
                    cmd.Parameters.AddWithValue("@nombre", usuario.nombre);
                    cmd.Parameters.AddWithValue("@apellidos", usuario.apellidos);
                    cmd.Parameters.AddWithValue("@contrasena", usuario.contrasena);
                    cmd.Parameters.AddWithValue("@codigoempleado", usuario.codigoempleado);
                    cmd.Parameters.AddWithValue("@estado", usuario.estado);
                    cmd.Parameters.AddWithValue("@rol_id", rolId);
                    cmd.Parameters.AddWithValue("@departamento_id", departamentoId);
                    cmd.Parameters.AddWithValue("@fecha_ingreso", usuario.fecha_ingreso);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar usuario: {ex.Message}");
                throw;
            }
        }


        public async Task<int> ObtenerRolIdPorNombre(string nombreRol)
        {
            int rolId = 0;

            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("SELECT rol_id FROM Roles WHERE nombre_rol = @nombreRol", sql))
                {
                    await sql.OpenAsync();
                    cmd.Parameters.AddWithValue("@nombreRol", nombreRol);

                    object result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                    {
                        rolId = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener ID del rol por nombre: {ex.Message}");
                throw;
            }

            return rolId;
        }

        public async Task<int> ObtenerDepartamentoIdPorNombre(string nombreDepartamento)
        {
            int departamentoId = 0;

            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("SELECT departamento_id FROM Departamentos WHERE nombre_departamento = @nombreDepartamento", sql))
                {
                    await sql.OpenAsync();
                    cmd.Parameters.AddWithValue("@nombreDepartamento", nombreDepartamento);

                    object result = await cmd.ExecuteScalarAsync();
                    if (result != null)
                    {
                        departamentoId = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener ID del departamento por nombre: {ex.Message}");
                throw;
            }

            return departamentoId;
        }



        public async Task<bool> EliminarUsuario(int id)
        {
            try
            {
                using (var sql = new SqlConnection(cn.cadenaSQL()))
                using (var cmd = new SqlCommand("DELETE FROM Usuarios WHERE usuario_id = @id", sql))
                {
                    await sql.OpenAsync();
                    cmd.Parameters.AddWithValue("@id", id);

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar usuario: {ex.Message}");
                throw;
            }
        }

    }
}
