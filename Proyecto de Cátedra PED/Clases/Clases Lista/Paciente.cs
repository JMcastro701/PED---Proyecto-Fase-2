using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto_de_Cátedra_PED.Clases
{
     class Paciente
    {
        private string dui;
        private string nombres;
        private string apellidos;
        private DateTime fechaNacimiento;
        private string sexo;
        private string direccion;
        private int telefono;
        private string correo;
        private decimal peso;
        private decimal altura;


        public Paciente(string nombres, string apellidos, string dui, DateTime fechaNacimiento, string sexo, string direccion, int telefono, string correo, decimal peso, decimal altura )
        {
            this.nombres = nombres;
            this.apellidos = apellidos;
            this.dui = dui;
            this.fechaNacimiento = fechaNacimiento;
            this.sexo = sexo;
            this.direccion = direccion;
            this.telefono = telefono;
            this.correo = correo;
            this.peso = peso;
            this.altura= altura;
        }

        public string Dui { get => dui; }
        public string Nombres { get => nombres; }
        public string Apellidos { get => apellidos; }
        public DateTime FechaNacimiento { get => fechaNacimiento; }
        public string Sexo { get => sexo; }
        public string Direccion { get => direccion; }
        public int Telefono { get => telefono; }
        public string Correo { get => correo; }
        public decimal Peso { get => peso; }
        public decimal Altura { get => altura; }

    }
}
