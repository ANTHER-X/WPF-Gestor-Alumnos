/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using Gestor_De_Alumnos.Guardar_Datos;
using Gestor_De_Alumnos.Window_Add_Alumno;
using Gestor_De_Alumnos.Window_See_Student;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Gestor_De_Alumnos.Terminal;

namespace Gestor_De_Alumnos
{

    public partial class MainWindow : Window
    {
        private DispatcherTimer FechaTimer;
        private string CarpetaDatos = "";
        private SQLiteDataStudent DataBase;

        public MainWindow()
        {
            InitializeComponent();

            //Para la Base de datos
            SQLitePCL.Batteries_V2.Init();
            DataBase = new SQLiteDataStudent();

            //Para la fecha que se mostrara
            FechaTimer = new DispatcherTimer();
            FechaTimer.Interval = TimeSpan.FromSeconds(1);
            FechaTimer.Tick += HORA;
            FechaTimer.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                //Cargamos los datos
                CreaRutas();
            });
        }

        private void HORA(object? sender, EventArgs e) => LBFecha.Content = ($"Fecha: {DateTime.Now.ToString()}");

        private void CargaImagenEXE(string Ruta, string Carpeta)
        {
            if (!Directory.Exists(Carpeta)) Directory.CreateDirectory(Carpeta);

            using (Stream? Recurso = Assembly.GetExecutingAssembly().GetManifestResourceStream("Gestor_De_Alumnos.Imagenes_Iconos.alumno.png"))
            {
                using (FileStream ArchivoCopiado = new FileStream(Ruta, FileMode.Create, FileAccess.Write)) Recurso?.CopyTo(ArchivoCopiado);
            }
        }

        private void CreaRutas()
        {
            string StudentImageDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImagenesEstudiantes");
            string RutaStudenDefaultImage = System.IO.Path.Combine(StudentImageDirectory, "alumno.png");
            CargaImagenEXE(RutaStudenDefaultImage, StudentImageDirectory);

            CarpetaDatos = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos");

            if (!Directory.Exists(CarpetaDatos)) Directory.CreateDirectory(CarpetaDatos);

            SQLiteDataStudent.CreaDataBase();
        }

        public void EliminaAlumno(Alumno al)
        {
            SQLiteDataStudent.AddRemoveUpdateAlumno(al, true);
            MessageBox.Show($"Estudiante Matricula: {al.Matricula}. Eliminado", "DELETE STUDENT", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ADDAlumno(Alumno Al, string RutaImagen)
        {
            StudentData.CopiaImagen(Al, RutaImagen, AppDomain.CurrentDomain.BaseDirectory, "ImagenesEstudiantes");
            
            SQLiteDataStudent.AddRemoveUpdateAlumno(Al);
        }

        public Alumno ReturnAddAlumno(Alumno Al, string RutaImagen)
        {
            StudentData.CopiaImagen(Al, RutaImagen, AppDomain.CurrentDomain.BaseDirectory, "ImagenesEstudiantes");
            return Al;
        }

        private void BTNAddAlumno_Click(object sender, RoutedEventArgs e)
        {
            WNDAddAlumno NewAlumno = new WNDAddAlumno(this);//pasamos la ventana
            NewAlumno.ShowDialog();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void VerAlumnos_Click(object sender, RoutedEventArgs e)
        {
            WNDSeeStudent VerAlumnos = new WNDSeeStudent(this);
            VerAlumnos.ShowDialog();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void Creditos()
        {
            WndCreditos creditos = new WndCreditos();
            creditos.ShowDialog();
            /*
             MessageBoxResult msg = MessageBox.Show("Cual credito, wnd=Yes, msg=no, cancel=cancel", "Creditos", MessageBoxButton.YesNoCancel);
            if (msg == MessageBoxResult.Yes)
            {
                WndCreditos creditos = new WndCreditos();
                creditos.ShowDialog();
            }
            else if (msg == MessageBoxResult.No)
            {
                MessageBox.Show($"       Creditos:\n" +
                                $"Developer: ANTHER-X.\n" +
                                $"Instagram: https://www.instagram.com/fernandocisneroslemus\n" +
                                $"EMail: fernandocisneroslemus@gmail.com\n" +
                                $"GitHub: https://github.com/ANTHER-X" +
                                $"\n\n        Tecnologías y lenguajes Usados:" +
                                $"\nC#" +
                                $"\nWPF (Windows Presentation Foundation)" +
                                $"\nClosedXML\n" +
                                $"SQLCipher\n" +
                                $"EntityFrameworkCore"
                                , "Creditos", MessageBoxButton.OK, MessageBoxImage.Information);
            }*/
        }

        private void LBCreditos_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) => Creditos();

        private void BTNSalir_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        /*Para la Ventana de Notas*/
        private bool openNotas = false;

        private void NotasClosed(object? sender, EventArgs e) => openNotas = false;
        private void CreaNotas()
        {
            if (openNotas == false)
            {
                openNotas = true;
                Lib_Bloc_De_Notas.Notas wndNotas = new Lib_Bloc_De_Notas.Notas();
                wndNotas.Closed += NotasClosed;
                wndNotas.Show();
            }
        }

        private void BTNNotas_Click(object sender, RoutedEventArgs e) => CreaNotas();

        /*Codigos*/
        private void TXBCodes_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                switch (TXBCodes.Text)
                {
                    case "Notas": { CreaNotas(); break; }
                    case "Creditos": { Creditos(); break; }
                    case "Salir": { Application.Current.Shutdown(); break; }
                    case "Terminal":
                        {
                            MessageBoxResult mbr = MessageBox.Show("La Consola son solo accesos de prueba temporales\nPuede que esto MATE LOS PROCESOS la APP\nDecea continuar de igual forma?"
                            , "Creador de Consola", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (mbr == MessageBoxResult.Yes)
                            {
                                Task.Run(() =>
                                {
                                    TerminalWin Console = new TerminalWin(this);
                                });
                            }
                            break;
                        }

                    case "":
                        {
                            break;
                        }
                }
            }
        }

    }
}