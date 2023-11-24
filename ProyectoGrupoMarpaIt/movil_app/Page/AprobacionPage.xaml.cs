using movil_api.Modelo;
using movil_app.Models;
using movil_app.Models.Services;
using System.Linq;

namespace movil_app.Page;

public partial class AprobacionPage : ContentPage
{
    private readonly UsuarioServices _usuarioServices;
    private readonly PermisosServices _permisosServices;
    private int _solicitudId;

    public AprobacionPage(int solicitud_id = 0)
	{
		InitializeComponent();

        _usuarioServices = new UsuarioServices();
        _permisosServices = new PermisosServices();
        _solicitudId = solicitud_id;

        LlenarDetallesSolicitud();
        
    }

    private async void LlenarDetallesSolicitud()
    {
        SolicitudesPermisoModel[] solicitudes = await _permisosServices.ListarSolicitudesPermiso();
        UsuarioModel[] usuarios = await _usuarioServices.ListarUsuarios();

        if (solicitudes != null && solicitudes.Any())
        {

            
            SolicitudesPermisoModel solicitud = solicitudes.FirstOrDefault(s => s.solicitud_id == _solicitudId);

            UsuarioModel usuarioaprobacion = usuarios.FirstOrDefault(u => u.usuario_id == solicitud.usuario_id);

            DetallesSolicitudDiasModel detalledias = await _permisosServices.ObtenerDetallesPorDiasSolicitudId(_solicitudId);


            if (solicitud != null && detalledias != null )
            {

                MotivoEditor.Text = solicitud.motivo;
                CuentaDiasSwitch.IsToggled = solicitud.cuenta_vacaciones;
                FechaInicio.Date = detalledias.FechaInicio;
                FechaFin.Date = detalledias.FechaFin;
                DiasPermisoEntry.Text = detalledias.DiasPermisos.ToString();
                GoceSueldoSwitch.IsToggled = solicitud.goce_sueldo;

                NombreEntry.Text = usuarioaprobacion.nombre + " " + usuarioaprobacion.apellidos;
                DepartamentoEntry.Text = usuarioaprobacion.nombre_departamento;

            }
            else
            {
                Console.WriteLine("No se encontró la solicitud correspondiente o los detalles por días.");
            }
        }
        else
        {
            Console.WriteLine("No se pudieron obtener las solicitudes.");
        }
    }

    private void BotonAccion_Clicked(object sender, EventArgs e)
    {

    }

    private async void AprobarButton_Clicked(object sender, EventArgs e)
    {
        var estado = "";

        if (sender is Button button)
        {
            if (button == BotonAprobar)
            {
                estado = "Aprobado";
            }
            else if (button == BotonRechazar)
            {
                estado = "Rechazado";
            }
        }

        var cuenta_vacaciones = CuentaDiasSwitch.IsToggled;
        var goce_sueldo = GoceSueldoSwitch.IsToggled;
        SolicitudesPermisoModel solicitud = solicitudes.FirstOrDefault(s => s.solicitud_id == _solicitudId);
        var usuario_id = solicitud.usuario_id;

        bool cambioExitoso = await _permisosServices.CambiarEstadoSolicitud(_solicitudId, estado, cuenta_vacaciones, goce_sueldo);

        try
        {
            int diasSolicitados = DiasPermisoEntry.Text;

            if (diasSolicitados > 15)
            {
                await DisplayAlert("Error", "La cantidad de días solicitados no puede ser mayor a 15.", "OK");
                return;
            }

            CalendarizacionModel calendarizacion = await _permisosServices.ObtenerInformacionCalendarizacion(usuario_id);

            if (calendarizacion != null)
            {

                var calendarizacion = new CalendarizacionModel
                {
                    int diasTotales = calendarizacion.dias_totales;
                    int diasTomados = calendarizacion.dias_tomados + diasSolicitados;
                    int diasRestantes = diasTotales - diasTomados;
                };



                bool actualizacionExitosa = await _permisosServices.ActualizarCalendarizacion(usuario_id, calendarizacion);

                if (actualizacionExitosa)
                {
                    await DisplayAlert("Éxito", $"El estado de la solicitud con ID {_solicitudId} se ha cambiado a {estado}. La tabla Calendarizacion se ha actualizado.", "OK");
                    await Navigation.PopModalAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Error al cambiar el estado de la solicitud o actualizar la tabla Calendarizacion.", "OK");
                }
            }
            else
            {
                int diasTotales = 15;
                int diasTomados = diasSolicitados;
                int diasRestantes = diasTotales - diasTomados;

                bool insercionExitosa = await InsertarEnCalendarizacion(usuario_id, diasTotales, diasTomados, diasRestantes);

                if (insercionExitosa)
                {
                    await DisplayAlert("Éxito", $"El estado de la solicitud con ID {_solicitudId} se ha cambiado a {estado}. Se ha creado un nuevo registro en la tabla Calendarizacion.", "OK");
                    await Navigation.PopModalAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Error al cambiar el estado de la solicitud o insertar en la tabla Calendarizacion.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            await DisplayAlert("Error", "Ocurrió un error al procesar la solicitud.", "OK");
        }

        if (cambioExitoso)
        {
            await DisplayAlert("Éxito", $"El estado de la solicitud con ID {_solicitudId} se ha cambiado a {estado}.", "OK");
            await Navigation.PopModalAsync();
        }
        else
        {
            await DisplayAlert("Error", "Error al cambiar el estado de la solicitud.", "OK");
        }
    }




    private async void CancelarButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}