namespace movil_api.Modelo
{
    public class UsuarioModel
    {

        public int usuario_id { get; set; }
        public string correo { get; set; }
        public string nombre { get; set; }
        public string apellidos { get; set; }
        public string contrasena { get; set; }
        public string codigoempleado { get; set; }
        public string estado { get; set; }
        public string nombre_rol { get; set; } 
        public string nombre_departamento { get; set; }
        public DateTime fecha_ingreso { get; set; }

    }
}
