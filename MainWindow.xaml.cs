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
        private readonly string AUTO = "Auto";

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
            StringBuilder cmd = new StringBuilder("ffmpeg");

            if (this.cb_hw.Text != this.AUTO)
            {
                cmd.Append(" -hwaccel_output_format " + this.cb_hw.SelectedValue);
            }

            cmd.Append(" -i \"" + this.txt_fileIn.Text + "\"");            

            if (this.qsize.IsChecked == true)
            {
                cmd.Append(" -max_muxing_queue_size 1024 ");
            }

            //video codec
            if (this.cb_video.Text == this.AUTO && this.cb_vrate.Text == this.AUTO)
            {
                if (this.chk_inst.IsChecked != true)
                {
                    cmd.Append(" -vcodec copy");
                }
                else
                {
                    cmd.Append(" -vn");
                }
            }
            else
            {
                //video codes
                if (this.cb_video.Text != this.AUTO)
                {
                    cmd.Append(" -c:v " + this.cb_video.SelectedValue);
                }

                //video bitrate
                if (this.cb_vrate.Text != this.AUTO)
                {
                    cmd.Append(" -b:v " + this.cb_vrate.SelectedValue);
                }

                cmd.Append(" -bufsize " + this.cb_vrate.SelectedValue);
            }

            if (this.cb_res.Text != this.AUTO)
            {
                cmd.Append(" -s " + this.cb_res.SelectedValue);
            }

            if (this.cb_fps.Text != this.AUTO)
            {
                cmd.Append(" -r " + this.cb_fps.SelectedValue);
            }

            //8bit only
            if (this.bit8only.IsChecked == true)
            {
                cmd.Append(" -vf format=yuv420p");
            }

            //1080i compensate
            if (this.chk_deinter.IsChecked == true)
            {
                cmd.Append(" -deinterlace");
            }

            //profile level
            if (this.cb_profile.Text != this.AUTO)
            {
                switch(this.cb_profile.SelectedIndex)
                {
                    case 1:
                        cmd.Append(" -profile:v main -level:v 4");
                        break;
                    case 2:
                        cmd.Append(" -profile:v high -level:v 4.2");
                        break;
                    default:                       
                        break;
                }
            }
            
            if (this.cb_audio.Text == this.AUTO && this.cb_arate.Text == this.AUTO)
            {
                cmd.Append(" -acodec copy ");
            }
            else
            {
                //audio codes
                if (this.cb_audio.Text != this.AUTO)
                {
                    cmd.Append(" -c:a " + this.cb_audio.SelectedValue);
                }

                //audio bitrate 
                if (this.cb_arate.Text != this.AUTO)
                {
                    cmd.Append(" -b:a " + this.cb_arate.SelectedValue);
                }
            }

            //channel
            if (this.cb_chan.Text != this.AUTO)
            {
                cmd.Append(" -ac " + this.cb_chan.SelectedValue);
            }
            
            //simple rate
            if (this.cb_sr.Text != this.AUTO)
            {
                cmd.Append(" -ar " + this.cb_sr.SelectedValue);
            }
            
            //subclip
            int s = 0;

            if (this.txt_start.Text != "0")
            {
                if (int.TryParse(this.txt_start.Text, out s))
                {
                    if (s > 0) cmd.Append(" -ss " + s);
                }
            }

            if (this.txt_end.Text != "0")
            {                
                int n = 0;

                if (int.TryParse(this.txt_end.Text, out n))
                { 
                    if (n > 0 && n > s)
                    {
                        cmd.Append(" -to " + n);
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

            cmd.Append(" \"" + output + "\"");

            this.txt_cmd.Document.Blocks.Clear();

            string str = cmd.ToString();

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
