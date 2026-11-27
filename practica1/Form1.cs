using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace practica1
{
    public partial class Form1 : Form
    {
        string archivo = null;
        string archivoback, elemento;
        StreamWriter Escribir;
        StreamReader Leer;
        int i_caracter, N_error, Numero_linea;
                string token;

        readonly List<string> P_Res_Tipo = new List<string>
        {
            "char", "double", "float", "int", "long", "short", "signed", "unsigned", "void"
        };

        readonly List<string> P_Reservadas = new List<string>
        {
            "auto", "break", "case", "char", "const", "continue", "default", "do",
            "double", "else", "enum", "extern", "float", "for", "goto", "if",
            "int", "long", "register", "return", "short", "signed", "sizeof", "static",
            "struct", "switch", "typedef", "union", "unsigned", "void", "volatile", "while",
            "include", "define", "main"
        };

        public Form1()
        {
            InitializeComponent();
            compilarToolStripMenuItem.Enabled = false;
        }

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.Text != "" &&
                MessageBox.Show("¿Desea guardar el archivo?", "Confirmación", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                guardar();
            }
            richTextBox1.Clear();
            richTextBox2.Clear();
            archivo = null;
            Text = "Mi Compilador";
            compilarToolStripMenuItem.Enabled = false;
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog VentanaAbrir = new OpenFileDialog();
            VentanaAbrir.Filter = "Archivos C (*.c)|*.c";

            if (VentanaAbrir.ShowDialog() == DialogResult.OK)
            {
                archivo = VentanaAbrir.FileName;
                using (StreamReader Leer = new StreamReader(archivo))
                {
                    richTextBox1.Text = Leer.ReadToEnd();
                }
                Text = "Mi Compilador - " + archivo;
                compilarToolStripMenuItem.Enabled = true;
            }
        }

        private void guardar()
        {
            if (archivo != null)
            {
                using (StreamWriter Escribir = new StreamWriter(archivo))
                {
                    Escribir.Write(richTextBox1.Text);
                }
            }
            else
            {
                guardarComo();
            }
        }

        private void guardarComo()
        {
            SaveFileDialog VentanaGuardar = new SaveFileDialog();
            VentanaGuardar.Filter = "Archivos C (*.c)|*.c";
            VentanaGuardar.DefaultExt = "c";

            if (VentanaGuardar.ShowDialog() == DialogResult.OK)
            {
                archivo = VentanaGuardar.FileName;
                using (StreamWriter Escribir = new StreamWriter(archivo))
                {
                    Escribir.Write(richTextBox1.Text);
                }
                Text = "Mi Compilador - " + archivo;
                compilarToolStripMenuItem.Enabled = true;
            }
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guardar();
        }

        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            guardarComo();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            compilarToolStripMenuItem.Enabled = true;
        }

        private char Tipo_caracter(int caracter)
        {
            if (caracter >= 65 && caracter <= 90 || caracter >= 97 && caracter <= 122) return 'l';
            if (caracter >= 48 && caracter <= 57) return 'd';

            switch (caracter)
            {
                case 10: return 'n';
                case 34: return '"';
                case 39: return 'c';
                case 32:
                case 9:
                case 13: return 'e';
                case 47: return '/';
                default: return 's';
            }
        }

        private void Simbolo()
        {
            if (i_caracter == 33 ||
                i_caracter >= 35 && i_caracter <= 38 ||
                i_caracter >= 40 && i_caracter <= 45 ||
                i_caracter >= 58 && i_caracter <= 62 ||
                i_caracter == 91 ||
                i_caracter == 93 ||
                i_caracter == 94 ||
                i_caracter >= 123 && i_caracter <= 125)
            {
                elemento = (char)i_caracter + "\n";
            }
            else
            {
                Error(i_caracter);
            }
        }

        private void Cadena()
        {
            do
            {
                i_caracter = Leer.Read();
                if (i_caracter == 10) Numero_linea++;
            } while (i_caracter != 34 && i_caracter != -1);

            if (i_caracter == -1) Error(34);
        }

        private void Caracter()
        {
            i_caracter = Leer.Read();
            if (i_caracter == 92) i_caracter = Leer.Read();
            i_caracter = Leer.Read();
            if (i_caracter != 39) Error(39);
        }

        private void Archivo_Libreria()
        {
            i_caracter = Leer.Read();
            if (i_caracter == 'h')
            {
                elemento = "libreria\n";
                i_caracter = Leer.Read();
            }
            else
            {
                Error(i_caracter);
            }
        }

        private bool Palabra_Reservada()
        {
            return P_Reservadas.Contains(elemento);
        }

        private void Identificador()
        {
            do
            {
                elemento += (char)i_caracter;
                i_caracter = Leer.Read();
            } while (Tipo_caracter(i_caracter) == 'l' || Tipo_caracter(i_caracter) == 'd');

            if (Palabra_Reservada())
            {
                elemento += "\n";
            }
            else
            {
                switch (i_caracter)
                {
                    case '.': Archivo_Libreria(); break;
                    case '(': elemento = "funcion\n"; break;
                    default: elemento = "identificador\n"; break;
                }
            }
        }

        private bool Comentario()
        {
            i_caracter = Leer.Read();
            switch (i_caracter)
            {
                case 47:
                    do
                    {
                        i_caracter = Leer.Read();
                    } while (i_caracter != 10 && i_caracter != -1);
                    return true;

                case 42:
                    do
                    {
                        do
                        {
                            i_caracter = Leer.Read();
                            if (i_caracter == 10) { Numero_linea++; Escribir.Write("LF\n"); }
                        } while (i_caracter != 42 && i_caracter != -1);

                        if (i_caracter != -1) i_caracter = Leer.Read();
                        if (i_caracter == 10) { Numero_linea++; Escribir.Write("LF\n"); }
                    } while (i_caracter != 47 && i_caracter != -1);

                    if (i_caracter == -1) Error(42);
                    else i_caracter = Leer.Read();
                    return true;

                default:
                    return false;
            }
        }

        private void Numero()
        {
            do
            {
                i_caracter = Leer.Read();
            } while (Tipo_caracter(i_caracter) == 'd');

            if (i_caracter == '.')
            {
                do
                {
                    i_caracter = Leer.Read();
                } while (Tipo_caracter(i_caracter) == 'd');
                elemento = "numero_real\n";
            }
            else
            {
                elemento = "numero_entero\n";
            }
        }

        private void Error(int caracter)
        {
            richTextBox2.AppendText("Error léxico '" + (char)caracter + "', línea " + Numero_linea + "\n");
            N_error++;
            Escribir.Write("error\n");
        }

        private void analizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "Analizando...\n";
            guardar();
            if (archivo == null)
            {
                richTextBox2.AppendText("Guarda el archivo antes de analizarlo.\n");
                return;
            }

            N_error = 0;
            Numero_linea = 1;
            archivoback = Path.ChangeExtension(archivo, ".back");
            Escribir = new StreamWriter(archivoback);
            Leer = new StreamReader(archivo);

            i_caracter = Leer.Read();
            while (i_caracter != -1)
            {
                elemento = "";
                switch (Tipo_caracter(i_caracter))
                {
                    case 'l': Identificador(); Escribir.Write(elemento); break;
                    case 'd': Numero(); Escribir.Write(elemento); break;
                    case 's': Simbolo(); Escribir.Write(elemento); i_caracter = Leer.Read(); break;
                    case '"': Cadena(); Escribir.Write("cadena\n"); i_caracter = Leer.Read(); break;
                    case 'c': Caracter(); Escribir.Write("caracter\n"); i_caracter = Leer.Read(); break;
                    case '/':
                        if (Comentario()) Escribir.Write("comentario\n");
                        else Escribir.Write("/\n");
                        break;
                    case 'n': Escribir.Write("LF\n"); Numero_linea++; i_caracter = Leer.Read(); break;
                    case 'e': i_caracter = Leer.Read(); break;
                }
            }

            Escribir.Write("Fin");
            Escribir.Close();
            Leer.Close();

            richTextBox2.AppendText("Errores léxicos: " + N_error + "\n");
            if (N_error == 0) A_Sintactico();
        }

        private void A_Sintactico()
        {
            richTextBox2.AppendText("\nAnalizando sintaxis...\n");
            N_error = 0;
            Numero_linea = 1;
            Leer = new StreamReader(archivoback);

            if (Cabecera())
            {
                F_Main();
                if (token != "Fin") Definicion_funcion();
            }
            else
            {
                ErrorS(token, "función main()");
            }

            richTextBox2.AppendText("Errores sintácticos: " + N_error + "\n");
            Leer.Close();
        }

        private string Siguiente()
        {
            return Leer.ReadLine() ?? "Fin";
        }

        private bool Cabecera()
        {
            token = Siguiente();
            do
            {
                if (P_Res_Tipo.Contains(token)) token = "tipo";
                switch (token)
                {
                    case "#": Directriz(); break;
                    case "tipo":
                        token = Siguiente();
                        if (token == "main") return true;
                        Declaracion();
                        break;
                    case "": token = Siguiente(); break;
                    case "LF": Numero_linea++; token = Siguiente(); break;
                    default: token = Siguiente(); break;
                }
            } while (token != "Fin" && token != "main");
            return false;
        }

        private void Directriz()
        {
            token = Siguiente();
            switch (token)
            {
                case "include": Include(); break;
                case "define":
                case "if":
                    Saltar_linea();
                    break;
                default: ErrorS(token, "directriz de procesador"); break;
            }
        }

        private void Include()
        {
            token = Siguiente();
            switch (token)
            {
                case "<":
                    token = Siguiente();
                    if (token == "libreria")
                    {
                        token = Siguiente();
                        if (token == ">") token = Siguiente();
                        else ErrorS(token, ">");
                    }
                    else
                    {
                        ErrorS(token, "nombre de archivo librería");
                    }
                    break;
                case "cadena": token = Siguiente(); break;
                default: ErrorS(token, "inclusión válida"); break;
            }
        }

        private void Saltar_linea()
        {
            do
            {
                token = Siguiente();
            } while (token != "LF" && token != "Fin");
        }

        private void Declaracion()
        {
            while (token != ";" && token != "LF" && token != "Fin")
            {
                token = Siguiente();
            }
            if (token == ";") token = Siguiente();
            else ErrorS(token, ";");
        }

        private void F_Main()
        {
            token = Siguiente();
        }

        private void Definicion_funcion()
        {
        }

        private void ErrorS(string e, string s)
        {
            if (e == "LF") e = "salto de línea";
            richTextBox2.AppendText("Línea " + Numero_linea + ": error de sintaxis '" + e + "', se esperaba " + s + "\n");
            N_error++;
            if (token != "LF") token = "";
        }
    }
}
