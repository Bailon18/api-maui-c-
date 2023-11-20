using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using movil_app.Models;

namespace movil_app.Models.Services
{
    internal class ValidacionServices : IDisposable
    {
        private readonly HttpClient _httpClient;

        public ValidacionServices()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            _httpClient = new HttpClient(handler);
        }

        public async Task<UsuarioModel> ValidarCredenciales(CredencialesModel credenciales)
        {
            try
            {
                string url = "https://10.0.2.2:7121/api/usuarios/validar";

                using var response = await _httpClient.PostAsJsonAsync(url, credenciales);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UsuarioModel>();
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



        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
