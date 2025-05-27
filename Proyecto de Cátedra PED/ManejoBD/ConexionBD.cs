using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Proyecto_de_Cátedra_PED.ManejoBD
{
    internal class ConexionBD
    {
        
        private static string cadenaConexion = @"Server=localhost;Database=HospitalAtencion;Trusted_Connection=True;";


        //Método que se conbinará con bloques using para manejar las conexiones a la BD
        public static SqlConnection ObtenerConexion()
        {
            SqlConnection conexion = new SqlConnection(cadenaConexion);
            conexion.Open();
            return conexion;
        }
    }
}
