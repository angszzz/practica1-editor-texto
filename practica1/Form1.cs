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

        readonly List<string> P_Reservadas = new List<string>
        {
            "auto", "break", "case", "char", "const", "continue", "default", "do",
            "double", "else", "enum", "extern", "float", "for", "goto", "if",
            "int", "long", "register", "return", "short", "signed", "sizeof", "static",
            "struct", "switch", "typedef", "union", "unsigned", "void", "volatile", "while",
            "include", "define"
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
    }
}
