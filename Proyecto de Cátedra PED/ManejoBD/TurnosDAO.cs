using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Proyecto_de_Cátedra_PED.Clases.Clases_Cola;

using Proyecto_de_Cátedra_PED.Formularios;
using Proyecto_de_Cátedra_PED.Clases;


namespace Proyecto_de_Cátedra_PED.ManejoBD
{
    internal class TurnosDAO
    {
        /*
        public static void InsertarTurno(int codigoTurno, string dui, int prioridad)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "INSERT INTO Turnos (CodigoTurno, DUI, Prioridad) VALUES (@CodigoTurno, @DUI, @Prioridad)";

                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@CodigoTurno", codigoTurno);
                cmd.Parameters.AddWithValue("@DUI", dui);
                cmd.Parameters.AddWithValue("@Prioridad", prioridad);

                cmd.ExecuteNonQuery();
            }
        }
        */
        public static void InsertarTurno(int codigoTurno, string dui, int prioridad, DateTime fechaTurno, TimeSpan horaTurno)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = @"INSERT INTO Turnos (CodigoTurno, DUI, Prioridad, EstadoTurno, FechaTurno, HoraTurno)
                             VALUES (@CodigoTurno, @DUI, @Prioridad, @EstadoTurno, @FechaTurno, @HoraTurno)";

                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@CodigoTurno", codigoTurno);
                cmd.Parameters.AddWithValue("@DUI", dui);
                cmd.Parameters.AddWithValue("@Prioridad", prioridad);
                cmd.Parameters.AddWithValue("@EstadoTurno", 0);
                cmd.Parameters.AddWithValue("@FechaTurno", fechaTurno.Date);
                cmd.Parameters.AddWithValue("@HoraTurno", horaTurno);

                cmd.ExecuteNonQuery();
                
            }
        }

        public static void CargarColaTurnosNoAtendidos(ColaPrioridad cola)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = @"SELECT CodigoTurno, DUI, Prioridad, FechaTurno, HoraTurno
                             FROM Turnos
                              WHERE EstadoTurno = 0
                             ORDER BY FechaTurno, HoraTurno";

                SqlCommand cmd = new SqlCommand(query, conexion);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int codigoTurno = reader.GetInt32(0);
                    string dui = reader.GetString(1);
                    int prioridad = reader.GetInt32(2);
                    DateTime fecha = reader.GetDateTime(3);
                    TimeSpan hora = reader.GetTimeSpan(4);


                    // Combinar fecha y hora
                    DateTime fechaHoraCompleta = fecha.Date + hora;

                    //Obtener Paciente de la BD
                    Paciente paciente = PacientesDAO.ObtenerPacientePorDUI(dui);


                    NodoCola nodo = new NodoCola(codigoTurno, paciente, fechaHoraCompleta, prioridad);
                    cola.Encolar(codigoTurno,paciente, fechaHoraCompleta, prioridad);
                }

                reader.Close();
            }
        }

        public static int ObtenerCantidadTurnos()
        {
            int cantidad = 0;

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT COUNT(*) FROM Turnos";
                SqlCommand cmd = new SqlCommand(query, conexion);

                cantidad = (int)cmd.ExecuteScalar();
            }

            return cantidad;
        }

        public static void MarcarTurnoComoAtendido(int codigoTurno)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = @"UPDATE Turnos
                             SET EstadoTurno = 1
                             WHERE CodigoTurno = @CodigoTurno";

                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@CodigoTurno", codigoTurno);

                cmd.ExecuteNonQuery();
                return;
            }
        }

        public static List<ReporteTurno> ObtenerTurnosSinFiltrar()
        {
            List<ReporteTurno> lista = new List<ReporteTurno>();

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = @"
                SELECT 
                    t.CodigoTurno,  t.DUI,
                    p.Nombres,p.Apellidos,
                    t.Prioridad,t.EstadoTurno, t.FechaTurno,t.HoraTurno
                FROM Turnos t
                INNER JOIN Pacientes p ON t.DUI = p.DUI
                ORDER BY t.FechaTurno DESC, t.HoraTurno DESC";

                SqlCommand cmd = new SqlCommand(query, conexion);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {


                    ReporteTurno turno = new ReporteTurno
                    {
                        CodigoTurno = reader.GetInt32(0),
                        DUI = reader.GetString(1),
                        NombresPaciente = reader.GetString(2),
                        ApellidosPaciente = reader.GetString(3),
                        Prioridad = reader.GetInt32(4),
                        EstadoTurno = reader.GetBoolean(5),
                        FechaTurno = reader.GetDateTime(6),
                        HoraTurno = reader.GetTimeSpan(7)
                    };

                    lista.Add(turno);
                }

                reader.Close();
            }

            return lista;
        }

        public static List<ReporteTurno> ObtenerTurnosPorFecha(DateTime fecha)
        {
            List<ReporteTurno> lista = new List<ReporteTurno>();

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string query = @"
                SELECT 
                    t.CodigoTurno,  t.DUI,
                    p.Nombres, p.Apellidos,
                    t.Prioridad, t.EstadoTurno,
                    t.FechaTurno, t.HoraTurno
                FROM Turnos t
                INNER JOIN Pacientes p ON t.DUI = p.DUI
                WHERE t.FechaTurno = @Fecha
                ORDER BY t.HoraTurno DESC";

                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@Fecha", fecha.Date);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ReporteTurno turno = new ReporteTurno
                    {
                        CodigoTurno = reader.GetInt32(0),
                        DUI = reader.GetString(1),
                        NombresPaciente = reader.GetString(2),
                        ApellidosPaciente = reader.GetString(3),
                        Prioridad = reader.GetInt32(4),
                        EstadoTurno = reader.GetBoolean(5),
                        FechaTurno = reader.GetDateTime(6),
                        HoraTurno = reader.GetTimeSpan(7)
                    };

                    lista.Add(turno);
                }

                reader.Close();
            }

            return lista;
        }
        public struct ReporteTurno
        {
            public int CodigoTurno { get; set; }
            public string DUI { get; set; }
            public string NombresPaciente { get; set; }
            public string ApellidosPaciente { get; set; }
            public int Prioridad { get; set; }
            public bool EstadoTurno { get; set; }
            public DateTime FechaTurno { get; set; }
            public TimeSpan HoraTurno { get; set; }
        }
    }
}
