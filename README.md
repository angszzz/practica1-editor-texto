# Mi Compilador

Compilador de un subconjunto de C hecho en C# con Windows Forms (.NET Framework 4.7.2). Se construye por etapas: cada práctica de la materia Lenguajes y Autómatas I agrega una fase nueva sobre la anterior.

## Prácticas

Cada práctica vive en su propia rama. La práctica más reciente aparece primero.

| Práctica | Rama | Qué agrega |
|---|---|---|
| 4. Análisis sintáctico: `#include` | `practica4` | Verificación de la cabecera del programa y de la directiva de procesador `#include`. |
| 2. Análisis léxico | `practica2` | Lectura del archivo fuente carácter por carácter, reconocimiento de tokens y reporte de errores léxicos. |
| 1. Editor de texto | `main` | Editor tipo Bloc de notas con el menú Archivo. |

La práctica 3 fue un reporte independiente sobre traductores y no modifica este proyecto.

---

## Práctica 4: Análisis sintáctico de la directiva #include

Segunda fase del compilador. Si el análisis léxico termina sin errores, el analizador sintáctico lee el archivo `.back` token por token y verifica que la cabecera del programa, todo lo que va antes de `main`, tenga una estructura válida.

### Qué verifica

- **Directivas de procesador:** después de `#` debe venir `include`, `define` o `if`.
- **`#include`:** solo acepta `<libreria>` o `"archivo"` entre comillas.
- **Declaraciones globales:** una variable declarada antes de `main` debe terminar en `;`.
- **Función `main`:** el programa debe tenerla.

`#define` e `#if` se aceptan sin revisar su contenido. La estructura de `main` y de las demás funciones queda para las siguientes prácticas.

### Cómo funciona

Cada diagrama de sintaxis se programó como una función (análisis descendente recursivo):

| Función | Diagrama que implementa |
|---|---|
| `A_Sintactico` | Programa en C: cabecera, función `main` y definición de funciones |
| `Cabecera` | Directivas de procesador y declaraciones globales hasta encontrar `main` |
| `Directriz` | Directiva de procesador: `#` seguido de `include`, `define` o `if` |
| `Include` | `#include` con `<libreria>` o cadena |
| `Declaracion` | Declaración global terminada en `;` |
| `ErrorS` | Reporta la línea, el token encontrado y lo que se esperaba |

### Ejemplo

Para este archivo:

```c
#include <stdio.h
#includ<stdlib.h>
#include stdio.h
#include<"x">
int main
```

La caja de mensajes muestra:

```
Analizando...
Errores léxicos: 0

Analizando sintaxis...
Línea 1: error de sintaxis 'salto de línea', se esperaba >
Línea 2: error de sintaxis 'identificador', se esperaba directriz de procesador
Línea 3: error de sintaxis 'libreria', se esperaba inclusión válida
Línea 4: error de sintaxis 'cadena', se esperaba nombre de archivo librería
Errores sintácticos: 4
```

### Ajustes al analizador léxico

- `main` se escribe como token propio en el `.back`, para que el sintáctico sepa dónde termina la cabecera.
- Los saltos de línea dentro de comentarios `/* */` escriben un `LF`, para que el sintáctico reporte los errores en la línea correcta.

---

## Práctica 2: Análisis léxico

Primera fase del compilador. Lee un archivo fuente `.c`, reconoce cada elemento del código y genera un archivo auxiliar `.back` con la lista de tokens. Los caracteres que no pertenecen al lenguaje se reportan como errores léxicos con su número de línea.

### Interfaz

- Menú **Compilar → Analizar**, deshabilitado hasta que se abre, guarda o modifica un archivo.
- Segunda caja de texto en la parte inferior para los mensajes de compilación.
- El título de la ventana muestra la ruta del archivo abierto.

### Cómo funciona

1. **Analizar** guarda el archivo y lo lee carácter por carácter con `StreamReader.Read()`.
2. `Tipo_caracter` clasifica cada carácter por su código ASCII: letra, dígito, símbolo, comilla, diagonal, salto de línea o espacio.
3. Según el tipo, una función especializada lee el elemento completo y escribe su token en el `.back`.
4. Al terminar, la caja de mensajes muestra los errores encontrados y el total.

### Tokens

| Token en el `.back` | Qué reconoce | Ejemplo |
|---|---|---|
| La palabra misma | Palabra reservada de C | `int`, `return` |
| `identificador` | Nombre de variable | `x`, `contador` |
| `funcion` | Nombre seguido de paréntesis | `main(`, `printf(` |
| `libreria` | Nombre con extensión `.h` | `stdio.h` |
| `numero_entero` | Dígitos sin punto | `10` |
| `numero_real` | Dígitos con punto decimal | `3.14` |
| `cadena` | Texto entre comillas dobles | `"hola mundo"` |
| `caracter` | Carácter entre comillas simples | `'a'`, `'\n'` |
| `comentario` | Comentario de línea o de bloque | `// ...`, `/* ... */` |
| El símbolo mismo | Símbolo válido | `;`, `{`, `=` |
| `LF` | Salto de línea | |
| `error` | Carácter que no pertenece a C | `@` |

### Ejemplo

Para este archivo `prueba.c`:

```c
#include <stdio.h>
int main() {
	int x = 10;
	x = x @ 2;
	return 0;
}
```

La caja de mensajes muestra:

```
Analizando...
Error léxico '@', línea 4
Errores léxicos: 1
```

Y `prueba.back` contiene los tokens en orden, uno por línea:

```
#
include
<
libreria
>
LF
int
funcion
(
)
{
LF
...
Fin
```

### Cambios al menú Archivo

- **Abrir** y **Guardar como** filtran solo archivos `.c`.
- **Nuevo** pregunta si se desea guardar antes de limpiar el editor.
- La lógica de guardado se separó en las funciones `guardar()` y `guardarComo()` para reutilizarla en Nuevo y Analizar.

---

## Práctica 1: Editor de texto

Base del proyecto: un editor de texto tipo Bloc de notas.

### Funciones

Menú **Archivo**:

| Opción | Qué hace |
|---|---|
| Nuevo | Limpia el editor y olvida el archivo actual. |
| Abrir | Abre un archivo con `OpenFileDialog` y lo carga con `StreamReader`. |
| Guardar | Sobrescribe el archivo actual. Si es un documento nuevo, actúa como "Guardar como". |
| Guardar como | Elige una ruta con `SaveFileDialog` y escribe con `StreamWriter`. |
| Salir | Cierra la aplicación. |

---

## Cómo abrirlo

1. Clona el repositorio o descárgalo como ZIP.
2. Cámbiate a la rama de la práctica que quieras ver, por ejemplo `git checkout practica2`.
3. Abre `practica1.sln` en Visual Studio. Necesitas la carga de trabajo **Desarrollo de escritorio de .NET**.
4. Presiona **Iniciar** (F5).

## Estructura

```
practica1.sln
practica1/
├── Form1.cs            Lógica del menú Archivo y de los analizadores léxico y sintáctico
├── Form1.Designer.cs   Diseño: menús Archivo y Compilar, caja de código y caja de mensajes
├── Form1.resx
├── Program.cs          Punto de entrada
├── App.config
└── Properties/AssemblyInfo.cs
```
