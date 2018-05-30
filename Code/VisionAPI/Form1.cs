using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Google.Cloud.Vision.V1;

namespace VisionAPI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", ConfigurationManager.AppSettings["GOOGLE_APPLICATION_CREDENTIALS"]);
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            var client = ImageAnnotatorClient.Create();
            var image = Google.Cloud.Vision.V1.Image.FromFile("coca-cola.png");

            var response = client.DetectLabels(image);

            textBox1.Text = string.Empty;
            foreach (var annotation in response)
            {
                if (annotation.Description != null)
                {
                    textBox1.Text += annotation.Description + "\r\n";
                }
            }
        }

        private void btnSeleccionarImagen_Click(object sender, EventArgs e)
        {
            // open file dialog   
            OpenFileDialog open = new OpenFileDialog();
            // image filters  
            open.Filter = "Image Files(*.png; *.jpg; *.jpeg; *.gif; *.bmp)|*.png; *.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                // display image in picture box  
                picImagen.Image = new Bitmap(open.FileName);
                // image file path  
                txtRutaImagen.Text = open.FileName;
            }
        }
    }
}
