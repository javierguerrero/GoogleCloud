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
        IReadOnlyList<EntityAnnotation> labelAnnotations;
        IReadOnlyList<EntityAnnotation> textAnnotations;
        IReadOnlyList<EntityAnnotation> logoAnnotations;

        public Form1()
        {
            InitializeComponent();
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", ConfigurationManager.AppSettings["GOOGLE_APPLICATION_CREDENTIALS"]);
            btnProcesar.Enabled = false;
            txtRutaImagen.Enabled = false;
            picLoading.Visible = false;
        }

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            ProcesarImagenAsync();
        }

        private async Task ProcesarImagenAsync()
        {
            picLoading.Visible = true;
            btnProcesar.Enabled = false;
            txtLabelAnnotations.Text = string.Empty;
            txtTextAnnotations.Text = string.Empty;
            txtLogoAnnotations.Text = string.Empty;

            await Task.Run(() => {
                var client = ImageAnnotatorClient.Create();
                var image = Google.Cloud.Vision.V1.Image.FromFile(txtRutaImagen.Text);

                labelAnnotations = client.DetectLabels(image);
                textAnnotations = client.DetectText(image);
                logoAnnotations = client.DetectLogos(image);
            });

            foreach (var annotation in labelAnnotations)
            {
                if (annotation.Description != null)
                {
                    txtLabelAnnotations.Text += annotation.Description + "\r\n";
                }
            }

            foreach (var annotation in textAnnotations)
            {
                if (annotation.Description != null)
                {
                    txtTextAnnotations.Text += annotation.Description + "\r\n";
                }
            }

            foreach (var annotation in logoAnnotations)
            {
                if (annotation.Description != null)
                {
                    txtLogoAnnotations.Text += annotation.Description + "\r\n";
                }
            }

            btnProcesar.Enabled = true;
            picLoading.Visible = false;
        }

        private void btnSeleccionarImagen_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.png; *.jpg; *.jpeg; *.gif; *.bmp)|*.png; *.jpg; *.jpeg; *.gif; *.bmp";
            if (open.ShowDialog() == DialogResult.OK)
            {
                picImagen.Image = new Bitmap(open.FileName);
                txtRutaImagen.Text = open.FileName;
                btnProcesar.Enabled = true;
            }
        }

    }
}
