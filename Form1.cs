using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextEditor
{
    public partial class TextEditor : Form
    {
        string FileName = "document.txt";
        string CurrentFilePath = "";
        public TextEditor()
        {
            InitializeComponent();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void NewMenu_Click(object sender, EventArgs e)
        {
            string FilePath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName +"\\" + FileName;
            try
            {
                if(File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }
                using (FileStream fs = File.Create(FilePath))
                {
                    FileNameLabel.Text = FileName;
                    MainTextArea.Text = "New file created! Add some text to it in here";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainTextArea.Text = "";
            openFileDialog.Title = $"Open";
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "")
            {
                FileStream fs = (FileStream)openFileDialog.OpenFile();
                FileNameLabel.Text = openFileDialog.FileName;
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);
                while (fs.Read(b, 0, b.Length) > 0)
                {
                    MainTextArea.Text += temp.GetString(b);
                }
                fs.Close();
            }

        }

        public void LoadDirectory(string Dir)
        {
            DirectoryInfo di = new DirectoryInfo(Dir);
            progressBar.Maximum = Directory.GetFiles(Dir, "*.*", SearchOption.AllDirectories).Length + Directory.GetDirectories(Dir, "**", SearchOption.AllDirectories).Length;
            TreeNode treenode = treeView.Nodes.Add(di.Name);
            treenode.Tag = di.FullName;
            treenode.StateImageIndex = 0;
            LoadFiles(Dir, treenode);
            LoadSubDirectories(Dir, treenode);
        }


        private void LoadFiles(string dir, TreeNode td)
        {
            string[] Files = Directory.GetFiles(dir, "*.*");

            // Loop through them to see files  
            foreach (string file in Files)
            {
                FileInfo fi = new FileInfo(file);
                TreeNode tds = td.Nodes.Add(fi.Name);
                tds.Tag = fi.FullName;
                tds.StateImageIndex = 1;
                UpdateProgress();
            }

        }


        private void UpdateProgress()
        { 
               if (progressBar.Value < progressBar.Maximum)  
               {  
                   progressBar.Value++;  
                   int percent = (int)(((double)progressBar.Value / (double)progressBar.Maximum) * 100);
                   progressBar.CreateGraphics().DrawString(percent.ToString() + "%", new Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(progressBar.Width / 2 - 10, progressBar.Height / 2 - 7));  
                   Application.DoEvents();  
               }
        }


private void LoadSubDirectories(string dir, TreeNode td)
        {
            // Get all subdirectories  
            string[] subdirectoryEntries = Directory.GetDirectories(dir);
            // Loop through them to see if they have any other subdirectories  
            foreach (string subdirectory in subdirectoryEntries)
            {

                DirectoryInfo di = new DirectoryInfo(subdirectory);
                TreeNode tds = td.Nodes.Add(di.Name);
                tds.StateImageIndex = 0;
                tds.Tag = di.FullName;
                LoadFiles(subdirectory, tds);
                LoadSubDirectories(subdirectory, tds);
                UpdateProgress();

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult fontresult =  fontDialog.ShowDialog();
            if(fontresult == DialogResult.OK)
            {
                MainTextArea.Font = fontDialog.Font;
            }

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
           DialogResult colorresult = colorDialog.ShowDialog();
            if (colorresult == DialogResult.OK)
            {
                MainTextArea.ForeColor = colorDialog.Color;
            }
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void numericUpDownFontSize_ValueChanged(object sender, EventArgs e)
        {   
            MainTextArea.Font = new Font(MainTextArea.Font.FontFamily, (float)numericUpDownFontSize.Value);
        }

        private void BoldLabel_Click(object sender, EventArgs e)
        {
            if (BoldLabel.BackColor == Color.LightBlue)
            {
                BoldLabel.BackColor = Color.LightGray;
                MainTextArea.SelectionFont = new Font(MainTextArea.Font, FontStyle.Regular);
            }
            else
            {
                BoldLabel.BackColor = Color.LightBlue;
                MainTextArea.SelectionFont = new Font(MainTextArea.Font, FontStyle.Bold);
            }
        }

        private void ItalicLabel_Click(object sender, EventArgs e)
        {
            if (ItalicLabel.BackColor == Color.LightBlue)
            {
                ItalicLabel.BackColor = Color.LightGray;
                MainTextArea.SelectionFont = new Font(MainTextArea.Font, FontStyle.Regular);
            }
            else
            {
                ItalicLabel.BackColor = Color.LightBlue;
                MainTextArea.SelectionFont = new Font(MainTextArea.Font, FontStyle.Italic);
            }
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(FileNameLabel.Text == "" && MainTextArea.Text == "")
            {
                MessageBox.Show("Please create a new file first. Then you can save it");
            }
            else
            {
                //string FilePath = Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "\\" + FileName;
                //using (StreamWriter sw = new StreamWriter(FilePath))
                //{
                //}
                saveFileDialog.Title = $"Save {FileName}";
                saveFileDialog.ShowDialog();
                if(saveFileDialog.FileName != "")
                {
                    FileStream fs = (FileStream)saveFileDialog.OpenFile();
                    byte[] info = new UTF8Encoding(true).GetBytes(MainTextArea.Text);
                    fs.Write(info, 0, info.Length);
                    MessageBox.Show("File has been saved");

                    fs.Close();
                }
            }
        }

        private void TextEditor_Load(object sender, EventArgs e)
        {
            BoldLabel.BackColor = Color.LightGray;
            ItalicLabel.BackColor = Color.LightGray;
            progressBar.Value = 0;
            treeView.Nodes.Clear();
            WindowState = FormWindowState.Maximized;
            CurrentFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            treeView.ExpandAll();
        }
        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            MainTextArea.Text = "";
            CurrentFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+ "\\" + e.Node.Text;
            FileNameLabel.Text = CurrentFilePath;
            try
            {
                MainTextArea.Text = File.ReadAllText(CurrentFilePath);
            }
            catch(Exception x)
            {
                MainTextArea.Text = x.Message;
            }
        }
    }
}
