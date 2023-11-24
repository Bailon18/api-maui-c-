namespace movil_api.Modelo
{
    public class DetallesSolicitudHorasModel
    {
        public int SolicitudId { get; set; }

        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }

        public DateTime FechaPermiso { get; set; }
    }
}
