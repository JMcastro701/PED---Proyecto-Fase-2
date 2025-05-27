using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_de_Cátedra_PED.Clases.Clases_Arbol
{
    public class ArbolEspecialidades
    {
        public NodoEspecialidad Raiz;

        // Método público para insertar
        public void Insertar(string especialidad)
        {
            Raiz = Insertar(Raiz, especialidad);
        }

        // Método recursivo privado
        private NodoEspecialidad Insertar(NodoEspecialidad nodo, string especialidad)
        {
            if (nodo == null)
                return new NodoEspecialidad(especialidad);

            // Comparaciones alfabéticas:
            if (especialidad.CompareTo(nodo.Especialidad) < 0)
                nodo.Izquierda = Insertar(nodo.Izquierda, especialidad);
            else if (especialidad.CompareTo(nodo.Especialidad) > 0)
                nodo.Derecha = Insertar(nodo.Derecha, especialidad);
            else
                return nodo; // Ya existe, no lo agrega

            // Actualiza altura y balancea
            nodo.Altura = 1 + Math.Max(Altura(nodo.Izquierda), Altura(nodo.Derecha));
            int balance = ObtenerBalance(nodo);

            // Rotaciones si está desbalanceado
            if (balance > 1 && especialidad.CompareTo(nodo.Izquierda.Especialidad) < 0)
                return RotarDerecha(nodo);

            if (balance < -1 && especialidad.CompareTo(nodo.Derecha.Especialidad) > 0)
                return RotarIzquierda(nodo);

            if (balance > 1 && especialidad.CompareTo(nodo.Izquierda.Especialidad) > 0)
            {
                nodo.Izquierda = RotarIzquierda(nodo.Izquierda);
                return RotarDerecha(nodo);
            }

            if (balance < -1 && especialidad.CompareTo(nodo.Derecha.Especialidad) < 0)
            {
                nodo.Derecha = RotarDerecha(nodo.Derecha);
                return RotarIzquierda(nodo);
            }

            return nodo;
        }

        private int Altura(NodoEspecialidad nodo)
        {
            return nodo == null ? 0 : nodo.Altura;
        }

        private int ObtenerBalance(NodoEspecialidad nodo)
        {
            return nodo == null ? 0 : Altura(nodo.Izquierda) - Altura(nodo.Derecha);
        }

        private NodoEspecialidad RotarDerecha(NodoEspecialidad y)
        {
            NodoEspecialidad x = y.Izquierda;
            NodoEspecialidad T2 = x.Derecha;

            x.Derecha = y;
            y.Izquierda = T2;

            y.Altura = 1 + Math.Max(Altura(y.Izquierda), Altura(y.Derecha));
            x.Altura = 1 + Math.Max(Altura(x.Izquierda), Altura(x.Derecha));

            return x;
        }

        private NodoEspecialidad RotarIzquierda(NodoEspecialidad x)
        {
            NodoEspecialidad y = x.Derecha;
            NodoEspecialidad T2 = y.Izquierda;

            y.Izquierda = x;
            x.Derecha = T2;

            x.Altura = 1 + Math.Max(Altura(x.Izquierda), Altura(x.Derecha));
            y.Altura = 1 + Math.Max(Altura(y.Izquierda), Altura(y.Derecha));

            return y;
        }
    }

}

