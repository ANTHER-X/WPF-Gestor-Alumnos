/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using Gestor_De_Alumnos.Guardar_Datos;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;
using System.IO;

namespace Gestor_De_Alumnos.Window_See_Student.User_Controls
{

    public partial class VerAlumno : UserControl
    {
        MostrarDatos WNDMain;
        Alumno al;

        private void MuestraAlumno()
        {
            LBName.Content = al.name;
            LBApellido.Content = al.apellidos;
            LBFechaNacim.Content = al.fechaNacimiento;
            LBEdad.Content = al.edad;
            LBTelefono.Content = al.Telefono;

            LBGrupo.Content = al.grupo;
            LBMatricula.Content = al.Matricula;
            LBTurno.Content = al.turno;
            LBCapacitacion.Content = al.capacitacion;
            LBBach.Content = al.bachillerato;
            LBClub.Content = al.club;

            LBHServ.Content = al.horasServicioSocial;
            LBProf.Content = al.profServSoc;

            IMGAlumno.Source = StudentData.CargaImage(al.RutaImagen);
        }

        public VerAlumno(MostrarDatos WNDMain, Alumno Al)
        {
            InitializeComponent();
            this.WNDMain = WNDMain;
            this.al = Al;

            MuestraAlumno();
            Salir.Focus();
        }

        private void Salir_Click(object sender, RoutedEventArgs e)
        {
            WNDMain.Close();
        }

        private void BTNExportaExcel_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                MessageBoxResult MBR = MessageBox.Show("Quiere eleguir la ruta?", $"Exportar a {al.name + " " + al.apellidos + " " + al.grupo}", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (MBR == MessageBoxResult.Cancel) return;
                else if (MBR == MessageBoxResult.Yes)
                {
                    OpenFolderDialog ofd = new OpenFolderDialog();
                    ofd.Title = "Ruta Para Archivo Excel";

                    if (ofd.ShowDialog() == true)
                    {
                        ArchivosExcel.ExcelAlumno(Path.Combine(ofd.FolderName, $"{al.name}.xlsx"), al);
                    }
                }
                else if (MBR == MessageBoxResult.No) ArchivosExcel.ExcelAlumno(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos", $"{al.name}.xlsx"), al);
            });
        }
    }
}
