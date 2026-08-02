/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using Gestor_De_Alumnos.Guardar_Datos;
using Microsoft.Win32;

namespace Gestor_De_Alumnos.Terminal
{

    internal class TerminalWin
    {
        /*Usamos la API para abrir y cerrar una Consola de Terminal,
         las funciones ya vienen en el Kernel asi que solo las "sacamos"*/

        private delegate bool ConsoleEventDelegate(int eventType);
        private static ConsoleEventDelegate handler = new ConsoleEventDelegate(ConsoleEventCallback);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll")] static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        private static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2) // CTRL_CLOSE_EVENT
            {
                // Solo liberamos la consola, no el proceso
                Console.SetOut(TextWriter.Null);
                Console.SetIn(StreamReader.Null);
                FreeConsole();
                return true; // Evita que Windows cierre el proceso
            }
            return false;
        }

        MainWindow WNDMain;
        Alumno? Al = null;

        public TerminalWin(MainWindow Padre)
        {
            //copiamos la lista de los alumnos
            this.WNDMain = Padre;

            SetConsoleCtrlHandler(handler, true);

            //abrimos consola
            AllocConsole();

            //Asociamos el Handle de Console a la Consola que abrimos
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            StreamReader standarInput = new StreamReader(Console.OpenStandardInput());
            Console.SetOut(standardOutput);
            Console.SetIn(standarInput);

            //Portada
            string portada = @"
.---------------------------------------------.
|         ____           _                    |
|        / ___| ___  ___| |_ ___  _ __        |
|       | |  _ / _ \/ __| __/ _ \| '__|       |
|       | |_| |  __/\__ \ || (_) | |          |
|        \____|\___||___/\__\___/|_|          |
|                   __| | ___                 |
|                  / _` |/ _ \                |
|                 | (_| |  __/                |
|     _    _       \__,_|\___|                |
|    / \  | |_   _ _ __ ___  _ __   ___  ___  |
|   / _ \ | | | | | '_ ` _ \| '_ \ / _ \/ __| |
|  / ___ \| | |_| | | | | | | | | | (_) \__ \ |
| /_/   \_\_|\__,_|_| |_| |_|_| |_|\___/|___/ |
|                                             |
'---------------------------------------------'";
            bool salir = false;
            byte opc = 4;
            string AlumnoIndex = "none";

            //bucle de "Mensajes" jaja
            while(salir == false)
            {
                Console.WriteLine(portada);
                Console.Write($"Version: {Assembly.GetExecutingAssembly().GetName().Version}\n" +
                              $"Seleccione una opcion:\n" +
                              $"Manejar Tu DataBase: 1\n" +
                              $"Generar Exel De Alumno (Al IndexList): 2\n" +
                              $"Generar Archivo Excel con un grupo: 3\n" +
                              $"Generar Archivo Excel de todos los Alumnos: 4\n" +
                              $"Crear Archivo TXT de Alumno: 5\n" +
                              $"Crear Archivo TXT de un grupo: 6\n" +
                              $"Crear Archivo TXT de todos los Alumnos: 7\n" +
                              $"Ver la DataBase: 8\n" +
                              $"Ver Cantidad de Alumnos: 9\n" +
                              $"Salir: 10\n" +
                              $"Opcion: ");
                
                try
                {
                    opc = byte.Parse(Console.ReadLine() ?? "0");
                }
                catch
                {
                    Console.Clear();
                    Console.WriteLine("Error de Opcion.\nSaliendo...");
                    Thread.Sleep(3500);
                    FreeConsole();
                }

                switch (opc)
                {
                    //Manejar la DataBase del usuario
                    case 1:
                        {
                            SQLiteGestorDataBaseConsoleUser.ManejaDataBaseUser();
                            break;
                        }
                    //Crear Excel del alumno seleccionado
                    case 2:
                        {
                            Al = SeleccionaAlumno();
                            CrearExcel(Al);
                            break;
                        }
                    //crear Excel con un grupo
                    case 3:
                        {
                            short grupo = SeleccionaGrupo();
                            CrearExcel(null, grupo);
                            break;
                        }
                    //crear Excel con todos los Alumnos
                    case 4:
                        {
                            CrearExcel(null, -1, true);
                            break;
                        }
                    //Crear archivo TXT con los datos del Alumno
                    case 5:
                        {
                            Al = SeleccionaAlumno();
                            CreaTXTAlumnos(Al);
                            break;
                        }
                    // Crear Achivos TXT con un grupo
                    case 6:
                        {
                            short grupo = SeleccionaGrupo();
                            CreaTXTAlumnos(null, grupo);
                            break;
                        }
                    // Crear Achivos TXT con todos los Alumnos
                    case 7:
                        {
                            CreaTXTAlumnos(null, -1, true);
                            break;
                        }
                    //Ver la DataBase
                    case 8:
                        {
                            VerDataBase();
                            break;
                        }
                    case 9:
                        {
                            Console.WriteLine($"{SQLiteDataStudent.ReturnCantAlumnos()}\nPrecione una tecla para regresar...");
                            Console.ReadKey();
                            break;
                        }
                    //Salir del bucle
                    case 10:
                        {
                            salir = true;
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Error, intente de nuevo...");
                            Thread.Sleep(1500);
                            break;
                        }
                }

                Console.Clear();
            }

            //Fin de la Consola
            Console.WriteLine("Espero no se haya roto nada :v.\nSaliendo...");
            Thread.Sleep(2500);
            FreeConsole();
        }

        //VER DATABASE
        private void VerDataBase()
        {
            Console.Clear();
            using(DataBaseAlumnoContext DBAC = new DataBaseAlumnoContext("AntherDeveloperVS_SQLCipher"))
            {
                foreach (Alumno al in DBAC.DataBaseAlumno.ToList())
                    Console.WriteLine(al.ReturnDatocConRutaImagen()+"\n");
            }

            Console.WriteLine("\nPrecione Enter para regresar...");
            Console.ReadLine();
        }

        //SELECCIONADOR DE ALUMNOS
        private Alumno? SeleccionaAlumno()
        {
            int opc = 0;
            List<Alumno> Alumnos = SQLiteDataStudent.AllAlumnos(false);

            if(Alumnos.Count == 0)
            {
                Console.Write("No hay alumnos\nRegresando...\n");
                Thread.Sleep(1500);
                return null;
            }

            Console.Write($"Seleccione un alumno -> 0-{Alumnos.Count - 1}: ");
            try
            {
                opc = byte.Parse(Console.ReadLine() ?? "0");
                if (opc < 0)
                {
                    Console.Write("Usuario no valido, minimo el Usuaio '0', se usara el usuaio 0 por defecto");
                    opc = 0;
                }
                else if(opc > Alumnos.Count() - 1)
                {
                    Console.Write($"Usuario {opc} no valido, se usara el usuario {Alumnos.Count - 1}");
                    opc = Alumnos.Count - 1;
                }
                Console.Write($"Quiere ver al usuario {opc}? (si/NO): ");
                string usuaioMostrar = Console.ReadLine() ?? "no";
                if (usuaioMostrar == "SI" || usuaioMostrar == "si")
                {
                    Alumnos[opc].Mostrarse();
                }
                Console.Write($"Alumno ({opc}) Seleccionado.\n");
                Thread.Sleep(2000);
                return Alumnos[opc];
            }
            catch
            {
                Console.Write("No selecciono nada, sera el usuario 0 por defecto.\nRegresando...");
                Thread.Sleep(1500);
                return Alumnos[0];
            }
        }

        private short SeleccionaGrupo()
        {
            int opc = 0;

            List<short> Grupos = SQLiteDataStudent.ExtraeGrupos();

            if (Grupos.Count == 0) return -1;

            Console.WriteLine("Selecciona un Grupo:");
            
            for(int i=0; i< Grupos.Count; i++)
            {
                Console.WriteLine($"Grupo -> {Grupos[i]}: {i + 1}");
            }
            Console.Write("Opcion: ");
            try
            {
                opc = int.Parse(Console.ReadLine() ?? "0");

                if (opc < 1) opc = 1;
                if (opc > Grupos.Count) opc = Grupos.Count;

                Console.WriteLine($"Ah eleguido el grupo -{Grupos[opc - 1]}-");
                Thread.Sleep(2000);
                return Grupos[opc - 1];
            }
            catch
            {
                Console.WriteLine($"Error, Datos no validos, seleccion por defecto {Grupos[0]}");
                Thread.Sleep(2000);
                return Grupos[0];
            }
        }

        //CREADOR DE TXT DE UN ALUMNO
        private void CreaTXTAlumnos(Alumno? auxAl, short Grupo = -1, bool all = false)
        {
            if (auxAl == null && Grupo == -1 && !all) return;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", $"{((auxAl != null) ? (auxAl.name) : ((Grupo == -1) ? ("AllAlumnos") : (Grupo)))}.txt");
            char opc;
            Console.Write($"Quiere eleguir ruta (s/N): ");
            try
            {
                opc = (char.TryParse(Console.ReadLine(), out _)) ? (char.Parse(Console.ReadLine())) : ('N');

                if (opc == 'S' || opc == 's')
                {
                    OpenFolderDialog ofld = new OpenFolderDialog();
                    ofld.Title = "Selecciona Carpeta";

                    if (ofld.ShowDialog() == true)
                    {
                        path = Path.Combine(ofld.FolderName, $"{ ((auxAl != null) ? (auxAl.name) : ( (Grupo == -1)? ("AllAlumnos"):(Grupo) )) }.txt");
                        if (auxAl != null) StudentData.CreaTXTAlumno(auxAl, path);
                        else if (Grupo != -1) StudentData.CreaTXTGrupoAlumnosAllAlumnos(SQLiteDataStudent.ExtraeAlumnosPorGrupo(Grupo), path);
                        else StudentData.CreaTXTGrupoAlumnosAllAlumnos(SQLiteDataStudent.AllAlumnos(false), path);
                    }
                    else
                    {
                        if (auxAl != null) StudentData.CreaTXTAlumno(auxAl, path);
                        else if (Grupo != -1) StudentData.CreaTXTGrupoAlumnosAllAlumnos(SQLiteDataStudent.ExtraeAlumnosPorGrupo(Grupo), path);
                        else StudentData.CreaTXTGrupoAlumnosAllAlumnos(SQLiteDataStudent.AllAlumnos(false), path);
                    }
                }
                else
                {
                    if (auxAl != null) StudentData.CreaTXTAlumno(auxAl, path);
                    else if (Grupo != -1) StudentData.CreaTXTGrupoAlumnosAllAlumnos(SQLiteDataStudent.ExtraeAlumnosPorGrupo(Grupo), path);
                    else StudentData.CreaTXTGrupoAlumnosAllAlumnos(SQLiteDataStudent.AllAlumnos(false), path);
                }

                if (File.Exists(path)) Console.WriteLine("Archivo creado con exito");
                else Console.WriteLine("ups... Algo salio mal. Error {Archivo no creado}");

                Console.Write("Regresando...");
                Thread.Sleep(2500);
            }
            catch
            {
                Console.WriteLine("Opcion erronea\nRegresando");
                Thread.Sleep(2000);
            }

        }


        //CREADOR DEL EXCEL DEL ALUMNO O DE TODOS LOS ALUMNOS
        private void CrearExcel(Alumno? auxAl, short anGroup = -1, bool AllALumnos = false)
        {
            char opc;
            if (auxAl == null && AllALumnos == false && anGroup == -1) return;

            Console.Write($"Quiere Eleguir la ruta del archivo? (s/N): ");
            opc = (char.TryParse(Console.ReadLine(), out _)) ? (char.Parse(Console.ReadLine())) : ('N');
            if(opc == 'S' || opc == 's')
            {
                OpenFolderDialog ofld = new OpenFolderDialog();
                ofld.Title = "Selecciona Carpeta Destino";
                
                if(ofld.ShowDialog() == true)
                {
                    if (anGroup > -1)
                    {
                        ArchivosExcel.ExcelGrupo(Path.Combine(ofld.FolderName, $"{anGroup.ToString()}.xlsx"), SQLiteDataStudent.ExtraeAlumnosPorGrupo((short)anGroup));
                        return;
                    }
                    if (!AllALumnos && auxAl != null) ArchivosExcel.ExcelAlumno(Path.Combine(ofld.FolderName, $"{auxAl.name}.xlsx"), auxAl);
                    else ArchivosExcel.AllAlumnos(Path.Combine(ofld.FolderName, $"All_Alumnos_APP.xlsx"), SQLiteDataStudent.ExtraeAllAlumnosExcel());
                }

            }
            else
            {
                if (anGroup > -1)
                {
                    ArchivosExcel.ExcelGrupo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", $"{anGroup.ToString()}.xlsx"), SQLiteDataStudent.ExtraeAlumnosPorGrupo((short)anGroup));
                    return;
                }
                    if (!AllALumnos && auxAl != null) ArchivosExcel.ExcelAlumno(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", $"{auxAl.name}.xlsx"), auxAl);
                    else ArchivosExcel.AllAlumnos(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", "All_Alumnos_APP.xlsx"), SQLiteDataStudent.ExtraeAllAlumnosExcel());
            }
        }

        //end class
    }
}
