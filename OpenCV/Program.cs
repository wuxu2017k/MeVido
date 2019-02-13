using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using System.Threading;

namespace OpenCV
{
    class Program
    {
        static bool done = false;
        static readonly object locker = new object();
        static void Main(string[] args)
        {
            Thread t = new Thread(Go);
            t.Start();
            t.Join();
            Console.WriteLine("Thread t has ended!");
            Console.ReadKey();
        }
        static void Go()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.Write("y");
            }
        }

        private static void one()
        {
            Mat src = new Mat("boy.png", ImreadModes.Grayscale);
            Mat dst = new Mat();
            Cv2.Canny(src, dst, 50, 200);


            Cv2.PutText(src, "wuxu", new Point(20, 80), HersheyFonts.HersheyComplex, 5, Scalar.Red);
            using (new Window("src image", src))
            using (new Window("dst image", dst))
            {
                Cv2.WaitKey();
            }
        }
        private static void two()
        {
            Mat panda = new Mat(@"boy.png", ImreadModes.AnyColor);
            Rect roi = new Rect(100, 100, 320, 200);
            Mat ImageROI = new Mat(panda, roi);
            Rect rect = new Rect(0, 0, ImageROI.Rows, ImageROI.Cols);
            Mat pos = new Mat(panda, rect);
            ImageROI.CopyTo(pos);
            Cv2.ImShow("兴趣区域", ImageROI);
            Cv2.ImShow("boy", panda);
            Cv2.WaitKey();
            
        }
    }
}
