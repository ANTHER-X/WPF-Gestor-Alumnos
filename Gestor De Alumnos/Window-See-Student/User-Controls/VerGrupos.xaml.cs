/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using System.Windows;
using System.Windows.Controls;

namespace Gestor_De_Alumnos.Window_See_Student.User_Controls
{

    /*Este es el selector de las plantillas para el ListView, hereda de una clase abstracta de la que
    tenemos que reescribir un metodo que automaticamente usa el ListView para cargar sus plantillas de Items*/
    public class SelectorTemplateListView : DataTemplateSelector
    {

        public DataTemplate? AlumnoTemplate { get; set; }
        public DataTemplate? TituloTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object Item, DependencyObject Container)
        {
            if (Item is Alumno al) return AlumnoTemplate;
            else return TituloTemplate;
        }

    }

    public partial class VerGrupos : UserControl
    {
        private bool AllGroupsAnGroup { get; set; }
        static public bool ModStatsAlumno { get; private set; }

        public List<Alumno> Alumnos { get; private set; }
        public List<short> Grupos { get; private set; }

        public short auxGrupo { get; private set; }

        /*La lista Original de los alumnos y los grupos que hay*/
        public VerGrupos(bool ChangeStats = false)
        {
            InitializeComponent();
            Grupos = new List<short>();
            Alumnos = new List<Alumno>();
            /*Hacemos una copia en lo que carga lo demas*/
            ModStatsAlumno = ChangeStats;
        }

        private void OrganizarTodos(List<Alumno> Lista, List<short> Grupos)
        {
            this.Grupos = Grupos;
            AllGroupsAnGroup = true;
            Alumnos = Lista;
            for (int i = 0; i < Grupos.Count; i++)
            {
                LSTVMain.Items.Add($"Grupo: {Grupos[i]}");

                //filtramos
                List<Alumno> Seleccionados = Lista.Where(e => e.grupo == Grupos[i]).ToList();

                if (Seleccionados.Count > 0)
                {
                    foreach (Alumno al in Seleccionados)
                    {
                        LSTVMain.Items.Add(al);
                        LSTVMain.Items.Add("\n");
                    }
                }
                else LSTVMain.Items.Add("Sin Alumnos :(");

                //eliminamos los que ya estan dentro
                Lista.RemoveAll(e => e.grupo == Grupos[i]);

                LSTVMain.Items.Add("\n\n");
            }
        }

        public void OrganizaDatos(List<Alumno> Lista, List<short> Grupos) => OrganizarTodos(Lista, Grupos);

        public void OrganizaSoloHoras(List<Alumno> Lista, List<short> Grupos) => OrganizarTodos(Lista, Grupos);

        public void Organiza(List<Alumno> Lista, short Grupo)
        {
            AllGroupsAnGroup = false;
            Alumnos = Lista;
            auxGrupo = Grupo;

            LSTVMain.Items.Add($"Grupo: {Grupo}");

            if (Lista.Count == 0)
            {
                LSTVMain.Items.Add("Sin Alumnos :(");
                return;
            }

            foreach (Alumno al in Lista)
            {
                LSTVMain.Items.Add(al);
                LSTVMain.Items.Add("\n");
            }
        }

        public void EliminaCambiado(Alumno al)
        {
            Alumnos.Remove(al);
        }

        public void ReformaDatos(List<Alumno> Lista, List<short> Grupos)
        {
            LSTVMain.Items.Clear();
            if (AllGroupsAnGroup) OrganizarTodos(Lista, Grupos);
            else Organiza(Lista, auxGrupo);
        }

    }
}
