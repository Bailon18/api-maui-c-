using movil_app.Models;
using movil_app.Page;


namespace movil_app
{
    public partial class AppShell : Shell
    {

        bool IsLoggedIn = false;
        UsuarioModel UsuarioModel { get; set; }

        public AppShell()
        {
            InitializeComponent();
        }

        public void menuuu(bool autenticacion)
        {
            IsLoggedIn = autenticacion;

            if (IsLoggedIn && UsuarioModel != null) // Verificar que UsuarioModel no sea nulo
            {

                Items.Add(new ShellContent
                {
                    Title = "Inicio",
                    Icon = "https://cdn.icon-icons.com/icons2/1520/PNG/512/homeflat_106039.png",
                    ContentTemplate = new DataTemplate(typeof(InicioPage)),
                    Route = "Inicio"
                });

                Items.Add(new ShellContent
                {
                    Title = "Perfil",
                    Icon = "https://cdn.icon-icons.com/icons2/1520/PNG/512/homeflat_106039.png",
                    ContentTemplate = new DataTemplate(typeof(PerfilPage)),
                    Route = "Perfil"
                });


                if (UsuarioModel.nombre_rol == "ADMINISTRADOR")
                {
                    Items.Add(new ShellContent
                    {
                        Title = "Solicitudes",
                        Icon = "https://cdn.icon-icons.com/icons2/2144/PNG/512/diagram_paper_notes_documents_icon_131788.png",
                        ContentTemplate = new DataTemplate(typeof(SolicitudesPage)),
                        Route = "Solicitudes"
                    });

                    Items.Add(new ShellContent
                    {
                        Title = "Usuarios",
                        Icon = "https://cdn.icon-icons.com/icons2/2063/PNG/48/person_account_file_folder_document_icon_124634.png",
                        ContentTemplate = new DataTemplate(typeof(UsuariosPage)),
                        Route = "Usuarios"
                    });
                }
                else
                {
                    Items.Add(new ShellContent
                    {
                        Title = "Permisos",
                        Icon = "https://cdn.icon-icons.com/icons2/2144/PNG/48/person_paper_id_card_profile_user_icon_131800.png",
                        ContentTemplate = new DataTemplate(typeof(PermisosPage)),
                        Route = "Permisos"
                    });
                }
            }
        }


        public void setUsuarioLogeado(UsuarioModel usuariologeado) {

            UsuarioModel = usuariologeado;
        }

        public UsuarioModel getUsuarioLogeado()
        {
            return UsuarioModel;
        }


    }
}