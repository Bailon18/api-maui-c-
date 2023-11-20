using movil_app.Models;
using movil_app.Models.Services;
using Microsoft.Maui.Controls;

namespace movil_app.Page;

public partial class ModalAgregarUsuarioPage : ContentPage
{
    public List<string> Estados { get; set; } = new List<string> { "ACTIVO", "INACTIVO" };
    public List<string> Roles { get; set; } = new List<string> { "EMPLEADO", "SUPERVISOR", "ADMINISTRADOR" };
    public List<string> Departamentos { get; set; } = new List<string> { "SISTEMA", "VENTAS", "TECNOLOGIA" };

    private UsuarioServices _usuarioServices;
    private UsuarioModel UsuarioEditando { get; set; }

    private int idSeleccionado { get; set; }
    public ModalAgregarUsuarioPage(int usuarioId = 0)
    {
        InitializeComponent();

        EstadoPicker.ItemsSource = Estados;
        RolPicker.ItemsSource = Roles;
        DepartamentoPicker.ItemsSource = Departamentos;

        _usuarioServices = new UsuarioServices();

        idSeleccionado = usuarioId;


        if (idSeleccionado != 0) {
            tituloagregareditar.Text = "Editar Usuario";
            BotonAccion.Text = "Actualizar";
        }

        CargarUsuarioEditando(idSeleccionado);
    }


    private async Task CargarUsuarioEditando(int idSeleccionado)
    {
        UsuarioEditando = await _usuarioServices.ObtenerUsuarioPorId(idSeleccionado);

        LlenarCamposConUsuarioEditando();
    }


    private void LlenarCamposConUsuarioEditando()
    {
        if (UsuarioEditando != null)
        {

            CorreoEntry.Text = UsuarioEditando.correo;
            NombreEntry.Text = UsuarioEditando.nombre;
            ApellidosEntry.Text = UsuarioEditando.apellidos;
            ContraseñaEntry.Text = UsuarioEditando.contrasena;
            CodigoEmpleadoEntry.Text = UsuarioEditando.codigoempleado;
            FechaIngresoDatePicker.Date = UsuarioEditando.fecha_ingreso;

            EstadoPicker.SelectedItem = UsuarioEditando.estado;
            RolPicker.SelectedItem = UsuarioEditando.nombre_rol;
            DepartamentoPicker.SelectedItem = UsuarioEditando.nombre_departamento;
        }
    }

    private async void AgregarButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CorreoEntry.Text) ||
            string.IsNullOrWhiteSpace(NombreEntry.Text) ||
            string.IsNullOrWhiteSpace(ApellidosEntry.Text) ||
            string.IsNullOrWhiteSpace(ContraseñaEntry.Text) ||
            string.IsNullOrWhiteSpace(CodigoEmpleadoEntry.Text) ||
            EstadoPicker.SelectedItem == null ||
            RolPicker.SelectedItem == null ||
            DepartamentoPicker.SelectedItem == null)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Todos los campos son obligatorios. Por favor, llénelos.", "Aceptar");
            return;
        }

        var nuevoUsuario = new UsuarioModel
        {
            correo = CorreoEntry.Text,
            nombre = NombreEntry.Text,
            apellidos = ApellidosEntry.Text,
            contrasena = ContraseñaEntry.Text,
            codigoempleado = CodigoEmpleadoEntry.Text,
            estado = EstadoPicker.SelectedItem?.ToString(),
            nombre_rol = RolPicker.SelectedItem?.ToString(),
            nombre_departamento = DepartamentoPicker.SelectedItem?.ToString(),
            fecha_ingreso = FechaIngresoDatePicker.Date
        };

        if (idSeleccionado != 0)
        {
            nuevoUsuario.usuario_id = idSeleccionado;

            var exito = await _usuarioServices.ActualizarUsuario(nuevoUsuario);

            if (exito)
            {
                await Application.Current.MainPage.DisplayAlert("Mensaje", "Usuario actualizado correctamente.", "Aceptar");
                await Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Error al actualizar el usuario.", "Aceptar");
            }
        }
        else
        {
            var exito = await _usuarioServices.InsertarUsuario(nuevoUsuario);

            if (exito)
            {
                await Application.Current.MainPage.DisplayAlert("Mensaje", "Registro correcto de usuario.", "Aceptar");
                await Navigation.PopModalAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Error al insertar el usuario.", "Aceptar");
            }
        }
    }





    private async void CancelarButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}