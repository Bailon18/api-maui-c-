using movil_app.Models;
using movil_app.Models.Services;

namespace movil_app.Page;

public partial class ModalSolicitudPermisoDiasPage : ContentPage
{
    UsuarioModel usuariologeado { get; set; }
    private readonly PermisosServices _permisosServices;
    private int _solicitudId;

    public ModalSolicitudPermisoDiasPage(int solicitud_id = 0)
    {
        InitializeComponent();


        FechaInicio.DateSelected += CalcularDiasPermiso;
        FechaFin.DateSelected += CalcularDiasPermiso;


        _permisosServices = new PermisosServices();
        _solicitudId = solicitud_id;

        if (Application.Current.MainPage is AppShell appShell)
        {
            usuariologeado = appShell.getUsuarioLogeado();

            NombreEntry.Text = usuariologeado.nombre + " " + usuariologeado.apellidos;
            NumeroEmpleadoEntry.Text = usuariologeado.codigoempleado;
            DepartamentoEntry.Text = usuariologeado.nombre_departamento;
        }

        if (_solicitudId != 0)
        {
            titulo.Text = "Actualizar solicitud por dias";
            BotonAccion.Text = "Actualizar";
            LlenarDetallesSolicitud();
        }
    }

    private void CalcularDiasPermiso(object sender, DateChangedEventArgs e)
    {
        if (FechaInicio.Date == default || FechaFin.Date == default)
        {
            return; 
        }

        if (FechaInicio.Date > FechaFin.Date)
        {
            DisplayAlert("Error", "La fecha de inicio no puede ser mayor que la fecha final", "OK");
            FechaInicio.Date = e.OldDate;
            return;
        }

        if (FechaInicio.Date.DayOfWeek == DayOfWeek.Saturday || FechaInicio.Date.DayOfWeek == DayOfWeek.Sunday ||
            FechaFin.Date.DayOfWeek == DayOfWeek.Saturday || FechaFin.Date.DayOfWeek == DayOfWeek.Sunday)
        {
            DisplayAlert("Mensaje", "Por favor, elige días laborables (de lunes a viernes).", "OK");
            FechaInicio.Date = e.OldDate; 
            FechaFin.Date = e.OldDate;
            return; 
        }

   
        TimeSpan diferencia = FechaFin.Date - FechaInicio.Date;
        int diasDeDiferencia = diferencia.Days;

        DiasPermisoEntry.Text = diasDeDiferencia.ToString();
    }



    private async void LlenarDetallesSolicitud()
    {
        SolicitudesPermisoModel[] solicitudes = await _permisosServices.ListarSolicitudesPermiso();

        if (solicitudes != null && solicitudes.Any())
        {
            SolicitudesPermisoModel solicitud = solicitudes.FirstOrDefault(s => s.solicitud_id == _solicitudId);

            DetallesSolicitudDiasModel detalledias = await _permisosServices.ObtenerDetallesPorDiasSolicitudId(_solicitudId);


            if (solicitud != null && detalledias != null)
            {
                MotivoEditor.Text = solicitud.motivo;
                CuentaDiasSwitch.IsToggled = solicitud.cuenta_vacaciones;
                FechaInicio.Date = detalledias.FechaInicio;
                FechaFin.Date = detalledias.FechaFin;
                DiasPermisoEntry.Text = detalledias.DiasPermisos.ToString();

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


    private async void BotonAccion_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(MotivoEditor.Text))
        {
            await DisplayAlert("Error", "El motivo no puede estar vacío.", "OK");
            return;
        }

        if (FechaInicio.Date > FechaFin.Date)
        {
            await DisplayAlert("Error", "La fecha de inicio debe ser anterior a la fecha de fin.", "OK");
            return;
        }

        SolicitudCompletaModel solicitudCompleta = new SolicitudCompletaModel
        {
            Solicitud = new SolicitudesPermisoModel
            {
                usuario_id = usuariologeado.usuario_id,
                motivo = MotivoEditor.Text,
                cuenta_vacaciones = CuentaDiasSwitch.IsToggled,
                tipo_permiso = "Dias"
            },

            DetallesDias = new DetallesSolicitudDiasModel
            {
                FechaInicio = FechaInicio.Date,
                FechaFin = FechaFin.Date,
                DiasPermisos = int.Parse(DiasPermisoEntry.Text)
            }
        };

        if (_solicitudId == 0)
        {
            // Si _solicitudId es cero, es una solicitud nueva
            bool solicitudGuardada = await _permisosServices.InsertarSolicitudCompleta(solicitudCompleta);

            if (solicitudGuardada)
            {
                await DisplayAlert("Éxito", "La solicitud ha sido guardada correctamente.", "OK");
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo guardar la solicitud. Inténtalo de nuevo.", "OK");
            }
        }
        else
        {
            // Si _solicitudId tiene valor, es una solicitud para actualizar
            solicitudCompleta.Solicitud.solicitud_id = _solicitudId;

            bool solicitudActualizada = await _permisosServices.ActualizarSolicitudCompleta(solicitudCompleta);

            if (solicitudActualizada)
            {
                await DisplayAlert("Éxito", "La solicitud ha sido actualizada correctamente.", "OK");
                await Navigation.PopModalAsync();
            }
            else
            {
                await DisplayAlert("Error", "No se pudo actualizar la solicitud. Inténtalo de nuevo.", "OK");
            }
        }
    }



    private async void CancelarButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
