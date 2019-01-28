using AviFile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeVido
{
    public partial class Form2 : Form
    {
        string file;
        public Form2()
        {
            InitializeComponent();
        }
        AviManager aviManager = null;
        VideoStream aviStream = null;
        Bitmap bmp = null;
        static int i = 0;
        int framecount = 0;
        private void button1_Click(object sender, EventArgs e)
        {
        
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "选择播放文件|*.mpg;*.avi;*.mov;*.wma;*.wav;*.mp3|All Files|*.*";
            if (DialogResult.OK == openFile.ShowDialog())
            {
                //System.IO.Path.GetFileName(openFile.FileName);//得到文件名
                //System.IO.Path.GetDirectoryName(openFile.FileName);//得到路径
                file = System.IO.Path.GetDirectoryName(openFile.FileName) + "\\" + System.IO.Path.GetFileName(openFile.FileName);        
            }
            open();
        }
        private void open()
        {
            aviManager = new AviManager(file, true);
            aviStream = aviManager.GetVideoStream();
            aviStream.GetFrameOpen();
            framecount = aviStream.CountFrames;
            label2.Text = framecount.ToString();
            bmp= aviStream.GetBitmap(i);
            label6.Text = bmp.Width.ToString();//宽度
            label4.Text = bmp.Height.ToString();//高度
            pictureBox1.Width = (bmp.Width);
            pictureBox1.Height = (bmp.Height);
            pictureBox1.Image = bmp;
            label8.Text = i.ToString();//当前帧数
            Form2 f = new Form2();
            f.Width = (bmp.Width) + 200;
            f.Height = (bmp.Height) + 300;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            i++;
            bmp = aviStream.GetBitmap(i);
            pictureBox1.Image = bmp;
            label8.Text = i.ToString();//当前帧数
            if(i==framecount-1)
            {
                i = 0;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (i == 0)
            {
                i = 0;
            }
            else
            { 
            i--;
            bmp = aviStream.GetBitmap(i);
            pictureBox1.Image = bmp;
            label8.Text = i.ToString();//当前帧数
            
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Point p = e.Location;
            label10.Text = p.X.ToString();
            label11.Text = p.Y.ToString();
            var c = bmp.GetPixel(p.X, p.Y);
            label14.Text = Convert.ToString(c.R);
            label15.Text = Convert.ToString(c.G);
            label17.Text = Convert.ToString(c.B);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            i = 0;
            aviStream.Close();
            aviManager.Close();
           
        }
    }
}
