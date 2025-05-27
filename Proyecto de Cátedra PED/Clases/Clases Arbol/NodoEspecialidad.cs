using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_de_Cátedra_PED.Clases.Clases_Arbol
{
    public class NodoEspecialidad
    {
        public string Especialidad;
        public NodoEspecialidad Izquierda;
        public NodoEspecialidad Derecha;
        public int Altura;

        public NodoEspecialidad(string especialidad)
        {
            Especialidad = especialidad;
            Altura = 1;
        }

    }
}
