using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_de_Cátedra_PED
{
    class Conexion
    {
        public static SqlConnection ObtenerConexion()
        {
            string cadena = @"Server=localhost;Database=HospitalAtencion;Trusted_Connection=True;";
            SqlConnection conexion = new SqlConnection(cadena);

            try
            {
                conexion.Open();
                return conexion;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 4060)
                {
                    MessageBox.Show("❌ No se encontró la base de datos 'HospitalAtencion'. Verifique que exista en el servidor.", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (ex.Number == 53)
                {
                    MessageBox.Show("❌ No se pudo encontrar el servidor SQL. Verifique que el servidor esté encendido y accesible.", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"❌ Error de conexión a la base de datos:\n{ex.Message}", "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return null; // importante: retornar null si no hay conexión
            }
        }

    }
}
