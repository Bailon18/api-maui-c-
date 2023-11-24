namespace movil_app.Page;

public partial class PerfilPage : ContentPage
{
	public PerfilPage()
	{
        InitializeComponent();

        if (Application.Current.MainPage is AppShell appShell)
        {
            var usuario = appShell.getUsuarioLogeado();
            usuario.NombreCompleto = usuario.nombre + " " + usuario.apellidos;
            BindingContext = usuario;
        }
    }
}