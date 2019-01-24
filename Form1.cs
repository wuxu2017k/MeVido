


using DirectShowLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeVido
{
    public partial class Form1 : Form
    {
        long time1;
        int i = 0;
        const int WMGraphNotify = 0x0400 + 13;
        string file;
        Guid TIME_FORMAT_FRAME = new Guid("7b785570-8c82-11cf-bc0c-00aa00ac74f6");
        Thread t = null;
        const int WM_APP = 0x8000;
        const int WM_GRAPHNOTIFY = WM_APP + 1;
        const int EC_COMPLETE = 0x01;
        const int WS_CHILD = 0x40000000;
        const int WS_CLIPCHILDREN = 0x2000000;
        IGraphBuilder GraphBuilder=null;//	 最为重用的COM接口,用于手动或者自动构造过滤通道Filter Graph Manager

        IMediaControl MediaControl = null;//	用来控制流媒体，例如流的启动和停止暂停等,播放控制接口

        IMediaSeeking MediaSeeking = null;//另一个播放的位置和播放速度控制接口,在位置选择方面功能较强.设置播放格式，多种控制播放方式.常用的有:(1)TIME_FORMAT_MEDIA_TIME单位100纳秒。(2)TIME_FORMAT_FRAME按帧播放

        IMediaPosition MediaPosition = null;//播放的位置和速度控制接口(控制播放位置只能为设置时间控制方式)

        IMediaEvent MediaEvent = null;//播放事件接口 ,该接口在Filter Graph发生一些事件时用来创建事件的标志信息并传送给应用程序

        IMediaEventEx MediaEventEx = null;//扩展播放事件接口

        IBasicAudio BasicAudio = null;//声音控制接口

        IBasicVideo BasicVideo = null;//图像控制接口(波特率，宽度，长度等信息)

        IVideoWindow VideoWindow = null;//显示窗口控制接口 (有关播放窗口的一切控制，包括caption显示，窗口位置控制等)

        ISampleGrabber SampleGrabber = null;//捕获图象接口(可用于抓图控制)

        IVideoFrameStep VideoFrameStep = null;//控制单帧播放的接口
        //MediaSeeking.SetTimeFormat(TIME_FORMAT_FRAME);VideoFrameStep.Step(1,);
        //VideoWindow.SetWindowPosition(0, 0, 100, 120);
        public Form1()
        {
            InitializeComponent();



        }
        [DllImport("user32.dll")]//取设备场景 
        private static extern IntPtr GetDC(IntPtr hwnd);//返回设备场景句柄 
        [DllImport("gdi32.dll")]//取指定点颜色 
        private static extern int GetPixel(IntPtr hdc, Point p);
        private void button1_Click(object sender, EventArgs e)
        {
            
           
            TIME_FORMAT_FRAME.ToString();
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "选择播放文件|*.mpg;*.avi;*.mov;*.wma;*.wav;*.mp3|All Files|*.*";
            if (DialogResult.OK == openFile.ShowDialog())
            {
                //System.IO.Path.GetFileName(openFile.FileName);//得到文件名
                //System.IO.Path.GetDirectoryName(openFile.FileName);//得到路径
                file = System.IO.Path.GetDirectoryName(openFile.FileName) +"\\"+ System.IO.Path.GetFileName(openFile.FileName);
                label17.Text =" "+ file;


            }
            open();
            close();
            open();
         
        }
        private void open()
        {
            int hr;
            if (this.GraphBuilder == null)
            {
                this.GraphBuilder = (IGraphBuilder)new FilterGraph();

                hr = GraphBuilder.RenderFile(file, null);//读取文件
                DsError.ThrowExceptionForHR(hr);
                this.MediaControl = (IMediaControl)this.GraphBuilder;
                this.MediaEventEx = (IMediaEventEx)this.GraphBuilder;
                MediaSeeking = (IMediaSeeking)this.GraphBuilder;
                MediaSeeking.SetTimeFormat(TIME_FORMAT_FRAME);
                MediaSeeking.SetRate(0.3);
                this.VideoFrameStep = (IVideoFrameStep)this.GraphBuilder;
                // MediaPosition= (IMediaPosition)this.GraphBuilder;
                this.VideoWindow = this.GraphBuilder as IVideoWindow;
                this.BasicVideo = this.GraphBuilder as IBasicVideo;
                this.BasicAudio = this.GraphBuilder as IBasicAudio;

                hr = this.MediaEventEx.SetNotifyWindow(this.Handle, WM_GRAPHNOTIFY, IntPtr.Zero);
                DsError.ThrowExceptionForHR(hr);


                hr = this.VideoWindow.put_Owner(this.Handle);
                DsError.ThrowExceptionForHR(hr);
                hr = this.VideoWindow.put_WindowStyle(WindowStyle.Child |
                    WindowStyle.ClipSiblings | WindowStyle.ClipChildren);
                DsError.ThrowExceptionForHR(hr);
                this.Focus();
                hr = InitVideoWindow(1, 1);
                DsError.ThrowExceptionForHR(hr);
                long time;
                MediaSeeking.GetDuration(out time);
                label20.Text = time.ToString();
                trackBar1.SetRange(0, (int)time);
                t = new Thread(new ThreadStart(updateTimeBarThread));

            }
        }
        private int InitVideoWindow(int nMultiplier, int nDivider)
        {
            int hr = 0;
            int Height, Width;
            if (this.BasicVideo == null)
                return 0;
            hr = this.BasicVideo.GetVideoSize(out Width, out Height);
            if (hr == DsResults.E_NoInterface)
            {
                return 0;
            }
            //Width = Width * nMultiplier / nDivider;
            //Height = Height * nMultiplier / nDivider;
        
            Application.DoEvents();
            if (Width <= 500 && Height <= 500)
            {
                hr = this.VideoWindow.SetWindowPosition(0, 0, Width , Height);
                label18.Text = (Width ).ToString();
                label19.Text = (Height ).ToString();
            }
            else
            {
                hr = this.VideoWindow.SetWindowPosition(0, 0, Width / 2, Height / 2);
                label18.Text =(Width / 2).ToString();
                label19.Text =(Height / 2).ToString();
            }
            
            return hr;
        }
        private void updateTimeBarThread()
        {
            long time;
            int volu;
            while(true)
            {
                try {
                    MediaSeeking.GetCurrentPosition(out time);

                    BasicAudio.get_Volume(out volu);

                    this.BeginInvoke(new MethodInvoker(() => { trackBar1.Value = (int)time; }));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                
                Thread.Sleep(1000);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
               

          var b  = MediaControl.Run();
          

           
            timer1.Enabled = true;
           



        }

     

        private void closeVideo()
        {
            int hr = 0;
            if(this.MediaControl!=null)
            {
                hr = this.MediaControl.Stop();
                closeInterfaces();

            }

        }
        private void closeInterfaces()
        {
            int hr = 0;
            try
            {
                lock (this)
                {
                    //relinquish ownership (IMPORTANT!) after hiding video window
                    hr = this.VideoWindow.put_Visible(OABool.False);
                    DsError.ThrowExceptionForHR(hr);
                    hr = this.VideoWindow.put_Owner(IntPtr.Zero);
                    DsError.ThrowExceptionForHR(hr);

                    if (this.MediaEventEx != null)
                    {
                        hr = this.MediaEventEx.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);
                        DsError.ThrowExceptionForHR(hr);
                    }
                    //release and zero DirectShow interfaces
                    if (this.MediaEventEx != null) this.MediaEventEx = null;
                    if (this.MediaSeeking != null) this.MediaSeeking = null;
                    if (this.MediaPosition != null) this.MediaPosition = null;
                    if (this.MediaControl != null) this.MediaControl = null;
                    if (this.BasicAudio != null) this.BasicAudio = null;
                    if (this.BasicVideo != null) this.BasicVideo = null;
                    if (this.VideoWindow != null) this.VideoWindow = null;
                    if (this.VideoFrameStep != null) this.VideoFrameStep = null;
                    if (this.GraphBuilder != null)
                    {
                       
                        this.GraphBuilder = null;
                    }
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {

            close();

        }
        private void close()
        {
            long time = 0;
           
            MediaSeeking.GetCurrentPosition(out time);
            closeVideo();
            t.Abort();
            yy = 0;
            //textBox1.Text = "";
            trackBar1.Value = 0;
           
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            long time = (long)trackBar1.Value;

       
            MediaSeeking.GetCurrentPosition(out time);
            
        }
        static long yy;

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (MediaSeeking != null)
            {
                long time=0;
                MediaSeeking.GetCurrentPosition(out time);
                label11.Text = time.ToString();
                if(time-yy==1||time==trackBar1.Maximum)
                {
                    yy = time;
                    timer1.Enabled = false;
                    MediaControl.Pause();
                   // textBox1.Text += trackBar1.Value+" ";
                }
                trackBar1.Value = (int)time;
              
            }
        }

    

      

        private void timer2_Tick(object sender, EventArgs e)
        {
            Point screenPoint = Control.MousePosition;//鼠标相对于屏幕左上角的坐标
            Point formPoint = this.PointToClient(Control.MousePosition);//鼠标相对于窗体左上角的坐标
           label12.Text = formPoint.X.ToString();
            label13.Text = formPoint.Y.ToString();
            //label12.Text = Control.MousePosition.X.ToString();
            // label13.Text = Control.MousePosition.Y.ToString();
            Point p = new Point(MousePosition.X, MousePosition.Y);//取置顶点坐标 
            IntPtr hdc = GetDC(new IntPtr(0));//取到设备场景(0就是全屏的设备场景)
            int c = GetPixel(hdc, p);//取指定点颜色
            label14.Text = (c & 0xFF).ToString();//转换R
            label15.Text = ((c & 0xFF00) / 256).ToString();//转换G
            label16.Text = ((c & 0xFF0000) / 65536).ToString();//转换B

           

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.timer2.Enabled = true;
            this.timer1.Interval = 10;//timer控件的执行频率
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Process m_Process = null;
            m_Process = new Process();
            m_Process.StartInfo.FileName = "";
            m_Process.Start();
        }
    }
}
