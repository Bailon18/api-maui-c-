using movil_app.Models;
using movil_app.Models.Services;
using System.Collections.ObjectModel;

namespace movil_app.Page;

public partial class PermisosPage : ContentPage
{

    private readonly PermisosServices _permisosServices;
    public ObservableCollection<SolicitudesPermisoModel> SolictudPermisos { get; set; }

    UsuarioModel usuariologeado { get; set; }

    public PermisosPage()
	{
		InitializeComponent();
        _permisosServices = new PermisosServices();
        SolictudPermisos = new ObservableCollection<SolicitudesPermisoModel>();
        SolicitudCollectionView.ItemsSource = SolictudPermisos;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CargarSolicitud();
    }

    private async Task CargarSolicitud()
    {
        if (Application.Current.MainPage is AppShell appShell)
        {

            usuariologeado = appShell.getUsuarioLogeado();

        }

        var solicitudes = await _permisosServices.ListarSolicitudesPorUsuario(usuariologeado.usuario_id);

        if (solicitudes != null)
        {
            SolictudPermisos.Clear();
            foreach (var permiso in solicitudes)
            {
                SolictudPermisos.Add(permiso);
            }
        }
        else
        {
            Console.WriteLine("No se pudieron cargar los usuarios.");
        }
    }

    private async void AgregarSolicitudDia_Clicked(object sender, EventArgs e)
    {
        var modalPage = new ModalSolicitudPermisoDiasPage();
        await Navigation.PushModalAsync(modalPage);
    }

    private async void AgregarSolicitudHora_Clicked(object sender, EventArgs e)
    {
        var modalPage = new ModalSolicitudPermisoHorasPage();
        await Navigation.PushModalAsync(modalPage);
    }

    private async void EditarButton_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is SolicitudesPermisoModel solicitud)
        {
            int solicitudId = solicitud.solicitud_id;

            if (solicitud.tipo_permiso == "Horas")
            {
                var modalPageHoras = new ModalSolicitudPermisoHorasPage(solicitudId);
                await Navigation.PushModalAsync(modalPageHoras);
            }
            else
            {
                var modalPageDias = new ModalSolicitudPermisoDiasPage(solicitudId);
                await Navigation.PushModalAsync(modalPageDias);
            }

        }
    }


    private void EliminarButton_Clicked(object sender, EventArgs e)
    {

    }


}