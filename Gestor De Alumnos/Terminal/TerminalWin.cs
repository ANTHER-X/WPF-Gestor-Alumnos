/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using System.IO;
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
              _                          _
             | |_ ______ ______ ______ _| |_
           |_   _|______|______|______|_   _|
            _|_| _____   ____    __  __ |_|
            | |  / ____| |  _ \  |  \/  | |   
            | | | |      | |_) | | \  / | |   
            | | | |      |  _ <  | |\/| | |   
            | | | |____  | |_) | | |  | | |   
            | |  \_____| |____/  |_|  |_| |   
            | |                         | |   
            |_|_                        |_|   
            _| |_ ______ ______ ______ _| |_ 
           |_   _|______|______|______|_   _|
             |_|                        |_|\";
            bool salir = false;
            byte opc = 4;
            string AlumnoIndex = "none";

            //bucle de "Mensajes" jaja
            while(salir == false)
            {
                Console.WriteLine(portada);
                Console.Write($"V: 1.0.0\nSeleccione una opcion:\nManejar Tu DataBase: 1\nGenerar Exel De Alumno (Al IndexList = {AlumnoIndex}): 2\n" +
                    $"Generar Archivo Excel con un grupo: 3\nGenerar Archivo Excel de todos los Alumnos: 4\nCrear Archivo TXT de Alumno (index -> ={AlumnoIndex}): 5\nVer la DataBase: 6\nVer Cantidad de Alumnos: 7\nSalir: 8\nOpcion: ");
                
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
                            if (Al != null) CrearExcel(Al, grupo);
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
                            CreaTXTunAlumno(Al);
                            break;
                        }
                    //Ver la DataBase
                    case 6:
                        {
                            VerDataBase();
                            break;
                        }
                    case 7:
                        {
                            Console.WriteLine($"{SQLiteDataStudent.ReturnCantAlumnos()}\nPrecione una tecla para regresar...");
                            Console.ReadKey();
                            break;
                        }
                    //Salir del bucle
                    case 8:
                        {
                            salir = true;
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Error, intente de nuevo...");
                            Thread.Sleep(2000);
                            break;
                        }
                }

                Console.Clear();
            }

            //Fin de la Consola
            Console.WriteLine("Espero no se haya roto nada :v.\nSaliendo...");
            Thread.Sleep(3500);
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
                Console.Write("No hay alumnos\nRegresando...");
                Thread.Sleep(2500);
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
                Console.Write($"Alumno ({opc}) Seleccionado.\nRegresando...");
                Thread.Sleep(3000);
                return Alumnos[opc];
            }
            catch
            {
                Console.Write("No selecciono nada, sera el usuario 0 por defecto.\nRegresando...");
                Thread.Sleep(2500);
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
                Thread.Sleep(3000);
                return Grupos[opc - 1];
            }
            catch
            {
                Console.WriteLine($"Error, Datos no validos, seleccion por defecto {Grupos[0]}");
                Thread.Sleep(3000);
                return Grupos[0];
            }
        }

        //CREADOR DE TXT DE UN ALUMNO
        private void CreaTXTunAlumno(Alumno? auxAl)
        {
            if(auxAl != null)
            {
                string path;
                char opc;
                Console.Write($"Quiere eleguir ruta (S/N): ");
                try
                {
                    opc = char.Parse(Console.ReadLine() ?? "0");

                    if(opc == 'S' || opc == 's')
                    {
                        OpenFolderDialog ofld = new OpenFolderDialog();
                        ofld.Title = "Selecciona Carpeta";

                        if(ofld.ShowDialog() == true)
                        {
                            path = Path.Combine(ofld.FolderName, $"{auxAl.name}.txt");
                            StudentData.CreaTXTAlumno(auxAl, path);
                        }
                        else
                        {
                            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", $"{auxAl.name}.txt");
                            StudentData.CreaTXTAlumno(auxAl, path);
                        }
                    }
                    else
                    {
                        path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", $"{auxAl.name}.txt");
                        StudentData.CreaTXTAlumno(auxAl, path);
                    }

                    if (File.Exists(path)) Console.WriteLine("Archivo creado con exito");
                    else Console.WriteLine("ups... Algo salio mal. Error {Archivo no creado}");

                    Console.Write("Regresando...");
                    Thread.Sleep(3500);
                }
                catch
                {
                    Console.WriteLine("Opcion erronea\nRegresando");
                    Thread.Sleep(3000);
                }
            }

        }


        //CREADOR DEL EXCEL DEL ALUMNO O DE TODOS LOS ALUMNOS
        private void CrearExcel(Alumno? auxAl, short anGroup = -1, bool AllALumnos = false)
        {
            char opc;
            if (auxAl == null && AllALumnos == false && anGroup == -1) return;

            Console.Write($"Quiere Eleguir la ruta del archivo? (S/N): ");
            opc = char.Parse(Console.ReadLine() ?? "0");
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
                    else ArchivosExcel.AllAlumnos(Path.Combine(ofld.FolderName, $"Alumnos_Cobaem.xlsx"), SQLiteDataStudent.ExtraeAllAlumnosExcel());
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
                    else ArchivosExcel.AllAlumnos(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", "Alumnos_Cobaem.xlsx"), SQLiteDataStudent.ExtraeAllAlumnosExcel());
            }
        }

        //end class
    }
}
