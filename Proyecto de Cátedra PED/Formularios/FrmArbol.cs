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
using Proyecto_de_Cátedra_PED.Clases.Clases_Arbol;
using System.Text.RegularExpressions;

namespace Proyecto_de_Cátedra_PED.Formularios
{
    public partial class FrmArbol : Form
    {
        private FrmMenuP menu;

        private ArbolEspecialidades miArbol = new ArbolEspecialidades();  // Instancia de tu árbol AVL

        private int ultimaPosicionCentralX = 0; // De CargarYMostrarArbolAVL

        private int offsetXCentrado = 0;
        private bool centrarRaizPendiente = true;
        private int centroRaizReal = 0;

        private Point? posicionScrollPersonalizada = null;

        string especialidadAResaltar = "";
        bool mostrarMensajeExito = false; // btnInsertar.
        bool modoEliminacion = false; // btnEliminar.

        private NodoEspecialidad nodoAResaltar = null;
        private bool mostrarResaltado = false;
        private int parpadeosRealizados = 0;
        private int totalParpadeos = 4; // 2 segundos si intervalo = 500ms
        bool parpadeoPorError = false;

        int idSeleccionado = -1;
        private string nombreOriginal = "";
        private string especialidadOriginal = "";

        private bool salirAplicacion = true; // Por defecto, se asume que es un cierre normal
        private bool confirmandoSalida = false;

        public FrmArbol(FrmMenuP frmMenu)
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

        private void btnCerrar_Click(object sender, EventArgs e)
        {
          
                DialogResult resultado = MessageBox.Show(
                    "¿Está seguro que desea salir de la aplicación?",
                    "Confirmar salida",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado != DialogResult.No)
                {
                    Application.ExitThread();
                }
            
            
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

        private void FrmArbol_Load(object sender, EventArgs e)
        {
            pnlArbol.AutoScroll = true;
            pnlArbol.AutoSize = false;
            CargarYMostrarArbolAVL();
            CargarEspecialidadesEnComboBox();
        }

        private void pnlArbol_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (miArbol == null || miArbol.Raiz == null)
                return;

            int espacioBase = 100;
            int verticalStart = 20;

            int centroInicial = pnlArbol.Width / 2;

            int minX = centroInicial, maxX = centroInicial, maxY = verticalStart;
            CalcularLimites(miArbol.Raiz, centroInicial, verticalStart, espacioBase, ref minX, ref maxX, ref maxY, 0);

            int margen = 50;
            int ancho = maxX - minX + 2 * margen;
            int alto = maxY + margen;

            pnlArbol.AutoScrollMinSize = new Size(ancho, alto);

            offsetXCentrado = -minX + margen;

            centroRaizReal = centroInicial + offsetXCentrado;

            if (posicionScrollPersonalizada.HasValue)
            {
                pnlArbol.AutoScrollPosition = new Point(
                    Math.Abs(posicionScrollPersonalizada.Value.X),
                    Math.Abs(posicionScrollPersonalizada.Value.Y)
                );
                posicionScrollPersonalizada = null;
            }

            else if (centrarRaizPendiente)
            {
                pnlArbol.AutoScrollPosition = new Point(
                    Math.Max(0, centroRaizReal - pnlArbol.ClientSize.Width / 2),
                    0
                );
                centrarRaizPendiente = false;
            }

            e.Graphics.TranslateTransform(
                pnlArbol.AutoScrollPosition.X + offsetXCentrado,
                pnlArbol.AutoScrollPosition.Y
            );

            minX = centroInicial; maxX = centroInicial; maxY = verticalStart;
            DibujarNodo(e.Graphics, miArbol.Raiz, centroInicial, verticalStart, espacioBase, ref minX, ref maxX, ref maxY, 0);
        }

        private bool ObtenerPosicionNodo(NodoEspecialidad nodo, string especialidadBuscada,
       int x, int y, int espacioBase, int nivel, out Point posicion, out NodoEspecialidad nodoEncontrado)
        {
            posicion = Point.Empty;
            nodoEncontrado = null;

            if (nodo == null)
                return false;

            int spacing = espacioBase * (int)Math.Pow(2, 3 - nivel);
            spacing = Math.Max(spacing, 80);
            int verticalSpacing = 120;

            if (nodo.Especialidad.Equals(especialidadBuscada, StringComparison.OrdinalIgnoreCase))
            {
                posicion = new Point(x, y);
                nodoEncontrado = nodo;
                return true;
            }

            if (ObtenerPosicionNodo(nodo.Izquierda, especialidadBuscada, x - spacing, y + verticalSpacing,
                espacioBase, nivel + 1, out posicion, out nodoEncontrado))
                return true;

            if (ObtenerPosicionNodo(nodo.Derecha, especialidadBuscada, x + spacing, y + verticalSpacing,
                espacioBase, nivel + 1, out posicion, out nodoEncontrado))
                return true;

            return false;
        }// Usado en: btnIngresar (validación 4), btnIngresar (éxito), btnEliminar, btnBuscar, ObtenerPosicionNodo(nodo.Izquierda), ObtenerPosicionNodo(nodo.Derecha)

        private void ActivarControles(bool estado)
        {
            // Desactiva o activa según el valor de `estado`
           
            txtEspecialidad.Enabled = estado;
            btnIngresar.Enabled = estado;
            btnEliminar.Enabled = estado;
            btnBuscar.Enabled = estado;
            btnLimpiar.Enabled = estado;           
            txtPersonal.Enabled = estado;
            cmbEspecialidad.Enabled = estado;
            btnIngresarPer.Enabled = estado;
            btnBuscarPer.Enabled = estado;
            btnLimpiarPer.Enabled = estado;
            btnEliminarPer.Enabled = estado;
            btnActualizar.Enabled = estado;
            pnlArbol.Enabled = estado;
            dgvEspPer.Enabled = estado;
        }// Usado en: btnIngresar, btnIngresar, btnEliminar, btnBuscar, TimerParpadeo. 

        private void CargarEspecialidadesEnComboBox()
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT Id, Especialidad FROM Especialidades ORDER BY Especialidad ASC", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                DataRow row = dt.NewRow();
                row["Id"] = 0;
                row["Especialidad"] = "";
                dt.Rows.InsertAt(row, 0);

                cmbEspecialidad.DataSource = dt;
                cmbEspecialidad.DisplayMember = "Especialidad";
                cmbEspecialidad.ValueMember = "Id";
                cmbEspecialidad.SelectedIndex = 0;
            }
        } // Configuración del cmbEspecialidad. Usado en: Especialidades_Load, btnIngresar, btnEliminar.

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string especialidad = txtEspecialidad.Text.Trim();
            errorProvider1.Clear(); // Limpia errores anteriores

            // 1. Validar que la especialidad no esté vacía
            if (string.IsNullOrEmpty(especialidad))
            {
                errorProvider1.SetError(txtEspecialidad, "Debe ingresar una especialidad.");
                txtEspecialidad.Clear();
                txtEspecialidad.Focus();
                return;
            }

            // 2. Validar que la especialidad no contenga números
            if (especialidad.Any(char.IsDigit))
            {
                errorProvider1.SetError(txtEspecialidad, "La especialidad no puede contener números.");
                txtEspecialidad.Clear();
                txtEspecialidad.Focus();
                return;
            }

            // 3. Validar que la especialidad no contenga símbolos
            if (System.Text.RegularExpressions.Regex.IsMatch(especialidad, @"[^a-zA-ZáéíóúÁÉÍÓÚ ]"))
            {
                errorProvider1.SetError(txtEspecialidad, "La especialidad no puede contener símbolos.");
                txtEspecialidad.Clear();
                txtEspecialidad.Focus();
                return;
            }

            // 4. Validar que la especialidad no esté repetida
            if (EspecialidadRepetida(especialidad))
            {
                // Buscar la posición del nodo ya existente
                int centroInicial = pnlArbol.Width / 2;
                int verticalStart = 20;
                int espacioBase = 100;

                if (ObtenerPosicionNodo(miArbol.Raiz, especialidad, centroInicial, verticalStart, espacioBase, 0, out Point posNodo, out NodoEspecialidad nodo))
                {
                    nodoAResaltar = nodo;
                    mostrarResaltado = true;
                    parpadeosRealizados = 0;
                    ActivarControles(false);
                    parpadeoPorError = true; // << NUEVO

                    int scrollX = posNodo.X + offsetXCentrado - pnlArbol.ClientSize.Width / 2;
                    int scrollY = posNodo.Y - pnlArbol.ClientSize.Height / 2;
                    posicionScrollPersonalizada = new Point(Math.Max(0, scrollX), Math.Max(0, scrollY));

                    TimerParpadeo.Start();
                    pnlArbol.Invalidate();
                }

                errorProvider1.SetError(txtEspecialidad, "Esta especialidad ya está registrada.");
                txtEspecialidad.Clear();
                txtEspecialidad.Focus();
                return;
            }

            // Si pasa todas las validaciones, insertar
            try
            {
                using (SqlConnection conn = Conexion.ObtenerConexion())
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    string sql = "INSERT INTO Especialidades (Especialidad) VALUES (@Especialidad)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Especialidad", especialidad);
                        int filas = cmd.ExecuteNonQuery();

                        if (filas > 0)
                        {
                            // Guardar la especialidad recién insertada
                            especialidadAResaltar = especialidad;
                            mostrarMensajeExito = true;

                            // Recargar árbol y combo
                            CargarYMostrarArbolAVL();
                            CargarEspecialidadesEnComboBox();

                            // Buscar el nodo recién insertado
                            int centroInicial = pnlArbol.Width / 2;
                            int verticalStart = 20;
                            int espacioBase = 100;

                            if (ObtenerPosicionNodo(miArbol.Raiz, especialidadAResaltar, centroInicial, verticalStart, espacioBase, 0, out Point posNodo, out NodoEspecialidad nodo))
                            {
                                nodoAResaltar = nodo;
                                mostrarResaltado = true;
                                parpadeosRealizados = 0;
                                ActivarControles(false); // Desactivar controles

                                int scrollX = posNodo.X + offsetXCentrado - pnlArbol.ClientSize.Width / 2;
                                int scrollY = posNodo.Y - pnlArbol.ClientSize.Height / 2;
                                posicionScrollPersonalizada = new Point(Math.Max(0, scrollX), Math.Max(0, scrollY));

                                TimerParpadeo.Start();
                                pnlArbol.Invalidate();
                            }

                            txtEspecialidad.Clear();
                            txtEspecialidad.Focus();
                        }
                        else
                        {
                            MessageBox.Show("No se insertó ninguna fila.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar en la base de datos: " + ex.Message);
            }
        }
        private bool EspecialidadRepetida(string especialidad)
        {
            bool repetida = false;

            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string sql = "SELECT COUNT(*) FROM Especialidades WHERE Especialidad = @Especialidad";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Especialidad", especialidad);
                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                    {
                        repetida = true;
                    }
                }
            }

            return repetida;
        } // Uso en: btnIngresar.

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            string especialidad = txtEspecialidad.Text.Trim();
            errorProvider1.Clear();

            // Validaciones
            if (string.IsNullOrEmpty(especialidad))
            {
                errorProvider1.SetError(txtEspecialidad, "Debe ingresar una especialidad.");
                txtEspecialidad.Clear();
                txtEspecialidad.Focus();
                return;
            }

            if (especialidad.Any(char.IsDigit))
            {
                errorProvider1.SetError(txtEspecialidad, "La especialidad no puede contener números.");
                txtEspecialidad.Clear();
                txtEspecialidad.Focus();
                return;
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(especialidad, @"[^a-zA-ZáéíóúÁÉÍÓÚ ]"))
            {
                errorProvider1.SetError(txtEspecialidad, "La especialidad no puede contener símbolos.");
                txtEspecialidad.Clear();
                txtEspecialidad.Focus();
                return;
            }

            if (!EspecialidadExiste(especialidad))
            {
                errorProvider1.SetError(txtEspecialidad, "La especialidad no existe.");
                txtEspecialidad.Clear();
                txtEspecialidad.Focus();
                return;
            }

            // Buscar el nodo para centrar y parpadear
            int centroInicial = pnlArbol.Width / 2;
            int verticalStart = 20;
            int espacioBase = 100;

            if (ObtenerPosicionNodo(miArbol.Raiz, especialidad, centroInicial, verticalStart, espacioBase, 0, out Point posNodo, out NodoEspecialidad nodo))
            {
                especialidadAResaltar = especialidad;
                nodoAResaltar = nodo;
                mostrarResaltado = true;
                parpadeosRealizados = 0;
                mostrarMensajeExito = false;
                modoEliminacion = true;

                ActivarControles(false); // Desactivar mientras parpadea

                int scrollX = posNodo.X + offsetXCentrado - pnlArbol.ClientSize.Width / 2;
                int scrollY = posNodo.Y - pnlArbol.ClientSize.Height / 2;
                posicionScrollPersonalizada = new Point(Math.Max(0, scrollX), Math.Max(0, scrollY));

                TimerParpadeo.Start();
                pnlArbol.Invalidate();
            }
        }

        private bool EspecialidadExiste(string especialidad)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string sql = "SELECT COUNT(*) FROM Especialidades WHERE Especialidad = @Especialidad";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Especialidad", especialidad);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }// Uso en: btnEliminar.

        private void EliminarEspecialidadYPersonal(string especialidad)
        {
            try
            {
                using (SqlConnection conn = Conexion.ObtenerConexion())
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        string sqlDeletePersonal = "DELETE FROM Personal WHERE Especialidad = @Especialidad";
                        using (SqlCommand cmd = new SqlCommand(sqlDeletePersonal, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Especialidad", especialidad);
                            cmd.ExecuteNonQuery();
                        }

                        string sqlDeleteEspecialidad = "DELETE FROM Especialidades WHERE Especialidad = @Especialidad";
                        using (SqlCommand cmd = new SqlCommand(sqlDeleteEspecialidad, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@Especialidad", especialidad);
                            int filas = cmd.ExecuteNonQuery();

                            if (filas > 0)
                            {
                                transaction.Commit();
                                MessageBox.Show("Especialidad y personal asociado eliminados con éxito.");
                                miArbol = new ArbolEspecialidades();
                                CargarYMostrarArbolAVL();
                                pnlArbol.Invalidate();
                                CargarEspecialidadesEnComboBox();
                                txtEspecialidad.Clear();
                                txtEspecialidad.Focus();
                            }
                            else
                            {
                                transaction.Rollback();
                                MessageBox.Show("No se eliminó ninguna fila.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Error al eliminar: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar de la base de datos: " + ex.Message);
            }
        }// Usado en: btnEliminar.

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string especialidadBuscada = txtEspecialidad.Text.Trim();
            errorProvider1.Clear(); // Limpia errores anteriores

            // Validación 1: Campo vacío
            if (string.IsNullOrEmpty(especialidadBuscada))
            {
                errorProvider1.SetError(txtEspecialidad, "Debe ingresar una especialidad.");
                txtEspecialidad.Clear();
                txtEspecialidad.Focus();
                return;
            }

            // Validación 2 y 3: Contiene números/símbolos.
            if (!Regex.IsMatch(especialidadBuscada, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                errorProvider1.SetError(txtEspecialidad, "Solo se permiten letras (sin números ni símbolos).");
                txtEspecialidad.Clear();
                txtEspecialidad.Focus();
                return;
            }

            // Validación extra: Validar si el árbol está creado.
            if (miArbol == null || miArbol.Raiz == null)
            {
                errorProvider1.SetError(txtEspecialidad, "No hay árbol cargado.");
                txtEspecialidad.Clear();
                txtEspecialidad.Focus();
                return;
            }

            // Validación 4: Buscar si la especialidad existe en el árbol.
            int espacioBase = 100;
            int verticalStart = 20;
            int centroInicial = pnlArbol.Width / 2;

            if (ObtenerPosicionNodo(miArbol.Raiz, especialidadBuscada, centroInicial, verticalStart, espacioBase, 0, out Point posNodo, out NodoEspecialidad nodoEncontrado))
            {
                // Ajustar scroll al nodo encontrado
                int scrollX = posNodo.X + offsetXCentrado - pnlArbol.ClientSize.Width / 2;
                int scrollY = posNodo.Y - pnlArbol.ClientSize.Height / 2;

                posicionScrollPersonalizada = new Point(
                    Math.Max(0, scrollX),
                    Math.Max(0, scrollY)
                );

                // Iniciar parpadeo.
                especialidadAResaltar = especialidadBuscada;
                nodoAResaltar = nodoEncontrado;
                mostrarResaltado = true;
                parpadeosRealizados = 0;
                ActivarControles(false); // Desactiva controles momentáneamente.
                TimerParpadeo.Start();
                pnlArbol.Invalidate();
            }
            else
            {
                errorProvider1.SetError(txtEspecialidad, "La especialidad no se encuentra en el árbol.");
                txtEspecialidad.Clear();
                txtEspecialidad.Focus();
            }
        }

        private void TimerParpadeo_Tick(object sender, EventArgs e)
        {
            parpadeosRealizados++;

            // alternar visibilidad para forzar cambio de color
            mostrarResaltado = !mostrarResaltado;
            pnlArbol.Invalidate();

            if (parpadeosRealizados >= totalParpadeos * 2)
            {
                TimerParpadeo.Stop();
                mostrarResaltado = false;
                nodoAResaltar = null;
                pnlArbol.Invalidate();
                ActivarControles(true);

                if (parpadeoPorError)
                {
                    // Solo fue un parpadeo por error, no mostrar mensaje
                    parpadeoPorError = false;
                    return;
                }

                if (modoEliminacion)
                {
                    DialogResult resultado = MessageBox.Show(
                        $"¿Está seguro de que desea eliminar la especialidad \"{especialidadAResaltar}\"?",
                        "Confirmar eliminación",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning
                    );

                    if (resultado == DialogResult.Yes)
                    {
                        EliminarEspecialidadYPersonal(especialidadAResaltar);
                    }

                    modoEliminacion = false;
                }
                else if (mostrarMensajeExito)
                {
                    MessageBox.Show("Especialidad guardada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    mostrarMensajeExito = false;
                }
                else
                {
                    MostrarEspecialidadesPorEspecialidad(especialidadAResaltar);
                    MessageBox.Show("Especialidad encontrada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void MostrarEspecialidadesPorEspecialidad(string especialidad)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string sql = @"SELECT Id, Nombre, Especialidad
                       FROM Personal
                       WHERE LTRIM(RTRIM(Especialidad)) COLLATE Latin1_General_CI_AI = LTRIM(RTRIM(@Especialidad)) COLLATE Latin1_General_CI_AI";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Especialidad", especialidad.Trim());
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("No se encontró personal con esa especialidad.");
                    }

                    dgvEspPer.DataSource = dt;
                    if (dgvEspPer.Columns.Contains("Id"))
                    {
                        dgvEspPer.Columns["Id"].Visible = false;
                    }
                }
            }

            ConfigurarDataGridView();
            SeleccionarColumnaEspecialidad();

            dgvEspPer.ClearSelection();
            dgvEspPer.CurrentCell = null;

            if (dgvEspPer.Columns.Contains("Especialidad"))
            {
                int colIndex = dgvEspPer.Columns["Especialidad"].Index;

                foreach (DataGridViewRow row in dgvEspPer.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        row.Cells[colIndex].Selected = true;
                    }
                }
            }
        }// Usado en: TimerParpadeo (guardada), TimerParpadeo (encontrar).

        private void SeleccionarColumnaEspecialidad()
        {
            // Solo para selección de una columna
            dgvEspPer.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvEspPer.ClearSelection();
            dgvEspPer.CurrentCell = null;

            if (!dgvEspPer.Columns.Contains("Especialidad"))
                return;

            int colIndex = dgvEspPer.Columns["Especialidad"].Index;

            foreach (DataGridViewRow fila in dgvEspPer.Rows)
            {
                if (!fila.IsNewRow && fila.Cells[colIndex].Visible)
                {
                    fila.Cells[colIndex].Selected = true;
                }
            }
        }// Usado en: MostrarEspecialidadesPorEspecialidad.

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Limpia errores anteriores
            errorProvider1.Clear();

            if (!string.IsNullOrWhiteSpace(txtEspecialidad.Text))
            {
                DialogResult resultado = MessageBox.Show(
                    "¿Está seguro de que desea borrar lo escrito?",
                    "Confirmar limpieza",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (resultado == DialogResult.Yes)
                {
                    txtEspecialidad.Clear();
                    txtEspecialidad.Focus();
                }
            }
            else
            {
                // Usa el ErrorProvider en lugar del MessageBox
                errorProvider1.SetError(txtEspecialidad, "No hay nada que borrar.");
                txtEspecialidad.Focus();
            }
        }

        private void btnIngresarPer_Click(object sender, EventArgs e)
        {
            string nombreDoctor = txtPersonal.Text.Trim();
            string especialidadDoctor = cmbEspecialidad.Text.Trim();
            errorProvider1.Clear();

            // Validación 1: Campo vacío
            if (string.IsNullOrEmpty(nombreDoctor))
            {
                errorProvider1.SetError(txtPersonal, "Debe ingresar un nombre.");
                txtPersonal.Clear();
                txtPersonal.Focus();
                return;
            }

            // Validación 2: Contiene números
            if (nombreDoctor.Any(char.IsDigit))
            {
                errorProvider1.SetError(txtPersonal, "El nombre no puede contener números.");
                txtPersonal.Clear();
                txtPersonal.Focus();
                return;
            }

            // Validación 3: Contiene símbolos
            if (System.Text.RegularExpressions.Regex.IsMatch(nombreDoctor, @"[^a-zA-ZáéíóúÁÉÍÓÚ\s]"))
            {
                errorProvider1.SetError(txtPersonal, "El nombre no puede contener símbolos.");
                txtPersonal.Clear();
                txtPersonal.Focus();
                return;
            }

            // Validación 4: Especialidad seleccionada
            if (cmbEspecialidad.SelectedIndex <= 0)
            {
                errorProvider1.SetError(cmbEspecialidad, "Debe seleccionar una especialidad.");
                cmbEspecialidad.Focus();
                return;
            }

            // Validación 5: Nombre repetido en la tabla Personal
            if (NombrePersonalExiste(nombreDoctor))
            {
                errorProvider1.SetError(txtPersonal, "Este nombre ya está registrado.");
                txtPersonal.Focus();
                MostrarDoctorEntreDoctores(nombreDoctor); // función que llenará el dgv

                return;
            }

            // Validación 6: nombre registrado en otra especialidad
            if (NombreRegistradoConOtraEspecialidad(nombreDoctor, especialidadDoctor))
            {
                errorProvider1.SetError(txtPersonal, "Este médico ya está registrado, pero en una especialidad distinta.");
                txtPersonal.Focus();
                MostrarDoctorEntreDoctores(nombreDoctor); // Opcional, si quieres mostrar en qué especialidad está
                return;
            }


            // Obtener el nombre de la especialidad seleccionada
            string especialidadNombre = cmbEspecialidad.Text;

            // 1. Insertar en la tabla Personal
            try
            {
                using (SqlConnection conn = Conexion.ObtenerConexion())
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    string sql = "INSERT INTO Personal (Nombre, Especialidad) VALUES (@Nombre, @Especialidad)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombreDoctor);
                        cmd.Parameters.AddWithValue("@Especialidad", especialidadNombre);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Personal registrado con éxito.");
                        txtPersonal.Clear();
                        cmbEspecialidad.SelectedIndex = 0;
                        txtPersonal.Focus();


                        // Mostrar solo los doctores de esa especialidad en dgvEspPer y seleccionar el nuevo
                        MostrarDoctoresPorEspecialidad(especialidadNombre, nombreDoctor);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar el personal: " + ex.Message);
            }
        }

        private void MostrarDoctoresPorEspecialidad(string especialidad, string nombreDoctorSeleccionado = "")
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string sql = "SELECT Nombre, Especialidad FROM Personal WHERE Especialidad = @Especialidad";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Especialidad", especialidad);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvEspPer.DataSource = dt;
                }
            }

            // Configura DataGridView
            dgvEspPer.MultiSelect = true;
            ConfigurarDataGridView();

            // Selecciona todas las celdas de la columna "Especialidad"
            dgvEspPer.ClearSelection();
            if (dgvEspPer.Columns.Contains("Especialidad"))
            {
                int colIndex = dgvEspPer.Columns["Especialidad"].Index;
                for (int i = 0; i < dgvEspPer.Rows.Count; i++)
                {
                    dgvEspPer.Rows[i].Cells[colIndex].Selected = true;
                }
            }

            // Seleccionar la fila del doctor recién añadido
            if (!string.IsNullOrEmpty(nombreDoctorSeleccionado))
            {
                foreach (DataGridViewRow row in dgvEspPer.Rows)
                {
                    if (row.Cells["Nombre"].Value?.ToString() == nombreDoctorSeleccionado)
                    {
                        row.Selected = true;
                        dgvEspPer.CurrentCell = row.Cells[0]; // Enfoca la celda
                        break;
                    }
                }
            }
        }// Uso en: btnIngresarPer. Inserción exitosa.

        private bool NombrePersonalExiste(string nombre)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string sql = "SELECT COUNT(*) FROM Personal WHERE Nombre = @Nombre";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }// Uso en: btnIngresarPer (validación5).

        private bool NombreRegistradoConOtraEspecialidad(string nombre, string especialidad)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string sql = "SELECT COUNT(*) FROM Personal WHERE Nombre = @Nombre AND Especialidad <> @Especialidad";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombre);
                    cmd.Parameters.AddWithValue("@Especialidad", especialidad);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }// Uso en: btnIngresarPer (validacion 6).

        private void MostrarDoctorEntreDoctores(string nombreDoctor)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                // 1. Obtener especialidad del doctor
                string especialidad = "";
                using (SqlCommand cmd = new SqlCommand("SELECT Especialidad FROM Personal WHERE Nombre = @Nombre", conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nombreDoctor);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        especialidad = result.ToString();
                    }
                    else
                    {
                        MessageBox.Show("Doctor no encontrado en la base de datos.", "Doctor no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }

                // 2. Mostrar todos los doctores de esa especialidad
                string sql = "SELECT Id, Nombre AS Doctor, Especialidad FROM Personal WHERE Especialidad = @Especialidad ORDER BY Nombre";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Especialidad", especialidad);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvEspPer.DataSource = dt;
                    // Ocultar columna Id (si existe)
                    if (dgvEspPer.Columns.Contains("Id"))
                        dgvEspPer.Columns["Id"].Visible = false;

                    // Forzar el orden correcto: Doctor (0), Especialidad (1)
                    if (dgvEspPer.Columns.Contains("Doctor"))
                        dgvEspPer.Columns["Doctor"].DisplayIndex = 0;

                    if (dgvEspPer.Columns.Contains("Especialidad"))
                        dgvEspPer.Columns["Especialidad"].DisplayIndex = 1;
                }

                // 3. Aplicar configuración
                ConfigurarDataGridView();

                // 4. Buscar y seleccionar la fila del doctor
                foreach (DataGridViewRow row in dgvEspPer.Rows)
                {
                    string valorCelda = row.Cells["Doctor"].Value?.ToString().Trim();
                    if (valorCelda.Equals(nombreDoctor.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        row.Selected = true;
                        // Buscar la primera celda visible
                        foreach (DataGridViewCell celda in row.Cells)
                        {
                            if (celda.Visible)
                            {
                                dgvEspPer.CurrentCell = celda;
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }// Uso en: btnIngresarPer (validación 5), btnIngresarPer (validación 6), btnActualizar.

        private void btnBuscarPer_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            string nombre = txtPersonal.Text.Trim();
            string especialidad = cmbEspecialidad.Text.Trim();

            // Caso 1: Nombre y especialidad ingresados
            if (!string.IsNullOrEmpty(nombre) && !string.IsNullOrEmpty(especialidad))
            {
                using (SqlConnection conn = Conexion.ObtenerConexion())
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    string sql = "SELECT Nombre, Especialidad FROM Personal WHERE LTRIM(RTRIM(Nombre)) COLLATE Latin1_General_CI_AI = LTRIM(RTRIM(@Nombre)) COLLATE Latin1_General_CI_AI";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                        {
                            dgvEspPer.DataSource = null;
                            errorProvider1.SetError(txtPersonal, "No se encontró ningún médico con ese nombre.");
                            txtPersonal.Focus();
                            return;
                        }

                        dgvEspPer.DataSource = dt;
                        ConfigurarDataGridView();
                        dgvEspPer.SelectionMode = DataGridViewSelectionMode.FullRowSelect; // Asegura selección de fila completa
                        dgvEspPer.ClearSelection();                                        // Limpia selección previa
                        dgvEspPer.Rows[0].Selected = true;                                 // Selecciona la fila
                        dgvEspPer.CurrentCell = dgvEspPer.Rows[0].Cells[0];                // Opcional: muestra activamente la celda

                        string especialidadReal = dt.Rows[0]["Especialidad"].ToString().Trim();

                        if (!especialidadReal.Equals(especialidad, StringComparison.OrdinalIgnoreCase))
                        {
                            errorProvider1.SetError(cmbEspecialidad, "Médico encontrado, pero no en la especialidad buscada.");

                            // Nueva consulta por la especialidad real
                            string sqlEspecialidad = "SELECT Nombre, Especialidad FROM Personal WHERE Especialidad = @EspecialidadReal";
                            using (SqlCommand cmdEspecialidad = new SqlCommand(sqlEspecialidad, conn))
                            {
                                cmdEspecialidad.Parameters.AddWithValue("@EspecialidadReal", especialidadReal);
                                SqlDataAdapter daEsp = new SqlDataAdapter(cmdEspecialidad);
                                DataTable dtEsp = new DataTable();
                                daEsp.Fill(dtEsp);

                                if (dtEsp.Rows.Count > 0)
                                {
                                    dgvEspPer.DataSource = dtEsp;
                                    ConfigurarDataGridView();
                                    dgvEspPer.ClearSelection();

                                    // Buscar el médico por nombre y seleccionar su fila
                                    for (int i = 0; i < dtEsp.Rows.Count; i++)
                                    {
                                        string nombreFila = dtEsp.Rows[i]["Nombre"].ToString().Trim();
                                        if (nombreFila.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                                        {
                                            dgvEspPer.Rows[i].Selected = true;
                                            dgvEspPer.CurrentCell = dgvEspPer.Rows[i].Cells[0];
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Caso 2: Solo nombre
            // Caso 2: Solo nombre
            else if (!string.IsNullOrEmpty(nombre))
            {
                using (SqlConnection conn = Conexion.ObtenerConexion())
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    string sql = "SELECT Nombre, Especialidad FROM Personal WHERE LTRIM(RTRIM(Nombre)) COLLATE Latin1_General_CI_AI LIKE '%' + LTRIM(RTRIM(@Nombre)) + '%' COLLATE Latin1_General_CI_AI";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                        {
                            dgvEspPer.DataSource = null;
                            errorProvider1.SetError(txtPersonal, "No se encontró ningún médico con ese nombre.");
                            txtPersonal.Focus();
                            return;
                        }

                        dgvEspPer.DataSource = dt;
                        ConfigurarDataGridView();
                        dgvEspPer.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                        dgvEspPer.ClearSelection();

                        string nombreBuscar = nombre.ToLower();
                        int coincidencias = 0;

                        for (int i = 0; i < dgvEspPer.Rows.Count; i++)
                        {
                            string nombreFila = dgvEspPer.Rows[i].Cells["Nombre"].Value.ToString().ToLower();
                            if (nombreFila.Contains(nombreBuscar))
                            {
                                dgvEspPer.Rows[i].Selected = true;
                                if (dgvEspPer.CurrentCell == null)
                                    dgvEspPer.CurrentCell = dgvEspPer.Rows[i].Cells[0];

                                coincidencias++;
                            }
                        }

                        if (coincidencias > 1)
                        {
                            MessageBox.Show($"Se encontraron {coincidencias} médicos con nombres similares.", "Coincidencias encontradas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            // Caso 3: Solo especialidad
            else if (!string.IsNullOrEmpty(especialidad))
            {
                MostrarEspecialidadesPorEspecialidad(especialidad);
            }
            else
            {
                errorProvider1.SetError(txtPersonal, "Ingrese al menos un nombre para buscar.");
                errorProvider1.SetError(cmbEspecialidad, "Ingrese al menos una especialidad para buscar.");
            }
        }    // Fin btnBuscarPer

        private void btnLimpiarPer_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear(); // Limpia errores previos

            bool hayTexto = !string.IsNullOrWhiteSpace(txtPersonal.Text);
            bool haySeleccion = cmbEspecialidad.SelectedIndex > 0;

            if (hayTexto || haySeleccion)
            {
                DialogResult resultado = MessageBox.Show(
                    "¿Está seguro de que desea borrar lo escrito y seleccionado?",
                    "Confirmar limpieza",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (resultado == DialogResult.Yes)
                {
                    txtPersonal.Clear();
                    cmbEspecialidad.SelectedIndex = 0; // Selecciona la opción en blanco
                    txtPersonal.Focus();
                }
            }
            else
            {
                // Usa el ErrorProvider para avisar que no hay nada que borrar
                if (!hayTexto)
                {
                    errorProvider1.SetError(txtPersonal, "No hay nombre de personal que borrar.");
                }

                if (!haySeleccion)
                {
                    errorProvider1.SetError(cmbEspecialidad, "No hay especialidad seleccionada que borrar.");
                }

                txtPersonal.Focus(); // Lleva el foco al primer campo
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();

            if (idSeleccionado == -1)
            {
                MessageBox.Show(txtPersonal, "Debe seleccionar un registro para actualizar.", "Error de selección", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dgvEspPer.SelectedRows.Count != 1)
            {
                MessageBox.Show("Debe mantener seleccionado un registro para actualizar.", "Error de selección", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string nuevoNombre = txtPersonal.Text.Trim();
            string nuevaEspecialidad = cmbEspecialidad.Text.Trim();

            // VALIDACION: NO VACIO TXTPERSONAL
            if (string.IsNullOrWhiteSpace(nuevoNombre))
            {
                errorProvider1.SetError(txtPersonal, "El nombre no puede estar vacío.");
                return;
            }

            // VALIDACIÓN: NO NUMEROS O SIMBOLOS TXTPERSONAL
            if (!System.Text.RegularExpressions.Regex.IsMatch(nuevoNombre, @"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$"))
            {
                errorProvider1.SetError(txtPersonal, "El nombre solo debe contener letras y espacios.");
                return;
            }

            // VALIDACION: NO VACIO CMBESPECIALIDAD
            if (string.IsNullOrWhiteSpace(nuevaEspecialidad))
            {
                errorProvider1.SetError(cmbEspecialidad, "Debe seleccionar una especialidad.");
                return;
            }

            DataGridViewRow filaSeleccionada = dgvEspPer.SelectedRows[0];

            // Obtener el nombre correcto de la columna para el nombre
            string nombreColumna = dgvEspPer.Columns.Contains("Doctor") ? "Doctor" : "Nombre";

            if (filaSeleccionada.Cells[nombreColumna].Value?.ToString().Trim() != nombreOriginal ||
                filaSeleccionada.Cells["Especialidad"].Value?.ToString().Trim() != especialidadOriginal)
            {
                MessageBox.Show("Debe mantener seleccionado el registro original que desee actualizar.", "Error de selección", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (nuevoNombre == nombreOriginal && nuevaEspecialidad == especialidadOriginal)
            {
                MessageBox.Show(btnActualizar, "Debe editar el nombre o la especialidad antes de actualizar.", "Error de actualización", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Actualización
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string sql = "UPDATE Personal SET Nombre = @Nombre, Especialidad = @Especialidad WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nuevoNombre);
                    cmd.Parameters.AddWithValue("@Especialidad", nuevaEspecialidad);
                    cmd.Parameters.AddWithValue("@Id", idSeleccionado);

                    int filasAfectadas = cmd.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Registro actualizado correctamente.", "Registro exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MostrarDoctorEntreDoctores(nuevoNombre);
                        txtPersonal.Clear();
                        cmbEspecialidad.SelectedIndex = 0;
                        idSeleccionado = -1;
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar el registro.", "Error de actualización", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnEliminarPer_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();

            if (idSeleccionado == -1)
            {
                MessageBox.Show(dgvEspPer, "Debe seleccionar un registro completo para eliminar.", "Error de selección", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dgvEspPer.SelectedRows.Count != 1)
            {
                MessageBox.Show("Debe mantener seleccionado un registro para eliminar.", "Error de selección", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataGridViewRow filaSeleccionada = dgvEspPer.SelectedRows[0];

            // Verificar que el registro seleccionado siga siendo el original
            string nombreColumna = dgvEspPer.Columns.Contains("Doctor") ? "Doctor" : "Nombre";

            if (filaSeleccionada.Cells[nombreColumna].Value?.ToString().Trim() != nombreOriginal ||
                filaSeleccionada.Cells["Especialidad"].Value?.ToString().Trim() != especialidadOriginal)
            {
                MessageBox.Show("Debe mantener seleccionado el registro original que desea eliminar.", "Error de selección", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Confirmar eliminación
            DialogResult confirmacion = MessageBox.Show("¿Está seguro de que desea eliminar este registro?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmacion == DialogResult.Yes)
            {
                using (SqlConnection conn = Conexion.ObtenerConexion())
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    string sql = "DELETE FROM Personal WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", idSeleccionado);
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("Registro eliminado correctamente.", "Eliminación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Recargar doctores de la misma especialidad
                            CargarPersonalPorEspecialidad(especialidadOriginal);

                            // Limpiar campos y estado
                            txtPersonal.Clear();
                            cmbEspecialidad.SelectedIndex = 0;
                            idSeleccionado = -1;
                        }
                        else
                        {
                            MessageBox.Show("No se pudo eliminar el registro.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }

        private void CargarPersonalPorEspecialidad(string especialidad)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string sql = "SELECT Id, Nombre AS Doctor, Especialidad FROM Personal WHERE Especialidad = @Especialidad ORDER BY Nombre";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Especialidad", especialidad);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvEspPer.DataSource = dt;
                    if (dgvEspPer.Columns.Contains("Id"))
                        dgvEspPer.Columns["Id"].Visible = false;
                }

                ConfigurarDataGridView(); // Aplica estilos y configuraciones comunes al DataGridView
            }
        }// Usado en: btnEliminarPer

        private void FrmArbol_FormClosing(object sender, FormClosingEventArgs e)
        {
            /*
            if (!confirmandoSalida)
            {
                DialogResult resultado = MessageBox.Show(
                    "¿Está seguro que desea salir de la aplicación?",
                    "Confirmar salida",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    confirmandoSalida = true;
                    Application.ExitThread();// No hace falta Application.Exit() aquí, se cerrará naturalmente
                }
            }*/
        }

        private void dgvEspPer_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Establecer modo de selección si aún no está activado
                if (dgvEspPer.SelectionMode != DataGridViewSelectionMode.FullRowSelect)
                {
                    dgvEspPer.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                }

                // Quitar cualquier selección previa
                dgvEspPer.ClearSelection();

                // Seleccionar manualmente la fila completa
                dgvEspPer.Rows[e.RowIndex].Selected = true;

                DataGridViewRow fila = dgvEspPer.Rows[e.RowIndex];

                string nombreColumna = dgvEspPer.Columns.Contains("Doctor") ? "Doctor" : "Nombre";

                if (fila.Cells[nombreColumna].Value != null && fila.Cells["Especialidad"].Value != null)
                {
                    txtPersonal.Text = fila.Cells[nombreColumna].Value.ToString();
                    cmbEspecialidad.Text = fila.Cells["Especialidad"].Value.ToString();

                    nombreOriginal = txtPersonal.Text;
                    especialidadOriginal = cmbEspecialidad.Text;

                    if (dgvEspPer.Columns.Contains("Id"))
                        idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
                }
            }
        }

        private void ConfigurarDataGridView()
        {
            // Eliminar la fila en blanco automática
            dgvEspPer.AllowUserToAddRows = false;

            // Hacer que las columnas llenen todo el ancho del DataGridView
            dgvEspPer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Hacer que las filas ajusten su altura automáticamente
            dgvEspPer.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            DesactivarOrdenColumnas(dgvEspPer);

            // Desactivar selección de encabezados
            dgvEspPer.RowHeadersVisible = false;

            // Desactivar el modo de selección de múltiples filas
            dgvEspPer.MultiSelect = true;

            // Opcional: desactivar edición de celdas por el usuario
            dgvEspPer.ReadOnly = true;

            // Personalizar encabezado de columnas
            dgvEspPer.EnableHeadersVisualStyles = false; // Importante para que se apliquen los estilos manuales
            dgvEspPer.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvEspPer.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvEspPer.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvEspPer.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        } // Configuración general del dgv. Usado en: MostrarDoctoresPorEspecialidad,  MostrarDoctorEntreDoctores, MostrarEspecialidadesPorEspecialidad

        private void DesactivarOrdenColumnas(DataGridView dgv)
        {
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }// Subconfiguracion del dgv. Usado: ConfigurarDataGridView.

        private void DibujarNodo(Graphics g, NodoEspecialidad nodo, int x, int y, int espacioBase, ref int minX, ref int maxX, ref int maxY, int nivel)
        {
            int margen = 50;
            if (nodo == null) return;

            Font fuente = new Font("Arial", 10);
            SizeF size = g.MeasureString(nodo.Especialidad, fuente);
            int anchoNodo = (int)size.Width + 20;  // Ancho del nodo
            int altoNodo = 40;  // Altura del nodo
            int verticalSpacing = 120; // Espaciado vertical ajustado entre nodos

            Brush fondo = Brushes.LightBlue;
            Brush texto = Brushes.Black;

            if (nodo == nodoAResaltar && mostrarResaltado)
            {
                fondo = Brushes.Yellow; // o cualquier color que resalte
            }

            Rectangle rect = new Rectangle(x - anchoNodo / 2, y, anchoNodo, altoNodo);

            g.FillRectangle(fondo, rect);
            g.DrawRectangle(Pens.Black, rect);
            g.DrawString(nodo.Especialidad, fuente, texto, rect.X + 10, rect.Y + 10);

            // Actualizar los límites de desplazamiento
            minX = Math.Min(minX, rect.Left - margen);
            maxX = Math.Max(maxX, rect.Right + margen);
            maxY = Math.Max(maxY, rect.Bottom);

            // Calcular el espaciado en X para los hijos
            int spacing = espacioBase * (int)Math.Pow(2, 3 - nivel); // Aumenta el espaciado para los niveles más profundos

            // Para no dejar que el espaciado se vuelva demasiado pequeño, ajustamos el espaciado mínimo
            spacing = Math.Max(spacing, 80);  // Espaciado mínimo entre nodos

            if (nodo.Izquierda != null)
            {
                int hijoX = x - spacing;
                int hijoY = y + verticalSpacing;
                g.DrawLine(Pens.Black, x, y + altoNodo, hijoX, hijoY);  // Línea hacia el hijo izquierdo
                DibujarNodo(g, nodo.Izquierda, hijoX, hijoY, espacioBase, ref minX, ref maxX, ref maxY, nivel + 1);
            }

            if (nodo.Derecha != null)
            {
                int hijoX = x + spacing;
                int hijoY = y + verticalSpacing;
                g.DrawLine(Pens.Black, x, y + altoNodo, hijoX, hijoY);  // Línea hacia el hijo derecho
                DibujarNodo(g, nodo.Derecha, hijoX, hijoY, espacioBase, ref minX, ref maxX, ref maxY, nivel + 1);
            }
        } // Usado en: pnlArbol_Paint, DibujarNodo(nodo.Izquierda), DibujarNodo(nodo.Derecha)

        private void CalcularLimites(NodoEspecialidad nodo, int x, int y, int espacioBase,
            ref int minX, ref int maxX, ref int maxY, int nivel)
        {
            if (nodo == null) return;

            int anchoNodo = 100; // fijo
            int altoNodo = 40;
            int verticalSpacing = 120;

            Rectangle rect = new Rectangle(x - anchoNodo / 2, y, anchoNodo, altoNodo);

            minX = Math.Min(minX, rect.Left);
            maxX = Math.Max(maxX, rect.Right);
            maxY = Math.Max(maxY, rect.Bottom);

            int spacing = espacioBase * (int)Math.Pow(2, 3 - nivel);
            spacing = Math.Max(spacing, 80);

            CalcularLimites(nodo.Izquierda, x - spacing, y + verticalSpacing, espacioBase, ref minX, ref maxX, ref maxY, nivel + 1);
            CalcularLimites(nodo.Derecha, x + spacing, y + verticalSpacing, espacioBase, ref minX, ref maxX, ref maxY, nivel + 1);
        } // Usado en: pnlArbol, CalcularLimites(nodo.Izquierda), CalcularLimites(nodo.Derecha).

        private void CargarYMostrarArbolAVL()
        {
            List<string> especialidades = new List<string>();

            // 1. Usar using correctamente para asegurar cierre de recursos
            using (SqlConnection conn = Conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand("SELECT Especialidad FROM Especialidades", conn))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open(); // Abre la conexión

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        especialidades.Add(reader.GetString(0));
                }
            }

            // 2. Crear nuevo árbol AVL e insertar
            miArbol = new ArbolEspecialidades();
            foreach (string esp in especialidades)
                miArbol.Insertar(esp);

            // 3. Redibujar árbol
            pnlArbol.Invalidate();

            BeginInvoke((MethodInvoker)delegate
            {
                pnlArbol.AutoScrollPosition = new Point(ultimaPosicionCentralX - pnlArbol.Width / 2, 0);
            });
        } // Usado en: Especialidades_Load, btnIngresar, btnEliminar


    }
}

