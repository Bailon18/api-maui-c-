using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace movil_app.Models
{
    public class SolicitudCompletaModel
    {
        public SolicitudesPermisoModel Solicitud { get; set; }
        public DetallesSolicitudDiasModel? DetallesDias { get; set; }
        public DetallesSolicitudHorasModel? DetallesHoras { get; set; }
    }
}
