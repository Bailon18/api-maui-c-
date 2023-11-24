using movil_app.Models;
using movil_app.Models.Services;
using System;
using System.Collections.Generic;


namespace movil_app.Page
{
    public partial class SolicitudesPage : ContentPage
    {
        private readonly PermisosServices _permisosServices;
        private readonly UsuarioServices _usuarioServices;
        public List<DepartamentosModel> Departamentos { get; set; } = new List<DepartamentosModel>
        {
            new DepartamentosModel { departamento_id = 1, nombre_departamento = "SISTEMA" },
            new DepartamentosModel { departamento_id = 2, nombre_departamento = "VENTAS" },
            new DepartamentosModel { departamento_id = 3, nombre_departamento = "TECNOLOGIA" }
        };

        public DepartamentosModel SelectedDepartamento { get; set; }

        public SolicitudesPage()
        {
            InitializeComponent();
            _permisosServices = new PermisosServices();
            _usuarioServices = new UsuarioServices();
            BindingContext = this;
            SelectedDepartamento = Departamentos.FirstOrDefault();
            ObtenerIdDepartamento();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            ObtenerIdDepartamento();
        }



        public async void ObtenerIdDepartamento()
        {
            
            var listadosolicitudes = await _permisosServices.ListarSolicitudesPermiso();

            var usuarios = await _usuarioServices.ListarUsuarios();

            foreach (var solicitud in listadosolicitudes)
            {
                var usuario = usuarios.FirstOrDefault(u => u.usuario_id == solicitud.usuario_id);
                if (usuario != null)
                {
                    solicitud.nombre_usuario = $"{usuario.nombre} {usuario.apellidos}";
                }
            }

            SolicitudCollectionView.ItemsSource = listadosolicitudes;

            Console.WriteLine("ENTROOOOOO");

        }

        private async void DepartamentoPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && picker.SelectedItem is DepartamentosModel departamento)
            {
                int idSeleccionado = departamento.departamento_id;

                var solicitudes = await _permisosServices.ObtenerSolicitudesPorDepartamento(idSeleccionado);
                var usuarios = await _usuarioServices.ListarUsuarios();

                foreach (var solicitud in solicitudes)
                {
                    var usuario = usuarios.FirstOrDefault(u => u.usuario_id == solicitud.usuario_id);
                    if (usuario != null)
                    {
                        solicitud.nombre_usuario = $"{usuario.nombre} {usuario.apellidos}";
                    }
                }

                SolicitudCollectionView.ItemsSource = solicitudes;
            }
        }



        private async void EntryBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = e.NewTextValue;

            try
            {
                var solicitudes = await _permisosServices.BuscarSolicitudesPorNombreOApellido(searchText);
                var usuarios = await _usuarioServices.ListarUsuarios();

                if (solicitudes != null)
                {
                    foreach (var solicitud in solicitudes)
                    {
                        var usuario = usuarios.FirstOrDefault(u => u.usuario_id == solicitud.usuario_id);
                        if (usuario != null)
                        {
                            solicitud.nombre_usuario = $"{usuario.nombre} {usuario.apellidos}";
                        }
                    }

                    SolicitudCollectionView.ItemsSource = solicitudes;
                }
                else
                {
                    Console.WriteLine("No se encontraron solicitudes para el término de búsqueda proporcionado.");
                    SolicitudCollectionView.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar solicitudes: {ex.Message}");
            }
        }


        private async void RevisionButton_Clicked(object sender, EventArgs e)
        {


            if (sender is ImageButton button && button.BindingContext is SolicitudesPermisoModel solicitud)
            {
                int solicitudId = solicitud.solicitud_id;


                var modalPage = new AprobacionPage(solicitudId);
                await Navigation.PushModalAsync(modalPage);

            }

        }

     
    }
}
