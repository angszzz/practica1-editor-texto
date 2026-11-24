# Práctica 1: Editor de texto

Editor de texto básico tipo Bloc de notas, hecho en C# con Windows Forms (.NET Framework 4.7.2).

## Funciones

Menú **Archivo**:

| Opción | Qué hace |
|---|---|
| Nuevo | Limpia el editor y olvida el archivo actual. |
| Abrir | Abre un archivo con `OpenFileDialog` y lo carga con `StreamReader`. |
| Guardar | Sobrescribe el archivo actual. Si es un documento nuevo, actúa como "Guardar como". |
| Guardar como | Elige una ruta con `SaveFileDialog` y escribe con `StreamWriter`. |
| Salir | Cierra la aplicación. |

## Cómo abrirlo

1. Clona el repositorio o descárgalo como ZIP.
2. Abre `practica1.sln` en Visual Studio. Necesitas la carga de trabajo **Desarrollo de escritorio de .NET**.
3. Presiona **Iniciar** (F5).

## Estructura

```
practica1.sln
practica1/
├── Form1.cs            Lógica de cada opción del menú
├── Form1.Designer.cs   Diseño: MenuStrip + RichTextBox (Dock = Fill)
├── Form1.resx
├── Program.cs          Punto de entrada
├── App.config
└── Properties/AssemblyInfo.cs
```
