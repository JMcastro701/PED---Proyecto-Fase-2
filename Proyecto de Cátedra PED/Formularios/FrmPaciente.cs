using Proyecto_de_Cátedra_PED.Clases;
using Proyecto_de_Cátedra_PED.Clases.Clases_Lista;
using Proyecto_de_Cátedra_PED.Formularios;
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

namespace Proyecto_de_Cátedra_PED
{
    public partial class FrmPaciente : Form
    {
        private ListaEnlazadaPacientes listaPacientes;
        private FrmMenuP menu;

        public FrmPaciente(FrmMenuP frmMenu)
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


        //CODIFICACION DE BOTONES DE BARRA DE TITULO
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

        private void FrmPaciente_Load(object sender, EventArgs e)
        {
            cmbSexo.Items.Clear();
            cmbSexo.Items.Add("Hombre");
            cmbSexo.Items.Add("Mujer");
            cmbSexo.SelectedIndex = -1;
            listaPacientes = new ListaEnlazadaPacientes();
            dtpNacimiento.MaxDate = DateTime.Today;

            // Inicializa las columnas para el DataGridView
            dgvRegistro.Columns.Add("DUI", "DUI");
            dgvRegistro.Columns.Add("Nombres", "Nombres");
            dgvRegistro.Columns.Add("Apellidos", "Apellidos");
            dgvRegistro.Columns.Add("Sexo", "Sexo");
            dgvRegistro.Columns.Add("FechaNacimiento", "Fecha Nacimiento");
            dgvRegistro.Columns.Add("Telefono", "Telefono");
            dgvRegistro.Columns.Add("Correo", "Correo");
            dgvRegistro.Columns.Add("Direccion", "Direccion");
            dgvRegistro.Columns.Add("Peso", "Peso");
            dgvRegistro.Columns.Add("Altura", "Altura");

            CargarPacientesDesdeBD();
        }

        private void CargarPacientesDesdeBD()
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                string query = "SELECT * FROM Pacientes";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Paciente p = new Paciente(
                        reader["Nombres"].ToString(),
                        reader["Apellidos"].ToString(),
                        reader["DUI"].ToString(),
                        Convert.ToDateTime(reader["FechaNacimiento"]),
                        reader["Sexo"].ToString(),
                        reader["Direccion"].ToString(),
                        Convert.ToInt32(reader["Telefono"]),
                        reader["Correo"].ToString(),
                        Convert.ToDecimal(reader["Peso"]),
                        Convert.ToDecimal(reader["Altura"])
                    );

                    listaPacientes.AgregarPaciente(p);
                }
                reader.Close();
            }

            MostrarPacientesEnGrid();
        }

        //Para que textbox solo permita numeros no letras 
        private void txtTelefono_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }


        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtNombre.Clear();
            txtApellidos.Clear();
            txtDUI.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            txtCorreo.Clear();
            cmbSexo.SelectedIndex = -1;
            dtpNacimiento.Value = DateTime.Today;
            nudPeso.Value = 1.0M;     
            nudAltura.Value = 0.30M;  
        }

        private void btnRegresar_Click(object sender, EventArgs e)
        {
            this.Close();
            menu.Show();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            // Validaciones para campos obligatorios
            if (
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellidos.Text) ||
                string.IsNullOrWhiteSpace(cmbSexo.Text) ||
                string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                MessageBox.Show("Por favor complete los campos obligatorios: Nombres, Apellidos, Sexo y Dirección.",
                                "Campos obligatorios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validacion para que peso y altura sean números positivos
            decimal peso = nudPeso.Value;
            decimal altura = nudAltura.Value;

            if (peso <= 0 || altura <= 0)
            {
                MessageBox.Show("Peso y altura deben ser mayores a cero.", "Dato incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Validacion para que la fecha de necimiento sea entre 1990 y la actual (en caso de ser paciente recion nacido)
            DateTime fechaNacimiento = dtpNacimiento.Value.Date;
            DateTime hoy = DateTime.Today;
            DateTime fechaMinima = new DateTime(1900, 1, 1);

            if (fechaNacimiento < fechaMinima || fechaNacimiento > hoy)
            {
                MessageBox.Show("La fecha de nacimiento debe estar entre el 01/01/1900 y hoy.", "Fecha no válida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Calcula la edad del Paciente
            int edad = hoy.Year - fechaNacimiento.Year;
            if (fechaNacimiento > hoy.AddYears(-edad)) edad--;
            //Validacion: Si el paciente es mayor de edad, Debera presentar  DUI o NIT si aun no lo tramita; Si el paciente es menor de edad debera escribir DUI del responsable 
            if (edad >= 18)
            {
                if (string.IsNullOrWhiteSpace(txtDUI.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text))
                {
                    MessageBox.Show("Para mayores de edad, ingrese el DUI y teléfono del paciente.", "Datos requeridos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(txtDUI.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text) || string.IsNullOrWhiteSpace(txtCorreo.Text))
                {
                    MessageBox.Show("Para menores de edad, ingrese el DUI, Correo electrónico y teléfono del responsable.", "Datos requeridos del responsable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            //Validacion para que se seleccione una opcion en el combox
            if (cmbSexo.SelectedIndex == -1)
            {
                MessageBox.Show("Debe seleccionar el sexo del paciente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbSexo.Focus();
                return;
            }


            // Validacion para que el no. de teléfono solo sean numeros positivos y no admita letras ni numeros negativos
            if (txtTelefono.Text.Length != 8 || !int.TryParse(txtTelefono.Text, out int telefono) || telefono < 0)
            {
                MessageBox.Show("Ingrese un número de teléfono válido de 8 dígitos positivos.",
                                "Teléfono inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validacion para formato de correo
            if (!txtCorreo.Text.Contains("@") || !txtCorreo.Text.Contains("."))
            {
                MessageBox.Show("Ingrese un correo electrónico válido.", "Correo inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //  Validación para DUI único
            if (DUIExiste(txtDUI.Text.Trim()))
            {
                MessageBox.Show("El DUI ingresado ya existe en el sistema. Ingrese uno diferente.",
                                "DUI duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // validacion para que correos no se repitan
            if (CorreoExisteEnBD(txtCorreo.Text.Trim()))
            {
                MessageBox.Show("El correo ingresado ya está registrado en la base de datos. Ingrese uno diferente.",
                                "Correo duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (TelefonoExisteEnBD(txtTelefono.Text.Trim()))
            {
                MessageBox.Show("El número de teléfono ingresado ya está registrado. Ingrese uno diferente.",
                                "Teléfono duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            // OBJETO: Crear nuevo paciente
            Paciente nuevo = new Paciente(
                txtNombre.Text.Trim(),
                txtApellidos.Text.Trim(),
                txtDUI.Text.Trim(),
                dtpNacimiento.Value.Date,
                cmbSexo.SelectedItem?.ToString() ?? "",
                txtDireccion.Text.Trim(),
                telefono,
                txtCorreo.Text.Trim(),
                peso,
                altura
                );       

            // Agregar a la lista personalizada
            try
            {
                InsertarPacienteEnBD(nuevo);
                listaPacientes.AgregarPaciente(nuevo);
                MostrarPacientesEnGrid();
                MessageBox.Show("Paciente registrado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool TelefonoExisteEnBD(string telefono)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = "SELECT COUNT(*) FROM Pacientes WHERE Telefono = @Telefono";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Telefono", telefono);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private bool CorreoExisteEnBD(string correo)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = "SELECT COUNT(*) FROM Pacientes WHERE Correo = @Correo";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Correo", correo);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private bool DUIExiste(string dui)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                string query = "SELECT COUNT(*) FROM Pacientes WHERE DUI = @DUI";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DUI", dui);

                    // Solo abrir si está cerrada
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        //METODO: Mostrar los pacientes agregados al DataGridView
        private void MostrarPacientesEnGrid()
        {
            dgvRegistro.Rows.Clear();
            NodoPaciente actual = listaPacientes.ObtenerCabeza();

            while (actual != null)
            {
                Paciente p = actual.Paciente;
                dgvRegistro.Rows.Add(
                    p.Dui,
                    p.Nombres,
                    p.Apellidos,
                    p.Sexo,
                    p.FechaNacimiento.ToShortDateString(),
                    p.Telefono,
                    p.Correo,
                    p.Direccion,
                    p.Peso,
                    p.Altura
                );

                actual = actual.Siguiente;
                dgvRegistro.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        //METODO: Insertar los pacientes guardados en Memoria a la BD
        private void InsertarPacienteEnBD(Paciente nuevo)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                string query = "INSERT INTO Pacientes (DUI, Nombres, Apellidos, FechaNacimiento, Sexo, Direccion, Telefono, Correo, Peso, Altura) " +
                               "VALUES (@DUI, @Nombres, @Apellidos, @FechaNacimiento, @Sexo, @Direccion, @Telefono, @Correo, @Peso, @Altura)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DUI", nuevo.Dui);
                cmd.Parameters.AddWithValue("@Nombres", nuevo.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", nuevo.Apellidos);
                cmd.Parameters.AddWithValue("@FechaNacimiento", nuevo.FechaNacimiento);
                cmd.Parameters.AddWithValue("@Sexo", nuevo.Sexo);
                cmd.Parameters.AddWithValue("@Direccion", nuevo.Direccion);
                cmd.Parameters.AddWithValue("@Telefono", nuevo.Telefono);
                cmd.Parameters.AddWithValue("@Correo", nuevo.Correo);
                cmd.Parameters.AddWithValue("@Peso", nuevo.Peso);
                cmd.Parameters.AddWithValue("@Altura", nuevo.Altura);

                cmd.ExecuteNonQuery();
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            if (dgvRegistro.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un paciente para actualizar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            DataGridViewRow filaSeleccionada = dgvRegistro.SelectedRows[0];

            // Validar que no sea la fila vacía (nueva fila)
            if (filaSeleccionada.IsNewRow || filaSeleccionada.Cells["DUI"].Value == null)
            {
                MessageBox.Show("La fila seleccionada no contiene datos válidos.", "Fila inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string duiSeleccionado = dgvRegistro.SelectedRows[0].Cells["DUI"].Value.ToString();

            Paciente p = new Paciente(
                txtNombre.Text.Trim(),
                txtApellidos.Text.Trim(),
                duiSeleccionado,
                dtpNacimiento.Value.Date,
                cmbSexo.SelectedItem?.ToString() ?? "",
                txtDireccion.Text.Trim(),
                int.Parse(txtTelefono.Text.Trim()),
                txtCorreo.Text.Trim(),
                nudPeso.Value,
                nudAltura.Value
            );

            try
            {
                ActualizarPacienteEnBD(p);
                listaPacientes.ActualizarPaciente(p); // Debes implementar este método en tu clase lista enlazada
                MostrarPacientesEnGrid();
                MessageBox.Show("Paciente actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar paciente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void ActualizarPacienteEnBD(Paciente p)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                string query = @"UPDATE Pacientes SET
                            Nombres = @Nombres,
                            Apellidos = @Apellidos,
                            FechaNacimiento = @FechaNacimiento,
                            Sexo = @Sexo,
                            Direccion = @Direccion,
                            Telefono = @Telefono,
                            Correo = @Correo,
                            Peso = @Peso,
                            Altura = @Altura
                        WHERE DUI = @DUI";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@DUI", p.Dui);
                cmd.Parameters.AddWithValue("@Nombres", p.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", p.Apellidos);
                cmd.Parameters.AddWithValue("@FechaNacimiento", p.FechaNacimiento);
                cmd.Parameters.AddWithValue("@Sexo", p.Sexo);
                cmd.Parameters.AddWithValue("@Direccion", p.Direccion);
                cmd.Parameters.AddWithValue("@Telefono", p.Telefono);
                cmd.Parameters.AddWithValue("@Correo", p.Correo);
                cmd.Parameters.AddWithValue("@Peso", p.Peso);
                cmd.Parameters.AddWithValue("@Altura", p.Altura);

                cmd.ExecuteNonQuery();
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                string dui = txtDUI.Text.Trim();

                if (string.IsNullOrEmpty(dui))
                {
                    MessageBox.Show("Por favor, ingrese el DUI del paciente que desea eliminar.");
                    return;
                }

                // Busca al paciente en la lista enlazada
                Paciente pacienteAEliminar = listaPacientes.BuscarPorDUI(dui);

                if (pacienteAEliminar == null)
                {
                    MessageBox.Show("No se encontró ningún paciente con el DUI proporcionado.");
                    return;
                }

                // Lo Elimina de la lista enlazada
                listaPacientes.EliminarPaciente(pacienteAEliminar);

                // Elimina el dato de la base de datos
                EliminarPacienteDeBD(dui);

                // Refresca el DataGridView
                MostrarPacientesEnGrid();

                MessageBox.Show("Paciente eliminado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar paciente: " + ex.Message);
            }

        }

        private void EliminarPacienteDeBD(string dui)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                string query = "DELETE FROM Pacientes WHERE DUI = @DUI";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DUI", dui);

                int filasAfectadas = cmd.ExecuteNonQuery();
                if (filasAfectadas == 0)
                {
                    MessageBox.Show("No se encontró el paciente en la base de datos para eliminar.");
                }
            }
        }

        private void dgvRegistro_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verifica que la fila no sea la de "nueva fila"
            if (e.RowIndex >= 0 && !dgvRegistro.Rows[e.RowIndex].IsNewRow)
            {
                DataGridViewRow fila = dgvRegistro.Rows[e.RowIndex];

                txtDUI.Text = fila.Cells["DUI"].Value?.ToString() ?? "";
                txtNombre.Text = fila.Cells["Nombres"].Value?.ToString() ?? "";
                txtApellidos.Text = fila.Cells["Apellidos"].Value?.ToString() ?? "";
                cmbSexo.SelectedItem = fila.Cells["Sexo"].Value?.ToString() ?? "";
                dtpNacimiento.Value = DateTime.TryParse(fila.Cells["FechaNacimiento"].Value?.ToString(), out DateTime fecha) ? fecha : DateTime.Today;
                txtTelefono.Text = fila.Cells["Telefono"].Value?.ToString() ?? "";
                txtCorreo.Text = fila.Cells["Correo"].Value?.ToString() ?? "";
                txtDireccion.Text = fila.Cells["Direccion"].Value?.ToString() ?? "";
                nudPeso.Value = decimal.TryParse(fila.Cells["Peso"].Value?.ToString(), out decimal peso) ? peso : nudPeso.Minimum;
                nudAltura.Value = decimal.TryParse(fila.Cells["Altura"].Value?.ToString(), out decimal altura) ? altura : nudAltura.Minimum;
            }
        }


    }
}
