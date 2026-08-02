/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Gestor_De_Alumnos.Guardar_Datos
{
    class StudentData
    {

        public StudentData() { }


        //Para guardar los datos del alumno en un archivo de texto
        static public void CreaTXTGrupoAlumnosAllAlumnos(List<Alumno> Lista, string ruta)
        {
            using (StreamWriter SW = new StreamWriter(new FileStream(ruta, FileMode.Create, FileAccess.Write)))
            {
                foreach (Alumno aux in Lista)
                {
                    SW.WriteLine(aux.ReturnDatos());
                }
            }
        }

        static public void CreaTXTAlumno(Alumno al, string ruta)
        {
            using (FileStream fs = new FileStream(ruta, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(al.ReturnDatos());
                }
            }
        }


        //Sacamos una imagen de una ruta dada
        static public BitmapImage CargaImage(string RutaArchivo)
        {
            BitmapImage BM = new BitmapImage();
            BM.BeginInit();

            BM.UriSource = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RutaArchivo), UriKind.Absolute);

            BM.EndInit();
            return BM;
        }

        static public void CopiaImagen(Alumno Al, string URIOriginal, string rutaEXE, string Carpeta, Image? IMG = null)
        {
            //si la imagen es la que esta por defecto, esa la dejamos, asi nos podemos decemas de MB
            if (URIOriginal == Path.Combine(AppDomain.CurrentDomain.BaseDirectory, App.RelativeStudentPath))
            {
                Al.RutaImagen = App.RelativeStudentPath;
                return;
            }

            //si la ruta de la imagen es una imagen propia, la copiamos a la carpeta
            using (FileStream Original = new FileStream(URIOriginal, FileMode.Open, FileAccess.Read))
            {
                string RelativeImage = System.IO.Path.Combine(Carpeta, $"{Al.Id.ToString()}.png");
                using (FileStream Copia = new FileStream(System.IO.Path.Combine(rutaEXE, RelativeImage), FileMode.OpenOrCreate, FileAccess.Write))
                {
                    Original.CopyTo(Copia);
                    Al.RutaImagen = RelativeImage;
                    if (IMG != null) IMG.Source = CargaImage(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, RelativeImage));
                }
            }

        }

    }
}
