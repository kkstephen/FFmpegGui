using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows; 
using Microsoft.Win32;

namespace FFGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btn_in_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                this.txt_fileIn.Text = dialog.FileName;

                string tmp = System.IO.Path.GetExtension(dialog.FileName);

                this.txt_fileOut.Text = dialog.FileName.Replace(tmp, "_out" + tmp);
            }
        }

        private void Btn_start_Click(object sender, RoutedEventArgs e)
        {
            string str = "ffmpeg";

            if (this.cb_hw.Text != "Auto")
            {
                str += " -hwaccel_output_format " + this.cb_hw.SelectedValue;
            }

            str += " -i \"" + this.txt_fileIn.Text + "\"";

            if (this.cb_video.Text != "Auto")
            {
                str += " -c:v " + this.cb_video.SelectedValue;
            }

            if (this.cb_vrate.Text != "Auto")
            {
                str += " -b:v " + this.cb_vrate.SelectedValue;
            }

            if (this.cb_audio.Text != "Auto")
            {
                str += " -c:a " + this.cb_audio.SelectedValue;
            }

            if (this.cb_arate.Text != "Auto")
            {
                str += " -b:a " + this.cb_arate.SelectedValue;
            }

            if (this.cb_res.Text != "Auto")
            {
                str += " -s " + this.cb_res.SelectedValue;
            }

            if (this.cb_fps.Text != "Auto")
            {
                str += " -r " + this.cb_fps.SelectedValue;
            }

            str += " \"" + this.txt_fileOut.Text + "\"";

            this.txt_cmd.Document.Blocks.Clear();
            this.txt_cmd.AppendText(str);
        }
    }
}
