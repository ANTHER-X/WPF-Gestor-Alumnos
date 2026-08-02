/* 
 * Project: SchoolManager
 * Author: ANTHER
 * License: MIT
 * GitHub: https://github.com/ANTHER-X/SchoolManager
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gestor_De_Alumnos.Window_See_Student
{

    public partial class Creditos
    {
        public string Resource { get; init; } = string.Empty;
        public string Author { get; init; } = string.Empty;
        public string URL { get; init; } = string.Empty;
    }

    public partial class WndCreditos : Window
    {

        public ObservableCollection<Creditos> CreditosList { get;} =
        [
            new()
            {
                Resource = "alumno.png",
                Author = "RIkas Dzihab",
                URL = "https://www.flaticon.es/iconos-gratis/estudiante"
            },
            new()
            {
                Resource = "Orquidea.ico",
                Author = "Magnific",
                URL = "https://www.flaticon.es/iconos-gratis/orquidea"
            },
            new()
            {
                Resource = "rosa.png",
                Author = "Chanut-is-Industries",
                URL = "https://www.flaticon.es/iconos-gratis/rosa"
            }
        ];

        public WndCreditos()
        {
            InitializeComponent();
            Loaded += Carga;
        }
        private void Exit_Click(object sender, RoutedEventArgs e) => Close();

        private void Carga(object sender, EventArgs e)
        {
            LBVersion.Content = Assembly.GetExecutingAssembly().GetName().Version;
        }


        // Clicks de eventos
        private void OpenGitHub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/ANTHER-X",
                UseShellExecute = true
            });
        }
        private void Instagram_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://www.instagram.com/fernandocisneroslemus",
                UseShellExecute = true
            });
        }

        private void Copy_Email_Click(object sender, RoutedEventArgs e) => Clipboard.SetText("fernandocisneroslemus@gmail.com");
        private void Send_Email_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "mailto:fernandocisneroslemus@gmail.com?subject=GestorAlumnos&body=Hi",
                    UseShellExecute = true
                });
            }
            // Al parecer da error de Win32 si no hay cliente de correo configurado
            catch (Win32Exception)
            {
                Clipboard.SetText("fernandocisneroslemus@gmail.com");
                MessageBox.Show( "No hay un cliente de correo configurado.\n\n" +
                                 "La dirección se copió al portapapeles.",
                                 "Correo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OpenCreditsURL_Click(object sender, RoutedEventArgs e)
        {
            string?url = ((Button)sender).Tag.ToString();
            if (url == null) return;
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }
}
