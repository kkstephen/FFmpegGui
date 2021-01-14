using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Documents;

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

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txt_fileIn.Text = dialog.FileName;
                this.txt_fileOut.Text = System.IO.Path.GetDirectoryName(dialog.FileName) + "\\" + GetFileName(dialog.FileName, this.chk_format.IsChecked);
            }
        }

        private void Btn_out_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fname = GetFileName(this.txt_fileIn.Text, this.chk_format.IsChecked);
                this.txt_fileOut.Text = dialog.SelectedPath + "\\" + fname;
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

            if (this.qsize.IsChecked == true)
            {
                str += " -max_muxing_queue_size 1024 ";
            }

            //video codec
            if (this.cb_video.Text == "Auto" && this.cb_vrate.Text == "Auto")
            {
                str += " -vcodec copy";
            }
            else
            {
                if (this.cb_video.Text != "Auto")
                {
                    str += " -c:v " + this.cb_video.SelectedValue;
                }

                if (this.cb_vrate.Text != "Auto")
                {
                    str += " -b:v " + this.cb_vrate.SelectedValue;
                }

                str += " -bufsize " + this.cb_vrate.SelectedValue;
            }

            //8bit only
            if (this.bit8only.IsChecked == true)
            {
                str += " -vf format=yuv420p";
            }

            //1080i compensate
            if (this.chk_deinter.IsChecked == true)
            {
                str += " -deinterlace";
            }

            //profile level
            if (this.cb_profile.Text != "Auto")
            {
                switch(this.cb_profile.SelectedIndex)
                {
                    case 1:
                        str += " -profile:v main -level:v 4";
                        break;
                    case 2: str += " -profile:v high -level:v 4.2";
                        break;
                    default:                       
                        break;
                }
            }
            
            if (this.cb_audio.Text == "Auto" && this.cb_arate.Text == "Auto")
            {
                str += " -acodec copy ";
            }
            else
            {
                if (this.cb_audio.Text != "Auto")
                {
                    str += " -c:a " + this.cb_audio.SelectedValue;
                }

                if (this.cb_arate.Text != "Auto")
                {
                    str += " -b:a " + this.cb_arate.SelectedValue;
                }
            }

            if (this.cb_chan.Text != "Auto")
            {
                str += " -ac " + this.cb_chan.SelectedValue;
            }

            if (this.cb_res.Text != "Auto")
            {
                str += " -s " + this.cb_res.SelectedValue;
            }

            if (this.cb_fps.Text != "Auto")
            {
                str += " -r " + this.cb_fps.SelectedValue;
            }

            //subclip
            int s = 0;

            if (this.txt_start.Text != "0")
            {
                if (int.TryParse(this.txt_start.Text, out s))
                {
                    if (s > 0) str += " -ss " + s;
                }
            }

            if (this.txt_end.Text != "0")
            {                
                int n = 0;

                if (int.TryParse(this.txt_end.Text, out n))
                { 
                    if (n > 0 && n > s)
                    {
                        str += " -to " + n;
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Time factor number is worng!");
                    }
                }             
            }

            string output = this.txt_fileOut.Text;

            if (string.IsNullOrEmpty(this.txt_fileOut.Text))
            {
                output = System.IO.Path.GetDirectoryName(this.txt_fileIn.Text) + "\\" + GetFileName(this.txt_fileIn.Text, this.chk_format.IsChecked);
            }

            str += " \"" + output + "\"";

            this.txt_cmd.Document.Blocks.Clear();
            this.txt_cmd.AppendText(str);
        }

        private void Btn_clear_Click(object sender, RoutedEventArgs e)
        {
            this.txt_cmd.Document.Blocks.Clear();
        }
        
        private static string GetFileName(string file, bool? mp4_fm)
        {
            string tmp = System.IO.Path.GetFileNameWithoutExtension(file);
            string ext = System.IO.Path.GetExtension(file);

            if (mp4_fm != null && mp4_fm == true)
            {
                ext = ".mp4";
            }

            return tmp + "_out" + ext;
        }

        private void Btn_copy_Click(object sender, RoutedEventArgs e)
        {
            TextRange doc = new TextRange(this.txt_cmd.Document.ContentStart, this.txt_cmd.Document.ContentEnd);

            System.Windows.Clipboard.SetDataObject(doc.Text);
        }
    }
}
