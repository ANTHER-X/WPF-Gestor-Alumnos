/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using Gestor_De_Alumnos.Window_See_Student.User_Controls;
using System.Windows;

namespace Gestor_De_Alumnos.Window_See_Student
{
    public partial class MostrarDatos : Window
    {
        //Para guardar la ventana padre y actualizar los datos de los combobox
        private WNDSeeStudent? WNDMain;
        private bool ActualizaStatsCombobox = false;

        public MostrarDatos(WNDSeeStudent? WNDMain)
        {
            InitializeComponent();

            this.WNDMain = WNDMain;
        }

        //Mostramos el UserControl para ver un alumno
        public void VerAlumno(Alumno Al)
        {
            ContCtrlMain.Content = new VerAlumno(this, Al);
            this.Height = 413;
            this.Title = $"Viendo Alumno: {Al.name + " " + Al.apellidos} -- {Al.grupo}";
        }

        //Abrimmos el UserControl para anidar horas de servicio social
        public void AnidarHorasAlumno(Alumno Al)
        {
            ContCtrlMain.Content = new AddHorasServicio(this, Al);
            this.Height = 256;
            this.Width = 324;
            this.Title = "Anidar Horas";
        }

        //Abrimos el UserControl para cambiar las estadisticas del alumno
        public void CambiarStats(Alumno Al)
        {
            ActualizaStatsCombobox = true;
            ContCtrlMain.Content = new CambiarStats(this,Al);
            this.Width = 610;
            this.Height = 482;
            this.Title = "Cambiar Estadisticas";
        }

        //Abrimos el UserControl para ver un grupo de alumnos, si mod es true, se mostraran los botones para modificar las estadisticas de cada alumno
        public void UnGrupo(List<Alumno> Lista, short grupo, bool mod = false)
        {
            VerGrupos VG = new VerGrupos(mod);
            VG.Organiza(Lista, grupo);
            ContCtrlMain.Content = VG;
            this.Height = 450;
            this.Width = 710;
            if (mod)
            {
                ActualizaStatsCombobox = true;
                this.Height += 30;
                this.Width += 120;
            }
            if (Lista.Count > 0) this.Title = $"Viendo Grupo: {grupo}";
            else this.Title = $"No Hay Alumnos en El grupo: {grupo}";
        }

        //Lo mismo que UnGrupo pero para todos los grupos
        public void TodosGrupos(List<Alumno> ListaOriginal, List<short> Grupos, bool mod = false)
        {
            VerGrupos VG = new VerGrupos(mod);
            VG.OrganizaDatos(ListaOriginal, Grupos);
            ContCtrlMain.Content = VG;
            this.Height = 450;
            this.Width = 710;
            if (mod)
            {
                ActualizaStatsCombobox = true;
                this.Height += 30;
                this.Width += 120;
            }
            this.Title = "Visualizando Todos Los Grupos";
        }

        //Al cerrar vemos si se tienen que actualizar los datos de los combobox
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ActualizaStatsCombobox == true && WNDMain != null)
            {
                WNDMain.ActualizaCombobox();
            }
        }
    }
}
