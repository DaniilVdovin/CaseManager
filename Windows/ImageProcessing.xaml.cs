using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CaseManager.Windows
{
    /// <summary>
    /// Логика взаимодействия для ImageProcessing.xaml
    /// </summary>
    public partial class ImageProcessing : Window
    {
        Bitmap sourceBitmap, targetBitmap;
        public ImageProcessing()
        {
            InitializeComponent();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.FileOk += (s, e) =>
            {
                if (openFileDialog.FileName != "")
                {
                    sourceBitmap = (Bitmap)System.Drawing.Image.FromFile(filename: openFileDialog.FileName);
                    targetBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, sourceBitmap.PixelFormat);
                    
                    i_source.Source = Convert(sourceBitmap);
                    targetBitmap = EdgeDetection(sourceBitmap, 10f);
                    i_result.Source = Convert(targetBitmap);
                }
            };
            openFileDialog.ShowDialog();
        }
        public static Bitmap EdgeDetection(Bitmap bitmap, float threshold)
        {
            Bitmap bSrc = (Bitmap)bitmap.Clone();
            Bitmap b = (Bitmap)bitmap.Clone();

            BitmapData bmData = b.LockBits(new System.Drawing.Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BitmapData bmSrc = bSrc.LockBits(new System.Drawing.Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;

            unsafe
            {
                byte* p = (byte*)(void*)bmData.Scan0;
                byte* pSrc = (byte*)(void*)bmSrc.Scan0;

                int nOffset = stride - b.Width * 3;
                int nWidth = b.Width - 1;
                int nHeight = b.Height - 1;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        var p0 = ToGray(pSrc);
                        var p1 = ToGray(pSrc + 3);
                        var p2 = ToGray(pSrc + 3 + stride);

                        if (Math.Abs(p1 - p2) + Math.Abs(p1 - p0) > threshold)
                            p[0] = p[1] = p[2] = 255;
                        else
                            p[0] = p[1] = p[2] = 0;

                        p += 3;
                        pSrc += 3;
                    }
                    p += nOffset;
                    pSrc += nOffset;
                }
            }            
            b.UnlockBits(bmData);
            bSrc.UnlockBits(bmSrc);
            return b;
        }
        static unsafe float ToGray(byte* bgr)
        {
            return bgr[2] * 0.3f + bgr[1] * 0.59f + bgr[0] * 0.11f;
        }
        public BitmapImage Convert(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            src.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
