using movil_app.Page;


namespace movil_app
{
    public partial class AppShell : Shell
    {

        bool IsLoggedIn = false;

        public AppShell()
        {
            InitializeComponent();
        }

        public void menuuu(bool autenticacion) {

            IsLoggedIn = autenticacion;

            if (IsLoggedIn)
            {
                Items.Add(new ShellContent
                {
                    Title = "Inicio",
                    ContentTemplate = new DataTemplate(typeof(InicioPage)),
                    Route = "Inicio"
                });

                Items.Add(new ShellContent
                {
                    Title = "Permisos",
                    Icon = "https://cdn.icon-icons.com/icons2/2144/PNG/48/person_paper_id_card_profile_user_icon_131800.png",
                    ContentTemplate = new DataTemplate(typeof(PermisosPage)),
                    Route = "Permisos"
                });

                Items.Add(new ShellContent
                {
                    Title = "Usuarios",
                    Icon = "https://cdn.icon-icons.com/icons2/2063/PNG/48/person_account_file_folder_document_icon_124634.png",
                    ContentTemplate = new DataTemplate(typeof(UsuariosPage)),
                    Route = "Usuarios"
                });
            }

        }

    }
}