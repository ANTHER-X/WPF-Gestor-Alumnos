/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using Gestor_De_Alumnos.Guardar_Datos;
using System.Windows.Media;

namespace Gestor_De_Alumnos.Window_See_Student.User_Controls
{

    /*Como las rutas de las imagenes son relativas, y al momento de mostrar una imagen el Source espera
     rutas absolutas, vamos a tomar y convertir la ruta pasada por Binding al Source de la Image y le
    vamos a anidar la ruta del .exe, esto se hace usando una Plantilla y pasandola como parametro*/
    public class ReescrituraRutaImagenAlumno : IValueConverter
    {
        object? IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? rutaRelativa = (value as string);
            if (string.IsNullOrWhiteSpace(rutaRelativa)) return null;
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rutaRelativa);
        }

        object? IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class ListViewSeeStudent : UserControl
    {
        private short auxGrupo;

        public ListViewSeeStudent()
        {
            InitializeComponent();

            if (VerGrupos.ModStatsAlumno == true)
            {
                BTNModificaAlumno.Visibility = Visibility.Visible;
                BTNModificaAlumno.IsEnabled = true;
                BTNModificaAlumno.Height = 36;
                BTNModificaAlumno.Width = 94;
            }
            else
            {
                BTNModificaAlumno.Visibility = Visibility.Collapsed;
                BTNModificaAlumno.IsEnabled = false;
            }
        }

        // Método genérico para buscar el primer padre de cierto tipo en el árbol visual
        //Me lo dio ChatGPT :)
        public static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null && !(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as T;
        }

        private void Actualiza()
        {
            if(this.DataContext is Alumno al)
            {
                /*Si cambia de grupo lo quitamos de la lista que vemos y actualizamos los combobox,
                 esto en caso de que sea el ultimo usuario del grupo (o sea que el grupo se muera), y para
                actualizar los usuarios disponibles por cierto grupo*/
                if(auxGrupo != al.grupo)
                {
                    /*Usamos el FindParent de ChatGPT y pasamos este UserControl para buscar el padre
                     que tambien es un user control*/
                    UserControl? userControl = FindParent<UserControl>(this);

                    /*Vemos que si sea el padre y no este UserControl y despues vemos si es
                     el UserControl que necesitamos, si es asi, pues lo usamos para reformar los datos*/
                    if (userControl != null && userControl != this)
                    {
                        if(userControl is VerGrupos VG)
                        {
                            VG.EliminaCambiado(al);
                            VG.ReformaDatos(VG.Alumnos,VG.Grupos);
                        }
                    }
                }

                //si no cambia de grupo pues solo actualizamos los datos que se hayan tomado
                //pero como no sabemos cuales se cambiaron pues reformamos todo (solo de este usuaio)*/
                LBNombre.Content = $"Nombre: {al.name}";
                LBApellidos.Content = $"Apellidos: {al.apellidos}";
                LBTelefono.Content = $"Telefono (+52: {al.Telefono})";
                LBMatricula.Content = $"Matricula: {al.Matricula}";
                LBEdad.Content = $"Edad: {al.edad}";
                LBTurno.Content = $"Turno: {al.turno}";
                LBProfServSoc.Content = $"Profesor Que Libera Horas: {al.profServSoc}";
                LBClub.Content = $"Club: {al.club}";
                LBCapacit.Content = $"Capacitacion: {al.capacitacion}";
                LBBach.Content = $"Bachillerato: {al.bachillerato}";
                LBHorasServ.Content = $"Horas De Servicio Social:\nTotal Horas: {al.horasServicioSocial}";

                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, al.RutaImagen);
                IMGAlumno.Source = StudentData.CargaImage((System.IO.File.Exists(path)) ? (al.RutaImagen) : (App.RelativeStudentPath));
            }
        }

        private void BTNModificaAlumno_Click(object sender, RoutedEventArgs e)
        {
            //Vemos si el contenido del UserControl si es un Alumno
            if(this.DataContext is Alumno Al)
            {
                auxGrupo = Al.grupo;
                MostrarDatos MD = new MostrarDatos(null);
                MD.CambiarStats(Al);
                MD.ShowDialog();
                Actualiza();
            }
        }
    }
}
