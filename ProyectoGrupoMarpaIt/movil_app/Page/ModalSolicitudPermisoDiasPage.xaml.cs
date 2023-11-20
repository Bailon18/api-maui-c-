using movil_app.Models;
using movil_app.Models.Services;

namespace movil_app.Page;

public partial class ModalSolicitudPermisoDiasPage : ContentPage
{
    UsuarioModel usuariologeado { get; set; }
    private readonly PermisosServices _permisosServices;
    private int _solicitudId; // Identificador de la solicitud, 0 para agregar

    // Constructor que recibe solicitud_id para editar o agregar
    public ModalSolicitudPermisoDiasPage(int solicitud_id = 0)
    {
        InitializeComponent();
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


            LlenarDetallesSolicitud();
        }
    }


    private async void LlenarDetallesSolicitud()
    {
        SolicitudesPermisoModel[] solicitudes = await _permisosServices.ListarSolicitudesPermiso();

        if (solicitudes != null && solicitudes.Any())
        {
            SolicitudesPermisoModel solicitud = solicitudes.FirstOrDefault(s => s.solicitud_id == _solicitudId);

            if (solicitud != null)
            {
                MotivoEditor.Text = solicitud.motivo;
                // Rellenar el valor del check
                CuentaDiasSwitch.IsToggled = solicitud.cuenta_vacaciones; // Asumiendo que cuenta_vacaciones es la propiedad del switch
            }
            else
            {
                Console.WriteLine("No se encontró la solicitud correspondiente.");
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
                FechaFin = FechaFin.Date
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
            // Lógica para actualizar la solicitud con _solicitudId
        }
    }

    private async void CancelarButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
