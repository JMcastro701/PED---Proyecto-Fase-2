using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Proyecto_de_Cátedra_PED.Formularios;
using Proyecto_de_Cátedra_PED.Clases.Clases_Lista;
namespace Proyecto_de_Cátedra_PED.ManejoBD
{
    internal class ReportesDAO
    {

        public static void InsertarReporte(Reporte reporte, string dui)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                

                string consulta = @"INSERT INTO HistorialMedico (NumeroReporte, DUI, Fecha, Diagnostico, Tratamiento, TipoReporte, Codigo_Medico)
                            VALUES (@NumeroReporte, @DUI, @Fecha, @Diagnostico, @Tratamiento, @TipoReporte, @Codigo_Medico)";

                SqlCommand comando = new SqlCommand(consulta, conexion);

                comando.Parameters.AddWithValue("@NumeroReporte", reporte.NumeroReporte);
                comando.Parameters.AddWithValue("@DUI", dui);
                comando.Parameters.AddWithValue("@Fecha", reporte.Fecha);
                comando.Parameters.AddWithValue("@Diagnostico", reporte.Diagnostico);
                comando.Parameters.AddWithValue("@Tratamiento", reporte.Tratamiento);
                comando.Parameters.AddWithValue("@TipoReporte", reporte.TipoReporte);
                comando.Parameters.AddWithValue("@Codigo_Medico", reporte.LlavePrimariaMedico);

                comando.ExecuteNonQuery();
            }
        }


        //Se debe cambiar a nueva llave primaria

        public static int ObtenerUltimoNumeroReporte()
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
               

                string consulta = "SELECT COUNT(*) FROM HistorialMedico";

                SqlCommand comando = new SqlCommand(consulta, conexion);
                object resultado = comando.ExecuteScalar();

                if (resultado != null && resultado != DBNull.Value)
                {
                    return Convert.ToInt32(resultado)+ new Random().Next(1000, 5000);
                }

                return 1; // Si no hay ningún registro
            }
        }



        public static void EliminarReporte(int numeroReporte)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
               

                string consulta = "DELETE FROM HistorialMedico WHERE NumeroReporte = @numeroReporte";

                SqlCommand comando = new SqlCommand(consulta, conexion);
                comando.Parameters.AddWithValue("@numeroReporte", numeroReporte);

                comando.ExecuteNonQuery();
            }
        }



       

    }
}
