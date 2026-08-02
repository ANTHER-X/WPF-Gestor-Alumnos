/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using Gestor_De_Alumnos.Guardar_Datos;

/*Esto nos ayudara a cargar el Dialog del gestor de archivos para guardar o cargar archivos.
 Vaya, el que usamos al crear el bloc de notas (que no acabamos)*/
using Microsoft.Win32;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Gestor_De_Alumnos.Clases_utiles;

namespace Gestor_De_Alumnos.Window_Add_Alumno
{

    public partial class WNDAddAlumno : Window
    {
        MainWindow WNDPrincipal;
        private Key auxTextKey;
        private byte Turno = 0;
        string FechaNacimeinto = "none";
        private BitmapImage BMImageDefault = StudentData.CargaImage(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, App.RelativeStudentPath));

        public WNDAddAlumno(MainWindow Principal)
        {
            InitializeComponent();
            this.WNDPrincipal = Principal;
            IMGStudent.Source = BMImageDefault;
        }


        private void ADDAlumno()
        {
            bool Entrar = true;
            string MensajeDatosError = "";

            if (string.IsNullOrEmpty(FechaNacimeinto)) { MensajeDatosError += $"Error: Fecha no establecida\n"; Entrar = false; }
            if (string.IsNullOrWhiteSpace(TXTName.Text)) { MensajeDatosError += $"Error: Nombre no establecido\n"; Entrar = false; }
            if (string.IsNullOrWhiteSpace(TXTTelefono.Text)) { MensajeDatosError += $"Error: Telefono no establecido\n"; Entrar = false; }
            else if (TXTTelefono.Text.Length != 12) { MensajeDatosError += $"Error: Telefono incompleto\n"; Entrar = false; }
            if (string.IsNullOrWhiteSpace(TXTGrupo.Text)) { MensajeDatosError += $"Error: Grupo no establecido\n"; Entrar = false; }
            else if (TXTGrupo.Text.Length != 3) { MensajeDatosError += $"Error: Grupo Incompleto\n"; Entrar = false; }
            if (string.IsNullOrWhiteSpace(TXTMatricula.Text)) { MensajeDatosError += $"Error: Matricula no establecida\n"; Entrar = false; }
            else if (TXTMatricula.Text.Length != 8) { MensajeDatosError += $"Error: Matricula incompleta\n"; Entrar = false; }
            if (Turno == 0) { MensajeDatosError += $"Error: Turno no establecido\n"; Entrar = false; }
            

            if (Entrar)
            {
                string auxTurn, cap = "none", Bach = "none", club = "none", maestro = "none";
                int HorasServ = 0;

                if (Turno == 1) auxTurn = "Matutino";
                else auxTurn = "Vespertino";

                if (!string.IsNullOrWhiteSpace(TXTCapacitacion.Text)) cap = TXTCapacitacion.Text;
                if (!string.IsNullOrWhiteSpace(TXTBachillerato.Text)) Bach = TXTBachillerato.Text;
                if (!string.IsNullOrWhiteSpace(TXTClub.Text)) club = TXTClub.Text;
                if (!string.IsNullOrWhiteSpace(TXTHorasServicio.Text)) HorasServ = int.Parse(TXTHorasServicio.Text);
                if (!string.IsNullOrWhiteSpace(TXTMaestroServicioSocial.Text)) maestro = TXTMaestroServicioSocial.Text;

                if (HorasServ < 0) HorasServ = 0;
                else if (HorasServ > 120) HorasServ = 120;

                string? raux = (IMGStudent.Source as BitmapImage)?.UriSource?.LocalPath;
                string Ruta = (raux == null) ? (""):(raux);

                //Vamos a ver si alguien en su grupo ya tiene el mismo nombre, si es asi le agregamos un numero al final del nombre.
                //En caso de que el alumno tenga alguna matricula ya existente, no se agregara nada y 
                //mostraremos un mensaje de error especificando esto.
                if(SQLiteDataStudent.ExistMatricula(TXTMatricula.Text))
                {
                    MessageBox.Show("Error: Ya existe un alumno con esa matricula", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                List<short> auxGrupos = SQLiteDataStudent.ExtraeGrupos();
                foreach (short g in auxGrupos)
                {
                    if (g == short.Parse(TXTGrupo.Text))
                    {
                        List<Alumno> AlumnosGrupo = SQLiteDataStudent.ExtraeAlumnosPorGrupo(g);
                        foreach (Alumno al in AlumnosGrupo)
                        {
                            if (al.name == TXTName.Text && al.apellidos == TXTApellidos.Text)
                            {
                                TXTName.Text += $" (1)";
                                break;
                            }
                        }
                    }
                }

                WNDPrincipal.ADDAlumno(new Alumno(TXTName.Text, TXTApellidos.Text, FechaNacimeinto, TXTTelefono.Text,
                short.Parse(TXTGrupo.Text), TXTMatricula.Text, auxTurn, "NULO, SERA DESPUES", HorasServ, maestro, cap, Bach, club), Ruta);
                
                MessageBox.Show("Alumno agregado", "New ADD", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else MessageBox.Show(MensajeDatosError, "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);

        }

        /*Procesamos que las teclas insertadas sean solo numeros o caracteres de lanzamiento*/
        //obtenemos la tecla
        private void TXTTelefono_PreviewKeyDown(object sender, KeyEventArgs e) => auxTextKey = e.Key;

        private void TXTTelefono_PreviewTextInput(object sender, TextCompositionEventArgs e) => InputClassTXB.DetectaPreviewInpunNumber(TXTTelefono, e, 12);
        
        private void TXTTelefono_TextChanged(object sender, TextChangedEventArgs e) => InputClassTXB.InsertarTextoTXB(TXTTelefono, auxTextKey, "-", new List<int> { 3, 8 });

        /*Solo tomamos numeros y hasta una cierta cantidad de estos*/
        private void TXTGrupo_PreviewTextInput(object sender, TextCompositionEventArgs e) => InputClassTXB.DetectaPreviewInpunNumber(TXTGrupo, e, 3);
        
        private void TXTMatricula_PreviewTextInput(object sender, TextCompositionEventArgs e) => InputClassTXB.DetectaPreviewInpunNumber(TXTMatricula, e, 8);
        
        private void TXTHorasServicio_PreviewTextInput(object sender, TextCompositionEventArgs e) => InputClassTXB.DetectaPreviewInpunNumber(TXTHorasServicio, e, 3);

        /*Verificamos la seleccion de los turnos*/
        private void Mat_Checked(object sender, RoutedEventArgs e) => Turno = 1;

        private void Ves_Checked(object sender, RoutedEventArgs e) => Turno = 2;

        private void BTNADD_Click(object sender, RoutedEventArgs e) => ADDAlumno();

        //cerramos la ventana
        private void BTNSalir_Click(object sender, RoutedEventArgs e) => this.Close();

        //anidamos una imagen al usuario
        private void BTNAddImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ofd.Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp";
            ofd.Title = "Abrir imagen";

            if (ofd.ShowDialog() == true) IMGStudent.Source = StudentData.CargaImage(ofd.FileName);
        }

        private void DTPKFechaNacimiento_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DTPKFechaNacimiento.SelectedDate.HasValue) FechaNacimeinto = DTPKFechaNacimiento.SelectedDate.Value.ToShortDateString();
        }
    }
}
