namespace movil_api.Modelo
{
    public class SolicitudesPermisoModel
    {
        public int solicitud_id { get; set; }
        public int usuario_id { get; set; }
        public string motivo { get; set; }
        public string estado_aprobacion { get; set; }
        public bool cuenta_vacaciones { get; set; }
        public bool cuenta_dias_laborales { get; set; }
        public bool goce_sueldo { get; set; }
        public string tipo_permiso { get; set; }
        public DateTime fecha_solicitud { get; set; }
    }
}
