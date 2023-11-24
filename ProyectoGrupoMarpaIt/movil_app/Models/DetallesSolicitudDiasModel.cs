using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace movil_app.Models
{
    public class DetallesSolicitudDiasModel
    {
        public int SolicitudId { get; set; } 
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DiasPermisos { get; set; }
    }
}
