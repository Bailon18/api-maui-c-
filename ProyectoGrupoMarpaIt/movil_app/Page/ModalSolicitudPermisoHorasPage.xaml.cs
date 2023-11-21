using movil_app.Models;
using movil_app.Models.Services;

namespace movil_app.Page
{
    public partial class ModalSolicitudPermisoHorasPage : ContentPage
    {
        UsuarioModel usuariologeado { get; set; }
        private readonly PermisosServices _permisosServices;
        private int _solicitudId;

        public ModalSolicitudPermisoHorasPage(int solicitud_id = 0)
        {
            InitializeComponent();

            HoraInicio.Time = new TimeSpan(8, 0, 0); 
            HoraFin.Time = new TimeSpan(18, 0, 0); 


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
                titulo.Text = "Actualizar solicitud por horas";
                BotonAccion.Text = "Actualizar";
                LlenarDetallesSolicitud();
            }
        }


        private async void LlenarDetallesSolicitud()
        {
            SolicitudesPermisoModel[] solicitudes = await _permisosServices.ListarSolicitudesPorUsuario(usuariologeado.usuario_id);

            if (solicitudes != null && solicitudes.Any())
            {
                SolicitudesPermisoModel solicitud = solicitudes.FirstOrDefault(s => s.solicitud_id == _solicitudId);

                DetallesSolicitudHorasModel detalleHoras = await _permisosServices.ObtenerDetallesHorasPorSolicitudId(_solicitudId);

                if (solicitud != null && detalleHoras != null)
                {
                    MotivoEditor.Text = solicitud.motivo;
                    HoraInicio.Time = detalleHoras.HoraInicio;
                    HoraFin.Time = detalleHoras.HoraFin;
                }
                else
                {
                    Console.WriteLine("No se encontró la solicitud correspondiente o los detalles por horas.");
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

            var horaInicio = HoraInicio.Time;
            var horaFin = HoraFin.Time;

            var horaMinima = new TimeSpan(8, 0, 0);
            var horaMaxima = new TimeSpan(18, 0, 0);

            if (horaInicio < horaMinima || horaFin > horaMaxima || horaFin < horaMinima)
            {
                await DisplayAlert("Error", "Las horas de inicio y fin deben estar entre las 8:00 AM y las 6:00 PM.", "OK");
                return;
            }

            SolicitudCompletaModel solicitudCompleta = new SolicitudCompletaModel
            {
                Solicitud = new SolicitudesPermisoModel
                {
                    usuario_id = usuariologeado.usuario_id,
                    motivo = MotivoEditor.Text,
                    tipo_permiso = "Horas"
                },

                DetallesHoras = new DetallesSolicitudHorasModel
                {
                    HoraInicio = HoraInicio.Time,
                    HoraFin = HoraFin.Time
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

        private async void Cancelar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}
