using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using FontAwesome.Sharp;
using System.Windows.Forms;
using Proyecto_de_Cátedra_PED.Clases;

namespace Proyecto_de_Cátedra_PED.Formularios
{
    public partial class FrmMenuP : Form
    {
        private string usuario;
        private string rol;

        public FrmMenuP(string nombreUsuario, string nombreRol)
        {
            InitializeComponent();
            //Estas lineas eliminan los parpadeos del formulario o controles en la interfaz grafica (Pero no en un 100%)
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
            this.MaximizedBounds=Screen.FromHandle(this.Handle).WorkingArea;

            usuario = nombreUsuario;
            rol = nombreRol;
            aplicarPermisos();

        }


        private void aplicarPermisos()
        {
            // Deshabilitar todos por defecto
            btnPaciente.Enabled = false;
            btnHistorialM.Enabled = false;
            btnUsuarios.Enabled = false;
            btnTurnos.Enabled = false;
            btnPersonalM.Enabled = false;

            switch (rol)
            {
                case "Administrador":
                    btnPaciente.Enabled = true;
                    btnHistorialM.Enabled = true;
                    btnUsuarios.Enabled = true;
                    btnTurnos.Enabled = true;
                    btnPersonalM.Enabled = true;
                    break;

                case "Recepcion":
                    btnPaciente.Enabled = true;
                    btnTurnos.Enabled = true;
                    btnHistorialM.Enabled = true;
                    break;

                case "Jefe de especialidad":
                    btnHistorialM.Enabled = true;
                    btnPersonalM.Enabled = true;
                    break;

                case "Soporte tecnico":                   
                    btnUsuarios.Enabled = true;
                    break;

                case "Director":
                    btnPaciente.Enabled = true;
                    btnTurnos.Enabled = true;
                    btnHistorialM.Enabled = true;
                    btnPersonalM.Enabled = true;
                    break;

                default:
                    MessageBox.Show("Rol no reconocido. Se desactivarán todas las opciones por seguridad.");
                    break;
            }
        }


        //------------------------METODO PARA REDIMENCIONAR/CAMBIAR TAMAÑO A FORMULARIO EN TIEMPO DE EJECUCION -------------------------------
        private int tolerance = 12;
        private const int WM_NCHITTEST = 132;
        private const int HTBOTTOMRIGHT = 17;
        private Rectangle sizeGripRectangle;
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    var hitPoint = this.PointToClient(new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16));
                    if (sizeGripRectangle.Contains(hitPoint))
                        m.Result = new IntPtr(HTBOTTOMRIGHT);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        //----------------DIBUJAR RECTANGULO / EXCLUIR ESQUINA PANEL 
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            var region = new Region(new Rectangle(0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height));
            sizeGripRectangle = new Rectangle(this.ClientRectangle.Width - tolerance, this.ClientRectangle.Height - tolerance, tolerance, tolerance);
            region.Exclude(sizeGripRectangle);
            this.panelContenedor.Region = region;
            this.Invalidate();
        }
        //----------------COLOR Y GRIP DE RECTANGULO INFERIOR
        protected override void OnPaint(PaintEventArgs e)
        {
            SolidBrush blueBrush = new SolidBrush(Color.FromArgb(244, 244, 244));
            e.Graphics.FillRectangle(blueBrush, sizeGripRectangle);
            base.OnPaint(e);
            ControlPaint.DrawSizeGrip(e.Graphics, Color.Transparent, sizeGripRectangle);
        }

        //---------------- CODIFICACION DE BOTONES CERRAR, MAXIMIZAR Y MINIMIZAR
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnCerrar_MouseLeave(object sender, EventArgs e)
        {
            Color colorOriginal = Color.FromArgb(208, 1, 27);
            btnCerrar.BackColor = colorOriginal;
        }

        private void btnCerrar_MouseEnter(object sender, EventArgs e)
        {
            Color colorHover = Color.FromArgb(255, 50, 50);
            btnCerrar.BackColor = colorHover;
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

        private void btnMinimizar_MouseLeave(object sender, EventArgs e)
        {
            Color colorOriginalMax = Color.FromArgb(255, 191, 54);
            btnMinimizar.BackColor = colorOriginalMax;
        }

        private void btnMinimizar_MouseEnter(object sender, EventArgs e)
        {
            Color colorHoverMax = Color.FromArgb(255, 215, 0);
            btnMinimizar.BackColor = colorHoverMax;
        }

        //-------- FUNCION PARA ARRATRAR EL FORMULARIO
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void panelBarraTitulo_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        //----------- CODIFICACION PARA BOTONES DE LA BARRA LATERAL --------------------------------------------
        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult resultado = MessageBox.Show("¿Estás seguro de que deseas salir?", "Confirmar salida", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        //Hora y Fecha
        private void HorayFecha_Tick(object sender, EventArgs e)
        {
            lblHora.Text = DateTime.Now.ToLongTimeString();
            lblFecha.Text=DateTime.Now.ToShortDateString();
        }

        //Mandare a llamar a los formularios restantes por los eventos click de los Botones 
        private void btnPaciente_Click(object sender, EventArgs e)
        {
            FrmPaciente frm = new FrmPaciente(this); // ← pasas el mismo menú
            frm.FormClosed += (s, args) => this.Show(); 
            frm.Show();
            this.Hide();
        }

        private void btnTurnos_Click(object sender, EventArgs e)
        {
            FrmColaP frm = new FrmColaP(this); // ← le pasas el mismo menú
            frm.FormClosed += (s, args) => this.Show(); // opcional por seguridad
            frm.Show();
            this.Hide();
        }

        private void btnHistorialM_Click(object sender, EventArgs e)
        {
            FrmHistorialesM frm = new FrmHistorialesM(this); // ← le pasas el mismo menú
            frm.FormClosed += (s, args) => this.Show(); // opcional por seguridad
            frm.Show();
            this.Hide();
        }

        private void btnPersonalM_Click(object sender, EventArgs e)
        {
            FrmArbol frm = new FrmArbol(this); // ← le pasas el mismo menú
            frm.FormClosed += (s, args) => this.Show(); // opcional por seguridad
            frm.Show();
            this.Hide();
        }

        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            FrmUusarios frm = new FrmUusarios(this); // ← le pasas el mismo menú
            frm.FormClosed += (s, args) => this.Show(); // opcional por seguridad
            frm.Show();
            this.Hide();
        }

      
    }
}
