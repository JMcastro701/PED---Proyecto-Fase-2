using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_de_Cátedra_PED.Formularios
{
    public partial class FrmUusarios : Form
    {
        private FrmMenuP menu;

        public FrmUusarios(FrmMenuP frmMenu)
        {
            InitializeComponent();
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
            txtIDUser.Clear();
            txtNombre.Clear();
            txtTRol.Clear();
            txtCorreo.Clear();
            txtContraseña.Clear();
            txtTelefono.Clear();
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (!ValidarCamposObligatorios() ||
               !ValidarCorreo(txtCorreo.Text) ||
               !ValidarTelefono(txtTelefono.Text))
            {
                return;
            }

            if (ExisteUsuarioID(txtIDUser.Text))
            {
                MessageBox.Show("El ID de usuario ya existe. Elija otro.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidarContraseña(txtContraseña.Text))
                return;

            if (ExisteCorreo(txtCorreo.Text.Trim()))
            {
                MessageBox.Show("El correo electrónico ya está registrado.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ExisteTelefono(txtTelefono.Text.Trim()))
            {
                MessageBox.Show("El número de teléfono ya está registrado.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ExisteNombreRol(txtTRol.Text.Trim()))
            {
                MessageBox.Show("El nombre de rol ya está registrado. Ingrese uno diferente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                string consulta = "INSERT INTO Usuarios (UsuarioID, Nombre_Usuario, Nombre_Rol, Correo, Contraseña, Telefono) " +
                                  "VALUES (@id, @nombre, @rol, @correo, @contra, @tel)";

                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@id", txtIDUser.Text);
                comando.Parameters.AddWithValue("@nombre", txtNombre.Text);
                comando.Parameters.AddWithValue("@rol", txtTRol.Text);
                comando.Parameters.AddWithValue("@correo", txtCorreo.Text);
                comando.Parameters.AddWithValue("@contra", txtContraseña.Text);
                comando.Parameters.AddWithValue("@tel", txtTelefono.Text);

                comando.ExecuteNonQuery();
                MessageBox.Show("Usuario registrado correctamente.");
                CargarUsuarios();
                LimpiarCampos();
            }
        }

        private void CargarUsuarios()
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                string consulta = "SELECT * FROM Usuarios";
                SqlDataAdapter adaptador = new SqlDataAdapter(consulta, conexion);
                DataTable tabla = new DataTable();
                adaptador.Fill(tabla);
                dtgUsuarios.DataSource = tabla;

                dtgUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (txtIDUser.Text == "")
            {
                MessageBox.Show("Seleccione un usuario para actualizar.");
                return;
            }

            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                string consulta = "UPDATE Usuarios SET Nombre_Usuario = @nombre, Nombre_Rol = @rol, Correo = @correo, " +
                                  "Contraseña = @contra, Telefono = @tel WHERE UsuarioID = @id";

                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@id", txtIDUser.Text);
                comando.Parameters.AddWithValue("@nombre", txtNombre.Text);
                comando.Parameters.AddWithValue("@rol", txtTRol.Text);
                comando.Parameters.AddWithValue("@correo", txtCorreo.Text);
                comando.Parameters.AddWithValue("@contra", txtContraseña.Text);
                comando.Parameters.AddWithValue("@tel", txtTelefono.Text);

                comando.ExecuteNonQuery();
                MessageBox.Show("Usuario actualizado correctamente.");
                CargarUsuarios();
                LimpiarCampos();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (txtIDUser.Text == "")
            {
                MessageBox.Show("Seleccione un usuario para eliminar.");
                return;
            }

            DialogResult result = MessageBox.Show("¿Está seguro de eliminar este usuario?", "Confirmación", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                using (SqlConnection conexion = Conexion.ObtenerConexion())
                {
                    string consulta = "DELETE FROM Usuarios WHERE UsuarioID = @id";
                    SqlCommand comando = new SqlCommand(consulta, conexion);
                    comando.Parameters.AddWithValue("@id", txtIDUser.Text);

                    comando.ExecuteNonQuery();
                    MessageBox.Show("Usuario eliminado correctamente.");
                    CargarUsuarios();
                    LimpiarCampos();
                }
            }
        }

        private void FrmUusarios_Load(object sender, EventArgs e)
        {
            CargarUsuarios();
        }

        private void dtgUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dtgUsuarios.Rows[e.RowIndex];
                txtIDUser.Text = fila.Cells["UsuarioID"].Value.ToString();
                txtNombre.Text = fila.Cells["Nombre_Usuario"].Value.ToString();
                txtTRol.Text = fila.Cells["Nombre_Rol"].Value.ToString();
                txtCorreo.Text = fila.Cells["Correo"].Value.ToString();
                txtContraseña.Text = fila.Cells["Contraseña"].Value.ToString();
                txtTelefono.Text = fila.Cells["Telefono"].Value.ToString();
            }
        }


        //-----------VALIDACIONES-------------------------------------------
        private bool ValidarCamposObligatorios()
        {
            if (string.IsNullOrWhiteSpace(txtIDUser.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtTRol.Text) ||
                string.IsNullOrWhiteSpace(txtCorreo.Text) ||
                string.IsNullOrWhiteSpace(txtContraseña.Text) ||
                string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Todos los campos son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private bool ValidarCorreo(string correo)
        {
            try
            {
                var mail = new System.Net.Mail.MailAddress(correo);
                return true;
            }
            catch
            {
                MessageBox.Show("El correo electrónico no tiene un formato válido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        private bool ValidarTelefono(string telefono)
        {
            if (!telefono.All(char.IsDigit))
            {
                MessageBox.Show("El teléfono solo debe contener números.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (telefono.StartsWith("0"))
            {
                MessageBox.Show("El teléfono no puede comenzar con 0.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (telefono.Length < 8 || telefono.Length > 9)
            {
                MessageBox.Show("El teléfono debe tener entre 8 y 9 dígitos.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool ExisteUsuarioID(string idUsuario)
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                string consulta = "SELECT COUNT(*) FROM Usuarios WHERE UsuarioID = @id";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@id", idUsuario);
                int count = (int)comando.ExecuteScalar();
                return count > 0;
            }
        }

        private bool ValidarContraseña(string contra)
        {
            if (contra.Length < 6)
            {
                MessageBox.Show("La contraseña debe tener al menos 6 caracteres.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private bool ExisteCorreo(string correo)
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                string consulta = "SELECT COUNT(*) FROM Usuarios WHERE Correo = @correo";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@correo", correo);
                int count = (int)comando.ExecuteScalar();
                return count > 0;
            }
        }

        private bool ExisteTelefono(string telefono)
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                string consulta = "SELECT COUNT(*) FROM Usuarios WHERE Telefono = @telefono";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@telefono", telefono);
                int count = (int)comando.ExecuteScalar();
                return count > 0;
            }
        }

        private bool ExisteNombreRol(string rol)
        {
            using (SqlConnection conexion = Conexion.ObtenerConexion())
            {
                string consulta = "SELECT COUNT(*) FROM Usuarios WHERE Nombre_Rol = @rol";
                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@rol", rol);
                int count = (int)comando.ExecuteScalar();
                return count > 0;
            }
        }

    }
}
