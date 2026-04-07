/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows;

namespace Gestor_De_Alumnos
{
    //agregamos matricula unica
    [Index(nameof(Matricula), IsUnique = true)]
    public class Alumno
    {
        [Key]
        public int Id { get; private set; }

        public string name { get; private set; } = "none";
        public string apellidos { get; private set; } = "none";
        public string fechaNacimiento { get; private set; } = "none";
        public byte edad { get; private set; }
        public string Telefono { get; private set; } = "none";

        public short grupo { get; private set; } = 0;
        public string Matricula { get; private set; } = "0";
        public string turno { get; private set; } = "none";
        public string capacitacion { get; private set; } = "none";
        public string bachillerato { get; private set; } = "none"; 
        public string club { get; private set; } = "none";

        public int horasServicioSocial { get; private set; } = 0;
        public string profServSoc { get; private set; } = "none";

        public string RutaImagen { get; set; } = "none";

        [NotMapped]
        public bool IsModifique { get; private set; }

        protected Alumno() { }

        public Alumno(int ID, string nombre, string apellidos, string fechanacimiento, string telefono, short grupo, string Matricula, string turno,
                      string RutaImagen, int HorasServicio = 0, string profServSoc = "none", string capacitacion = "none",
                      string bachillerato = "none", string club = "none")
        {
            CambiaAllDatos(ID, nombre, apellidos, fechanacimiento, telefono, grupo, Matricula, turno, HorasServicio, profServSoc, capacitacion, bachillerato, club, RutaImagen);
        }

        public Alumno(string nombre, string apellidos, string fechanacimiento, string telefono, short grupo, string Matricula, string turno,
                      string RutaImagen, int HorasServicio = 0, string profServSoc = "none", string capacitacion = "none",
                      string bachillerato = "none", string club = "none", int ID = 0)
        {
            CambiaAllDatos(ID, nombre, apellidos, fechanacimiento, telefono, grupo, Matricula, turno, HorasServicio, profServSoc, capacitacion, bachillerato, club, RutaImagen);
        }

        public void CambiaAllDatos(int ID, string nombre, string apellidos, string fechanacimiento, string telefono, short grupo, string Matricula, string turno, int HorasServicio,
                                   string profServSoc, string capacitacion, string bachillerato, string club, string Ruta = "")
        {
            if (ID != 0) this.Id = ID;
            this.name = nombre;
            this.apellidos = apellidos;
            this.fechaNacimiento = fechanacimiento;
            this.Telefono = telefono;
            this.grupo = grupo;
            this.Matricula = Matricula;
            this.turno = turno;
            this.horasServicioSocial = HorasServicio;
            this.capacitacion = capacitacion;
            this.bachillerato = bachillerato;
            this.profServSoc = profServSoc;
            this.club = club;
            if (!string.IsNullOrWhiteSpace(Ruta)) this.RutaImagen = Ruta;
            edad = SeleccionaEdad();
        }

        private byte SeleccionaEdad()
        {
            byte Edad = 0;

            DateTime Actual = DateTime.Now;
            DateTime Fecha = new DateTime(2000, 1, 1);
            TimeSpan edad;

            if (DateTime.TryParse(fechaNacimiento, out _) == false) edad = Actual - Fecha;
            else
            {
                Fecha = DateTime.Parse(fechaNacimiento);
                edad = Actual - Fecha;
            }

            Edad = (byte)(edad.TotalDays / 365);

            return Edad;
        }

        //vemos si el usuario fue modificado
        public bool AlumnoModificado() => IsModifique;

        public void Mostrarse()
        {
            MessageBox.Show($"Datos\nID: {Id}\nNombre: {name}\nApellidos: {apellidos}\nFecha De Nacimiento: {fechaNacimiento}\nEdad: {edad}\nTelefono: {Telefono}\nGrupo: {grupo}\nMatricula: {Matricula}\n" +
                $"Turno: {turno}\nHoras De Servicio Social: {horasServicioSocial}\nCapacitacion: {capacitacion}\nBachillerato: {bachillerato}\nProfe Serv.Soc: {profServSoc}\nClub: {club}");
        }

        public void ChangeID(int ID) => this.Id = ID;

        public void AddHoras(int CanHoras)
        {
            this.horasServicioSocial += CanHoras;
            if (this.horasServicioSocial > 120) this.horasServicioSocial = 120;
            this.IsModifique = true;
        }

        public void DeleteHoras(int CantHoras)
        {
            this.horasServicioSocial -= CantHoras;
            if (this.horasServicioSocial < 0) this.horasServicioSocial = 0;
            this.IsModifique = true;
        }

        public string ReturnDatos()
        {
            return name + "--" + apellidos + "--" + fechaNacimiento + "--" + edad + "--" + Telefono + "--" + grupo + "--" + Matricula + "--" + turno + "--\n" +
                     "--" + capacitacion + "--" + bachillerato + "--" + club + "--" + horasServicioSocial + "--" + profServSoc;
        }

        public string ReturnDatocConRutaImagen()
        {
            return name + "--" + apellidos + "--" + fechaNacimiento + "--" + edad + "--" + Telefono + "--" + grupo + "--" + Matricula + "--" + turno + "--\n" +
                     "--" + capacitacion + "--" + bachillerato + "--" + club + "--" + horasServicioSocial + "--" + profServSoc + "--" + RutaImagen;
        }

    }

}
