using ObjectDetection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using winobjectDetect.classes;
using Yolov5Net.Scorer;

namespace winobjectDetect
{
    public partial class lblDesc : Form
    {


        public lblDesc()
        {
            InitializeComponent();
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            label1.Text = "---------------------------------------------------";

            // Create an OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set filter options and filter index
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            openFileDialog.FilterIndex = 1;
            openFileDialog.Multiselect = false;

            // Show the dialog and get result
            DialogResult result = openFileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                // Get the selected file name and display it in a PictureBox
                string filePath = openFileDialog.FileName;

                // Load the image from the selected file
                Image image = Image.FromFile(filePath);

                // Set the PictureBox's Image property
                pbNormalImg.Image = image;

                // Optionally, set the PictureBox's SizeMode property
                pbNormalImg.SizeMode = PictureBoxSizeMode.StretchImage; // or other modes like AutoSize, CenterImage, etc.
            }
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            label1.Text = "Detecting Please Wait";

            Image currentImg = pbNormalImg.Image;
            yolov5sprocess pi = new yolov5sprocess();
            pbMLImage.SizeMode = PictureBoxSizeMode.StretchImage;


            #region old working
            var output = pi.ProcessImages(currentImg);

            if (output.Item1 == null)
            {
                MessageBox.Show("None of the objects detected in the current image");
            }

            pbMLImage.Image = output.Item1;

            try
            {
                int score = Convert.ToInt32((output.Item2[0].Score) * 100);
                string label = output.Item2[0].Label.Name;
                label1.Text = "Detection Score = " + score.ToString() + " label - " + label;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected Error");
            }

            #endregion




        }

        private void fileSystemWatcher1_Changed(object sender, System.IO.FileSystemEventArgs e)
        {

        }
    }
}
