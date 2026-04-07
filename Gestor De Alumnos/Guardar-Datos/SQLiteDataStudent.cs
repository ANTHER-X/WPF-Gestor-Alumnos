/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace Gestor_De_Alumnos.Guardar_Datos
{

    public class DBContextUser : DbContext
    {
        public DbSet<Alumno> DBAlumno { get; set; }

        private string Path, Password;

        public DBContextUser(string Ruta, string Contrasenia)
        {
            this.Path = Ruta;
            this.Password = Contrasenia;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!string.IsNullOrWhiteSpace(this.Path) && !string.IsNullOrWhiteSpace(this.Password))
            {
                string conexion = $"Data Source={Path};Password={Password}";
                optionsBuilder.UseSqlite(conexion);
            }
        }
    }



    internal class DataBaseAlumnoContext : DbContext
    {
        public DbSet<Alumno> DataBaseAlumno { get; set; }
        private string Password;

        public DataBaseAlumnoContext(string PassWord)
        {
            this.Password = PassWord;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder opciones) => opciones.UseSqlite($"Data Source=Datos/DBAlumnos.db;Password={Password};");
    }

    internal class SQLiteDataStudent
    {
        //Puedes cambiarlo en caso de que si lo vayas a usar y tus datos sean reales :)
        private static string Password = "AntherDeveloperVS_SQLCipher";


        public SQLiteDataStudent() { }

        //Crea la base de datos si no existe
        static public void CreaDataBase()
        { using (DataBaseAlumnoContext DBAC = new DataBaseAlumnoContext(Password)) DBAC.Database.EnsureCreated(); }

        //Agrega, Elimina o actualiza un Alumno
        static public async void AddRemoveUpdateAlumno(Alumno al, bool remove = false, bool update = false)
        {
            using (DataBaseAlumnoContext DBAC = new DataBaseAlumnoContext(Password)) DBAC.Database.EnsureCreated();

            using (DataBaseAlumnoContext DBAC = new DataBaseAlumnoContext(Password))
            {
                if (remove == true) DBAC.Remove(al);
                else if (update == true)
                {
                    try
                    {
                        DBAC.Update(al);
                    }
                    catch (DbUpdateException)
                    {
                        MessageBox.Show("Lo siento pero la matricula ya existe, por favor ingresa otra", "No Updating Alumno", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    try
                    {
                        DBAC.Add(al);
                    }
                    catch (DbUpdateException)
                    {
                        MessageBox.Show("Lo siento pero la matricula ya existe, por favor ingresa otra", "No Add Alumno", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                try
                {
                    await DBAC.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException?.Message);
                }
            }
        }

        //Extrae todos los grupos que existen en la base de datos (para el combobox)
        static public List<short> ExtraeGrupos()
        {
            using (DataBaseAlumnoContext DBAC = new DataBaseAlumnoContext(Password))
                return DBAC.DataBaseAlumno.Select(e => e.grupo).Distinct().OrderBy(e => e).ToList();
        }

        //Extrae los nombres de los alumnos por grupo (para el combobox)
        static public List<string> ExtraeNombresPorGrupo(short Grupo)
        {
            using (DataBaseAlumnoContext DBAC = new DataBaseAlumnoContext(Password))
                return DBAC.DataBaseAlumno.Where(e => e.grupo == Grupo).Select(e => e.name + " " + e.apellidos + " " + e.Matricula).ToList();
        }

        //extrae todos los Alumnos por grupo (para el combobox)
        static public List<Alumno> ExtraeAlumnosPorGrupo(short Grupo, bool onlyHoras = false)
        {
            using (DataBaseAlumnoContext DBAC = new DataBaseAlumnoContext(Password))
                return DBAC.DataBaseAlumno.Where(e => onlyHoras ? (e.grupo == Grupo && e.horasServicioSocial > 0) : (e.grupo == Grupo)).ToList();
        }

        //Extrae Todos los Alumnos Existentes
        static public List<Alumno> AllAlumnos(bool OnlyHoras)
        {
            using (DataBaseAlumnoContext DBAC = new DataBaseAlumnoContext(Password))
            {
                if (!OnlyHoras) return DBAC.DataBaseAlumno.ToList();
                else return DBAC.DataBaseAlumno.Where(e => e.horasServicioSocial > 0).ToList();
            }
        }

        //Extrae una lista de listas de Alumno para poder crear un archivo excel de todos los Alumnos de forma organizada
        static public List<List<Alumno>> ExtraeAllAlumnosExcel(bool OnlyHoras = false)
        {
            List<List<Alumno>> aux = new List<List<Alumno>>();
            List<short> auxG = ExtraeGrupos();
            for (int i = 0; i < auxG.Count; i++)
            {
                aux.Add(ExtraeAlumnosPorGrupo(auxG[i], OnlyHoras));
            }
            return aux;
        }

        //Agrega una gran cantidad de Alumnos (es para pruebas o lo que quieras)
        static public void ADDListAlumnos(int cantGrupos, int cantAlumnoPorGrupo)
        {
            using (DataBaseAlumnoContext DBAC = new DataBaseAlumnoContext(Password))
            {
                //Para una verificacion mas rapida de las matriculas.
                List<int> matriculasTemporales = DBAC.DataBaseAlumno.Select(e => int.Parse(e.Matricula)).ToList();
                Random random = new Random();

                for (int i=0; i< cantGrupos; i++)
                {
                    if (DBAC.DataBaseAlumno.Where(e => e.grupo == (i + 1)).Count() >= cantAlumnoPorGrupo) continue;

                    for (int j=0; j< cantAlumnoPorGrupo; j++)
                    {
                        int matricula = random.Next(00000000,99999999);

                        //Verificamos que la matricula no exista ya
                        if(matricula == 0 || matriculasTemporales.Contains(matricula))
                        {
                            j--;
                            continue;
                        }

                        matriculasTemporales.Add(matricula);

                        Alumno al = new Alumno($"Alumno {j + 1}", $"Apellidos {j + 1}", "01/01/2000", "4412345678", short.Parse((i + 1).ToString()), matricula.ToString(), "Matutino", "NONE");
                        StudentData.CopiaImagen(al, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImagenesEstudiantes", "DefaultStudentImage.png"), AppDomain.CurrentDomain.BaseDirectory, "ImagenesEstudiantes");

                        try
                        {
                            DBAC.DataBaseAlumno.Add(al);
                            DBAC.SaveChanges();
                        }
                        catch (DbUpdateException)
                        {
                            //nada, no hay nada...
                        }
                    }

                }
            }
        }

        //nos da la cantidad de entidades que hay en la database, en este caso Alumnos
        static public int ReturnCantAlumnos()
        {
            using (DataBaseAlumnoContext DBAC = new DataBaseAlumnoContext(Password))
            {
                return DBAC.DataBaseAlumno.Count();
            }
        }

        //Vamos a ver si existe la matricula dada, esto para evitar duplicaciones
        static public bool ExistMatricula(string Matricula)
        {
            using (DataBaseAlumnoContext DBAC = new DataBaseAlumnoContext(Password))
            {
                return (DBAC.DataBaseAlumno.Any(e => e.Matricula == Matricula));
            }
        }

        //Pruebas
        static public List<Alumno> Return_5_Alumnos()
        {
            using(DataBaseAlumnoContext DBAC = new DataBaseAlumnoContext(Password))
            {
                return DBAC.DataBaseAlumno.Where(e => e.Id < 15).ToList();
            }
        }
    }
}
