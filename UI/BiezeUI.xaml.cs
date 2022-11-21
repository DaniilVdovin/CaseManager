using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CaseManager
{
    /// <summary>
    /// Логика взаимодействия для BiezeUI.xaml
    /// </summary>
    public partial class BiezeUI : UserControl
    {
        UIElement start, end;
        Size start_size, end_size;
        double
                 start_left,
                 start_top,
                 end_left,
                 end_top;
        bool _isEdit = false;
        public BiezeUI(UIElement start, UIElement end)
        {
            InitializeComponent();
            this.start = start;
            this.end = end;
            MouseEventHandler action = (s, e) => {
                if (_isEdit)
                    Position();
            }; 
            MouseButtonEventHandler down = (s, e) => {
                _isEdit = true;
            };
            MouseButtonEventHandler up = (s, e) => {
                _isEdit = false;
            };
            start.MouseMove += action;
            end.MouseMove += action;
            start.MouseLeftButtonDown += down;
            end.MouseLeftButtonDown += down;
            start.MouseLeftButtonUp += up;
            end.MouseLeftButtonUp += up;

            start_size = start.DesiredSize;
            end_size = end.DesiredSize;
            start_left = Canvas.GetLeft(start);
            start_top = Canvas.GetTop(start);
            end_left = Canvas.GetLeft(end);
            end_top = Canvas.GetTop(end);
            Position();
        }
        private void Position()
        {
            start_left = Canvas.GetLeft(start);
            start_top = Canvas.GetTop(start);
            end_left = Canvas.GetLeft(end);
            end_top = Canvas.GetTop(end);
            if (end_left > start_left + start_size.Width)
            {
                if(end_top + end_size.Height/2> start_top + start_size.Height / 2)
                {
                    if (start_top + end_size.Height > end_top)
                        RightToLeft();
                    else
                        RightDownToLeft();
                }
                else if (end_top + end_size.Height / 2 < start_top + start_size.Height / 2)
                {
                    if (start_top > end_top)
                        RightUPToLeft();

                    Console.WriteLine("B:RightUPToLeft");
                }
            }
            else if (end_left < start_left)
            {
                if (end_top + end_size.Height > start_top)
                {
                    Console.WriteLine("B:leftDownToRight");
                }
                else
                {
                    Console.WriteLine("B:leftUPToRicht");
                }
            }
        }
        private void RightToLeft()
        {
            Canvas.SetLeft(this, start_left + start_size.Width);
            Canvas.SetTop(this, start_top + start_size.Height / 2);
            u_root.Width = Width = end_left - start_left - start_size.Width;
            u_root.Height = Height =end_top - start_top;
            p_path.Data = Geometry.Parse($"M 7,7 C {(int)Width},0 0,{(int)Height} {(int)Width - 7},{(int)Height - 7}");
            e_start.VerticalAlignment = VerticalAlignment.Top;
            e_end.VerticalAlignment = VerticalAlignment.Bottom;
        }
        private void RightUPToLeft()
        {
            Canvas.SetLeft(this, start_left + start_size.Width);
            Canvas.SetTop(this, end_top + end_size.Height/2);
            u_root.Width = Width = end_left - start_left - start_size.Width;
            u_root.Height = Height = start_top - end_top;
            p_path.Data = Geometry.Parse($"M 7,{(int)Height - 7} C {(int)Width},{(int)Height} 0,0  {(int)Width - 7},7");
            e_start.VerticalAlignment = VerticalAlignment.Bottom;
            e_end.VerticalAlignment = VerticalAlignment.Top;
        }
        private void RightDownToLeft()
        {
            Canvas.SetLeft(this, start_left +start_size.Width/2);
            Canvas.SetTop(this, start_top + start_size.Height);
            u_root.Width = Width = end_left - start_left - start_size.Width/2;
            u_root.Height = Height = end_top - start_top - end_size.Height/2;
            p_path.Data = Geometry.Parse($"M 7,7 C 0,{(int)Height} {(int)(Width)},{(int)(Height)} {(int)Width - 7},{(int)Height - 7}");
            e_start.VerticalAlignment = VerticalAlignment.Top;
            e_end.VerticalAlignment = VerticalAlignment.Bottom;
        }
        private void Resize()
        {

        }
    }
}
