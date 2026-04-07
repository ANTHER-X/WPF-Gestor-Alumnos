/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Gestor_De_Alumnos.Guardar_Datos;
using Gestor_De_Alumnos.Clases_utiles;

namespace Gestor_De_Alumnos.Window_See_Student.User_Controls
{

    public partial class AddHorasServicio : UserControl
    {
        //Datos
        MostrarDatos MD;
        Alumno Al;

        //Mostramos los datos
        public AddHorasServicio(MostrarDatos MD, Alumno al)
        {
            InitializeComponent();
            this.MD = MD;
            this.Al = al;
            LBStudentData.Content = $"Grupo: {Al.grupo} --  Nombre: {Al.name + " " + Al.apellidos}";
            LBActualHoras.Content = $"Anidar Horas, Horas Actuales: {Al.horasServicioSocial}";
        }

        //Verifica que no se salga de las horas de Servicio
        private int TomarHoras()
        {
            int Horas = int.Parse(TXBHoras.Text);

            if (Horas > 120) Horas = 120;
            if (Horas < 0) Horas = 0;

            return Horas;
        }

        //Actualiza los datos del alumno y la DB y actualiza el control de horas
        private void BTNAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TXBHoras.Text))
            {
                Al.AddHoras(TomarHoras());
                LBActualHoras.Content = $"Anidar Horas, Horas Actuales: {Al.horasServicioSocial}";
                SQLiteDataStudent.AddRemoveUpdateAlumno(Al, false, true);
                MessageBox.Show("Horas Aniadidas");
            }
            else MessageBox.Show("Faltan Datos", "Error de datos", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //Hace lo mismo pero para eliminar horas
        private void BTNDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TXBHoras.Text))
            {
                Al.DeleteHoras(TomarHoras());
                SQLiteDataStudent.AddRemoveUpdateAlumno(Al, false, true);
                MessageBox.Show("Horas Eliminadas");
                LBActualHoras.Content = $"Anidar Horas, Horas Actuales: {Al.horasServicioSocial}";
            }
            else MessageBox.Show("Faltan Datos", "Error de datos", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BTNSalir_Click(object sender, RoutedEventArgs e) => MD.Close();

        private void TXBHoras_PreviewTextInput(object sender, TextCompositionEventArgs e) => InputClassTXB.DetectaPreviewInpunNumber(TXBHoras, e, 3);
    }
}
