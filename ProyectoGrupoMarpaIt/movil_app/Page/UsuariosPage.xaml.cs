using System;
using System.Collections.ObjectModel;
using movil_app.Models;
using movil_app.Models.Services;

namespace movil_app.Page
{
    public partial class UsuariosPage : ContentPage
    {
        private readonly UsuarioServices _usuarioServices;
        public ObservableCollection<UsuarioModel> Usuarios { get; set; }

        public UsuariosPage()
        {
            InitializeComponent();
            _usuarioServices = new UsuarioServices();
            Usuarios = new ObservableCollection<UsuarioModel>();
            UsuariosCollectionView.ItemsSource = Usuarios;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarUsuarios();
        }

        private async Task CargarUsuarios()
        {
            var usuarios = await _usuarioServices.ListarUsuarios();

            if (usuarios != null)
            {
                Usuarios.Clear();
                foreach (var usuario in usuarios)
                {
                    usuario.NombreCompleto = usuario.nombre + " " + usuario.apellidos;
                    Usuarios.Add(usuario);
                }
            }
            else
            {
                Console.WriteLine("No se pudieron cargar los usuarios.");
            }
        }

        private async void AgregarUsuario_Clicked(object sender, EventArgs e)
        {
            var modalPage = new ModalAgregarUsuarioPage();
            await Navigation.PushModalAsync(modalPage);
        }

        private async void EditarButton_Clicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is UsuarioModel usuario)
            {
                int usuarioId = usuario.usuario_id; 
                var modalPage = new ModalAgregarUsuarioPage(usuarioId);
                await Navigation.PushModalAsync(modalPage);
            }
        }


        private async void EliminarButton_Clicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is UsuarioModel usuario)
            {
                int usuarioId = usuario.usuario_id;

                bool confirmacion = await DisplayAlert("Confirmar eliminación", "¿Estás seguro de que quieres eliminar este usuario?", "Sí", "Cancelar");

                if (confirmacion)
                {
                    bool exito = await _usuarioServices.EliminarUsuario(usuarioId);

                    if (exito)
                    {
                        await DisplayAlert("Éxito", "Usuario eliminado correctamente.", "Aceptar");
                        await CargarUsuarios();
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se pudo eliminar el usuario.", "Aceptar");
                    }
                }
            }
        }
    }
}
