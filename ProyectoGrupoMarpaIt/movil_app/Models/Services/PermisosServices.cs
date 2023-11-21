using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace movil_app.Models.Services
{
    internal class PermisosServices
    {
        private readonly HttpClient _httpClient;

        public PermisosServices()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            _httpClient = new HttpClient(handler);
        }


        public async Task<SolicitudesPermisoModel[]> ListarSolicitudesPermiso()
        {
            try
            {
                string url = "https://10.0.2.2:7121/api/solicitudes-permiso";

                using var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SolicitudesPermisoModel[]>();
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


        public async Task<SolicitudesPermisoModel[]> ListarSolicitudesPorUsuario(int usuarioId)
        {
            try
            {
                string url = $"https://10.0.2.2:7121/api/solicitudes-permiso/usuario/{usuarioId}";

                using var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SolicitudesPermisoModel[]>();
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


        public async Task<SolicitudesPermisoModel[]> ListarSolicitudesPorEstado(string estado)
        {
            try
            {
                string url = $"https://10.0.2.2:7121/api/solicitudes-permiso/estado/{estado}";

                using var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SolicitudesPermisoModel[]>();
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

        public async Task<SolicitudesPermisoModel[]> ObtenerSolicitudesPorDepartamento(int departamentoId)
        {
            try
            {
                string url = $"https://10.0.2.2:7121/api/solicitudes-permiso/departamento/{departamentoId}";

                using var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<SolicitudesPermisoModel[]>();
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

        public async Task<bool> InsertarSolicitudCompleta(SolicitudCompletaModel solicitudCompletaModel)
        {
            try
            {
                string url = "https://10.0.2.2:7121/api/solicitudes-permiso/insertar-solicitud-completa";

                using var response = await _httpClient.PostAsJsonAsync(url, solicitudCompletaModel);

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

        public async Task<bool> EliminarSolicitudPermiso(int id)
        {
            try
            {
                string url = $"https://10.0.2.2:7121/api/solicitudes-permiso/{id}";

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


        public async Task<DetallesSolicitudDiasModel> ObtenerDetallesPorDiasSolicitudId(int solicitudId)
        {
            try
            {
                string url = $"https://10.0.2.2:7121/api/solicitudes-permiso/detallesDias/{solicitudId}";

                using var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<DetallesSolicitudDiasModel>();
                }
                else
                {
                    Console.WriteLine($"Error al hacer la solicitud. Código de estado: {response.StatusCode}");
                    return null;
                }
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


        public async Task<bool> ActualizarSolicitudCompleta(SolicitudCompletaModel solicitudCompletaModel)
        {
            try
            {
                string url = "https://10.0.2.2:7121/api/solicitudes-permiso/actualizar-solicitud-completa";

                using var response = await _httpClient.PutAsJsonAsync(url, solicitudCompletaModel);

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

        public async Task<DetallesSolicitudHorasModel> ObtenerDetallesHorasPorSolicitudId(int solicitudId)
        {
            try
            {
                string url = $"https://10.0.2.2:7121/api/solicitudes-permiso/detallesHoras/{solicitudId}";

                using var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<DetallesSolicitudHorasModel>();
                }
                else
                {
                    Console.WriteLine($"Error al hacer la solicitud. Código de estado: {response.StatusCode}");
                    return null;
                }
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
