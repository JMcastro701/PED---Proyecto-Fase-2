
using Proyecto_de_Cátedra_PED.Clases;
using Proyecto_de_Cátedra_PED.Clases.Clases_Lista;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_de_Cátedra_PED.ManejoBD
{
    internal class PacientesDAO
    {
        public static Paciente ObtenerPacientePorDUI(string dui)
        {
            Paciente paciente = null;

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = @"SELECT DUI, Nombres, Apellidos, FechaNacimiento, Sexo, Direccion, Telefono, Correo, Peso, Altura, FechaRegistro
                             FROM Pacientes
                             WHERE DUI = @DUI";

                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@DUI", dui);

                SqlDataReader reader = cmd.ExecuteReader();

                //Crear objetos Paciente a partir de los datos obtenidos
                if (reader.Read())
                {
                    paciente = new Paciente
                    (
                        nombres: reader.GetString(1),
                        apellidos: reader.GetString(2),
                        dui: reader.GetString(0),

                        fechaNacimiento: reader.GetDateTime(3),
                        sexo: reader.IsDBNull(4) ? null : reader.GetString(4),
                        direccion: reader.IsDBNull(5) ? null : reader.GetString(5),
                        telefono: reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                        correo: reader.IsDBNull(7) ? "No asignado" : reader.GetString(7),
                        peso: reader.IsDBNull(8) ? 0 : reader.GetDecimal(8),
                        altura: reader.IsDBNull(9) ? 0 : reader.GetDecimal(9)
                        //FechaRegistro = reader.GetDateTime(10)
                    );
                }
                else throw new Exception("No se ha encontrado un paciente con el DUI introducido");

                reader.Close();
            }

            return paciente;
        }
        /*
        public static ListaEnlazadaPacientes CargarPacientesConReportes()
        {

            ListaEnlazadaPacientes listaPacientes = new ListaEnlazadaPacientes();

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
              

                string consultaPacientes = "SELECT * FROM Pacientes";
                SqlCommand comandoPacientes = new SqlCommand(consultaPacientes, conexion);
                SqlDataReader lectorPacientes = comandoPacientes.ExecuteReader();

                while (lectorPacientes.Read())
                {
                    Paciente paciente = new Paciente
                    {
                        DUI = lectorPacientes["DUI"].ToString(),
                        Nombres = lectorPacientes["Nombres"].ToString(),
                        Apellidos = lectorPacientes["Apellidos"].ToString(),
                        FechaNacimiento = Convert.ToDateTime(lectorPacientes["FechaNacimiento"]),
                        Sexo = lectorPacientes["Sexo"].ToString(),
                        Direccion = lectorPacientes["Direccion"].ToString(),
                        Telefono = Convert.ToInt32(lectorPacientes["Telefono"]),
                        Correo = lectorPacientes["Correo"].ToString(),
                        Peso = Convert.ToDecimal(lectorPacientes["Peso"]),
                        Altura = Convert.ToDecimal(lectorPacientes["Altura"])
                    };

                    NodoPaciente nodoPaciente = new NodoPaciente(paciente);

                    // Cargar reportes del paciente
                    nodoPaciente.Reportes = CargarReportesPorDUI(paciente.DUI, conexion);

                    // Agregar el nodo a la lista enlazada
                    listaPacientes.InsertarNodo(nodoPaciente);
                }

                lectorPacientes.Close();
            }

            return listaPacientes;
        }
        */

        public static ListaEnlazadaPacientes CargarPacientesConReportes()
        {
            ListaEnlazadaPacientes listaPacientes = new ListaEnlazadaPacientes();
            

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                

                string consultaPacientes = "SELECT * FROM Pacientes";
                SqlCommand comandoPacientes = new SqlCommand(consultaPacientes, conexion);
                SqlDataReader lectorPacientes = comandoPacientes.ExecuteReader();

                while (lectorPacientes.Read())
                {
                    Paciente paciente = new Paciente
                    (
                        nombres: lectorPacientes["Nombres"].ToString(),
                        apellidos: lectorPacientes["Apellidos"].ToString(),
                        dui: lectorPacientes["DUI"].ToString(),
                        Convert.ToDateTime(lectorPacientes["FechaNacimiento"]),
                        lectorPacientes["Sexo"].ToString(),
                        lectorPacientes["Direccion"].ToString(),
                        Convert.ToInt32(lectorPacientes["Telefono"]),
                        lectorPacientes["Correo"].ToString(),
                        Convert.ToDecimal(lectorPacientes["Peso"]),
                        Convert.ToDecimal(lectorPacientes["Altura"])
                    );

                    NodoPaciente nodoPaciente = new NodoPaciente(paciente);

                    listaPacientes.InsertarNodo(nodoPaciente);
                    // No cargamos los reportes aún
                   // nodosPendientes.Add(nodoPaciente);
                }

                
                lectorPacientes.Close();  // Cerramos el DataReader antes de usar otro
                listaPacientes.CargarReportesBD(conexion);
       
            }

            return listaPacientes;
        }
        public static ListaEnlazadaReportes CargarReportesPorDUI(string dui, SqlConnection conexion)
        {
            ListaEnlazadaReportes listaReportes = new ListaEnlazadaReportes();

            string consultaReportes = @"SELECT * FROM HistorialMedico 
                                WHERE DUI = @dui 
                                ORDER BY NumeroReporte DESC";
            SqlCommand comandoReportes = new SqlCommand(consultaReportes, conexion);
            comandoReportes.Parameters.AddWithValue("@dui", dui);

            SqlDataReader lector = comandoReportes.ExecuteReader();
            
                while (lector.Read())
                {
                    Reporte reporte = new Reporte(
                        Convert.ToDateTime(lector["Fecha"]),
                        lector["Diagnostico"].ToString(),
                        lector["Tratamiento"].ToString(),
                        lector["TipoReporte"].ToString(),
                        Convert.ToInt32(lector["NumeroReporte"]),
                         Convert.ToInt32 (lector["Codigo_Medico"])// Llave primaria como identificador
                    );

                    NodoReporte nodo = new NodoReporte(reporte);
                    listaReportes.InsertarNodo(nodo);
                }
            lector.Close();

            return listaReportes;
        }

       
    }
}
