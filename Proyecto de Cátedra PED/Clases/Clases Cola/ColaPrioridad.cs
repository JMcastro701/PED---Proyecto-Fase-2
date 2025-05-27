
using Proyecto_de_Cátedra_PED.ManejoBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_de_Cátedra_PED.Clases.Clases_Cola
{
    class ColaPrioridad
    {
        private NodoCola frente;

        //Al inciarse el Form el objeto de la Cola almacenará todos los datos de la BD que no hayan sido atendidos y
        //obtendrá el "contador pacientes" de la cantidad de registros totales, para servir como el identificador único de cada turno

        private int contadorPacientes;

        //Constructor Predeterminado
        public ColaPrioridad()
        {
            
        }
        //COnstructor utilizado al cargar cola desde BD
        public ColaPrioridad (int contadorInicial)
        {
            contadorPacientes = contadorInicial;
        }

        public void Encolar(Paciente paciente, DateTime fecha, int prioridad)
        {

            contadorPacientes++;
            NodoCola nuevoNodo = new NodoCola(contadorPacientes, paciente, fecha, prioridad);

            if (frente == null || prioridad < frente.Prioridad)
            {
                nuevoNodo.Siguiente = frente;
                frente = nuevoNodo;
            }
            else
            {
                NodoCola actual = frente;
                while (actual.Siguiente != null && actual.Siguiente.Prioridad <= prioridad)
                {
                    actual = actual.Siguiente;
                }

                nuevoNodo.Siguiente = actual.Siguiente;
                actual.Siguiente = nuevoNodo;
            }
        }

        public void GuardarEnBD(Paciente paciente, DateTime fecha, int prioridad)
        {
            TurnosDAO.InsertarTurno(contadorPacientes, paciente.Dui,prioridad,fecha.Date, fecha.TimeOfDay);
        }
        public void Encolar(int codigoTurno, Paciente paciente, DateTime fecha, int prioridad)
        {

            
            NodoCola nuevoNodo = new NodoCola(codigoTurno, paciente, fecha, prioridad);

            if (frente == null || prioridad < frente.Prioridad)
            {
                nuevoNodo.Siguiente = frente;
                frente = nuevoNodo;
            }
            else
            {
                NodoCola actual = frente;
                while (actual.Siguiente != null && actual.Siguiente.Prioridad <= prioridad)
                {
                    actual = actual.Siguiente;
                }

                nuevoNodo.Siguiente = actual.Siguiente;
                actual.Siguiente = nuevoNodo;
            }
        }


        public NodoCola Desencolar()
        {
            if (frente == null)
                return null;

            NodoCola nodoDesencolado = frente;
            frente = frente.Siguiente;
            return nodoDesencolado;
        }
         
        public void MostrarCola(DataGridView dgvTurnos)
        {
            dgvTurnos.Rows.Clear();
            dgvTurnos.Columns.Clear();

            dgvTurnos.Columns.Add("codigoTurno", "Código de Turno");
           // dgvTurnos.Columns.Add("codigoPaciente", "Código de Paciente");
            dgvTurnos.Columns.Add("nombres", "Nombres");
            dgvTurnos.Columns.Add("apellidos", "Apellidos");
            dgvTurnos.Columns.Add("fechaTurno", "Fecha");
            dgvTurnos.Columns.Add("horaTurno", "Hora");
            //dgvTurnos.Columns.Add("observaciones", "Observaciones");
            //dgvTurnos.Columns.Add("tipoAtencion", "Tipo de Atención");
            dgvTurnos.Columns.Add("prioridad", "Prioridad");
           // dgvTurnos.Columns.Add("estado", "Estado");

            NodoCola actual = frente;
            while (actual != null)
            {
                dgvTurnos.Rows.Add(
                    actual.CodigoTurno,
                    //actual.Paciente.code
                    actual.Paciente.Nombres,
                    actual.Paciente.Apellidos,
                    actual.FechaTurno.ToShortDateString(),
                    actual.FechaTurno.ToShortTimeString(),
                    //  actual.Observaciones,
                    // actual.TipoAtencion,
                    ObtenerNombrePrioridad(actual.Prioridad)
                    //actual.Estado
                );

                actual = actual.Siguiente;
            }
        }

        public bool EstaVacia()
        {
            return frente == null;
        }
        public static string ObtenerNombrePrioridad(int prioridad)
        {
            switch (prioridad)
            {
                case 1: return "Crítico";
                case 2: return "Estable";
                case 3: return "Leve";
                default: return "Desconocido";
            }
        }
        public static string ObtenerStringEstado(bool estado)
        {
            switch (estado)
            {
                case true: return "Atendido";
                    
                case false: return "No Atendido";

                default: return null;
            }
        }
    }
}
