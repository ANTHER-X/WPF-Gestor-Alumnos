/* 
 * Project: WPF-Gestor-Alumnos
 * Author: ANTHER
 * Licence: MIT
 * GitHub: https://github.com/ANTHER-X/WPF-Gestor-Alumnos
 */
using ClosedXML.Excel;
using System.IO;
using System.Windows;

namespace Gestor_De_Alumnos.Guardar_Datos
{
    public static class ArchivosExcel
    {
        /*
         * Desing de Colores y estilo, en este caso seran 2 para hacer un desing de rayas
         */
        static private void StyleExcel(IXLRange rango)
        {
            //Agregamos el estilo de letra y color de letra blanco
            rango.Style.Font.Bold = true;
            rango.Style.Font.FontColor = XLColor.White;

            //Damos un borde un poquito mas ancho
            rango.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
            rango.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            //Centramos el texto
            rango.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rango.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }

        static private void StyleExcelPurple(IXLWorksheet hoja, int row)
        {
            //Tomamos las celdas que se usaran para cambiar el estilo
            IXLRange rango = hoja.Range($"A{row}:M{row}");

            //Agragamos el estilo
            StyleExcel(rango);

            //Agregamos el color de fondo
            rango.Style.Fill.BackgroundColor = XLColor.FromHtml("#C131F5"); // Morado claro

            //Damos un color mas oscuro que el fondo
            rango.Style.Border.OutsideBorderColor = XLColor.FromHtml("#0F4C8A"); // Azul fuertito
        }

        //Lo dejamos igual pero con colores un poco distintos
        static private void StyleExcelWarm(IXLWorksheet hoja, int row)
        {
            IXLRange rango = hoja.Range($"A{row}:M{row}");

            StyleExcel(rango);

            rango.Style.Fill.BackgroundColor = XLColor.FromHtml("#F4511E"); // Color entre rojo y naranja
            rango.Style.Border.OutsideBorderColor = XLColor.FromHtml("#8B1E00"); // El mismo pero mas obscuro
        }



        //Encabezado de los colores
        static private void DatosAlumnos(IXLWorksheet Hoja)
        {
            Hoja.Cell("A1").Value = "Nombre";
            Hoja.Cell("B1").Value = "Apellido";
            Hoja.Cell("C1").Value = "Fecha de Nacimiento";
            Hoja.Cell("D1").Value = "Edad";
            Hoja.Cell("E1").Value = "Telefono";
            Hoja.Cell("F1").Value = "Grupo";
            Hoja.Cell("G1").Value = "Matricula";
            Hoja.Cell("H1").Value = "Turno";
            Hoja.Cell("I1").Value = "Capacitacion";
            Hoja.Cell("J1").Value = "Bachillerato";
            Hoja.Cell("K1") .Value = "Club";
            Hoja.Cell("L1").Value = "Profesor que Libera Servicio";
            Hoja.Cell("M1").Value = "Horas de servicio Social";

            StyleExcelPurple(Hoja, 1);

        }

        static public void ExcelAlumno(string ruta, Alumno al)
        {
            //creamos el paquete del exe
            using (XLWorkbook book = new XLWorkbook())
            {
                //creamos una hoja
                book.AddWorksheet(al.name);
                //La tomamos
                IXLWorksheet Hoja;
                Hoja = book.Worksheet(al.name);

                //metemos el nombre de los datos que vamos a meter
                DatosAlumnos(Hoja);

                //Metemos los datos
                Hoja.Cell("A2").Value = al.name;
                Hoja.Cell("B2").Value = al.apellidos;
                Hoja.Cell("C2").Value = al.fechaNacimiento;
                Hoja.Cell("D2").Value = al.edad;
                Hoja.Cell("E2").Value = al.Telefono;
                Hoja.Cell("F2").Value = al.grupo;
                Hoja.Cell("G2").Value = al.Matricula;
                Hoja.Cell("H2").Value = al.turno;
                Hoja.Cell("I2").Value = al.capacitacion;
                Hoja.Cell("J2").Value = al.bachillerato;
                Hoja.Cell("K2").Value = al.club;
                Hoja.Cell("L2").Value = al.profServSoc;
                Hoja.Cell("M2").Value = al.horasServicioSocial;

                StyleExcelWarm(Hoja, 2);

                //Guardamos y verificamos
                book.SaveAs(ruta);
                if (File.Exists(ruta)) MessageBox.Show($"Archivo excel con {al.name}.xlsx creado");
                else MessageBox.Show("Ocurrio un error");
            }

        }

        static public void ExcelGrupo(string ruta, List<Alumno> GrupoAlumnos)
        {
            if (GrupoAlumnos.Count == 0) return;

            using(XLWorkbook book = new XLWorkbook())
            {
                book.AddWorksheet(GrupoAlumnos[0].grupo.ToString());
                IXLWorksheet Hoja = book.Worksheet(GrupoAlumnos[0].grupo.ToString());

                DatosAlumnos(Hoja);

                GrupoAlumnosExcel(Hoja, GrupoAlumnos);

                book.SaveAs(ruta);
                if (File.Exists(ruta)) MessageBox.Show($"Archivo excel del grupo -> {GrupoAlumnos[0].grupo} creado");
                else MessageBox.Show("Ocurrio un error");
            }

        }

        static private void GrupoAlumnosExcel(IXLWorksheet Hoja, List<Alumno> GrupoAlumnos)
        {
            if (GrupoAlumnos.Count == 0) return;

            //empieza en 2 de cada columna
            for (int i = 2; i < GrupoAlumnos.Count + 2; i++)
            {
                Hoja.Cell($"A{i}").Value = GrupoAlumnos[i - 2].name;
                Hoja.Cell($"B{i}").Value = GrupoAlumnos[i - 2].apellidos;
                Hoja.Cell($"C{i}").Value = GrupoAlumnos[i - 2].fechaNacimiento;
                Hoja.Cell($"D{i}").Value = GrupoAlumnos[i - 2].edad;
                Hoja.Cell($"E{i}").Value = GrupoAlumnos[i - 2].Telefono;
                Hoja.Cell($"F{i}").Value = GrupoAlumnos[i - 2].grupo;
                Hoja.Cell($"G{i}").Value = GrupoAlumnos[i - 2].Matricula;
                Hoja.Cell($"H{i}").Value = GrupoAlumnos[i - 2].turno;
                Hoja.Cell($"I{i}").Value = GrupoAlumnos[i - 2].capacitacion;
                Hoja.Cell($"J{i}").Value = GrupoAlumnos[i - 2].bachillerato;
                Hoja.Cell($"K{i}").Value = GrupoAlumnos[i - 2].club;
                Hoja.Cell($"L{i}").Value = GrupoAlumnos[i - 2].profServSoc;
                Hoja.Cell($"M{i}").Value = GrupoAlumnos[i - 2].horasServicioSocial;

                //Damos el desing
                if(i % 2 == 0) StyleExcelWarm(Hoja, i);
                else StyleExcelPurple(Hoja, i);
            }

        }

        static public void AllAlumnos(string ruta, List<List<Alumno>> Alumnos)
        {
            if (Alumnos.Count == 0) return;

            using(XLWorkbook book = new XLWorkbook())
            {
                for(int i=0; i<Alumnos.Count; i++)
                {
                    if (Alumnos[i].Count > 0)
                    {
                        book.AddWorksheet(Alumnos[i][0].grupo.ToString());
                        IXLWorksheet Hoja = book.Worksheet(Alumnos[i][0].grupo.ToString());
                        DatosAlumnos(Hoja);
                        GrupoAlumnosExcel(Hoja, Alumnos[i]);
                    }
                }

                book.SaveAs(ruta);
                if (File.Exists(ruta)) MessageBox.Show($"Archivo Excel Con TODOS LOS GRUPOS creado");
                else MessageBox.Show("Ocurrio un error");
            }

        }
    }
}
