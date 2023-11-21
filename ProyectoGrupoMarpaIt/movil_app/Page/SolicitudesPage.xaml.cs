using movil_app.Models;
using movil_app.Models.Services;
using System;
using System.Collections.Generic;


namespace movil_app.Page
{
    public partial class SolicitudesPage : ContentPage
    {
        private readonly PermisosServices _permisosServices;
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
            BindingContext = this;
            SelectedDepartamento = Departamentos.FirstOrDefault();
            ObtenerIdDepartamento(SelectedDepartamento);
        }

        public async void ObtenerIdDepartamento(DepartamentosModel departamento)
        {
            if (departamento != null)
            {
                int idSeleccionado = departamento.departamento_id;
                var solicitudes = await _permisosServices.ObtenerSolicitudesPorDepartamento(idSeleccionado);
                SolicitudCollectionView.ItemsSource = solicitudes;
            }
        }

        private async void DepartamentoPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && picker.SelectedItem is DepartamentosModel departamento)
            {
                int idSeleccionado = departamento.departamento_id;
                var solicitudes = await _permisosServices.ObtenerSolicitudesPorDepartamento(idSeleccionado);
                SolicitudCollectionView.ItemsSource = solicitudes;
            }
        }

        private void EditarButton_Clicked(object sender, EventArgs e)
        {
            // Lógica para el botón Editar
        }

        private void EliminarButton_Clicked(object sender, EventArgs e)
        {
            // Lógica para el botón Eliminar
        }
    }
}
