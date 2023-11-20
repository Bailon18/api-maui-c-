using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace movil_app.Models
{
    public class UsuarioModel
    {
        public int usuario_id { get; set; }
        public string correo { get; set; }
        public string nombre { get; set; }

        public string NombreCompleto { get; set; }
        public string apellidos { get; set; }
        public string contrasena { get; set; }
        public string codigoempleado { get; set; }
        public string estado { get; set; }
        public string nombre_rol { get; set; } 
        public string nombre_departamento { get; set; }
        public DateTime fecha_ingreso { get; set; }

    }
}
