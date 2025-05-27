using Proyecto_de_Cátedra_PED.Clases.Clases_Cola;
using Proyecto_de_Cátedra_PED.ManejoBD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Proyecto_de_Cátedra_PED.ManejoBD.TurnosDAO;

namespace Proyecto_de_Cátedra_PED.Formularios
{
    public partial class FrmColaReporte : Form
    {
        private List<ReporteTurno> listaTurnos;
        public FrmColaReporte()
        {
            InitializeComponent();
            listaTurnos = new List<ReporteTurno>();
        }

        private void FrmColaReporte_Load(object sender, EventArgs e)
        {
            listaTurnos = TurnosDAO.ObtenerTurnosSinFiltrar();
            MostrarListaTurnos();
        }


        public void MostrarListaTurnos()
        {
            dgvTurnos.Rows.Clear();
            dgvTurnos.Columns.Clear();

            dgvTurnos.Columns.Add("codigoTurno", "Código de Turno");
            dgvTurnos.Columns.Add("dui", "DUI");
         
            dgvTurnos.Columns.Add("nombres", "Nombres");
            dgvTurnos.Columns.Add("apellidos", "Apellidos");
            dgvTurnos.Columns.Add("prioridad", "Prioridad");
            dgvTurnos.Columns.Add("estadoTurno", "EstadoTurno");
            dgvTurnos.Columns.Add("fechaTurno", "Fecha");
            dgvTurnos.Columns.Add("horaTurno", "Hora");

            dgvTurnos.Columns[0].Width = 100;
            dgvTurnos.Columns[1].Width = 100;
            dgvTurnos.Columns[2].Width = 100;
            dgvTurnos.Columns[4].Width = 60;


            foreach (var turno in listaTurnos)
            {
                dgvTurnos.Rows.Add(turno.CodigoTurno, turno.DUI, turno.NombresPaciente, turno.ApellidosPaciente,
                    ColaPrioridad.ObtenerNombrePrioridad( turno.Prioridad), ColaPrioridad.ObtenerStringEstado( turno.EstadoTurno), turno.FechaTurno.ToShortDateString(), turno.HoraTurno.ToString());
            }
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmColaReporte_Load(null, null);
        }

        private void btnFlitrar_Click(object sender, EventArgs e)
        {
            listaTurnos = TurnosDAO.ObtenerTurnosPorFecha(dtpFechaTurno.Value);
            MostrarListaTurnos();
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
