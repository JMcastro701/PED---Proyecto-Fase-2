using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_de_Cátedra_PED.Clases.Clases_Lista
{
    internal class NodoReporte
    {
        private Reporte reporte;
        private NodoReporte siguiente;

        public NodoReporte(Reporte reporte)
        {
            this.reporte = reporte;
            siguiente = null;
        }

        public Reporte Reporte { get => reporte; set => reporte = value; }
        public NodoReporte Siguiente { get => siguiente; set => siguiente = value; }
    }

}
