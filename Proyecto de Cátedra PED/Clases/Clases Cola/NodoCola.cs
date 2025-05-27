
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_de_Cátedra_PED.Clases.Clases_Cola
{
    class NodoCola
    {
       
        public int CodigoTurno { get; }
        public Paciente Paciente { get; }
        
        public int Prioridad { get;  }
        public DateTime FechaTurno { get; }
        private NodoCola siguiente = null;

        //Constructor utilizado al recuperar de la BD
        public NodoCola(int codigoTurno,Paciente paciente,  DateTime fechaTurno, int prioridad)
        {
            this.CodigoTurno = codigoTurno;
            
            this.FechaTurno = fechaTurno;
          
            this.Paciente = paciente;
           
            this.Prioridad = prioridad;
            
        }

        //COnstructor utilizado al agregar nuevo nodo a la cola
      

        public NodoCola Siguiente
        {
            get { return siguiente; }
            set { siguiente = value; }
        }

    }
}
