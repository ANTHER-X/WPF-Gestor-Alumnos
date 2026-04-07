/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using Gestor_De_Alumnos.Guardar_Datos;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Gestor_De_Alumnos.Window_See_Student
{
    public partial class WNDSeeStudent : Window
    {
        private MainWindow WNDMain;
        private List<Alumno> AuxCBAlumnos;

        public WNDSeeStudent(MainWindow WNDPadre)
        {
            InitializeComponent();
            this.WNDMain = WNDPadre;
            AuxCBAlumnos = new List<Alumno>();

            ActualizaCombobox();

        }

        public void ActualizaCombobox()
        {
            CBGrupoAlumno1.ItemsSource = null;
            CBSeeGroup.ItemsSource = null;
            CBNombreAlumno.ItemsSource = null;

            CBSeeGroup.Items.Clear();
            CBGrupoAlumno1.Items.Clear();
            CBNombreAlumno.Items.Clear();
            Task.Run(() =>
            {
                List<short> auxGroup = SQLiteDataStudent.ExtraeGrupos();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CBSeeGroup.ItemsSource = CBGrupoAlumno1.ItemsSource = auxGroup;
                    if (CBSeeGroup.Items.Count != 0)
                    {
                        CBGrupoAlumno1.SelectedIndex = 0;
                        AuxCBAlumnos = SQLiteDataStudent.ExtraeAlumnosPorGrupo((short)CBGrupoAlumno1.SelectedItem);
                        CBNombreAlumno.ItemsSource = SQLiteDataStudent.ExtraeNombresPorGrupo((short)CBGrupoAlumno1.SelectedItem);
                        if (CBNombreAlumno.Items.Count > 0) CBNombreAlumno.SelectedIndex = 0;
                        CBSeeGroup.SelectedIndex = CBGrupoAlumno1.SelectedIndex = 0;
                    }
                });
            });

        }
        
        private void ActualizaCBNombreAlumno()
        {
            if (CBGrupoAlumno1.SelectedIndex != -1)
            {
                CBNombreAlumno.ItemsSource = SQLiteDataStudent.ExtraeNombresPorGrupo((short)CBGrupoAlumno1.SelectedItem);

                if (CBNombreAlumno.Items.Count > 0) CBNombreAlumno.SelectedIndex = 0;

                AuxCBAlumnos = SQLiteDataStudent.ExtraeAlumnosPorGrupo((short)CBGrupoAlumno1.SelectedItem);
            }
        }

        private void CBGrupoAlumno1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActualizaCBNombreAlumno();
        }

        private void SelectAnStudent(byte see)
        {
            if (CBGrupoAlumno1.SelectedIndex != -1 && CBNombreAlumno.SelectedIndex != -1)
            {
                foreach (Alumno al in AuxCBAlumnos)
                {
                    if ((al.name + " " + al.apellidos + " " + al.Matricula) == (string)CBNombreAlumno.SelectedItem)
                    {
                        MostrarDatos MD = new(this);

                        if (see == 0) MD.VerAlumno(al);
                        else if (see == 1) MD.CambiarStats(al);
                        else if (see == 2) MD.AnidarHorasAlumno(al);

                        MD.ShowDialog();
                        return;
                    }
                }

            }
        }

        private void VerGrupo(bool OnlyHoras, bool mod = false)
        {
            if (CBSeeGroup.SelectedIndex != -1)
            {
                MostrarDatos MD = new(this);
                MD.UnGrupo(SQLiteDataStudent.ExtraeAlumnosPorGrupo((short)CBSeeGroup.SelectedItem, OnlyHoras), (short)CBSeeGroup.SelectedItem, mod);
                MD.ShowDialog();
            }
        }

        private void VerTodosGrupos(bool OnlyHours, bool Mod = false)
        {
            MostrarDatos MD = new(this);
            if (!OnlyHours) MD.TodosGrupos(SQLiteDataStudent.AllAlumnos(false), SQLiteDataStudent.ExtraeGrupos(),Mod);
            else MD.TodosGrupos(SQLiteDataStudent.AllAlumnos(true), SQLiteDataStudent.ExtraeGrupos(),Mod);
            MD.ShowDialog();
        }

        private void BTNAddHorasStudent_Click(object sender, RoutedEventArgs e) => SelectAnStudent(2);

        private void BTNChangeStats_Click(object sender, RoutedEventArgs e) => SelectAnStudent(1);

        private void BTNSeeAStudent_Click(object sender, RoutedEventArgs e) => SelectAnStudent(0);

        private void BTNVerGrupo_Click(object sender, RoutedEventArgs e) => VerGrupo(false);

        private void BTNVerGrupoSoloHoras_Click(object sender, RoutedEventArgs e) => VerGrupo(true);
        
        private void BTNSeeAllGroup_Click(object sender, RoutedEventArgs e) => VerTodosGrupos(false);
        
        private void BTNSeeAllGroupOnlyHoras_Click(object sender, RoutedEventArgs e) => VerTodosGrupos(true);

        private void BTNModificarGrupo_Click(object sender, RoutedEventArgs e) => VerGrupo(false, true);

        private void BTNModificadorTotal_Click(object sender, RoutedEventArgs e) => VerTodosGrupos(false, true);


        private void BTNEliminarStudent_Click(object sender, RoutedEventArgs e)
        {
            if (CBGrupoAlumno1.SelectedIndex != -1 && CBNombreAlumno.SelectedIndex != -1)
            {
                MessageBoxResult MensajeMuerteAlumno = MessageBox.Show($"Reconoce las Consecuencias de eliminar\nUn Estudiante?.\nQuiere Eliminarlo?", "Matando Estudiante",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if(MensajeMuerteAlumno == MessageBoxResult.Yes)
                {
                    for (int i = 0; i < AuxCBAlumnos.Count(); i++)
                    {
                        if ((AuxCBAlumnos[i].name + " " + AuxCBAlumnos[i].apellidos + " " + AuxCBAlumnos[i].Matricula) == CBNombreAlumno.SelectedItem.ToString())
                        {
                            WNDMain.EliminaAlumno(AuxCBAlumnos[i]);
                            ActualizaCombobox();
                        }
                    }

                }
                
            }
        }

        //Change
        private void CreaExcelGrupoAndAllAlumnos(bool OnlyGroup, short grupo = 0)
        {
            string path;
            MessageBoxResult MBR = MessageBox.Show("Solo horas de Servicio?", $"Creando Excel de los Alumnos", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (MBR == MessageBoxResult.Cancel) return;

            MessageBoxResult MBRRuta = MessageBox.Show("Quieres eleguir Ruta?", $"Creando Excel de los Alumnos", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (MBRRuta == MessageBoxResult.Cancel) return;

            if (MBRRuta == MessageBoxResult.Yes)
            {
                OpenFolderDialog ofld = new();
                ofld.Title = "Abriendo carpeta";
                if (ofld.ShowDialog() == true)
                {
                    path = ofld.FolderName;
                }
                else path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos");
            }
            else path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Datos");

            if (MBR == MessageBoxResult.Yes)
            {
                if (OnlyGroup == true) ArchivosExcel.ExcelGrupo(Path.Combine(path, $"{grupo}.xlsx"), SQLiteDataStudent.ExtraeAlumnosPorGrupo(grupo, true));
                else ArchivosExcel.AllAlumnos(Path.Combine(path, $"Alumnos.xlsx"), SQLiteDataStudent.ExtraeAllAlumnosExcel(true));
            }
            else
            {
                if (OnlyGroup == true) ArchivosExcel.ExcelGrupo(Path.Combine(path, $"{grupo}.xlsx"), SQLiteDataStudent.ExtraeAlumnosPorGrupo(grupo));
                else ArchivosExcel.AllAlumnos(Path.Combine(path, $"Alumnos.xlsx"), SQLiteDataStudent.ExtraeAllAlumnosExcel(false));
            }
        }

        private void BTNCreaExcelAllAlumnos_Click(object sender, RoutedEventArgs e)
        {
            CreaExcelGrupoAndAllAlumnos(false);
        }

        private void BTNCreaExcelGrupo_Click(object sender, RoutedEventArgs e)
        {
            if (CBSeeGroup.SelectedIndex != -1)
            {
                CreaExcelGrupoAndAllAlumnos(true, (short)CBSeeGroup.SelectedItem);
            }
        }

    }
}
