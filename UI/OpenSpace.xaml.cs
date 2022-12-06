using CaseManager.RecordSystem;
using CaseManager.RecordSystem.RecordModel;
using CaseManager.UI;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace CaseManager
{
    public class OpenSpace_Constrain
    {
        public class ControlPoint
        {
            public Point position;
            public ControlPoint(Point point)
            {
                this.position = point;
            }
            public void SetPosition(Point point)
            {
                this.position = point;
            }
        }
        public UIElement start, end;
        public Line debug_line;
        readonly Ellipse debug_elepse_start;
        readonly Polygon debug_triangle_end;
        readonly RotateTransform rt;
        readonly Canvas canvas;
        private Size start_size, end_size;
        Point start_offset, end_offset;
        readonly List<ControlPoint> start_control_points;
        readonly List<ControlPoint> end_control_points;
        bool _isEdit = false;
        public OpenSpace_Constrain(Canvas canvas, UIElement start, UIElement end)
        {
            this.canvas = canvas;
            this.start = start;
            this.end = end;
            void action(object s, MouseEventArgs e)
            { if (_isEdit) Position(); }
            void down(object s, MouseButtonEventArgs e)
            { _isEdit = true; }
            void up(object s, MouseButtonEventArgs e)
            { _isEdit = false; }
            start.MouseMove += action;
            end.MouseMove += action;
            start.MouseLeftButtonDown += down;
            end.MouseLeftButtonDown += down;
            start.MouseLeftButtonUp += up;
            end.MouseLeftButtonUp += up;
            start.LayoutUpdated += (s, e) => { Update(); };
            end.LayoutUpdated += (s, e) => { Update(); };


            start_offset = new Point(Canvas.GetLeft(start), Canvas.GetTop(start));
            end_offset = new Point(Canvas.GetLeft(end), Canvas.GetTop(end));
            start_control_points = new List<ControlPoint>
            {
                new ControlPoint(start_offset),
                new ControlPoint(start_offset),
                new ControlPoint(start_offset),
                new ControlPoint(start_offset)
            };
            end_control_points = new List<ControlPoint>
            {
                new ControlPoint(end_offset),
                new ControlPoint(end_offset),
                new ControlPoint(end_offset),
                new ControlPoint(end_offset)
            };

            debug_elepse_start = new Ellipse();
            debug_triangle_end = new Polygon();
            debug_triangle_end.Points.Add(new Point(50, 150));
            debug_triangle_end.Points.Add(new Point(150, 50));
            debug_triangle_end.Points.Add(new Point(250, 150));
            debug_triangle_end.IsHitTestVisible = false;
            debug_elepse_start.IsHitTestVisible = false;
            debug_elepse_start.Width = debug_elepse_start.Height = 10;
            debug_elepse_start.Fill = debug_triangle_end.Fill = new SolidColorBrush(Colors.White);
            debug_triangle_end.RenderTransform = rt = new RotateTransform();

            canvas.Children.Add(debug_elepse_start);
            canvas.Children.Add(debug_triangle_end);

            debug_line = new Line
            {
                Stroke = new SolidColorBrush(Colors.White),
                StrokeThickness = 2,
                IsHitTestVisible = false
            };
            canvas.Children.Add(debug_line);

            Update();
        }
        internal void Update()
        {
            start_size = start.RenderSize;
            end_size = end.RenderSize;
        }
        internal void Position() {
            Update();
            start_offset = new Point(Canvas.GetLeft(start), Canvas.GetTop(start));
            end_offset = new Point(Canvas.GetLeft(end), Canvas.GetTop(end));
            start_control_points[2].SetPosition(new Point(start_offset.X + start_size.Width, start_offset.Y + start_size.Height / 2));  //Right
            start_control_points[0].SetPosition(new Point(start_offset.X, start_offset.Y + start_size.Height / 2));                     //left
            start_control_points[1].SetPosition(new Point(start_offset.X + start_size.Width / 2, start_offset.Y + 10));                 //Up
            start_control_points[3].SetPosition(new Point(start_offset.X + start_size.Width / 2, start_offset.Y + start_size.Height));  //down
            end_control_points[2].SetPosition(new Point(end_offset.X + end_size.Width, end_offset.Y + end_size.Height / 2));            //Right
            end_control_points[0].SetPosition(new Point(end_offset.X, end_offset.Y + end_size.Height / 2));                             //Left
            end_control_points[1].SetPosition(new Point(end_offset.X + end_size.Width / 2, end_offset.Y + 10));                         //UP
            end_control_points[3].SetPosition(new Point(end_offset.X + end_size.Width / 2, end_offset.Y + end_size.Height));            //Down
            int m_i = 0, m_j = 0;
            Vector tottal_min = new Vector(10000, 10000);
            for (int i = 0; i < start_control_points.Count; i++)
            {
                if (!(start as IElement).ControlPoints[i]) continue;
                Vector min = new Vector(10000, 10000);
                for (int j = 0; j < end_control_points.Count; j++)
                {
                    if (!(end as IElement).ControlPoints[j]) continue;
                    Vector now = end_control_points[j].position - start_control_points[i].position;
                    if (min.Length > now.Length)
                    {
                        min = now;
                        m_j = j;
                    }
                }
                if (tottal_min.Length > min.Length)
                {
                    tottal_min = min;
                    m_i = i;
                }
            }
            debug_line.X1 = start_control_points[m_i].position.X;
            debug_line.Y1 = start_control_points[m_i].position.Y;
            debug_line.X2 = end_control_points[m_j].position.X;
            debug_line.Y2 = end_control_points[m_j].position.Y;
            debug_triangle_end.Points[0] = new Point(debug_line.X2 - 10, debug_line.Y2 - 10);
            debug_triangle_end.Points[1] = new Point(debug_line.X2 - 10, debug_line.Y2 + 10);
            debug_triangle_end.Points[2] = new Point(debug_line.X2 + 5, debug_line.Y2);
            rt.CenterX = debug_line.X2;
            rt.CenterY = debug_line.Y2;
            rt.Angle = (Math.Atan2(debug_line.Y2 - debug_line.Y1, debug_line.X2 - debug_line.X1) * 180 / Math.PI);
            Canvas.SetLeft(debug_elepse_start, debug_line.X1 - 5);
            Canvas.SetTop(debug_elepse_start, debug_line.Y1 - 5);
        }
        public void Clear()
        {
            canvas.Children.Remove(debug_line);
            canvas.Children.Remove(debug_triangle_end);
            canvas.Children.Remove(debug_elepse_start);
        }
        /*private Point GetIntersection(Point p1, Point p2, Point p3, Point p4)
        {
            Point IntersectionPoint = new Point();
            double a1 = 1e+10, a2 = 1e+10;
            double b1, b2;
            if ((p2.X - p1.X) != 0)
                a1 = (p2.Y - p1.Y) / (p2.X - p1.X);
            if ((p4.X - p3.X) != 0)
                a2 = (p4.Y - p3.Y) / (p4.X - p3.X);
            b1 = p1.Y - a1 * p1.X;
            b2 = p3.Y - a2 * p3.X;
            IntersectionPoint.X = (b2 - b1) / (a1 - a2);
            IntersectionPoint.Y = a2 * IntersectionPoint.X + b2;
            return IntersectionPoint;
        }*/
    }
    public class OpenSpace_Cursor {
        private readonly Line line1, line2;
        private readonly UIElement View;
        public OpenSpace_Cursor(Canvas canvas, UIElement View)
        {
            this.View = View;
            DoubleCollection dashes = new DoubleCollection();
            dashes.Add(6);
            dashes.Add(6);
            line1 = new Line(); line2 = new Line();
            canvas.Children.Add(line1);
            canvas.Children.Add(line2);
            line1.Stroke = line2.Stroke = new SolidColorBrush(Colors.WhiteSmoke);
            line1.StrokeDashArray = line2.StrokeDashArray = dashes;
            line1.StrokeThickness = line2.StrokeThickness = 1d;
        }
        public void SetPosition(Point point)
        {
            line1.X1 = line1.X2 = point.X;
            line1.Y1 = point.Y - View.DesiredSize.Height; line1.Y2 = point.Y + View.DesiredSize.Height;
            line2.Y1 = line2.Y2 = point.Y;
            line2.X1 = point.X - View.DesiredSize.Width; line2.X2 = point.X + View.DesiredSize.Width;
        }
        public void SetVisible(bool visible)
        {
            line1.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
            line2.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }
    }
    public class OpenSpace_Ruler
    {
        private readonly Canvas left, top;
        private Line line_left, line_top;
        private Rectangle view_rect_top, view_rect_left;

        public int OneDecimal = 10;
        public int OneWhole = 50;

        public int Size_OneDecimal = 10;
        public int Size_OneWhole = 15;

        public Point Coefficient;

        private readonly TranslateTransform tt_t = new TranslateTransform(),
                                            tt_l = new TranslateTransform(),
                                            tt_v_l = new TranslateTransform(),
                                            tt_v_t = new TranslateTransform();
        private readonly ScaleTransform st_t = new ScaleTransform(),
                                            st_l = new ScaleTransform(),
                                            st_v_l = new ScaleTransform(),
                                            st_v_t = new ScaleTransform();
        private readonly TransformGroup tg_t = new TransformGroup(),
                                            tg_l = new TransformGroup(),
                                            tg_v_l = new TransformGroup(),
                                            tg_v_t = new TransformGroup();

        private readonly Brush brush;
        public OpenSpace_Ruler(Canvas canvas, Canvas left, Canvas top)
        {
            this.left = left;
            this.top = top;

            left.Height = canvas.Height;
            top.Width = canvas.Width;

            brush = new SolidColorBrush(Colors.Gray);

            tg_t.Children.Add(tt_t);
            tg_t.Children.Add(st_t);

            tg_l.Children.Add(tt_l);
            tg_l.Children.Add(st_l);

            top.RenderTransform = tg_t;
            left.RenderTransform = tg_l;

            Create_Cursor_Line();
            Dimension();
            CreateView();
            SetCoefficient(new Point(10000, 5000), new Point(1283, 725));
            SetVisible(true);
        }
        public void CreateView()
        {
            view_rect_top = new Rectangle();
            view_rect_left = new Rectangle();
            view_rect_top.Stroke = view_rect_left.Stroke = view_rect_top.Fill = view_rect_left.Fill = new SolidColorBrush(Colors.Gray);
            view_rect_top.Opacity = view_rect_left.Opacity = 0.15d;
            view_rect_top.Height = view_rect_left.Width = 30;


            tg_v_t.Children.Add(tt_v_t);
            tg_v_t.Children.Add(st_v_t);

            tg_v_l.Children.Add(tt_v_l);
            tg_v_l.Children.Add(st_v_l);

            view_rect_left.RenderTransform = tg_v_l;
            view_rect_top.RenderTransform = tg_v_t;
            left.Children.Add(view_rect_left);
            top.Children.Add(view_rect_top);
        }
        public void SetCoefficient(Point c, Point v)
        {
            Coefficient = new Point(c.X / v.X, c.Y / v.Y);
            view_rect_top.Width = (v.X / Coefficient.X);
            view_rect_left.Height = (v.Y / Coefficient.Y);
            //Console.WriteLine($"Coefficient:{Coefficient}");
        }
        private void Create_Cursor_Line()
        {
            line_left = new Line(); line_top = new Line();
            left.Children.Add(line_left); top.Children.Add(line_top);
            line_left.Stroke = line_top.Stroke = new SolidColorBrush(Colors.White);
            line_left.StrokeThickness = line_top.StrokeThickness = 2d;
        }
        private void Dimension()
        {
            for (int i = 10; i < top.Width; i++)
            {
                if (i % OneWhole == 0)
                {
                    Line line = new Line()
                    {
                        X1 = i,
                        X2 = i,
                        Y1 = 0,
                        Y2 = Size_OneWhole,
                        Stroke = brush,
                        StrokeThickness = 1d
                    };
                    top.Children.Add(line);
                }
                else
                if (i % OneDecimal == 0) {
                    Line line = new Line() {
                        X1 = i,
                        X2 = i,
                        Y1 = 0,
                        Y2 = Size_OneDecimal,
                        Stroke = brush,
                        StrokeThickness = 1d
                    };
                    top.Children.Add(line);
                }
            }
            for (int i = 10; i < left.Height; i++)
            {
                if (i % OneWhole == 0)
                {
                    Line line = new Line
                    {
                        X1 = 0,
                        X2 = Size_OneWhole,
                        Y1 = i,
                        Y2 = i,
                        Stroke = brush,
                        StrokeThickness = 1d
                    };
                    left.Children.Add(line);
                }
                else
                if (i % OneDecimal == 0)
                {
                    Line line = new Line
                    {
                        X1 = 0,
                        X2 = Size_OneDecimal,
                        Y1 = i,
                        Y2 = i,
                        Stroke = brush,
                        StrokeThickness = 1d
                    };
                    left.Children.Add(line);
                }
            }
        }
        public void SetVisible(bool visible)
        {
            line_left.Visibility = line_top.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
        }
        public void SetOffset(Point point)
        {
            tt_t.X = point.X;
            tt_l.Y = point.Y;

            tt_v_l.Y = -(point.Y + (point.Y / Coefficient.Y));
            tt_v_t.X = -(point.X + (point.X / Coefficient.X));
        }
        public void SetScale(ScaleTransform scaleTransform)
        {
            st_t.CenterX = 0;
            st_t.CenterY = 0;
            st_l.CenterX = 0;
            st_l.CenterY = 0;

            st_t.ScaleX = scaleTransform.ScaleX;
            st_l.ScaleY = scaleTransform.ScaleY;

            st_v_t.ScaleX = scaleTransform.ScaleX;
            st_v_l.ScaleY = scaleTransform.ScaleY;
        }
        public void SetMousePosition(Point point)
        {
            SetVisible(true);
            line_left.X1 = 0; line_left.X2 = left.ActualWidth;
            line_left.Y1 = line_left.Y2 = point.Y;
            line_top.Y1 = 0; line_top.Y2 = top.ActualHeight;
            line_top.X1 = line_top.X2 = point.X;
        }
        public void Change_Dimension()
        {
            left.Children.Clear();
            top.Children.Clear();
            left.Children.Add(line_left); top.Children.Add(line_top);
            left.Children.Add(view_rect_left); top.Children.Add(view_rect_top);
            Dimension();
        }

    }
    public class OpenSpace_Propertis
    {
        public class Property
        {
            public Action<object> ChangesValue;
            public string Name { get; set; }
            public string Type { get; set; }
            public string Category { get; set; }
            private object _value;
            public object Value
            {
                get => _value;
                set
                {
                    if (value != _value)
                    {
                        _value = value;
                        ChangesValue.Invoke(_value);
                    }
                }
            }
            public bool IsEnabled { get; set; }
            public Property(string category, string name, object value, string type, Action<object> action)
            {
                this.Category = category;
                this.ChangesValue = action;
                this.Name = name;
                this.Value = value;
                this.Type = type;
            }
        }
        readonly UIElement propertis;
        readonly DataGrid dataGrid;
        readonly Label nonData;
        public OpenSpace_Propertis(UIElement propertis, DataGrid dataGrid, Label nonData)
        {
            this.propertis = propertis;
            this.dataGrid = dataGrid;
            this.nonData = nonData;
        }
        public void LoadProperty(List<Property> list)
        {
            ClearProperty();
            ICollectionView data =
                            CollectionViewSource.GetDefaultView(list);
            data.GroupDescriptions.Clear();
            data.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
            dataGrid.ItemsSource = data;
            this.dataGrid.Visibility = Visibility.Visible;
        }
        public void ClearProperty()
        {
            this.dataGrid.ItemsSource = null;
            this.dataGrid.Visibility = Visibility.Collapsed;
        }
        public void LoadByUIElement(UIElement element)
        {
            nonData.Visibility = Visibility.Collapsed;
            propertis.Visibility = Visibility.Visible;
            //Console.WriteLine($"Element type:{element.GetType()}\nI_Type:{typeof(IElement)}\nIsAssignableFrom:{typeof(IElement).IsAssignableFrom(element.GetType())}");
            if (typeof(IElement).IsAssignableFrom(element.GetType()))
            {
                var pps = (element as IElement).properties;
                if (pps != null) this.LoadProperty(pps);
                else DontHaveData();
            }
            else DontHaveData();
        }
        public void DontHaveData()
        {
            nonData.Visibility = Visibility.Visible;
            ClearProperty();
        }
        public void Close()
        {
            propertis.Visibility = Visibility.Collapsed;
        }
    }
    public class OpenSpace_Constrain_Manager {
        public List<OpenSpace_Constrain> _constrais;

        public UIElement current_strat, current_end;
        bool _isWaitEnd = false;
        private readonly Canvas canvas;

        public OpenSpace_Constrain_Manager(Canvas canvas)
        {
            this.canvas = canvas;
            Constrains = new List<OpenSpace_Constrain>();
        }

        public List<OpenSpace_Constrain> Constrains { get => _constrais; set
            {
                if (value != _constrais)
                {
                    _constrais = value;
                    UpdateList();
                }
            }
        }
        internal void SetStart(UIElement uIElement)
        {
            current_strat = uIElement;
            _isWaitEnd = true;
        }
        internal void SetEnd(UIElement uIElement)
        {
            if (uIElement == null) return;
            if (_isWaitEnd)
                if (current_strat != null)
                {
                    current_end = uIElement;
                    Add_New(current_strat, current_end);
                    Current_Clear();
                }
                else Current_Clear();
        }
        public OpenSpace_Constrain Add_New(UIElement start, UIElement end)
        {
            OpenSpace_Constrain bieze = new OpenSpace_Constrain(canvas, start, end);
            Constrains.Add(bieze);
            bieze.Position();
            return bieze;
        }
        internal void UpdateList()
        {
            foreach (OpenSpace_Constrain item in Constrains)
            {
                item.Update();
                item.Position();
            }
        }
        internal void Current_Clear()
        {
            current_strat = current_end = null;
            _isWaitEnd = false;
        }
        internal void Remove(UIElement element)
        {
            List<OpenSpace_Constrain> _onDelete = new List<OpenSpace_Constrain>();
            foreach (OpenSpace_Constrain b in _constrais)
            {
                if (b.start == element || b.end == element)
                {
                    b.Clear();
                    _onDelete.Add(b);
                }
            }
            foreach (OpenSpace_Constrain b in _onDelete)
            {
                _constrais.Remove(b);
            }
            _onDelete.Clear();
        }

        internal void Clear()
        {
            foreach (OpenSpace_Constrain item in Constrains)
            {
                item.Clear();
            }
            Constrains.Clear();
        }
    }
    public class OpenSpace_Grid : Canvas
    {
        private readonly Pen brush2;
        private readonly Pen bg;
        private readonly SolidColorBrush solidColorBrush2;
        private readonly SolidColorBrush bgbrush;
        public OpenSpace_Grid()
        {
            solidColorBrush2 = new SolidColorBrush(Colors.Gray);
            bgbrush = new SolidColorBrush(Colors.Transparent);
            solidColorBrush2.Opacity = 0.3d;
            solidColorBrush2.Freeze();
            brush2 = new Pen(solidColorBrush2, 0.5d);
            bg = new Pen(bgbrush, 0);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            for (int i = 50; i < this.Height; i += 50)
            {
                drawingContext.DrawLine(brush2, new Point(0, i), new Point(this.Width, i));
                drawingContext.DrawLine(brush2, new Point(i, 0), new Point(i, this.Height));
            }
            drawingContext.DrawRectangle(bgbrush, bg, new Rect(this.DesiredSize));
        }
    }
    public class OpenSpace_Focus
    {
        private UIElement _curent_Focus;
        public UIElement Curent_Focus
        {
            get => _curent_Focus; set
            {
                if (_curent_Focus != value)
                {
                    _curent_Focus = value;
                    ChangeFocusUI();
                }
            }
        }

        readonly OpenSpace openSpace;
        readonly Rectangle rectangle;
        readonly TranslateTransform tt;
        public OpenSpace_Focus(OpenSpace openSpace)
        {
            this.openSpace = openSpace;
            rectangle = new Rectangle
            {
                Visibility = Visibility.Collapsed,
                Fill = new SolidColorBrush(Colors.Transparent),
                IsHitTestVisible = false,
                StrokeThickness = 3,
                RadiusX = 10,
                RadiusY = 10
            };
            tt = new TranslateTransform();
            rectangle.RenderTransform = tt;
            ChangeColor(Colors.OrangeRed);
            openSpace.Canvas.Children.Add(rectangle);
            Panel.SetZIndex(rectangle, 3);
        }
        public void SetFocus(UIElement uI)
        {
            Curent_Focus = uI;
            //openSpace.canvas_Object_Manager.Select(uI);
        }
        public void ChangeColor(Color color)
        {
            rectangle.Effect = new DropShadowEffect
            {
                Color = color,
                Direction = 270,
                ShadowDepth = 0,
                Opacity = 1,
                BlurRadius = 20
            };
            rectangle.Stroke = new SolidColorBrush(color);
        }
        public void MoveToFocus()
        {
            Size view = openSpace.CanvasViewer.DesiredSize;
            var tt = (TranslateTransform)((TransformGroup)openSpace.Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
            tt.X = -(Canvas.GetLeft(Curent_Focus) - view.Width / 2 + Curent_Focus.DesiredSize.Width / 2);
            tt.Y = -(Canvas.GetTop(Curent_Focus) - view.Height / 2 + Curent_Focus.DesiredSize.Height / 2);
            openSpace.Corect_Size();
        }
        public void CleadFocus()
        {
            if (Curent_Focus == null) return;
            Curent_Focus.MouseMove -= UI_MouseMove;
            Curent_Focus.LayoutUpdated -= Curent_Focus_LayoutUpdated;
            rectangle.Visibility = Visibility.Collapsed;
            Curent_Focus = null;
        }
        private void ChangeFocusSize()
        {
            if (Curent_Focus != null)
            {
                rectangle.Width = Curent_Focus.DesiredSize.Width;
                rectangle.Height = Curent_Focus.DesiredSize.Height - 10;
            }
        }
        private void ChangeFocusUI()
        {
            if (Curent_Focus != null)
            {
                Curent_Focus.MouseMove += UI_MouseMove;
                Curent_Focus.LayoutUpdated += Curent_Focus_LayoutUpdated;
                rectangle.Visibility = Visibility.Visible;
                ChangeFocusSize();
                tt.X = Canvas.GetLeft(Curent_Focus);
                tt.Y = Canvas.GetTop(Curent_Focus) + 10;
                //Console.WriteLine("BIND FOCUS");
            }
        }

        private void Curent_Focus_LayoutUpdated(object sender, EventArgs e)
        {
            ChangeFocusSize();
        }
        private void UI_MouseMove(object sender, MouseEventArgs e)
        {
            if (Curent_Focus != null)
            {
                tt.X = Canvas.GetLeft(Curent_Focus);
                tt.Y = Canvas.GetTop(Curent_Focus) + 10;
            }
        }
    }
    public class OpenSpace_Object_Manager
    {
        public class ObjectItem
        {

            public ListBoxItem List_Item { get; set; }
            public UIElement UI_Item { get; set; }
            public UIElement UI_Item_parent { get; set; }
            public ObjectItem(int i, UIElement uI_Item, RoutedEventHandler action, UIElement UI_Item_parent = null)
            {
                this.UI_Item_parent = UI_Item_parent;
                UI_Item = uI_Item;
                string child = this.UI_Item_parent != null ? ">" : "";
                List_Item = new ListBoxItem
                {
                    Content = $"{child} {uI_Item.GetType().Name} ({i})"
                    , Opacity = this.UI_Item_parent != null ? .5 : 1
                };
                List_Item.Selected += action;
            }
            public override bool Equals(object obj)
            {
                if (obj is ListBoxItem)
                    return List_Item == obj as ListBoxItem;
                if (obj is UIElement)
                    return UI_Item == obj as UIElement;
                if (obj is ObjectItem)
                    return this == obj as ObjectItem;
                return false;
            }

            public override int GetHashCode()
            {
                int hashCode = 1792996892;
                hashCode = hashCode * -1521134295 + EqualityComparer<ListBoxItem>.Default.GetHashCode(List_Item);
                hashCode = hashCode * -1521134295 + EqualityComparer<UIElement>.Default.GetHashCode(UI_Item);
                return hashCode;
            }
        }
        private int index = 1;
        public List<ObjectItem> ObjectItems;
        private readonly Canvas canvas;
        private readonly ListBox listBox;
        private readonly OpenSpace openSpace;
        public OpenSpace_Object_Manager(OpenSpace openSpace, Canvas canvas, ListBox listBox)
        {
            this.openSpace = openSpace;
            this.listBox = listBox;
            this.canvas = canvas;
            ObjectItems = new List<ObjectItem>();
            listBox.Items.Clear();
        }
        internal void Add(UIElement element, UIElement parent = null, bool can_move = true)
        {
            ObjectItem item = new ObjectItem(index++, element, List_Item_Selected, parent);
            if (can_move)
            {
                element.MouseLeftButtonDown += openSpace.Adding_MouseLeftButtonDown;
                element.MouseLeftButtonUp += openSpace.Adding_MouseLeftButtonUp;
                element.MouseEnter += openSpace.Adding_MouseEnter;
                element.MouseLeave += openSpace.Adding_MouseLeave;
                element.MouseMove += openSpace.Adding_MouseMove;
            }
            ObjectItems.Add(item);
            canvas.Children.Add(item.UI_Item);
            listBox.Items.Add(item.List_Item);
        }
        private void List_Item_Selected(object sender, RoutedEventArgs e)
        {
            ObjectItem oi = FindItemByListBoxItem(sender as ListBoxItem);
            if (oi != null)
            {
                openSpace.canvas_Focus.SetFocus(oi.UI_Item);
                openSpace.canvas_Propertis.LoadByUIElement(oi.UI_Item);
                openSpace.canvas_Focus.MoveToFocus();
            }
        }
        ObjectItem FindItemByUIelement(UIElement element)
        {
            foreach (ObjectItem item in ObjectItems)
                if (item.Equals(element))
                    return item;
            return null;
        }
        ObjectItem FindItemByListBoxItem(ListBoxItem element)
        {
            foreach (ObjectItem item in ObjectItems)
                if (item.Equals(element))
                    return item;
            return null;
        }
        internal void Remove(UIElement element)
        {
            ObjectItem item = FindItemByUIelement(element);
            if (item == null) return;
            ObjectItems.Remove(item);
            listBox.Items.Remove(item.List_Item);
            if (typeof(IElement).IsAssignableFrom(item.UI_Item.GetType()))
                (item.UI_Item as IElement).Clear();
            canvas.Children.Remove(item.UI_Item);
        }
        internal void Select(UIElement uI)
        {
            listBox.SelectedItem = FindItemByUIelement(uI).List_Item;
        }
        internal void Clear()
        {
            foreach (ObjectItem item in ObjectItems)
            {
                listBox.Items.Remove(item.List_Item);
                canvas.Children.Remove(item.UI_Item);
            }
            ObjectItems.Clear();
            index = 1;
        }
    }
    public static class OpenSpace_Render
    {
        public static void SaveControlImage(
            Visual baseElement, Point point1, Point point2, string pathToOutputFile)
        {
            // 1) get current dpi
            var pSource = PresentationSource.FromVisual(Application.Current.MainWindow);
            Matrix m = pSource.CompositionTarget.TransformToDevice;
            int coef = 1;
            int scale = 96;
            double dpiX = m.M11 * scale;
            double dpiY = m.M22 * scale;
            int o_main_width = (int)((point2.X * coef));
            int o_main_height = (int)((point2.Y * coef));
            int main_width = (int)((baseElement as UIElement).RenderSize.Width);
            int main_height = (int)((baseElement as UIElement).RenderSize.Height);
            // 2) create RenderTargetBitmap
            var elementBitmap = new RenderTargetBitmap(
                o_main_width * coef,
                o_main_height * coef,
                dpiX, dpiY, PixelFormats.Default);
            Console.WriteLine($"RenderTargetBitmap ({main_width},{main_height})");
            // 3) undo element transformation
            var drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                var visualBrush = new VisualBrush(baseElement);
                drawingContext.DrawRectangle(
                new SolidColorBrush(Colors.DarkGray),
                null,
                new Rect(new Point(0,0),new Point(main_width* coef, main_height* coef)));
                drawingContext.DrawRectangle(
                    visualBrush,
                    null,
                    new Rect(new Point(0, 0), new Point(main_width* coef, main_height* coef)));
            }
           
            drawingVisual.Clip = new RectangleGeometry(new Rect((int)((point1.X * coef)), (int)((point1.Y * coef)),
            (int)((point2.X * coef)), (int)((point2.Y * coef))));
            drawingVisual.Offset = -new Vector((int)((point1.X * coef)), (int)((point1.Y * coef)));
            // 4) draw element
            elementBitmap.Render(drawingVisual);
            //Console.WriteLine($"elementBitmap ({elementBitmap.PixelWidth},{elementBitmap.PixelHeight})");
            //var int_c_rect = new Int32Rect((int)((point1.X* coef)), (int)((point1.Y* coef)),
            //    (int)((point2.X* coef)), (int)((point2.Y* coef)));
            //Console.WriteLine($"int_c_rect " + int_c_rect.ToString());
            // var crop = new CroppedBitmap(elementBitmap, int_c_rect);
            // 5) create PNG image
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(elementBitmap));

            // 6) save image to file
            using (var imageFile = new FileStream(pathToOutputFile, FileMode.Create, FileAccess.Write))
            {
                encoder.Save(imageFile);
                imageFile.Flush();
                imageFile.Close();
                IOCore.main.notifManager.Add(0, "Рендер");
            }
        }
    }
    public class OpenSpace_CopyPaste{

        private readonly OpenSpace os;
        public UIElement obj;
        public OpenSpace_CopyPaste(OpenSpace openSpace)
        {
            os = openSpace;
        }
        public void Copy(UIElement element)
        {
            if (element != null)
            {
                obj = element;
                os.canvas_Focus.ChangeColor(Colors.Blue);
                os.canvas_Focus.SetFocus(obj);
            }
        }
        public void Paste()
        {
            if (obj != null)
            {
                object result = Activator.CreateInstance(obj.GetType());
                if (typeof(IElement).IsAssignableFrom(obj.GetType()))
                {
                    for (int i = 0; i < (obj as IElement).properties.Count; i++)
                    {
                        (result as IElement).properties[i].Value = (obj as IElement).properties[i].Value;
                    }
                    (result as IElement).CanDelite = (obj as IElement).CanDelite;
                }
                os.Add_Element(result as UIElement);
                os.canvas_Focus.ChangeColor(Colors.OrangeRed);
            }
        }
    }
    public partial class OpenSpace : UserControl
    {
        private Point origin;
        private Point canvas_origin;
        private Point start;
        private Point isAdding_Move_start;
        private double zoom_scale = 1.1d;
        private bool _isAdding = false;
        private bool _isAdding_Move = false;
        private bool _isAdding_Hover = false;
        private bool _isAdding_Add_Constrain = false;
        private UIElement Adding = null;
        public  OpenSpace_Cursor canvas_Cursor;
        public  OpenSpace_Ruler canvas_Ruler;
        public  OpenSpace_Propertis canvas_Propertis;
        public  OpenSpace_Constrain_Manager constrain_Manager;
        public  OpenSpace_Focus canvas_Focus;
        public  OpenSpace_Object_Manager canvas_Object_Manager;
        public  OpenSpace_CopyPaste CopyPaste;
        private Line Constrain_Line;
        public  OpenSpace()
        {
            InitializeComponent();
            TransformGroup group = new TransformGroup();
            ScaleTransform xform = new ScaleTransform();
            group.Children.Add(xform);
            TranslateTransform tt = new TranslateTransform
            {
                X = -(Canvas.Width / 2),
                Y = -(Canvas.Height / 2)
            };
            group.Children.Add(tt); 
            Canvas.RenderTransform = group;
        }
        public  void Add_Element(UIElement uIElement)
        {
            if (!_isAdding)
            {
                Adding = uIElement;
                Adding.IsEnabled= false;
                canvas_Object_Manager.Add(uIElement);
                _isAdding = true;
            }else
                IOCore.main.notifManager.Add(2, "Добавить элемент сейчас невозможно");
        }
        internal void Adding_MouseLeave(object sender, MouseEventArgs e)
        {
            //canvas_Cursor.SetVisible(true);
            _isAdding_Hover = false;
            _isAdding_Move = false;
            //Console.WriteLine($"{sender.GetType().Name}:LEAVE");
        }
        internal void Adding_MouseEnter(object sender, MouseEventArgs e)
        {
            //canvas_Cursor.SetVisible(false);
            _isAdding_Hover = true;
            //Console.WriteLine($"{sender.GetType().Name}:ENTER");
        }
        internal void Adding_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isAdding_Add_Constrain)
            {
                if (constrain_Manager.current_strat != sender as UIElement)
                {
                    constrain_Manager.SetEnd(sender as UIElement);
                    //Console.WriteLine($"SetEnd({sender.GetType().Name})");
                    _isAdding_Add_Constrain = false;
                }
                else constrain_Manager.Current_Clear();
                Canvas.Children.Remove(Constrain_Line);
                Constrain_Line = null;
            }
            else
            {
                _isAdding_Move = false;
                Panel.SetZIndex(sender as UIElement, 1);
            }
            Mouse.OverrideCursor = Cursors.Cross;
        }
        internal void Adding_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isAdding_Move)
            {
                Point m_canvas = e.GetPosition(Canvas);
                //Point m_obj = e.GetPosition(sender as UIElement);
                //Console.WriteLine($"sender:{sender.GetType().Name}\nm_canvas:{m_canvas}\nm_obj:{m_obj}");
                Adding_Move(false, sender as UIElement, m_canvas, e.GetPosition(CanvasViewer));
            }
        }
        internal void Adding_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isAdding_Add_Constrain)
            {
                //Console.WriteLine($"SetStart({sender.GetType().Name})");
                constrain_Manager.SetStart(sender as UIElement);
                Constrain_Line = new Line
                {
                    Stroke = new SolidColorBrush(Colors.White),
                    StrokeThickness = 1,
                    IsHitTestVisible = false
                };
                Constrain_Line.X2 = Constrain_Line.X1 = (System.Windows.Controls.Canvas.GetLeft(sender as UIElement) + (sender as UIElement).DesiredSize.Width/2);
                Constrain_Line.Y2 = Constrain_Line.Y1 = (System.Windows.Controls.Canvas.GetTop(sender as UIElement) + (sender as UIElement).DesiredSize.Height/2);
                Canvas.Children.Add(Constrain_Line);
                Mouse.OverrideCursor = Cursors.Pen;
            }
            else
            {
                _isAdding_Move = true;

                Mouse.OverrideCursor = Cursors.Hand;
                isAdding_Move_start = e.GetPosition(sender as UIElement);
                canvas_Propertis.LoadByUIElement(sender as UIElement);
            }
            canvas_Focus.SetFocus(sender as UIElement);
        }
        private void UserControl_Initialized(object sender, EventArgs e)
        {

            this.AllowDrop = true;

            Canvas.MouseWheel           += Canvas_MouseWheel;
            Canvas.MouseLeftButtonDown  += Canvas_MouseLeftButtonDown;
            Canvas.MouseLeftButtonUp    += Canvas_MouseLeftButtonUp;
            Canvas.MouseMove            += Canvas_MouseMove;
            Canvas.MouseLeave           += Canvas_MouseLeave;
            Canvas.MouseEnter           += Canvas_MouseEnter;

            CanvasViewer.KeyDown        += CanvasViewer_KeyDown;
            this.Drop                   += CanvasViewer_Drop;
            
            //canvas_Cursor = new OpenSpace_Cursor(Canvas,CanvasViewer);
            canvas_Ruler = new OpenSpace_Ruler(Canvas,left_tape,top_tape);
            canvas_Propertis = new OpenSpace_Propertis(PropertisBar,propertisGrid,propertisGrid_nonData);
            constrain_Manager = new OpenSpace_Constrain_Manager(Canvas);
            canvas_Focus = new OpenSpace_Focus(this);
            canvas_Object_Manager = new OpenSpace_Object_Manager(this,Canvas,object_manager_list);
            CopyPaste = new OpenSpace_CopyPaste(this);

            PropertisBar_close.MouseLeftButtonDown += (s, b) => { canvas_Propertis.Close(); };
            bt_line_add.MouseLeftButtonDown += (s, b) => Add_Constrain();
            
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Cross;
        }

        private void CanvasViewer_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string file in files)
                {
                    if (file.EndsWith(".cmp"))
                    {
                        IOCore.Load(file, () => { });
                        return;
                    }
                    if (file.EndsWith(".png") || file.EndsWith(".jpg") || file.EndsWith(".jpeg"))
                    {
                        ImageHolderUI holderUI = new ImageHolderUI();
                        holderUI.properties[0].Value = file;
                        canvas_Object_Manager.Add(holderUI);
                        Point p = e.GetPosition(Canvas);
                        System.Windows.Controls.Canvas.SetLeft(holderUI, (p.X - 200));
                        System.Windows.Controls.Canvas.SetTop(holderUI, p.Y-200);
                        IOCore.main.notifManager.Add(1, "Фото добавленно");
                    }
                }
            }
        }
        public  void Add_Constrain()
        {
            _isAdding_Add_Constrain = true;
            Mouse.OverrideCursor = Cursors.Pen;
            //Console.WriteLine("_isAdding_Add_Constrain ACTIVATE");
        }
        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            //canvas_Cursor.SetVisible(false);
            Mouse.OverrideCursor = Cursors.Arrow;
        }
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isAdding_Hover) return;
            if (_isAdding_Move) return;
            if (_isAdding_Add_Constrain)
            {
                constrain_Manager.SetEnd(null);
                //Console.WriteLine($"SetEnd(LOST)");
                _isAdding_Add_Constrain = false;
                constrain_Manager.Current_Clear();
                Canvas.Children.Remove(Constrain_Line);
                Constrain_Line = null;
            }
            if (sender is Canvas)
                Canvas.ReleaseMouseCapture();
            Mouse.OverrideCursor = Cursors.Cross;
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }
        public  void Corect_Size()
        {
            var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
            if (tt.X > 0) tt.X = 0;
            else if (tt.X < -(Canvas.ActualWidth - CanvasViewer.ActualWidth)) tt.X = -(Canvas.ActualWidth - CanvasViewer.ActualWidth);
            if (tt.Y > 0) tt.Y = 0;
            else if (tt.Y < -(Canvas.ActualHeight - CanvasViewer.ActualHeight)) tt.Y = -(Canvas.ActualHeight - CanvasViewer.ActualHeight);
            canvas_Ruler.SetOffset(new Point(tt.X, tt.Y));
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isAdding_Hover) return;
            if (_isAdding_Move) return;
            if (_isAdding_Add_Constrain)
            {
                if (Constrain_Line != null)
                {
                    Point point = e.GetPosition(Canvas);
                    Constrain_Line.X2 = point.X;
                    Constrain_Line.Y2 = point.Y;
                    return;
                }
            }
            if (!_isAdding)
            {
                if (sender is Canvas)
                {
                    Point point = e.GetPosition(Canvas);
                    //canvas_Cursor.SetPosition(point);
                    canvas_Ruler.SetMousePosition(point);
                    if (!Canvas.IsMouseCaptured) return;
                    var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
                    Vector v = start - e.GetPosition(CanvasViewer);
                    tt.X = (origin.X - v.X);
                    tt.Y = (origin.Y - v.Y);
                    //Console.WriteLine($"MouseMove\nVector:{v.X},{v.Y}\nStart:{start.ToString()}\nOrigin:{origin.ToString()}\ntt:{tt.X},{tt.Y}");
                    Corect_Size();
                }
            }
            else
            {
                Adding_Move(true,Adding,e.GetPosition(Canvas),e.GetPosition(CanvasViewer));
            }
        }
        internal void Adding_Move(bool first,UIElement obj,Point point,Point point_view)
        {
            //canvas_Cursor.SetVisible(false);
            canvas_Ruler.SetMousePosition(point);
            canvas_Ruler.SetVisible(true);
            if (!first)
            {
                var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
                //Console.WriteLine($"point_view:{point_view}\npoint:{point}");
                //Console.WriteLine($"tt.XCC:{point_view.X - (obj.DesiredSize.Width / 2)}");
                if (point_view.X - 100 < 10)
                    tt.X += 5;
                if (point_view.Y - 100 < 10)
                    tt.Y += 5;
                if (point_view.X + 100 > CanvasViewer.ActualWidth - 10)
                    tt.X -= 5;
                if (point_view.Y + 100 > CanvasViewer.ActualHeight - 10)
                    tt.Y -= 5;
                
                Corect_Size();
                origin = new Point(tt.X, tt.Y);
                canvas_Ruler.SetOffset(origin);
            }
            else { isAdding_Move_start = new Point((obj.DesiredSize.Width / 2), (obj.DesiredSize.Height / 2)); }
            
            if (point.X - isAdding_Move_start.X > 0 && point.X - isAdding_Move_start.X < Canvas.ActualWidth)
                System.Windows.Controls.Canvas.SetLeft(obj, point.X - isAdding_Move_start.X);
            if (point.Y - isAdding_Move_start.Y > 0 && point.Y - isAdding_Move_start.Y < Canvas.ActualHeight)
                System.Windows.Controls.Canvas.SetTop(obj, point.Y - isAdding_Move_start.Y);

            Panel.SetZIndex(obj, 2);
        }
        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isAdding_Hover) return;
            if (_isAdding_Move) return;
            if (!_isAdding)
            {
                if (sender is Canvas)
                {
                    canvas_origin = e.GetPosition(Canvas);
                    Canvas.CaptureMouse();
                    var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
                    start = e.GetPosition(CanvasViewer);
                    tt.X = (start.X - canvas_origin.X);
                    tt.Y = (start.Y - canvas_origin.Y);
                    origin = new Point(tt.X, tt.Y);
                    Corect_Size();
                    Mouse.OverrideCursor = Cursors.Hand;
                }
            }
            else
            {
                _isAdding = false;
                Adding.IsEnabled = true;
                Adding = null;
            }
        }
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            return;
            if (_isAdding_Move) return;
            if (sender is Canvas)
            {
                var _scaleTransform = (ScaleTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is ScaleTransform);
                var _translateTransform = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
                var position = e.GetPosition(Canvas);

                _scaleTransform.CenterX = 0;
                _scaleTransform.CenterY = 0;
                
                if (e.Delta > 0)
                {
                    if (_scaleTransform.ScaleX < 1.5d)
                    {
                        _scaleTransform.ScaleX += .1;
                        _scaleTransform.ScaleY += .1;
                    }
                }
                if (e.Delta < 0)
                {
                    if (_scaleTransform.ScaleX > .7)
                    {
                        _scaleTransform.ScaleX-=.1;
                        _scaleTransform.ScaleY-=.1;
                    }
                }
                //_translateTransform.X += (_scaleTransform.CenterX - _OldPositionX) * (_scaleTransform.ScaleX - 1);
                //_translateTransform.Y += (_scaleTransform.CenterY - _OldPositionY) * (_scaleTransform.ScaleY - 1);
                zoom_scale = _scaleTransform.ScaleX;
                Corect_Size();
                canvas_Ruler.SetScale(_scaleTransform);
                t_zoom_p.Content = $"{(int)(zoom_scale * 100)}%";
            }
        }
        private void CanvasViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Corect_Size();
            canvas_Ruler.SetCoefficient(new Point(Canvas.ActualWidth, Canvas.ActualHeight), new Point(CanvasViewer.ActualWidth, CanvasViewer.ActualHeight));
            var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
            canvas_Ruler.SetOffset(new Point(tt.X, tt.Y));
        }
        private void Property_Action(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*",
            };
            openFileDialog.FileOk += (s, o) =>
            {
                if (openFileDialog.FileName != "")
                {
                    TextBox tb = (((sender as Button).Parent as StackPanel).Children[0] as TextBox);
                    tb.BeginChange();
                    tb.Text = openFileDialog.FileName;
                    tb.AcceptsReturn = true;
                    tb.EndChange();
                    tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
                }
            };
            openFileDialog.ShowDialog();
        }
        private void DeleteElement(UIElement element)
        {
            if (element != null)
            {
                if (typeof(IElement).IsAssignableFrom(element.GetType()))
                {
                    if ((element as IElement).CanDelite)
                    {
                        Delete(element);
                    }
                    else
                        IOCore.main.notifManager.Add(2, "Объект нельзя удалять");
                }
                else
                {
                    Delete(element);
                }
            }
            else
                IOCore.main.notifManager.Add(2, "Нет элемента в фокусе");
        }
        internal void Delete(UIElement element)
        {
            if (canvas_Focus.Curent_Focus != null)
                canvas_Focus.CleadFocus();
            constrain_Manager.Remove(element);
            canvas_Propertis.ClearProperty();
            canvas_Object_Manager.Remove(element);
        }
        public void ClearAll()
        {
            canvas_Focus.CleadFocus();
            canvas_Propertis.ClearProperty();
            canvas_Object_Manager.Clear();
            constrain_Manager.Clear();
            this.UpdateLayout();
        }
        public void ScreenOfItemsAll()
        {
            double mX = 0, mY = 0;
            double lX = 5000, lY = 5000;
            foreach (var item in canvas_Object_Manager.ObjectItems)
            {
                double l = System.Windows.Controls.Canvas.GetLeft(item.UI_Item);
                double t = System.Windows.Controls.Canvas.GetTop(item.UI_Item);
                if (l < lX)
                    lX = l;
                if (t < lY)
                    lY = t;
                if (l+item.UI_Item.DesiredSize.Width > mX)
                    mX = l + item.UI_Item.DesiredSize.Width;
                if (t + item.UI_Item.DesiredSize.Height > mY)
                    mY = t+item.UI_Item.DesiredSize.Height;
            }
            double 
                left = lX - 50,
                top = lY - 50,
                width = mX - lX + 100,
                height = mY - lY + 100;
            Screen(new Point(left, top), new Point(width, height));
        }
        public void ScreenOfCanvas() => Screen();
        internal void Screen() => Screen(new Point(0,0), new Point((int)Canvas.RenderSize.Width, (int)Canvas.RenderSize.Height));
        internal void Screen(Point point1,Point point2)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog()
            {
                FileName = IOCore.CurrentProjectName + "_Render",
                Filter = "ImageRender (*.png)|*.png",
                DefaultExt = ".png"
            };
            openFileDialog.FileOk += (b, i) =>
            {
                if (openFileDialog.FileName != "")
                {
                    OpenSpace_Render.SaveControlImage(Canvas, point1,point2, openFileDialog.FileName);
                    Console.WriteLine("Render");
                }
            };
            openFileDialog.ShowDialog();
        }
        private void CanvasViewer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    DeleteElement(canvas_Focus.Curent_Focus);
                    break;
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                CopyPaste.Copy(canvas_Focus.Curent_Focus);
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            {
                CopyPaste.Paste();
            }
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.R)
            {
                ScreenOfItemsAll();
            }
        }
        public void LoadFromFile(Record record)
        {
            ClearAll();
            foreach (ElementRecord item in record.elements)
            {
                if (typeof(IElement).IsAssignableFrom(Type.GetType(item.Type)))
                {
                    object obj = Activator.CreateInstance(Type.GetType(item.Type));
                    System.Windows.Controls.Canvas.SetLeft((obj as UIElement), item.Point.X);
                    System.Windows.Controls.Canvas.SetTop((obj as UIElement), item.Point.Y);
                    foreach (PropertyRecord pr in item.Property)
                    {
                        (obj as IElement).properties[pr.Index].Value = pr.Value;
                    }
                    canvas_Object_Manager.Add((obj as UIElement));
                }
            }
            foreach (ConstrainRecord constrain in record.constrains)
            {
                constrain_Manager.Add_New(canvas_Object_Manager.ObjectItems[constrain.StartIndex].UI_Item,
                    canvas_Object_Manager.ObjectItems[constrain.EndIndex].UI_Item);
            }
            this.UpdateLayout();
            constrain_Manager.UpdateList();
        }
    }
}
