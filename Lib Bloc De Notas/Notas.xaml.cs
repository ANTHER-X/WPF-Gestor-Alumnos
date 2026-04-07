/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Win32;

namespace Lib_Bloc_De_Notas
{
    public partial class Notas : Window
    {
        private string actualPath = "";

        public Notas()
        {
            InitializeComponent();
        }

        private void GuardaNuevoTXT()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Guardar Archivo";
            sfd.Filter = "Archivos txt|*.txt";

            if (sfd.ShowDialog() == true)
            {
                using (FileStream FS = new FileStream(sfd.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (StreamWriter SW = new StreamWriter(FS))
                    {
                        SW.Write(TXBMain.Text);
                    }
                }
            }
        }

        private void MIBTNSave_Click(object sender, RoutedEventArgs e)
        {
            if (actualPath == null) GuardaNuevoTXT();
            else
            {
                using (FileStream FS = new FileStream(actualPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (StreamWriter SW = new StreamWriter(FS))
                    {
                        SW.Write(TXBMain.Text);
                    }
                }
            }
        }

        private void MIBTNSaveAs_Click(object sender, RoutedEventArgs e) => GuardaNuevoTXT();

        private void MIBTNLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Selecciona Archivo TXT";
            ofd.Filter = "Archivox txt|*.txt";

            if (ofd.ShowDialog() == true)
            {
                using (FileStream FS = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader SR = new StreamReader(FS))
                    {
                        string aux = SR.ReadToEnd();
                        TXBMain.Text = aux;
                        actualPath = ofd.FileName;
                    }
                }
            }
        }

        private void TXBMain_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S) MIBTNSave_Click(sender, e);
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.L) MIBTNLoad_Click(sender, e);
        }

        private void CambiarFuente_Click(object sender, RoutedEventArgs e)
        {
            if(sender is MenuItem ITC)
            {
                TXBMain.FontFamily = new FontFamily(ITC.Header.ToString());
            }
        }
    }
}