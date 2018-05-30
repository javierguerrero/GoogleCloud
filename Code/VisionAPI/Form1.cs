using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
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
        WebDetection webAnnotations;
        
        public Form1()
        {
            InitializeComponent();
            btnProcesar.Enabled = false;
            txtRutaImagen.Enabled = false;
            picLoading.Visible = false;

            //string appPath = Path.GetDirectoryName(Application.ExecutablePath);
            //System.IO.DirectoryInfo directoryInfo = System.IO.Directory.GetParent(appPath);
            //System.IO.DirectoryInfo directoryInfo2 = System.IO.Directory.GetParent(directoryInfo.FullName);
            //string path = directoryInfo2.FullName + @"\data\KSINPI-cfc2106a59da.json";
            string path = ConfigurationManager.AppSettings["GOOGLE_APPLICATION_CREDENTIALS"];
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
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
                webAnnotations = client.DetectWebInformation(image);

            });

            foreach (var annotation in labelAnnotations)
            {
                if (annotation.Description != null)
                {
                    txtLabelAnnotations.Text += annotation.Description + $" ({ Math.Round(annotation.Score, 2) * 100}%)" + "\r\n";
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
                    txtLogoAnnotations.Text += annotation.Description + $" ({ Math.Round(annotation.Score, 2) * 100}%)" + "\r\n";
                }
            }

            webBrowser1.DocumentText = GetHTML(webAnnotations);

            btnProcesar.Enabled = true;
            picLoading.Visible = false;
        }

        private string GetHTML(WebDetection webAnnotations)
        {
            string html = string.Empty;
            html += "<html><body>";

            if (webAnnotations.WebEntities.Count > 0)
            {
                html += "<strong>Web Entities</strong>";
                html += "<ul>";
                foreach (var item in webAnnotations.WebEntities)
                {
                    html += $"<li><a target='_blank' href='https://www.google.com/search?q={item.Description}+logos&tbm=isch'>{item.Description}</a></li>";
                }
                html += "</ul>";
            }

            if (webAnnotations.PagesWithMatchingImages.Count > 0)
            {
                html += "<strong>Pages with Matched Images</strong>";
                html += "<ul>";
                foreach (var item in webAnnotations.PagesWithMatchingImages)
                {
                    html += $"<li><a target='_blank' href='{item.Url}'>{item.Url}</a></li>";
                }
                html += "</ul>";
            }

            if (webAnnotations.FullMatchingImages.Count > 0)
            {
                html += "<strong>Fully Matched Images</strong>";
                html += "<ul>";
                foreach (var item in webAnnotations.FullMatchingImages)
                {
                    html += $"<li><a target='_blank' href='{item.Url}'>{item.Url}</a></li>";
                }
                html += "</ul>";
            }

            if (webAnnotations.PartialMatchingImages.Count > 0)
            {
                html += "<strong>Partially Matched Images</strong>";
                html += "<ul>";
                foreach (var item in webAnnotations.PartialMatchingImages)
                {
                    html += $"<li><a target='_blank' href='{item.Url}'>{item.Url}</a></li>";
                }
                html += "</ul>";
            }
            html += "</body></html>";

            return html;
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
