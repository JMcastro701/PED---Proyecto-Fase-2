
using Proyecto_de_Cátedra_PED.Clases;
using Proyecto_de_Cátedra_PED.Clases.Clases_Cola;
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
    public partial class FrmColaP : Form
    {
        private ColaPrioridad colaTurnos;
        private ListaEnlazadaPacientes listaPacientes;
        private FrmMenuP menu;
        public FrmColaP(FrmMenuP frmMenu)
        {
            InitializeComponent();

            //Iniciar objeto Cola con cantidad de turnos igual a la de registros en BD
             colaTurnos = new ColaPrioridad(TurnosDAO.ObtenerCantidadTurnos());
            //colaTurnos = new ColaPrioridad();
            cmbPrioridad.Items.AddRange(new string[] { "Crítico", "Estable", "Leve" });
            cmbPrioridad.SelectedIndex = 0;
            //Oculta la barra por defecto del Form
            this.Text = string.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;

            menu = frmMenu;
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

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            this.Close();
            menu.Show();
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void LimpiarCampos()
        {
            mtxtIdPaciente.Clear();
          
            txtNombres.Clear();
            txtApellidos.Clear();
            txtFechaNac.Clear();
            txtSexoPaciente.Clear();
        }

        private void FrmColaP_Load(object sender, EventArgs e)
        {
            try
            {
                TurnosDAO.CargarColaTurnosNoAtendidos(colaTurnos);
            listaPacientes = PacientesDAO.CargarPacientesConReportes();
            listaPacientes.LlenarComboBoxDUI(cbDuiPaciente);
            MostrarPacientes();
            }
            catch 
            {
              MessageBox.Show("Ha Ocurrido un error al cargar los datos");
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string entradaDUI = mtxtIdPaciente.Text;

            try
            {
               Paciente paciente = PacientesDAO.ObtenerPacientePorDUI(entradaDUI);

                // Validación: Que haya seleccionado un estado
                if (cmbPrioridad.SelectedItem == null)
                {
                    MessageBox.Show("Por favor seleccione un estado para el paciente.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtenemos prioridad basada en el estado
                int prioridad = cmbPrioridad.SelectedIndex + 1;
                DateTime fecha = DateTime.Now;
                

                // Encolamos el nuevo paciente
                colaTurnos.Encolar(paciente,fecha, prioridad );

                //Agregar Turno a BD
                colaTurnos.GuardarEnBD(paciente, fecha, prioridad);

                // Refresca la visualización de la cola
                MostrarPacientes();
                MessageBox.Show($"Paciente identificación {paciente.Dui} agregado con prioridad {ColaPrioridad.ObtenerNombrePrioridad(prioridad)}.", "Paciente Agregado", MessageBoxButtons.OK, MessageBoxIcon.Information);

              

               
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            //Validar que exista paciente con dui ingresado

            //Obtener datos de paciente con el dui
            // Paciente paciente = new Paciente("N1", "ap1", 10, "dui1", DateTime.UtcNow, "m", "dir1", "tel1");

            
           
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
          
            
        }

        private void btnConfirmarCodigo_Click(object sender, EventArgs e)
        {
            string id = mtxtIdPaciente.Text;
            //Buscar Paciente a partir de identificación en la BD
            try
            {
                Paciente temp = PacientesDAO.ObtenerPacientePorDUI(id);

                //Mostrar Datos
                txtNombres.Text = temp.Nombres; //Recuperado de BD
                txtApellidos.Text = temp.Apellidos; //Recuperado de BD
                txtFechaNac.Text = temp.FechaNacimiento.ToShortDateString();
                txtSexoPaciente.Text = temp.Sexo;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            
           
            
        }

        private void MostrarPacientes()
        {
            // Refresca los datos en el DataGridView usando el método MostrarCola
            colaTurnos.MostrarCola(dgvTurnos);
        }

        private void btnDesencolar_Click(object sender, EventArgs e)
        {
            try
            {
                NodoCola temp = colaTurnos.Desencolar();

                TurnosDAO.MarcarTurnoComoAtendido(temp.CodigoTurno);

                MostrarPacientes();

                MessageBox.Show($"El paciente número {temp.CodigoTurno} ha sido atendido.", "Paciente Atendido", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show( "Ha ocurrido un error al atender al Paciente: \n" + ex.Message); }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            FrmColaReporte frmColaReporte = new FrmColaReporte();
            this.Hide();
            frmColaReporte.ShowDialog();
            this.Show();

        }

        private void cbDuiPaciente_SelectedIndexChanged(object sender, EventArgs e)
        {
            mtxtIdPaciente.Text = cbDuiPaciente.SelectedItem.ToString();
        }

        private void mtxtIdPaciente_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void pictureBox15_Click(object sender, EventArgs e)
        {

        }
    }
}
