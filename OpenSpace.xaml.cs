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
    /// Курсор для Canvas
    /// </summary>
    public class Canvas_Cursor {
        Line line1, line2;
        TextBlock text;
        Canvas canvas;
        public Canvas_Cursor(Canvas canvas)
        {
            this.canvas = canvas;
            line1 = new Line();line2 = new Line();
            text = new TextBlock();
            canvas.Children.Add(line1);
            canvas.Children.Add(line2);
            canvas.Children.Add(text);
            line1.Stroke = line2.Stroke = text.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            line1.StrokeThickness = line2.StrokeThickness = 1d;
        }
        public void SetPosition(Point point)
        {
            SetVisible(true);
            Canvas.SetLeft(text, point.X+10); 
            Canvas.SetTop(text, point.Y+10);
            text.Text = $"[{point.X},{point.Y}]";
            line1.X1 = line1.X2 = point.X;
            line1.Y1 = 0; line1.Y2 = canvas.Height;
            line2.Y1 = line2.Y2 = point.Y;
            line2.X1 = 0; line2.X2 = canvas.Width;
        }
        public void SetVisible(bool visible)
        {
            text.Visibility  = visible ? Visibility.Visible : Visibility.Hidden;
            line1.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            line2.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }
    }
    /// <summary>
    /// Логика взаимодействия для OpenSpace.xaml
    /// </summary>
    public partial class OpenSpace : UserControl
    {
        private Point origin;
        private Point canvas_origin;
        private Point start;
        private bool _isAdding = false;
        private UIElement Adding = null;
        public Canvas_Cursor canvas_Cursor;
        public OpenSpace()
        {
            InitializeComponent();
            TransformGroup group = new TransformGroup();
            ScaleTransform xform = new ScaleTransform();
            group.Children.Add(xform);
            TranslateTransform tt = new TranslateTransform();
            group.Children.Add(tt);
            Canvas.RenderTransform = group;

        }
        public void Add_Element(UIElement uIElement)
        {
            if (!_isAdding)
            {
                Adding = uIElement;
                Adding.IsEnabled= false;
                Canvas.Children.Add(uIElement);
                _isAdding = true;
            }
        }
        private void UserControl_Initialized(object sender, EventArgs e)
        {
            
            Canvas.MouseWheel           += image_MouseWheel;
            Canvas.MouseLeftButtonDown  += image_MouseLeftButtonDown;
            Canvas.MouseLeftButtonUp    += image_MouseLeftButtonUp;
            Canvas.MouseMove            += image_MouseMove;
            InitializeSizes();
            canvas_Cursor = new Canvas_Cursor(Canvas);
        }
        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(sender is Canvas)
                Canvas.ReleaseMouseCapture();
        }
        private void InitializeSizes()
        {
            Rectangle bacground = new Rectangle();
            bacground.Width = Canvas.Width;
            bacground.Height = Canvas.Height;
            bacground.Fill = new SolidColorBrush(Colors.Transparent);
            Canvas.Children.Add(bacground);
            for (int i = 10; i < Canvas.Height; i+=50)
            {
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Gray);
                line.StrokeThickness = 1;
                line.X1 = 0;
                line.X2 = Canvas.Width;
                line.Y1 = line.Y2 = i;
                Canvas.Children.Add(line);
            }
            for (int i = 10; i < Canvas.Width; i +=50)
            {
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Gray);
                line.StrokeThickness = 1;
                line.Y1 = 0;
                line.Y2 = Canvas.Height;
                line.X1 = line.X2 = i;
                Canvas.Children.Add(line);
            }
        }
        private void Corect_Size()
        {
            var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
            Console.WriteLine($"Start:{tt.X},{tt.Y}\nCanvas:{Canvas.Width},{Canvas.Height}\nCanvasViewer:{CanvasViewer.ActualWidth},{CanvasViewer.ActualHeight}");
            if (tt.X > 0 && tt.X < Canvas.ActualWidth - CanvasViewer.ActualWidth) tt.X = 0;
            if (tt.Y > 0 && tt.Y < Canvas.ActualHeight - CanvasViewer.ActualHeight) tt.Y = 0;
            Console.WriteLine($"End  :{tt.X},{tt.Y}\nCanvas:{Canvas.Width},{Canvas.Height}\nCanvasViewer:{CanvasViewer.ActualWidth},{CanvasViewer.ActualHeight}");
        }
        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isAdding)
            {
                if (sender is Canvas)
                {
                    canvas_Cursor.SetPosition(e.GetPosition(Canvas));
                    if (!Canvas.IsMouseCaptured) return;
                    var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
                    Vector v = start - e.GetPosition(CanvasViewer);
                    tt.X = origin.X - v.X;
                    tt.Y = origin.Y - v.Y;
                    Console.WriteLine($"MouseMove\nVector:{v.X},{v.Y}\nStart:{start.ToString()}\nOrigin:{origin.ToString()}\ntt:{tt.X},{tt.Y}");
                    Corect_Size();
                }
                else
                {
                    canvas_Cursor.SetVisible(false);
                }
            }
            else
            {
                canvas_Cursor.SetVisible(false);
                Point point = e.GetPosition(Canvas);
                Canvas.SetLeft(Adding, point.X-Adding.DesiredSize.Width/2);
                Canvas.SetTop(Adding, point.Y-Adding.DesiredSize.Height/2);
            }
        }
        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!_isAdding)
            {
                if (sender is Canvas)
                {

                    canvas_origin = e.GetPosition(Canvas);
                    Canvas.CaptureMouse();
                    var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
                    start = e.GetPosition(CanvasViewer);
                    tt.X = start.X - canvas_origin.X;
                    tt.Y = start.Y - canvas_origin.Y;
                    origin = new Point(tt.X , tt.Y);
                    Console.WriteLine($"MouseLeftButtonDown\nStart:{start.ToString()}\nOrigin:{origin.ToString()}\ntt:{tt.X},{tt.Y}\ncanvas_origin:{canvas_origin.X},{canvas_origin.Y}");
                    Corect_Size();
                }
            }
            else
            {
                _isAdding = false;
                Adding.IsEnabled = true;
                Adding = null;
            }
        }
        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is Canvas)
            {
                TransformGroup transformGroup = (TransformGroup)Canvas.RenderTransform;
                ScaleTransform transform = (ScaleTransform)transformGroup.Children[0];
                double zoom = e.Delta > 0 ? .1 : -.1;
                if ((transform.ScaleX >= .4d || transform.ScaleY >= .4d) && (transform.ScaleX <= 1.8d || transform.ScaleY <= 1.8d))
                {
                    Point mouse = e.GetPosition(Canvas);
                    transform.CenterX = mouse.X;
                    transform.CenterY = mouse.Y;
                    transform.ScaleY = transform.ScaleX += zoom;
                    Console.WriteLine($"Zoom:{transform.ScaleX}");
                    Corect_Size();
                }
                else
                {
                    if (transform.ScaleX < 1.0d)
                        transform.ScaleX = transform.ScaleY = .4d;
                    else
                        transform.ScaleX = transform.ScaleY = 1.8d;
                    Corect_Size();
                    return;
                }
            }
        }
        /// <summary>
        /// Блокирует прокрутку для CanvasViewer.
        /// OpenSpace.xaml
        /// </summary>
        private void CanvasViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }
    }
}
