/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using System.Windows.Controls;
using System.Windows.Input;

namespace Gestor_De_Alumnos.Clases_utiles
{
    //Con esto vamos a detectar los datos insertados en los TextBox, esto para asi poder reutilizar codigo.
    public class InputClassTXB
    {
        //Deteccion maximo de caracteres numericos, y tambien detectamos solo caracteres numericos
        static public void DetectaPreviewInpunNumber(TextBox TXT, TextCompositionEventArgs e, int MaxLength)
        {
            string aux = TXT.Text.Insert(TXT.SelectionStart, e.Text);
            e.Handled = !int.TryParse(e.Text, out _) || aux.Length > MaxLength;
        }

        /*
         Este metodo nos permite insertar tanto un texto como caracter a un TextBox, esto para poder estilizar un poco la entrada del texto,
         esto para poder permitir la creacion de cierto tipo de strings especiales
        */
        static public void InsertarTextoTXB(TextBox TXB, Key previewInput, string textoAnidado, List<int> indices)
        {
            //Si el texto a insertar es vacio o nulo, salimos
            if (string.IsNullOrEmpty(textoAnidado)) return;

            //Recorremos las pociciones de los indices donde se insertara el texto
            foreach (int i in indices)
            {
                //Si el indice sobrepasa el texto del TextBox, salimos
                if (i > TXB.Text.Length) return;

                /*
                 * Si esta escribiendo en el TextBox y mientras no este borrando y si el size actual 
                 * del texto esta en la pocicion deceada de indexacion, meteremos el texto y re-formatearemos 
                 * el cursor para que no quede bugeado
                 */
                if (TXB.Text.Length == i && previewInput != Key.Back)
                {
                    int aux = TXB.CaretIndex + 1;
                    TXB.Text += textoAnidado;
                    TXB.CaretIndex = aux;
                    return;
                }
            }
        }
    }
}
