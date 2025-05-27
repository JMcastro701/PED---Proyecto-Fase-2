using Proyecto_de_Cátedra_PED.ManejoBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_de_Cátedra_PED.Clases.Clases_Lista
{
    internal class ListaEnlazadaReportes
    {
        private NodoReporte cabeza;
        private int reportesActuales;
        private int contadorReportes;

        public ListaEnlazadaReportes()
        {
            cabeza = null;
            reportesActuales = 0;
        }

        public int ContadorReportes { get => contadorReportes; }
        internal NodoReporte Cabeza { get => cabeza; }

        public void AgregarReporte(Reporte reporte)
        {
            reporte.NumeroReporte = contadorReportes++;
            NodoReporte nuevoNodo = new NodoReporte(reporte);
            nuevoNodo.Siguiente = cabeza;
            cabeza = nuevoNodo;
            reportesActuales++;
        }

        public void BorrarReporte(int numeroReporte)
        {
            NodoReporte actual = cabeza;
            NodoReporte anterior = null;

            while (actual != null)
            {
                if (actual.Reporte.NumeroReporte == numeroReporte)
                {
                    if (anterior == null)
                    {
                        cabeza = actual.Siguiente;
                        //Eliminar registro de BD
                        ReportesDAO.EliminarReporte(numeroReporte);
                    }
                    else
                    {
                        anterior.Siguiente = actual.Siguiente;
                        //Eliminar registro de BD
                        ReportesDAO.EliminarReporte(numeroReporte);
                    }

                    reportesActuales--;
                    return;
                }

                anterior = actual;
                actual = actual.Siguiente;
            }

        }

        /*
        public void MostrarReportesListBox(ListBox listBox)
        {
            NodoReporte nodoReporteTemp = Cabeza;
            while (nodoReporteTemp != null)
            {
                Reporte r = nodoReporteTemp.Reporte;

                listBox.Items.Add(r);

                nodoReporteTemp = nodoReporteTemp.Siguiente;
            }
        }
        */

        public void MostrarReportesListBox(ListBox listBox)
        {
            NodoReporte nodoReporteTemp = Cabeza;
            while (nodoReporteTemp != null)
            {

                listBox.Items.Add(nodoReporteTemp.Reporte.ToString());

                nodoReporteTemp = nodoReporteTemp.Siguiente;
            }
        }


        public void InsertarNodo(NodoReporte nuevo)
        {
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
            reportesActuales++;
            contadorReportes++;
        }

        public Reporte ObtenerNodoEnPosicion(int posicion)
        {
            if (posicion < 0 || posicion >= reportesActuales)
                throw new Exception("Posición fuera de rango: debe estar entre 0 y < " + reportesActuales);

            NodoReporte actual = cabeza;
            for (int i = 0; i < posicion; i++)
            {
                actual = actual.Siguiente;
            }

            return actual.Reporte;
        }
    }
}
