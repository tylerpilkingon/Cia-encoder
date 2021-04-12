using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;


namespace Cia_project {
    public partial class Form1 : Form {
        private Encoding CIAimage = new Encoding();
        private Bitmap newImg;
        private bool encoded = false;

        public Form1() {
            InitializeComponent();
            pictureBox1.Image = Image.FromFile("C:\\Users\\Tyler\\Pictures\\Cia pic.jpg");
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            DialogResult drgtemp = ofdLoadImage.ShowDialog();
            if (drgtemp == DialogResult.OK) {

                richTextBox1.Text = null;
                textBox2.Text = null;
                CIAimage = new Encoding();

                string filename = (ofdLoadImage.FileName);
                Bitmap newImg = CIAimage.GenerateBitmap(filename);
                picImage.Image = newImg;

                if (newImg.Width * newImg.Height < 256) richTextBox1.MaxLength = (newImg.Width) * (newImg.Height);
                else richTextBox1.MaxLength = 256;

                richTextBox1.Enabled = true;
                Encode_Button.Enabled = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Close();
        }

        private void button1_Click(object sender, EventArgs e) {
            if (picImage.Image == null) MessageBox.Show("You have not put in a picture", "Image Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else if (richTextBox1.Text == "" || richTextBox1.Text == " ") MessageBox.Show("You have not entered text to be encoded", "Text Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else {
                
                Bitmap oldImg = new Bitmap(picImage.Image);
                newImg = CIAimage.PrepImage(oldImg);
                CIAimage.Encoder(newImg, CIAimage.BuildDictionary(), richTextBox1.Text.ToLower());
                textBox2.Text = "Image has been encoded";
                encoded = true;
            }

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (encoded != true) MessageBox.Show("Please encode a message before \n you save", "Encode Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
                    Bitmap newImage = newImg;

                    CIAimage.Saveimgae(saveFileDialog1.FileName,newImage);
                }
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e) {
            Counttxt.Text = string.Format("{0}/{1}", richTextBox1.Text.Length, richTextBox1.MaxLength);
            textBox2.Text = null;
        }

        
    }
}
