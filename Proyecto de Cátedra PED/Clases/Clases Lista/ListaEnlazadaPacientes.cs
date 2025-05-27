using Proyecto_de_Cátedra_PED.ManejoBD;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_de_Cátedra_PED.Clases.Clases_Lista
{
    class ListaEnlazadaPacientes
    {
        private NodoPaciente cabeza;

        public ListaEnlazadaPacientes()
        {
            cabeza = null;
        }

        public void AgregarPaciente(Paciente paciente)
        {

            //Verificar si existe un paciente en la lista con el mismo dui

            if (BuscarPorDUI(paciente.Dui) != null)
            {
                //Excepción, ya existe aciente con este dui
                throw new Exception("Ya existe paciente con este dui");

            }


            NodoPaciente nuevoNodo = new NodoPaciente(paciente);
            nuevoNodo.Siguiente = cabeza;
            cabeza = nuevoNodo;

        }

        public ListaEnlazadaReportes ObtenerReportesPorDUI(string dui)
        {


            NodoPaciente actual = cabeza;

            while (actual != null)
            {
                if (actual.Paciente.Dui == dui)
                {

                    return actual.Reportes;

                }

                actual = actual.Siguiente;
            }

            return null;
        }
        public void ActualizarPaciente(Paciente p)
        {
            NodoPaciente actual = cabeza;
            while (actual != null)
            {
                if (actual.Paciente.Dui == p.Dui)
                {
                    actual.Paciente = p;
                    break;
                }
                actual = actual.Siguiente;
            }
        }

        public void EliminarPaciente(Paciente paciente)
        {
            NodoPaciente actual = cabeza;
            NodoPaciente anterior = null;

            while (actual != null)
            {
                if (actual.Paciente.Dui == paciente.Dui)
                {
                    if (anterior == null)
                    {
                        cabeza = actual.Siguiente;
                    }
                    else
                    {
                        anterior.Siguiente = actual.Siguiente;
                    }
                    return;
                }

                anterior = actual;
                actual = actual.Siguiente;
            }
        }

        public Paciente BuscarPorDUI(string dui)
        {
            NodoPaciente actual = cabeza;

            while (actual != null)
            {
                if (actual.Paciente.Dui == dui)
                {
                    return actual.Paciente;
                }
                actual = actual.Siguiente;
            }

            return null;
        }

        public NodoPaciente ObtenerCabeza()
        {
            return cabeza;
        }


        public bool AgregarReporteAPaciente(string dui, Reporte nuevoReporte)
        {
            NodoPaciente actual = cabeza;

            while (actual != null)
            {
                if (actual.Paciente.Dui == dui)
                {
                    ReportesDAO.InsertarReporte(nuevoReporte, dui);
                    actual.Reportes.AgregarReporte(nuevoReporte);



                    return true; // Reporte agregado con éxito
                }
                actual = actual.Siguiente;
            }

            return false; // No se encontró el paciente, puede mandarse excepción al forms directamente
        }

        public bool EliminarReporteDePaciente(string dui, int numeroReporte)
        {
            NodoPaciente actual = cabeza;

            while (actual != null)
            {
                if (actual.Paciente.Dui == dui)
                {

                    //Verificar llave primaria, debe adaptarse a nuevas llaves
                    if (numeroReporte >= 0)
                    {
                        actual.Reportes.BorrarReporte(numeroReporte);


                        return true; // Reporte eliminado con éxito
                    }
                    else
                    {
                        return false; // Índice inválido, también podría mandarse excepcion al forms directamente
                    }
                }
                actual = actual.Siguiente;
            }

            return false; // Paciente no encontrado
        }

        public void InsertarNodo(NodoPaciente nuevo)
        {
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
        }

        public void CargarReportesBD(SqlConnection conexion)
        {
            NodoPaciente actual = cabeza;
            while (actual != null)
            {
                actual.Reportes = PacientesDAO.CargarReportesPorDUI(actual.Paciente.Dui, conexion);
                actual = actual.Siguiente;
            }
        }

        public void LlenarComboBoxDUI(ComboBox comboBox)
        {

            NodoPaciente actual = cabeza;

            while (actual != null)
            {
                if (actual.Paciente != null)
                {
                    comboBox.Items.Add(actual.Paciente.Dui);
                }
                actual = actual.Siguiente;
            }


        }
    }
}
