using movil_app.Models;
using movil_app.Models.Services;
using System;

namespace movil_app.Page
{
    public partial class LoginPage : ContentPage
    {
        private ValidacionServices _validacionServices;

        public LoginPage()
        {
            InitializeComponent();
            _validacionServices = new ValidacionServices();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {

            string correo = "admin@gmail.com";
            string contrasena = "admin";

            var credenciales = new CredencialesModel
            {
                correo = correo,
                contrasena = contrasena
            };


            var usuarioValidado = await _validacionServices.ValidarCredenciales(credenciales);

            if (usuarioValidado != null)
            {

                if (Application.Current.MainPage is AppShell appShell)
                {
                    appShell.menuuu(true);
                    appShell.setUsuarioLogeado(usuarioValidado);
                    await Shell.Current.GoToAsync("//Inicio");
                }
            }
            else
            {

                await DisplayAlert("Error", "Credenciales inválidas", "OK");
            }

            //if (Application.Current.MainPage is AppShell appshell)
            //{
            //    appshell.menuuu(true);
            //    await Shell.Current.GoToAsync("//Inicio");
            //}
        }
    }
}


