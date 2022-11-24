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
        public UIElement start, end;
        Size start_size, end_size;
        Point start_offset, end_offset;
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
            start_offset = new Point(Canvas.GetLeft(start), Canvas.GetTop(start));
            end_offset = new Point(Canvas.GetLeft(end), Canvas.GetTop(end));

            Position();
        }
        private void Position()
        {
            start_offset = new Point(Canvas.GetLeft(start), Canvas.GetTop(start));
            end_offset = new Point(Canvas.GetLeft(end), Canvas.GetTop(end));

            double P_l = 0, P_t = 0, S_w = 200,S_h=200 ;
            if (start_offset.X + start_size.Width / 2 > end_offset.X + end_size.Width / 2)
            {

                P_l = end_offset.X + end_size.Width;
                //Left
                if (start_offset.X-50 < end_offset.X + end_size.Width)
                {
                    //близко
                    S_w = start_offset.X - end_offset.X - end_size.Width/2;
                }
                else
                {
                    //далеко
                    S_w = start_offset.X - end_offset.X - end_size.Width;
                }
            } else {
                //Right
                if (start_offset.X + start_size.Width + 50 < end_offset.X)
                {
                    //близко
                    P_l = start_offset.X + start_size.Width;
                    S_w = end_offset.X - start_offset.X - start_size.Width;
                }
                else
                {
                    //далеко
                    P_l = start_offset.X + start_size.Width / 2;
                    S_w = end_offset.X - start_offset.X - start_size.Width/2;
                }
                
            }
            if (start_offset.Y + start_size.Height / 2 > end_offset.Y + end_size.Height / 2)
            {
                P_t = end_offset.Y + end_size.Height / 2;
                S_h = start_offset.Y - end_offset.Y;
                if(start_offset.X + start_size.Width / 2 > end_offset.X + end_size.Width / 2)
                {

                    e_start.VerticalAlignment = VerticalAlignment.Top;
                    e_end.VerticalAlignment = VerticalAlignment.Bottom;
                }
                else
                {

                    e_start.VerticalAlignment = VerticalAlignment.Bottom;
                    e_end.VerticalAlignment = VerticalAlignment.Top;
                }
            }
            else
            {
                P_t = start_offset.Y + start_size.Height / 2;
                S_h = end_offset.Y - start_offset.Y;
                if (start_offset.X + start_size.Width / 2 > end_offset.X + end_size.Width / 2)
                {
                    e_start.VerticalAlignment = VerticalAlignment.Bottom;
                    e_end.VerticalAlignment = VerticalAlignment.Top;
                }
                else
                {
                    e_start.VerticalAlignment = VerticalAlignment.Top;
                    e_end.VerticalAlignment = VerticalAlignment.Bottom;
                }
            }

            Canvas.SetLeft(this, P_l);
            Canvas.SetTop(this, P_t);
            u_root.Width = Math.Abs(S_w);
            u_root.Height = Math.Abs(S_h);
            //p_path.Data = Geometry.Parse($"M 7,7 C {(int)Width - (int)Height / 2},0 {(int)Height / 2},{(int)Height} {(int)Width - 7},{(int)Height - 7}");
        }
    }
}
