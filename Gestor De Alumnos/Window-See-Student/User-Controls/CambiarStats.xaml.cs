/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using Gestor_De_Alumnos.Guardar_Datos;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Gestor_De_Alumnos.Clases_utiles;
using System.Windows.Media.Imaging;

namespace Gestor_De_Alumnos.Window_See_Student.User_Controls
{

    public partial class CambiarStats : UserControl
    {
        private Key PreviewKeyInputTXB;
        private Alumno Al;
        private MostrarDatos WNDMD;
        //Ruta relativa, despues solo agregamos la ruta de la imagen.
        private string RutaCarpetaImagenes = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ImagenesEstudiantes");

        public CambiarStats(MostrarDatos Md, Alumno auxAl)
        {
            InitializeComponent();
            this.Al = auxAl;
            this.WNDMD = Md;
            MuestraDatosAlumno(Al);
            this.Loaded += UserControl_Loaded;
        }

        private void UserControl_Loaded(object? sender, RoutedEventArgs e)
        {
            if (DataContext is Alumno Al)
            {
                BTNExit.IsEnabled = false;
                BTNExit.Visibility = Visibility.Hidden;
                BTNExit.Margin = new Thickness(0, 0, 0, 0); 
                MuestraDatosAlumno(Al);
                this.Al = Al;
            }
        }

        //Actualizamos o mostramos los datos del alumno dando los datos a los controles
        private void MuestraDatosAlumno(Alumno Al)
        {
            // Ruta de imagen
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Al.RutaImagen);
            IMGCambiaImagen.Source = StudentData.CargaImage( (System.IO.File.Exists(path)) ? (Al.RutaImagen) : (App.RelativeStudentPath));
            TXBNombre.Text = Al.name;
            TXBApellidos.Text = Al.apellidos;
            TXBTelefono.Text = Al.Telefono;

            TXBMatricula.Text = Al.Matricula.ToString();
            TXBGrupo.Text = Al.grupo.ToString();
            TXBCapac.Text = Al.capacitacion;
            TXBClub.Text = Al.club;
            TXBBach.Text = Al.bachillerato;
            TXBTurno.Text = Al.turno;
            TXBProf.Text = Al.profServSoc;

            DTPKFechaNacimiento.SelectedDate = DateTime.Parse(Al.fechaNacimiento);
            DTPKFechaNacimiento.Text = Al.fechaNacimiento;

            LBHorasServProf.Content = $"Profesor que libera -- Horas de servicio actuales -> {Al.horasServicioSocial}";
        }

        private bool MSGChange()
        {
            MessageBoxResult MBRST = MessageBox.Show("Esta Conciente de las consecuencias que conlleva modificar un alumno", "Deteccion de cambio", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (MBRST == MessageBoxResult.Yes) return true;
            return false;
        }

        //Reutilizamos esto para mejor lectura y menos verbosidad. Esto detecta que si cambie el texto de acuerdo al original
        private bool DetectaCambio(string? TXTTextBox, string TXTOriginal, int LongText = -1)
        {
            if ((string.IsNullOrEmpty(TXTTextBox) || TXTTextBox == TXTOriginal) ||
                (LongText != -1 && TXTOriginal.Length != LongText)) return false;

            return false;
        }

        private void BTNChangeStats_Click(object sender, RoutedEventArgs e)
        {
            string errorMessage = "";
            if (MSGChange() == false) return;

            //Verificamos que los datos esten bien
            errorMessage = (!DetectaCambio(TXBNombre.Text, Al.name)) ? ("El nombre esta mal\n") : ("");
            errorMessage = (!DetectaCambio(TXBApellidos.Text, Al.apellidos)) ? ("Los apellidos estan mal\n") : ("");
            errorMessage = (!DetectaCambio(TXBTelefono.Text, Al.Telefono, 10)) ? ("El telefono esta mal\n") : ("");
            errorMessage = (!DetectaCambio(TXBGrupo.Text, Al.grupo.ToString(), 3)) ? ("El grupo esta mal\n") : ("");
            errorMessage = (!DetectaCambio(TXBCapac.Text, Al.capacitacion)) ? ("La capacitacion esta mal\n") : ("");
            errorMessage = (!DetectaCambio(TXBBach.Text, Al.bachillerato)) ? ("El bachillerato esta mal\n") : ("");
            errorMessage = (!DetectaCambio(TXBMatricula.Text, Al.Matricula.ToString(), 8)) ? ("La matricula esta mal\n") : ("");
            errorMessage = (!DetectaCambio(TXBClub.Text, Al.club)) ? ("El club esta mal\n") : ("");
            errorMessage = (!DetectaCambio(TXBTurno.Text, Al.turno)) ? ("El turno esta mal\n") : ("");
            errorMessage = (!DetectaCambio(TXBProf.Text, Al.profServSoc)) ? ("El capacitador esta mal\n") : ("");
            errorMessage = (!DetectaCambio(DTPKFechaNacimiento.SelectedDate?.ToShortDateString(), Al.fechaNacimiento, 10)) ? ("La fecha de nacimiento esta mal\n") : ("");

            // !string.IsNullOrEmpty(errorMessage);

            //Datos Personales
            if (!string.IsNullOrEmpty(errorMessage))
            {
                //si hay error en el turno
                if (TXBTurno.Text != "Matutino" && TXBTurno.Text != "Vespertino")
                {
                    MessageBox.Show($"Turno --{TXBTurno.Text}-- no disponible.\nTurnos:\nMatutino\nVespertino");
                    return;
                }

                //llamamos directamente a su cambiador de estadisticas
                Al.CambiaAllDatos(Al.Id, TXBNombre.Text, TXBApellidos.Text, (DTPKFechaNacimiento.SelectedDate?.ToShortDateString() ?? "01/01/2010"), TXBTelefono.Text, short.Parse(TXBGrupo.Text),
                    TXBMatricula.Text, TXBTurno.Text, Al.horasServicioSocial, TXBProf.Text, TXBCapac.Text, TXBBach.Text, TXBClub.Text, Al.RutaImagen);

                //Ponemos su ruta de imagen y nos aseguramos de copiar la imagen a la carpeta de imagenes de estudiantes, para evitar problemas con las rutas
                //Tambien nos aseguramos que la ruta de la imagen si exista para evitar problemas en caso de que no ponga nada
                string? aux = (IMGCambiaImagen.Source as BitmapImage)?.UriSource.LocalPath;
                StudentData.CopiaImagen(Al,
                    (aux == null) ? (System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, App.RelativeStudentPath)) : (aux)
                    , AppDomain.CurrentDomain.BaseDirectory, "ImagenesEstudiantes");

                //y llamamos a actualizar la base de datos
                SQLiteDataStudent.AddRemoveUpdateAlumno(Al, false, true);

                MuestraDatosAlumno(Al);

                MessageBox.Show("Cambios Guardados (si es que se hicieron) :)");
                return;
            }
            else MessageBox.Show("Error, Compruebe los Datos.\nSi persiste, Reinicie los Datos :)");

        }

        private void BTNResetStudent_Click(object sender, RoutedEventArgs e) => MuestraDatosAlumno(Al);

        private void BTNCambiaImagen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Cambiar Imagen";
            ofd.Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp";
            ofd.DefaultDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if (ofd.ShowDialog() == true)
            {
                if(ofd.FileName != null)
                {
                    IMGCambiaImagen.Source = StudentData.CargaImage(ofd.FileName);
                }
            }
        }

        /*AQUI VAMOS A CONTROLAR LOS DATOS INGRESADOS POR EL USUAIO*/

        //Este sera usado para todos los PreviewInputKey de los TexBox
        private void TXBPreviewKeyDown(object sender, KeyEventArgs e) => PreviewKeyInputTXB = e.Key;

        private void TXBTelefono_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) => InputClassTXB.DetectaPreviewInpunNumber(TXBTelefono, e, 12);

        private void TXBTelefono_TextChanged(object sender, TextChangedEventArgs e) => InputClassTXB.InsertarTextoTXB(TXBTelefono, PreviewKeyInputTXB, "-", new List<int> { 3, 8 });

        private void TXBGrupo_PreviewTextInput(object sender, TextCompositionEventArgs e) =>  InputClassTXB.DetectaPreviewInpunNumber(TXBGrupo, e, 3);

        private void TXBMatricula_PreviewTextInput(object sender, TextCompositionEventArgs e) => InputClassTXB.DetectaPreviewInpunNumber(TXBMatricula, e, 8);

        private void BTNExit_Click(object sender, RoutedEventArgs e) => WNDMD.Close();

    }
}
