/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using Gestor_De_Alumnos.Guardar_Datos;
using Microsoft.Win32;
using System.IO;

namespace Gestor_De_Alumnos.Terminal
{
    internal class SQLiteGestorDataBaseConsoleUser
    {

        static private Alumno? CreaAlumno()
        {
            Alumno? Alaux = null;
            Console.Clear();

            Console.Write("Nombre: ");
            string name = Console.ReadLine() ?? "Alumno";

            Console.Write("Apellidos: ");
            string apellidos = Console.ReadLine() ?? "Apellidos";

            Console.Write("FechaNacimiento (dd/mm/yyyy): ");
            string FechNac = Console.ReadLine() ?? "01/01/2010";
            try
            {
                DateTime dt = DateTime.ParseExact(FechNac, "dd/MM/yyyy", null);
            }
            catch
            {
                FechNac = "01/01/2010";
                Console.Write($"Formato no valido, la fecha sera: {FechNac}");
            }

            Console.Write("Telefono (+52): ");
            string Tel = Console.ReadLine() ?? "0000000000";

            Console.Write("Grupo: ");
            short Grupo;
            try
            {
                Grupo = short.Parse(Console.ReadLine() ?? "0");
            }
            catch
            {
                Grupo = 0;
                Console.Write($"Formato no valido, el grupo sera {Grupo}");
            }

            Console.Write("Matricula (Tam: 8): ");
            string Matricula = Console.ReadLine() ?? "00000000";

            if (Matricula.Count() != 8)
            {
                Matricula = "00000000";
                Console.Write($"Formato incompleto. Matricula base: {Matricula}");
            }

            Console.Write("Turno. Matutino(M) Vespertino(V): ");
            char T = char.Parse(Console.ReadLine() ?? "V");
            string Turno;
            if (T == 'M' || T == 'm') Turno = "Matutino";
            else if (T == 'V' || T == 'v') Turno = "Vespertino";
            else
            {
                Turno = "Vespertino";
                Console.Write($"Formato no valido. Turno {Turno}");
            }

            Console.Write("Horas de servicio Social: ");
            int Horas = int.Parse(Console.ReadLine() ?? "0");

            Console.Write("Profesor de servicio Social: ");
            string prof = Console.ReadLine() ?? "none";

            Console.Write("Capacitacion: ");
            string Cap = Console.ReadLine() ?? "none";

            Console.Write("Bachillerato: ");
            string Bach = Console.ReadLine() ?? "none";

            Console.Write("Club: ");
            string Club = Console.ReadLine() ?? "none";

            Alaux = new Alumno(name, apellidos, FechNac, Tel, Grupo, Matricula, Turno, "ImagenesEstudiantes/DefaultStudentImage.png"
                , Horas, prof, Cap, Bach, Club);

            if (Alaux != null) Console.Write("Alumno creado");
            else Console.Write("Ups... algo fallo... Alumno NO creado");
            return Alaux;
        }



        // Par poder borrar la DB que se ah creado
        static private void BorraDataBase()
        {
            /*Verificamos que la database exista*/
            string patdb = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos/Data.txt"); // Archivo donde esta el path de la DB
            string pathDataBase = "Datos/UserAlumnoData.db"; // Path base
            
            if (File.Exists(patdb))
            {
                using (StreamReader SW = new StreamReader(new FileStream(patdb, FileMode.Open, FileAccess.Read)))
                    pathDataBase = SW.ReadToEnd(); // Si existe la ruta, la sacamos
            }

            if (!File.Exists(pathDataBase))
            {
                /*Si no existe le decimos al user que no existe*/
                Console.WriteLine("Lo siento, no tiene database Creada, Quiere Crear una (s/N)");
                char resp = char.Parse(Console.ReadLine() ?? "n");
                if (resp == 'S' && resp == 's') ConsoleCreateDB();
                return;
            }

            //Preguntamos si en serio quiere eliminar la base de datos
            Console.Write("En seguro que quiere borrar su database? (s/N): ");
            char opcb = char.Parse(Console.ReadLine() ?? "n");

            if (opcb == 'N' || opcb == 'n') return;

            //si si quiere borrarla le pedimos la password
            Console.Write("Escriba Su Password: ");
            string? Password = Console.ReadLine();

            //si si inserto algo
            if (!string.IsNullOrWhiteSpace(Password))
            {
                //entramos a la database
                using (DBContextUser db = new DBContextUser(pathDataBase, Password))
                {
                    //verificamos que la password es la correcta con una pequenia consulta
                    try
                    {
                        int opSimple = db.DBAlumno.Count();
                        Console.WriteLine("Password Correcta.");
                    }
                    catch
                    {
                        /*si falla al administrar los datos es que la contrasenia es incorrecta
                        asi que le decimos que esta mal y salimos*/
                        Console.WriteLine("Password Incorrecta\nSaliendo...");
                        db.Dispose();
                        Thread.Sleep(2500);
                        return;
                    }
                }

                //y eliminamos el archivo de la database
                try
                {
                    File.Delete(pathDataBase);
                    if (!File.Exists(pathDataBase)) Console.WriteLine("DataBase Eliminada con exito");
                }
                catch
                {
                    Console.WriteLine("Ups... algo fallo...");
                }

                Thread.Sleep(2500);
            }

        }


        // Para la creacion de la DB
        static private void ConsoleCreateDB()
        {
            // Verificamos si ya creo o no la DB
            if(File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos/Data.txt")))
            {
                Console.WriteLine("Ya creo su database\nPuede Borrala para volver a Crearla");
                Console.ReadKey();
                return;
            }

            // Si no, comenzamos el setting
            Console.Write("Quiere eleguir la Carpeta? (s/N): ");
            char DBopc = (char.TryParse(Console.ReadLine(), out _)) ? (char.Parse(Console.ReadLine())) : ('N');

            // Verificamos la ruta de la nueva DB
            string ruta;
            if (DBopc == 'S' || DBopc == 's')
            {
                OpenFolderDialog ofd = new OpenFolderDialog();
                ofd.Title = "Selecciona Carpeta";

                if (ofd.ShowDialog() == true) ruta = Path.Combine(ofd.FolderName, "UserAlumnoData.db");
                else ruta = "Datos/UserAlumnoData.db";

            }
            else ruta = "Datos/UserAlumnoData.db";

            string? Password;

            //Pedimos una contrasenia
            while (true)
            {
                Console.Write("Escriba su Password RECUERDELA ya que NO SABEMOS CUAL ES.\nPassword: ");
                Password = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(Password)) break;
            }

            //si elije una ruta distinta a la por defecto creamos el archivo que tendra la ruta
            if (ruta != "Datos/UserAlumnoData.db")
            {
                using (StreamWriter SW = new StreamWriter(new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos/Data.txt"), FileMode.Create, FileAccess.Write)))
                    SW.Write(ruta);
            }

            using (DBContextUser db = new DBContextUser(ruta, Password))
            {
                db.Database.EnsureCreated();
                db.Dispose();
            }

            if (File.Exists(ruta)) Console.WriteLine("DataBase Creada...");
            else Console.WriteLine("NO se pudo CREAR la DataBase...");

            Thread.Sleep(3500);
        }


        static private void SeleccionaAlumnoGrupoParaArchoTXT(DBContextUser db, bool Grupo, bool AllAlumnos = false)
        {
            List<short> Grupos = db.DBAlumno.Select(e => e.grupo).ToList();
            List<Alumno> TotalAlumnos = db.DBAlumno.ToList();
            List<Alumno> Alumnos = Grupo ? (db.DBAlumno.Where(e => e.grupo == Grupos[0]).ToList()) : (TotalAlumnos);
            Alumno aux = TotalAlumnos[0];

            if(AllAlumnos == false)
            {
                for (int i = 0; i < (Grupo ? (Grupos.Count) : (TotalAlumnos.Count())); i++)
                {
                    Console.WriteLine(
                        (Grupo ?
                        ($"Grupo {Grupos[i]}: {i + 1}") :
                        (TotalAlumnos[i].Id + " " + TotalAlumnos[i].name + " " + TotalAlumnos[i].apellidos + " " + TotalAlumnos[i].Matricula + $": {i + 1}")
                        ));
                }

                Console.Write("Opcion: ");
                short gopc = 0;
                try
                {
                    gopc = short.Parse(Console.ReadLine() ?? "0");
                }
                catch
                {
                    Console.Write("Opcion no valida\nRegresando...");
                    Thread.Sleep(2100);
                    return;
                }
                if (gopc == 0 || (Grupo ? (gopc > Grupos.Count) : (gopc > TotalAlumnos.Count)))
                {
                    Console.Write("Opcion no valida\nRegresando...");
                    Thread.Sleep(2100);
                    return;
                }

                if(Grupo) Alumnos = Grupo ? (db.DBAlumno.Where(e => e.grupo == Grupos[gopc - 1]).ToList()) : (TotalAlumnos);
                else aux = TotalAlumnos[gopc - 1];
            }

            Console.Write("Quiere Eleguir la ruta (s/n): ");
            char Ropc = char.Parse(Console.ReadLine() ?? "n");
            string path;
            if (Ropc == 'S' || Ropc == 's')
            {
                OpenFolderDialog ofd = new OpenFolderDialog();
                ofd.Title = "Archivos TXT";

                if (ofd.ShowDialog() == true)
                {
                    if (AllAlumnos) path = Path.Combine(ofd.FolderName, $"Alumnos.txt");
                    else path = Path.Combine(ofd.FolderName, $"{((Grupo) ? (Alumnos[0].grupo) : (aux.name))}.txt");
                }
                else if (AllAlumnos) path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Datos/Alumnos.txt");
                else path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Datos/{(Grupo ? (Alumnos[0].grupo) : (aux.name))}.txt");
            }
            else if (AllAlumnos) path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Datos/Alumnos.txt");
            else path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Datos/{(Grupo ? (Alumnos[0].grupo) : (aux.name))}.txt");

            if(AllAlumnos) StudentData.CreaTXTGrupoAlumnosAllAlumnos(TotalAlumnos, path);
            else if (Grupo) StudentData.CreaTXTGrupoAlumnosAllAlumnos(Alumnos, path);
            else StudentData.CreaTXTAlumno(aux, path);

            if (File.Exists(path)) Console.Write("Archivo Creado...");
            Thread.Sleep(2400);
        }


        static private void CreaTXTAlumnos(DBContextUser db)
        {
            while (true)
            {

                Console.Clear();
                Console.Write("Creador de TXT\n" +
                              "Crear TXT de un alumno: 1\n" +
                              "Crear TXT de un Grupo: 2\n" +
                              "Crear TXT de todos los Alumnos: 3\n" +
                              "Salir: 4\n" +
                              "Opcion: ");
                short opc = byte.Parse(Console.ReadLine() ?? "0");
               
                if (opc > 0 && opc <5)
                {
                    switch (opc)
                    {
                        //Crea TXT de un Alumno
                        case 1:
                            {
                                SeleccionaAlumnoGrupoParaArchoTXT(db, false);
                                break;
                            }
                        //CreaTXT de un Grupo
                        case 2:
                            {
                                SeleccionaAlumnoGrupoParaArchoTXT(db, true);
                                break;
                            }
                        //creando TXT de todos los alumnos
                        case 3:
                            {
                                SeleccionaAlumnoGrupoParaArchoTXT(db,false,true);
                                break;
                            }
                        //saliendo
                        case 4:
                            {
                                Console.Write("Regresando...");
                                Thread.Sleep(2000);
                                return;
                            }
                        default:
                            break;

                    }

                }
            }

        }

        static private Alumno? ExtraeAlumno(DBContextUser db)
        {

            int opcDelete = -1;
            List<Alumno> auxAl = db.DBAlumno.ToList();
            
            if(auxAl.Count == 0)
            {
                Console.WriteLine("No hay alumnos para eliminar");
                Thread.Sleep(2500);
                return null;
            }

            for (int i = 0; i < auxAl.Count; i++)
            {
                Console.WriteLine("Alumno: " + auxAl[i].Id + " " + auxAl[i].name + " " + auxAl[i].apellidos + " " + auxAl[i].Matricula + $": {i + 1}");
            }
            Console.Write("Opcion: ");

            try
            {
                opcDelete = int.Parse(Console.ReadLine() ?? "-1");

                if (opcDelete < 1) opcDelete = 1;
                if (opcDelete > auxAl.Count()) opcDelete = auxAl.Count;
                return auxAl[opcDelete - 1];
            }
            catch(Exception e)
            {
                Console.WriteLine("Datos Erroneos");
                return null;
            }
        }


        static private void ManejaDB(string ruta, string Password)
        {
            DBContextUser db = new DBContextUser(ruta, Password);
            
            //Verificamos que la databse si sea desencriptada
            try { int opSimple = db.DBAlumno.Count(); }
            catch
            {
                Console.WriteLine("Algo Falló, intente despues...");
                db.Dispose();
                Thread.Sleep(4000);

                return;
            }
            //si se puede acceder y desencriptar abrimos otro menu
            while (true)
            {
                Console.Clear();

                Console.Write("ENTRASTE A TU DATABASE.\n" +
                              "Ver Cant. Alumnos: 1\n" +
                              "Agrega Alumno: 2\n" +
                              "Ver Alumnos: 3\n" +
                              "Eliminar Alumno: 4\n" +
                              "Dar Alumno a DBMain: 5\n" +
                              "Tomar Alumno de DBMain: 6\n" +
                              "Crear TXT Alumno(s): 7\n" +
                              "Regresar: 8\n" +
                              "Opcion: ");
                short opc = ((short.TryParse(Console.ReadLine(), out _)) ? (short.Parse(Console.ReadLine())) : ((short)-1));

                if (opc != -1)
                {
                    switch (opc)
                    {
                        //damos la cantidad de Alumnos
                        case 1:
                            {
                                Console.Write(db.DBAlumno.Count() +
                                    "\nPreciona una tecla para regresar");
                                Console.ReadKey();
                                break;
                            }
                        //agregar alumno
                        case 2:
                            {
                                Alumno? aux = CreaAlumno();
                                if (aux == null)
                                {
                                    Console.WriteLine("Lo siento, el alumno no existe :(");
                                    break;
                                }
                                db.Add(aux);
                                db.SaveChanges();
                                Thread.Sleep(2500);
                                break;
                            }
                        //Ver todos los alumnos
                        case 3:
                            {
                                List<Alumno> auxAl = db.DBAlumno.ToList();
                                foreach (Alumno al in auxAl) Console.WriteLine(al.ReturnDatos() + "\n");

                                Console.Write("Precione una tecla para regresar...");
                                Console.ReadKey();
                                break;
                            }
                        //Eliminar Alumno
                        case 4:
                            {
                                Alumno? al = ExtraeAlumno(db);
                                if (al == null) break;
                                db.Remove(al);
                                db.SaveChanges();

                                break;
                            }
                        //Mantar Alumno a DBMain
                        case 5:
                            {
                                Alumno? auxal = ExtraeAlumno(db);
                                if (auxal == null) break;

                                SQLiteDataStudent.AddRemoveUpdateAlumno(auxal);
                                Console.Write("Anidado");
                                Thread.Sleep(2500);
                                break;
                            }
                        //insertar alumno desde DBMain
                        case 6:
                            {
                                int opcAlMain;
                                List<Alumno> aux = SQLiteDataStudent.AllAlumnos(false);

                                if (aux.Count == 0) break;

                                for(int i=0; i < aux.Count; i++)
                                {
                                    Console.Write($"Alumno: {aux[i].Id} -- {aux[i].name} {aux[i].apellidos} -- {aux[i].Matricula}: {i + 1}.\n");
                                }
                                Console.Write("Opcion: ");

                                opcAlMain = int.Parse(Console.ReadLine() ?? "-1");

                                if (opcAlMain == -1 || opcAlMain > aux.Count)
                                {
                                    Console.Write("opcion invalida\nRegresando...");
                                    Thread.Sleep(2500);
                                    break;
                                }

                                db.Add(aux[opcAlMain - 1]);
                                db.SaveChanges();
                                
                                Console.Write("Agregado\nRegresando...");
                                Thread.Sleep(2500);
                                break;
                            }
                        //Crear TXT de los Alumnos
                        case 7:
                            {
                                CreaTXTAlumnos(db);
                                Thread.Sleep(2500);
                                break;
                            }
                        //Salir
                        case 8:
                            {
                                db.Dispose();
                                return;
                            }
                    }
                }
            }
        }

        // Aqui vamos a manejar las opciones de las DB externa que se creara
        static public void ManejaDataBaseUser()
        {
            while (true)
            {
                Console.Clear();
                Console.Write("Selecciona Opcion:\n" +
                              "CreaDataBase: 1\n" +
                              "Entrar a DataBase: 2\n" +
                              "Borrar DataBase: 3\n" +
                              "Salir 4\n" +
                              "Opcion: ");
                try
                {
                    short opc = short.Parse(Console.ReadLine() ?? "0");

                    switch (opc)
                    {
                        //CreaDatabase
                        case 1:
                            {
                                ConsoleCreateDB();
                                break;
                            }
                        //Maneja DataBase
                        case 2:
                            {
                                /*Al manejar la database podras, incluir un Alumno de los que tienes
                                 Sacar un alumno y meterlo a la database principal.
                                Mostrar alumnos, mostrar alumno especifico, mostrar grupos, seleccionar grupo, ver alumnos
                                solo de grupo, generar TXT de los alumnos... y Ya*/

                                string pathDBRuta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos/Data.txt");
                                string pathDB;
                                if (File.Exists(pathDBRuta))
                                {
                                    using (StreamReader SR = new StreamReader(new FileStream(pathDBRuta, FileMode.Open, FileAccess.Read)))
                                    {
                                        pathDB = SR.ReadToEnd();
                                    }
                                }
                                else pathDB = "Datos/UserAlumnoData.db";

                                if (!File.Exists(pathDB))
                                {
                                    Console.Write("Sin DataBase, Quiere Crearla? (s/N): ");
                                    if (Console.ReadLine() == "S" || Console.ReadLine() == "s")
                                    {
                                        ConsoleCreateDB();
                                    }
                                    else break;
                                }

                                Console.Write("Ingrese su Password: ");
                                string? Pass = Console.ReadLine();

                                if(Pass != null) ManejaDB(pathDB, Pass);
                                break;
                            }
                        //Borrar la database
                        case 3:
                            {
                                BorraDataBase();
                                break;
                            }
                        //salir
                        case 4:
                            {
                                Console.Write("Saliendo...");
                                Thread.Sleep(2300);
                                return;
                            }
                    }
                }
                catch
                {
                    Console.Write("Opcion invalida, intente de nuevo...");
                    Thread.Sleep(1500);
                }
            }
        }
    }
}
