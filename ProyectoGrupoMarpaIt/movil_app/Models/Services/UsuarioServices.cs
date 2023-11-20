using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace movil_app.Models.Services
{
    internal class UsuarioServices : IDisposable
    {
        private readonly HttpClient _httpClient;

        public UsuarioServices()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            _httpClient = new HttpClient(handler);
        }

        public async Task<UsuarioModel[]> ListarUsuarios()
        {
            try
            {
                string url = "https://10.0.2.2:7121/api/usuarios";

                using var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UsuarioModel[]>();
                }
                else
                {
                    Console.WriteLine($"Error al hacer la solicitud. Código de estado: {response.StatusCode}");
                }
                return null;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de solicitud HTTP: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return null;
            }
        }


        public async Task<bool> InsertarUsuario(UsuarioModel usuario)
        {
            try
            {
                string url = "https://10.0.2.2:7121/api/usuarios"; // Endpoint del API para insertar usuarios

                using var response = await _httpClient.PostAsJsonAsync(url, usuario);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine($"Error al hacer la solicitud. Código de estado: {response.StatusCode}");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de solicitud HTTP: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return false;
            }
        }


        public async Task<UsuarioModel> ObtenerUsuarioPorId(int usuarioId)
        {
            try
            {
                string url = $"https://10.0.2.2:7121/api/usuarios/{usuarioId}";

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UsuarioModel>();
                }
                else
                {
                    Console.WriteLine($"Error al obtener el usuario. Código de estado: {response.StatusCode}");
                }
                return null;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de solicitud HTTP: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return null;
            }
        }


        public async Task<bool> ActualizarUsuario(UsuarioModel usuario)
        {
            try
            {
                string url = $"https://10.0.2.2:7121/api/usuarios/{usuario.usuario_id}";

                using var response = await _httpClient.PutAsJsonAsync(url, usuario);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine($"Error al hacer la solicitud. Código de estado: {response.StatusCode}");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de solicitud HTTP: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> EliminarUsuario(int id)
        {
            try
            {
                string url = $"https://10.0.2.2:7121/api/usuarios/{id}";

                var response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    Console.WriteLine($"Error al hacer la solicitud. Código de estado: {response.StatusCode}");
                    return false;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error de solicitud HTTP: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                return false;
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
