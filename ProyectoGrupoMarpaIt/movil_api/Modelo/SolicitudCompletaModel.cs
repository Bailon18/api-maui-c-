namespace movil_api.Modelo
{
    public class SolicitudCompletaModel
    {
        public SolicitudesPermisoModel Solicitud { get; set; }
        public DetallesSolicitudDiasModel? DetallesDias { get; set; }
        public DetallesSolicitudHorasModel? DetallesHoras { get; set; }
    }
}
