using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace CaseManager
{
    /// <summary>
    /// Логика взаимодействия для OpenSpace.xaml
    /// </summary>
    public partial class OpenSpace : UserControl
    {
        private Point origin;
        private Point start;

        TransformGroup  group;
        ScaleTransform  xform;
        TranslateTransform tt;

        public OpenSpace()
        {
            InitializeComponent();
            group = new TransformGroup();
            xform = new ScaleTransform();
            group.Children.Add(xform);
            tt = new TranslateTransform();
            group.Children.Add(tt);
            Canvas.RenderTransform = group;
            Canvas.MouseWheel += image_MouseWheel;
            Canvas.MouseLeftButtonDown += image_MouseLeftButtonDown;
            Canvas.MouseLeftButtonUp += image_MouseLeftButtonUp;
            Canvas.MouseMove += image_MouseMove;
        }
        
        private void UserControl_Initialized(object sender, EventArgs e)
        {
            InitializeSizes();
        }
        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Canvas.ReleaseMouseCapture();
        }
        private void InitializeSizes()
        {
            //Canvas.Children.Clear();
            for (int i = 10; i < Canvas.Height; i+=50)
            {
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Gray);
                line.X1 = 0;
                line.X2 = Canvas.Width;
                line.Y1 = line.Y2 = i;
                Canvas.Children.Add(line);
            }
            for (int i = 10; i < Canvas.Width; i +=50)
            {
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Gray);
                line.Y1 = 0;
                line.Y2 = Canvas.Height;
                line.X1 = line.X2 = i;
                Canvas.Children.Add(line);
            }
        }
        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Canvas.IsMouseCaptured) return;
            var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
            Vector v = start - e.GetPosition(CanvasViewer);
            tt.X = origin.X - v.X;
            tt.Y = origin.Y - v.Y;
        }
        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas.CaptureMouse();
            var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
            start = e.GetPosition(CanvasViewer);
            origin = new Point(tt.X, tt.Y);
        }
        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            TransformGroup transformGroup = (TransformGroup)Canvas.RenderTransform;
            ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];
            double zoom = e.Delta > 0 ? .2 : -.2;
            if (!(e.Delta > 0) && (transform.ScaleX < .4 || transform.ScaleY < .4))
                return;
            transform.ScaleX += zoom;
            transform.ScaleY += zoom;
        }
    }
}
