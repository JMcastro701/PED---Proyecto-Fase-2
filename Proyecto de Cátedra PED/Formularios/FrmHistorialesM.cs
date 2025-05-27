
using Proyecto_de_Cátedra_PED.Clases;
using Proyecto_de_Cátedra_PED.Clases.Clases_Lista;
using Proyecto_de_Cátedra_PED.ManejoBD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_de_Cátedra_PED.Formularios
{
    public partial class FrmHistorialesM : Form
    {
        private ListaEnlazadaPacientes listaPacientes;
        private ListaEnlazadaReportes listaReportesActuales;
        private Paciente pacienteActual;
        private Reporte reporteActual;
        private FrmMenuP menu;
       
        public FrmHistorialesM(FrmMenuP menu)
        {
            InitializeComponent();
            //Oculta la barra por defecto del Form
            this.Text = string.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;
            this.menu = menu;   
        }

        //-------- FUNCION PARA ARRATRAR EL FORMULARIO POR LA BARRA DE TITULO
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void panelBarraTitulo_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnCerrar_MouseEnter(object sender, EventArgs e)
        {
            Color colorHover = Color.FromArgb(255, 50, 50);
            btnCerrar.BackColor = colorHover;
        }

        private void btnCerrar_MouseLeave(object sender, EventArgs e)
        {
            Color colorOriginal = Color.FromArgb(208, 1, 27);
            btnCerrar.BackColor = colorOriginal;
        }

        private void btnMaximizar_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private void btnMaximizar_MouseEnter(object sender, EventArgs e)
        {
            Color colorHoverMax = Color.FromArgb(0, 255, 100);
            btnMaximizar.BackColor = colorHoverMax;
        }

        private void btnMaximizar_MouseLeave(object sender, EventArgs e)
        {
            Color colorOriginalMax = Color.FromArgb(8, 202, 62);
            btnMaximizar.BackColor = colorOriginalMax;
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }     

        private void btnMinimizar_MouseEnter(object sender, EventArgs e)
        {
            Color colorHoverMax = Color.FromArgb(255, 215, 0);
            btnMinimizar.BackColor = colorHoverMax;
        }

        private void btnMinimizar_MouseLeave(object sender, EventArgs e)
        {
            Color colorOriginalMax = Color.FromArgb(255, 191, 54);
            btnMinimizar.BackColor = colorOriginalMax;
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            FrmHistorialesAgregar frmHistorialesAgregar = new FrmHistorialesAgregar();
            this.Hide();
            frmHistorialesAgregar.ShowDialog();
            this.Show();

            FrmHistorialesM_Load(null, null);
            if (pacienteActual != null)
            {
                ActualizarListBox(pacienteActual.Dui);
            }
        }

        private void btnConfirmarCodigo_Click(object sender, EventArgs e)
        {
            string duiInput = mtxtIdPaciente.Text;
            pacienteActual = listaPacientes.BuscarPorDUI(duiInput);

            if(pacienteActual == null)
            {
                MessageBox.Show("No se ha encontrado un Paciente con el DUI inregsado");
                return;
            }

            txtNombrePaciente.Text = pacienteActual.Nombres;
            txtApellidosPaciente.Text = pacienteActual.Apellidos;

            ActualizarListBox(duiInput);
        }

        private void FrmHistorialesM_Load(object sender, EventArgs e)
        {
            //Llenar lista a partir de BD
            try
            {
                listaPacientes = PacientesDAO.CargarPacientesConReportes();
                listaPacientes.LlenarComboBoxDUI(cbDuiPaciente);

           }
            catch 
            {
                MessageBox.Show("Ha Ocurrido un error al cargar los datos");
            }
        }

        private void lstbReportes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Reporte temp = listaReportesActuales.ObtenerNodoEnPosicion(lstbReportes.SelectedIndex);
                reporteActual = temp;
                MostrarReporte(temp);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MostrarReporte(Reporte reporte)
        {
            txtTipoReporte.Text = reporte.TipoReporte;
            txtFecha.Text = reporte.Fecha.ToShortDateString();
            txtDiagnostico.Text = reporte.Diagnostico;
            txtTratamiento.Text = reporte.Tratamiento;
            txtEspecialidadMedico.Text = PersonalMedicoDAO.ObtenerEspecialidadMedico(reporte.LlavePrimariaMedico);
            txtNombreMedico.Text = PersonalMedicoDAO.ObtenerNombreMedico(reporte.LlavePrimariaMedico);
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            if (lstbReportes.SelectedIndex < lstbReportes.Items.Count-1)
            {
                lstbReportes.SelectedIndex++;
            }
        }

        private void btnAnteriorReporte_Click(object sender, EventArgs e)
        {
            if (lstbReportes.SelectedIndex > 0)
            {
                lstbReportes.SelectedIndex--;
            }
        }
        private void ActualizarListBox(string duiInput)
        {

            //Cargar Lista de Reportes a ListBox
            lstbReportes.Items.Clear();

            listaReportesActuales = listaPacientes.ObtenerReportesPorDUI(duiInput);
            listaReportesActuales.MostrarReportesListBox(lstbReportes);

            lstbReportes.SelectedIndex = 0;
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (pacienteActual == null || reporteActual == null)
            {
                MessageBox.Show("Error: Debe seleccionar un paciente y su reportes a eliminar");
                return;
            }

           DialogResult resultado = MessageBox.Show("¿Desea eliminar el reporte con número: " + reporteActual.NumeroReporte +"?", "Confirmación",
                MessageBoxButtons.OKCancel,MessageBoxIcon.Warning);

            if (resultado == DialogResult.OK)
            {
                try
                {
                    listaPacientes.EliminarReporteDePaciente(pacienteActual.Dui, reporteActual.NumeroReporte);
                    ActualizarListBox(pacienteActual.Dui);
                }
                catch (Exception ex) { MessageBox.Show("Error al Eliminar Reporte: " + ex.Message); }
            }
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            this.Close();
            menu.Show();
        }

        private void cbDuiPaciente_SelectedIndexChanged(object sender, EventArgs e)
        {
            mtxtIdPaciente.Text = cbDuiPaciente.SelectedItem.ToString();
        }
    }
}
