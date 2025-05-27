using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_de_Cátedra_PED.Clases.Clases_Lista
{
    internal class NodoPaciente
    {
        private Paciente paciente;
        private NodoPaciente siguiente;
        private ListaEnlazadaReportes reportes;

        public NodoPaciente(Paciente paciente)
        {
            this.paciente = paciente;
            siguiente = null;
            reportes = new ListaEnlazadaReportes();
        }

        internal Paciente Paciente { get => paciente; set => paciente = value; }
        internal NodoPaciente Siguiente { get => siguiente; set => siguiente = value; }
        internal ListaEnlazadaReportes Reportes { get => reportes; set => reportes = value; }
    }
}
