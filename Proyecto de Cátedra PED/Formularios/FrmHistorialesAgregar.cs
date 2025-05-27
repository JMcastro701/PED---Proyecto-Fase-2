
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
    public partial class FrmHistorialesAgregar : Form
    {
        private ListaEnlazadaPacientes listaPacientes;
        private ListaEnlazadaReportes listaReportes;
        private Paciente pacienteActual;
        public FrmHistorialesAgregar()
        { 
            InitializeComponent();
            listaPacientes = new ListaEnlazadaPacientes();
            listaReportes = new ListaEnlazadaReportes();
        }

        private void FrmHistorialesAgregar_Load(object sender, EventArgs e)
        {
            //Llenar lista a partir de BD
            try
            {
                listaPacientes = PacientesDAO.CargarPacientesConReportes();
                listaPacientes.LlenarComboBoxDUI(cbDuiPaciente);

                PersonalMedicoDAO.CargarPersonalEnComboBox(cbNombreMédico);

                cbNombreMédico.SelectedIndex = 0;
            }
            catch 
            {
                MessageBox.Show("Ha Ocurrido un error al cargar los datos");
            }
        }

        private void btnConfirmarCodigo_Click(object sender, EventArgs e)
        {
            string duiInput = mtxtIdPaciente.Text;
            pacienteActual = listaPacientes.BuscarPorDUI(duiInput);

            if (pacienteActual == null)
            {
                MessageBox.Show("No se ha encontrado un Paciente con el DUI inregsado");
                return;
            }

            txtNombres.Text = pacienteActual.Nombres;
            txtApellidosPaciente.Text = pacienteActual.Apellidos;
            txtSexoPaciente.Text = pacienteActual.Sexo;
            txtFechaNacimiento.Text = pacienteActual.FechaNacimiento.ToShortDateString();

            txtMostrarDUI.Text = pacienteActual.Dui;
            txtFechaReporte.Text = DateTime.Now.ToString();

           
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            
            txtTipoReporte.Clear();
            txtDiagnostico.Clear(); 
            txtTratamiento.Clear();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            //Verificación ID de Médico
            if (!PersonalMedicoDAO.ExisteMedicoConId(int.Parse((cbNombreMédico.SelectedItem as dynamic).Value)))
            {
                MessageBox.Show("Debe Seleccionar un Nombre de médico válido");
                return;
            }

            if(pacienteActual == null)
            {

                MessageBox.Show("Debe ingresar el dui de un paciente");
                return;
            }

            if(txtDiagnostico.Text.Length<5 || txtTratamiento.Text.Length < 5 || txtTipoReporte.Text.Length < 5)
            {
                MessageBox.Show("ERROR: Todas las entradas deben contener un mínimo de 5 caracteres");
                return;
            }


            Reporte temp = new Reporte(DateTime.Now, txtDiagnostico.Text,txtTratamiento.Text,txtTipoReporte.Text, int.Parse( (cbNombreMédico.SelectedItem as dynamic).Value));

            bool resultado = listaPacientes.AgregarReporteAPaciente(pacienteActual.Dui, temp);

            if (resultado)
            {
                btnLimpiar_Click(null, null);
                MessageBox.Show("Reporte añadido con éxito"); 
            }
            else MessageBox.Show("Error al agregar Reporte");
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbDuiPaciente_SelectedIndexChanged(object sender, EventArgs e)
        {
            mtxtIdPaciente.Text = cbDuiPaciente.SelectedItem.ToString();
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

        private void btnMinimizar_DragLeave(object sender, EventArgs e)
        {
            Color colorOriginalMax = Color.FromArgb(255, 191, 54);
            btnMinimizar.BackColor = colorOriginalMax;
        }
    }
}
