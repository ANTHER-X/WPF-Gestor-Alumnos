# WPF-Gestor-Alumnos

Aplicación de escritorio en C# con WPF para la gestión de alumnos, pensada para un entorno escolar. Creada como parte de mi aprendizaje, pero completamente funcional y lista para usarse 🎒

Licencia **MIT**, así que puedes usarla, modificarla o tomar lo que necesites libremente.

---

## 📌 ¿Qué hace esta app?

Permite administrar información básica de estudiantes desde una interfaz gráfica sencilla. Guarda los datos en una base de datos SQLite con cifrado, permite exportar a Excel y trae algunas herramientas extra que le dan el toque, como una CLI interna y un bloc de notas integrado.

---

## 🚀 Características

- Gestión completa de alumnos (crear, leer, actualizar y eliminar)
- Base de datos SQLite con cifrado via **SQLCipher**
- Exportación a archivos **Excel** con **ClosedXML**
- Interfaz con ventanas y **UserControls**
- **CLI interna** con comandos útiles
- **Bloc de notas** integrado como submódulo
- Soporte de una **fotografía** por alumno

---

## 🛠️ Tecnologías usadas

- C# / WPF
- EntityFrameworkCore
- SQLite + SQLCipher
- ClosedXML

---

## ▶️ Uso general

Los datos que maneja la app por alumno son:

- Nombre y Apellidos
- Matrícula (única, no se repite)
- Capacitación
- Fotografía
- Grupo
- Matricula
- Capacitación
- Y más campos sobre datos escolares

Además, desde la sección de **"Codes"** en la pantalla principal puedes escribir algunos comandos rápidos:

| Comando    | Acción                                       |
| ---------- | -------------------------------------------- |
| `Notas`    | Abre el bloc de notas en ventana aparte      |
| `Créditos` | Muestra los créditos                         |
| `Salir`    | Guarda todo y cierra la aplicación           |
| `Terminal` | Abre la CLI interna de la app                |

---

## 💻 Terminal (CLI interna)

Escribir `Terminal` en la sección de Codes te lleva a una interfaz de línea de comandos dentro de la misma app. Desde ahí puedes:

- Manejar una **base de datos secundaria** independiente
- Exportar datos a **Excel** o **archivos de texto**
- Consultar el **total de alumnos** en la DB principal, entre otras cosas

> ⚠️ Para salir de la terminal, usa la opción del menú interno. Cerrar la ventana directamente cierra toda la app.

---

## 🗂️ Estructura del proyecto

El proyecto está organizado así (en términos generales):

- **UserControls** → varias vistas dentro de una sola ventana base  
  Ruta: `/Gestor De Alumnos/Window-See-Student/User-Controls/`
- **MostrarDatos** → ventana contenedora que se adapta al UserControl activo
- **ResourceDictionary** → estilos organizados por tipo de control  
  Ejemplo: `ButtonsThemas → Style: BotonRetro`
- **SQLiteDataStudent** → clase con métodos estáticos para manejar la DB
- **ArchivosExcel** → clase estática para la exportación a Excel

---

## 🗃️ Almacenamiento de datos

Al ejecutar la app por primera vez, se crean automáticamente dos carpetas:

- **Datos** → aquí vive la DB principal, los archivos Excel exportados y la DB secundaria (si no elegiste otra ruta)
- **ImagenesEstudiantes** → fotos de cada alumno + imagen default si no se asigna ninguna

> 💡 Las imágenes se guardan con **ruta relativa**, no absoluta. La app une la ruta base del ejecutable con esa ruta internamente, así que no hay que hacer nada manual. Solo lo menciono por si revisas el código.

---

## 📊 Base de datos

Se usa SQLite gestionado con EntityFrameworkCore. La clase `SQLiteDataStudent` se encarga de todo:

- No permite matrículas duplicadas
- Permite extraer grupos: `ExtraeGrupos`
- Extrae alumnos por grupo: `ExtraeAlumnosPorGrupo`
- Agrega, elimina y actualiza: `AddRemoveUpdateAlumno`
- Y más operaciones útiles

---

## 📄 Exportación a Excel

La clase estática `ArchivosExcel` permite generar archivos Excel de tres formas:

- Por alumno individual → `ExcelAlumno`
- Por grupo → `ExcelGrupo`
- Todos los alumnos de todos los grupos → `AllAlumnos`

---

## 🗒️ Subproyecto: Lib Bloc de Notas

Está separado del resto del proyecto con la intención de que pueda crecer por su cuenta en el futuro, aunque eso no está confirmado. Por ahora hace lo básico:

- Abrir archivos
- Guardar
- Guardar como

Trae 10 fuentes del sistema de Windows disponibles. Sin tipografías externas ni personalizables.

---

## 👤 Autor

**ANTHER-X**  
GitHub: [github.com/ANTHER-X](https://github.com/ANTHER-X)  
Email: fernandocisneroslemus@gmail.com

---

## 📄 Licencia

Este proyecto está bajo la licencia **MIT**. Consulta el archivo `LICENSE` para más detalles.