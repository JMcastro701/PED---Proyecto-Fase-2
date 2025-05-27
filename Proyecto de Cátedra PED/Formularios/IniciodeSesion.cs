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
using System.Data.SqlClient;
using Proyecto_de_Cátedra_PED.Clases;

namespace Proyecto_de_Cátedra_PED.Formularios
{
    public partial class IniciodeSesion : Form
    {
        public IniciodeSesion()
        {
            InitializeComponent();
        }

        private void IniciodeSesion_Load(object sender, EventArgs e)
        {
            //Oculta la barra por defecto del Form
            this.Text = string.Empty;
            this.ControlBox = false;
            this.DoubleBuffered = true;

            if (Properties.Settings.Default.RecordarUsuario)
            {
                txtUsuario.Text = Properties.Settings.Default.UsuarioRecordado;
                chkRecuerdame.Checked = true;
            }

            txtContraseña.PasswordChar = '*';
        }

        //-------- FUNCION PARA ARRATRAR EL FORMULARIO POR LA BARRA DE TITULO
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);



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

        private void panelBarraTitulo_MouseMove(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }


        private void btnAcceder_Click(object sender, EventArgs e)
        {
            if (txtUsuario.Text == "" || txtContraseña.Text == "")
            {
                MessageBox.Show("Por favor, complete ambos campos.");
                return;
            }

            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                string consulta = "SELECT Nombre_Usuario, Nombre_Rol FROM Usuarios WHERE Nombre_Usuario = @usuario AND Contraseña = @contrasena";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@usuario", txtUsuario.Text.Trim());
                comando.Parameters.AddWithValue("@contrasena", txtContraseña.Text.Trim());

                SqlDataReader lector = comando.ExecuteReader();

                if (lector.Read())
                {
                    string nombreUsuario = lector["Nombre_Usuario"].ToString();
                    string nombreRol = lector["Nombre_Rol"].ToString();

                    MessageBox.Show("Acceso concedido. Bienvenido " + nombreUsuario);

                    FrmMenuP menu = new FrmMenuP(nombreUsuario, nombreRol); // Pasas solo datos simples
                    this.Hide();
                    menu.Show();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos");
                    txtUsuario.Clear();
                    txtContraseña.Clear();
                    txtUsuario.Focus();
                }
            }


            if (chkRecuerdame.Checked)
            {
                Properties.Settings.Default.UsuarioRecordado = txtUsuario.Text;
                Properties.Settings.Default.RecordarUsuario = true;
            }
            else
            {
                Properties.Settings.Default.UsuarioRecordado = "";
                Properties.Settings.Default.RecordarUsuario = false;
            }
            Properties.Settings.Default.Save();


        }

        private void txtUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evita que el beep suene o se dispare otro evento
                txtContraseña.Focus();
            }
        }

        private void txtContraseña_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evita comportamiento no deseado
                btnAcceder.PerformClick(); // Simula clic en botón de acceso
            }
        }

        private void MostrarCon_Click(object sender, EventArgs e)
        {
            //La imagen de OcultarContraseña la mandamos al frente
            OcultarCon.BringToFront();
            txtContraseña.PasswordChar='\0';
        }

        private void OcultarCon_Click(object sender, EventArgs e)
        {

            //La imagen de OcultarContraseña la mandamos al frente
            MostrarCon.BringToFront();
            txtContraseña.PasswordChar = '*';
        }
    }
}
