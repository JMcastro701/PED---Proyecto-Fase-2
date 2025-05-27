using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_de_Cátedra_PED.ManejoBD
{
    internal class PersonalMedicoDAO
    {

        public static string ObtenerNombreMedico(int codigoMedico)
        {
            string nombreCompleto = string.Empty;

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT Nombre FROM Personal WHERE Id = @CodigoMedico";

                SqlCommand command = new SqlCommand(query, conexion);

                command.Parameters.AddWithValue("@CodigoMedico", codigoMedico);


                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    nombreCompleto = reader["Nombre"].ToString();


                }

                command.Dispose();
                reader.Dispose();

            }
            return nombreCompleto;
        }
            public static string ObtenerEspecialidadMedico(int codigoMedico)
        {
            string especialidad = string.Empty;

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT Especialidad FROM Personal WHERE Id = @CodigoMedico";

                SqlCommand command = new SqlCommand(query, conexion);

                command.Parameters.AddWithValue("@CodigoMedico", codigoMedico);


                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    especialidad = reader["Especialidad"].ToString();


                }

                command.Dispose();
                reader.Dispose();

            }

            return especialidad;
        }

        //ExisteMedicoConCodigo

        public static bool ExisteMedicoConId(int idMedico)
        {
            bool existe = false;    


            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT 1 FROM Personal WHERE Id = @Id";

                SqlCommand command = new SqlCommand(query, conexion);
                
                    command.Parameters.AddWithValue("@Id", idMedico);


                    SqlDataReader reader = command.ExecuteReader();
                    
                        if (reader.Read())
                        {
                            existe = true;
                        }
                    reader.Dispose();
                command.Dispose();
                
            }

            return existe;
        }

        public static void CargarPersonalEnComboBox(ComboBox comboPersonal)
        {
            try
            {
           
                using (SqlConnection connection = ConexionBD.ObtenerConexion())
                {
                    //Obtenemos tanto el Nombre como el Id para ser usado al almacenar el nuevo reporte

                    string query = "SELECT Id, Nombre FROM Personal ORDER BY Nombre";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Limpiar items existentes en el ComboBox
                            comboPersonal.Items.Clear();

                            // Configurar el ComboBox
                            comboPersonal.DisplayMember = "Text";
                            comboPersonal.ValueMember = "Value";

                            // Llenar el ComboBox
                            while (reader.Read())
                            {
                                comboPersonal.Items.Add(new
                                {
                                    Text = reader["Nombre"].ToString(),
                                    Value = reader["Id"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el personal: " + ex.Message);
            }
        }


    }
}
