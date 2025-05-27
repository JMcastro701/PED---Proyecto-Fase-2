using Proyecto_de_Cátedra_PED.ManejoBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_de_Cátedra_PED.Clases.Clases_Lista
{
    internal class Reporte
    {
        private DateTime fecha;
        private string diagnostico;
        private string tratamiento;
        private string tipoReporte;
        private int numeroReporte;
        private int llavePrimariaMedico;




        public Reporte(DateTime fecha, string diagnostico, string tratamiento, string tipoReporte, int numeroReporte, int llavePrimariaMedico)
        {
            this.fecha = fecha;
            this.diagnostico = diagnostico;
            this.tratamiento = tratamiento;
            this.tipoReporte = tipoReporte;
            this.numeroReporte = numeroReporte;
            this.llavePrimariaMedico = llavePrimariaMedico;


        }
        public Reporte(DateTime fecha, string diagnostico, string tratamiento, string tipoReporte, int llavePrimariaMedico)
        {
            this.fecha = fecha;
            this.diagnostico = diagnostico;
            this.tratamiento = tratamiento;
            this.tipoReporte = tipoReporte;
            this.numeroReporte = ReportesDAO.ObtenerUltimoNumeroReporte();
            this.llavePrimariaMedico = llavePrimariaMedico;

        }

        public int NumeroReporte
        {
            get => numeroReporte;
            set
            {
                if (value >= 0)
                {
                    numeroReporte = value;
                }
            }
        }

        public DateTime Fecha { get => fecha; set => fecha = value; }
        public string Diagnostico { get => diagnostico; }
        public string Tratamiento { get => tratamiento; }
        public string TipoReporte { get => tipoReporte; }
        public int LlavePrimariaMedico { get => llavePrimariaMedico; }

        public override string ToString()
        {
            return $"Reporte no. {NumeroReporte}, Fecha: " + Fecha.ToShortDateString();
        }
    }
}
