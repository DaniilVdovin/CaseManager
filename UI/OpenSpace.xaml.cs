using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using static CaseManager.Canvas_Object_Manager;

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
            //text.Text = $"[{point.X},{point.Y}]";
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
    public class Canvas_Ruler
    {
        Canvas canvas, left, top;
        Control view;
        Line line_left, line_top;
        Rectangle view_rect_top, view_rect_left;

        public int OneDecimal = 10;
        public int OneWhole = 50;

        public int Size_OneDecimal = 10;
        public int Size_OneWhole = 15;

        public Point Coefficient;

        TranslateTransform tt_t, tt_l;

        Brush brush;
        public Canvas_Ruler(Canvas canvas, Canvas left, Canvas top, Control view)
        {
            this.canvas = canvas;
            this.left = left;
            this.top = top;
            this.view = view;

            left.Height = canvas.Height;
            top.Width = canvas.Width;

            brush = new SolidColorBrush(Colors.Gray);
            tt_t = new TranslateTransform();
            tt_l = new TranslateTransform();
            
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
            left.Children.Add(view_rect_left);
            top.Children.Add(view_rect_top);
        }
        public void SetCoefficient(Point c,Point v)
        {
            Coefficient = new Point(c.X / v.X, c.Y /v.Y);
            view_rect_top.Width = (v.X/Coefficient.X);
            view_rect_left.Height = (v.Y/Coefficient.Y);
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
                    Line line = new Line();
                    line.X1 = line.X2 = i;
                    line.Y1 = 0; line.Y2 = Size_OneWhole;
                    line.Stroke = brush;
                    line.StrokeThickness = 1d;
                    top.Children.Add(line);
                }
                else 
                if (i % OneDecimal == 0) {
                    Line line = new Line();
                    line.X1 = line.X2 = i;
                    line.Y1 = 0; line.Y2 = Size_OneDecimal;
                    line.Stroke = brush;
                    line.StrokeThickness = 1d;
                    top.Children.Add(line);
                }
                
            }
            for (int i = 10; i < left.Height; i++)
            {
                if (i % OneWhole == 0)
                {
                    Line line = new Line();
                    line.X1 = 0; line.X2 = Size_OneWhole;
                    line.Y1 = line.Y2 = i;
                    line.Stroke = brush;
                    line.StrokeThickness = 1d;
                    left.Children.Add(line);
                }
                else
                if (i % OneDecimal == 0)
                {
                    Line line = new Line();
                    line.X1 = 0; line.X2 = Size_OneDecimal;
                    line.Y1 = line.Y2 = i;
                    line.Stroke = brush;
                    line.StrokeThickness = 1d;
                    left.Children.Add(line);
                }
            }
        }
        public void SetVisible(bool visible)
        {
            line_left.Visibility = line_top.Visibility =  visible ? Visibility.Visible : Visibility.Hidden;
        }
        public void SetOffset(Point point)
        {
            tt_t.X = point.X;
            tt_l.Y = point.Y; 
            top.RenderTransform = tt_t;
            left.RenderTransform = tt_l;

            TranslateTransform tt_v_l = new TranslateTransform();
            tt_v_l.Y = -(point.Y + (point.Y/Coefficient.Y));
            view_rect_left.RenderTransform = tt_v_l;
            TranslateTransform tt_v_t = new TranslateTransform();
            tt_v_t.X = -(point.X + (point.X/Coefficient.X));
            view_rect_top.RenderTransform = tt_v_t;
        }
        public void SetScale(double point)
        {
            
        }
        public void SetMousePosition(Point point)
        {
            SetVisible(true);
            line_left.X1 = 0; line_left.X2 = left.ActualWidth;
            line_left.Y1 = line_left.Y2 = point.Y;
            line_top.Y1 = 0; line_top.Y2 = top.ActualHeight;
            line_top.X1 = line_top.X2 = point.X;
        }
        public void Change_Dimension(Point tt)
        {
            left.Children.Clear();
            top.Children.Clear();
            left.Children.Add(line_left); top.Children.Add(line_top);
            left.Children.Add(view_rect_left); top.Children.Add(view_rect_top);
            Dimension();
        }

    }
    public class Canvas_Propertis
    {
        public class Property
        {
            public Action<object> ChangesValue;
            public string name { get; set; }
            public string type { get; set; }
            public string category { get; set; }
            public object _value;
            public object value
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
            public bool isEnabled { get; set; }
            public Property(string category, string name, object value, string type, Action<object> action)
            {
                this.category = category;
                this.ChangesValue = action;
                this.name = name;
                this.value = value;
                this.type = type;
            }
        }
        UIElement propertis;
        DataGrid dataGrid;
        Label nonData;
        public Canvas_Propertis(UIElement propertis,DataGrid dataGrid,Label nonData)
        {
            this.propertis = propertis;
            this.dataGrid = dataGrid;
            this.nonData = nonData;
        } 
        public void LoadProperty(List<Property> list)
        {
            ClearProperty();
            System.ComponentModel.ICollectionView data =
                             System.Windows.Data.CollectionViewSource.GetDefaultView(list);
            data.GroupDescriptions.Clear();
            data.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("category"));
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
            switch (element.GetType().Name)
            {
                case "PersonUI":
                    this.LoadProperty((element as PersonUI).properties);
                    break;
                default:
                    nonData.Visibility = Visibility.Visible;
                    ClearProperty();
                    break;
            }
        }
        public void close()
        {
            propertis.Visibility = Visibility.Collapsed;
        }
    }
    public class Canvas_Constrain_Manager {
        List<BiezeUI> _constrais;

        public UIElement current_strat, current_end;
        bool _isWaitEnd = false;
        private Canvas canvas;

        public Canvas_Constrain_Manager(Canvas canvas)
        {
            this.canvas = canvas;
            constrains = new List<BiezeUI>();
        }

        public List<BiezeUI> constrains { get => _constrais; set
            {
                if(value != _constrais)
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
            if (_isWaitEnd)
                if (current_strat != null)
                {
                    current_end = uIElement;
                    _isWaitEnd = false;
                    Add_New(current_strat, current_end);
                    current_strat = current_end = null;
                }
                else _isWaitEnd = false;
        }
        internal BiezeUI Add_New(UIElement start,UIElement end)
        {
            BiezeUI bieze = new BiezeUI(start, end);
            constrains.Add(bieze);
            canvas.Children.Add(bieze);
            UpdateList();
            return bieze;
        }
        internal void UpdateList()
        {

        }
        internal void Current_Clear()
        {
            current_strat = current_end = null;
            _isWaitEnd = false;
        }
        internal void Remove(UIElement element)
        {
            List<BiezeUI> _onDelete = new List<BiezeUI>();
            foreach (BiezeUI b in _constrais)
            {
                if (b.start == element || b.end == element)
                {
                    canvas.Children.Remove(b);
                    _onDelete.Add(b);
                }
            }
            foreach (BiezeUI b in _onDelete)
            {
                _constrais.Remove(b);
            }
            _onDelete.Clear();
        }
    }
    public class Canvas_Grid : Canvas
    {
        Pen brush2,bg;
        SolidColorBrush solidColorBrush2,bgbrush;
        public Canvas_Grid()
        {
            solidColorBrush2 = new SolidColorBrush(Colors.Gray);
            bgbrush = new SolidColorBrush(Colors.Transparent);
            solidColorBrush2.Opacity = 0.3d;
            solidColorBrush2.Freeze();
            brush2 = new Pen(solidColorBrush2, 0.5d);
            bg = new Pen(bgbrush,0);
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
    public class Canvas_Focus
    {
        private UIElement _curent_Focus;
        public UIElement curent_Focus
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
        Canvas canvas;
        OpenSpace openSpace;
        Rectangle rectangle;
        TranslateTransform tt;
        public Canvas_Focus(OpenSpace openSpace, Canvas canvas)
        {
            this.openSpace = openSpace;
            this.canvas = canvas;
            rectangle = new Rectangle();
            rectangle.Visibility = Visibility.Collapsed;
            rectangle.Fill = new SolidColorBrush(Colors.Transparent);
            rectangle.IsHitTestVisible = false;
            rectangle.Stroke = new SolidColorBrush(Colors.OrangeRed);
            rectangle.StrokeThickness = 3;
            rectangle.RadiusX = rectangle.RadiusY = 10;
            tt = new TranslateTransform();
            rectangle.RenderTransform = tt;
            rectangle.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = Colors.OrangeRed,
                Direction = 270,
                ShadowDepth = 0,
                Opacity = 1,
                BlurRadius = 20
            };
            canvas.Children.Add(rectangle);
        }
        public void SetFocus(UIElement uI)
        {
            curent_Focus = uI;
            openSpace.canvas_Object_Manager.Select(uI);
        }
        public void CleadFocus()
        {
            curent_Focus.MouseMove -= UI_MouseMove;
            rectangle.Visibility = Visibility.Collapsed;
            curent_Focus = null;
        }
        private void ChangeFocusUI()
        {
            if (curent_Focus != null)
            {
                curent_Focus.MouseMove += UI_MouseMove;
                rectangle.Visibility = Visibility.Visible;
                rectangle.Width = curent_Focus.DesiredSize.Width;
                rectangle.Height = curent_Focus.DesiredSize.Height-10;
                tt.X = Canvas.GetLeft(curent_Focus);
                tt.Y = Canvas.GetTop(curent_Focus)+10;
                //Console.WriteLine("BIND FOCUS");
            }
        }
        private void UI_MouseMove(object sender, MouseEventArgs e)
        {
            if (curent_Focus != null)
            {
                tt.X = Canvas.GetLeft(curent_Focus);
                tt.Y = Canvas.GetTop(curent_Focus)+10;
            }
        }
    }
    public class Canvas_Object_Manager
    {
        public class ObjectItem
        {
            public ListBoxItem List_Item { get; set; }
            public UIElement UI_Item { get; set; }
            public ObjectItem(int i,UIElement uI_Item,RoutedEventHandler action)
            {
                UI_Item = uI_Item;
                List_Item = new ListBoxItem();
                List_Item.Content = $"{uI_Item.GetType().Name} ({i})";
                List_Item.Selected += action;
            }
            public override bool Equals(object obj)
            {
                if(obj is ListBoxItem)
                    return List_Item == obj as ListBoxItem;
                if(obj is UIElement)
                    return UI_Item == obj as UIElement;
                if(obj is ObjectItem)
                    return this == obj as ObjectItem;
                return false;
            }
        }
        int index = 1;
        List<ObjectItem> ObjectItems;
        Canvas canvas;
        ListBox listBox;
        OpenSpace openSpace;
        public Canvas_Object_Manager(OpenSpace openSpace,Canvas canvas,ListBox listBox)
        {
            this.openSpace = openSpace;
            this.listBox = listBox;
            this.canvas = canvas;
            ObjectItems = new List<ObjectItem>();
            listBox.Items.Clear();
        }
        internal void Add(UIElement element)
        {
            ObjectItem item = new ObjectItem(index++, element, List_Item_Selected);
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
            if(item == null) return;
            ObjectItems.Remove(item);
            listBox.Items.Remove(item.List_Item);
            canvas.Children.Remove(item.UI_Item);
        }
        internal void Select(UIElement uI)
        {
            listBox.SelectedItem = FindItemByUIelement(uI).List_Item;
        }
    }
    /// <summary>
    /// Логика взаимодействия для OpenSpace
    /// </summary>
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
        public  Canvas_Cursor canvas_Cursor;
        public  Canvas_Ruler canvas_Ruler;
        public  Canvas_Propertis canvas_Propertis;
        public  Canvas_Constrain_Manager constrain_Manager;
        public  Canvas_Focus canvas_Focus;
        public  Canvas_Object_Manager canvas_Object_Manager;
        private Line Constrain_Line;
        public  OpenSpace()
        {
            InitializeComponent();
            TransformGroup group = new TransformGroup();
            ScaleTransform xform = new ScaleTransform();
            group.Children.Add(xform);
            TranslateTransform tt = new TranslateTransform();
            MatrixTransform matrix = new MatrixTransform();
            tt.X = -(Canvas.Width / 2);
            tt.Y = -(Canvas.Height / 2);
            group.Children.Add(tt); 
            group.Children.Add(matrix);
            Canvas.RenderTransform = group;
        }
        public  void Add_Element(UIElement uIElement)
        {
            if (!_isAdding)
            {
                Adding = uIElement;
                Adding.IsEnabled= false;
                Adding.MouseLeftButtonDown += Adding_MouseLeftButtonDown;
                Adding.MouseLeftButtonUp += Adding_MouseLeftButtonUp;
                Adding.MouseEnter += Adding_MouseEnter;
                Adding.MouseLeave += Adding_MouseLeave;
                Adding.MouseMove += Adding_MouseMove;
                canvas_Object_Manager.Add(uIElement);
                _isAdding = true;
            }
        }
        private void Adding_MouseLeave(object sender, MouseEventArgs e)
        {
            canvas_Cursor.SetVisible(true);
            _isAdding_Hover = false;
            _isAdding_Move = false;
            //Console.WriteLine($"{sender.GetType().Name}:LEAVE");
        }
        private void Adding_MouseEnter(object sender, MouseEventArgs e)
        {
            canvas_Cursor.SetVisible(false);
            _isAdding_Hover = true;
            //Console.WriteLine($"{sender.GetType().Name}:ENTER");
        }
        private void Adding_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isAdding_Add_Constrain)
            {
                if (constrain_Manager.current_strat != sender as UIElement)
                {
                    constrain_Manager.SetEnd(sender as UIElement);
                    Console.WriteLine($"SetEnd({sender.GetType().Name})");
                    _isAdding_Add_Constrain = false;
                }
                else constrain_Manager.Current_Clear();
                Canvas.Children.Remove(Constrain_Line);
                Constrain_Line = null;
            }
            else
                _isAdding_Move = false;
        }
        private void Adding_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isAdding_Move)
            {
                Point m_canvas = e.GetPosition(Canvas);
                //Point m_obj = e.GetPosition(sender as UIElement);
                //Console.WriteLine($"sender:{sender.GetType().Name}\nm_canvas:{m_canvas}\nm_obj:{m_obj}");
                Adding_Move(false, sender as UIElement, m_canvas, e.GetPosition(CanvasViewer));
            }
        }
        private void Adding_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isAdding_Add_Constrain)
            {
                Console.WriteLine($"SetStart({sender.GetType().Name})");
                constrain_Manager.SetStart(sender as UIElement);
                Constrain_Line = new Line();
                Constrain_Line.Stroke = new SolidColorBrush(Colors.White);
                Constrain_Line.StrokeThickness = 1;
                Constrain_Line.IsHitTestVisible = false;
                Constrain_Line.X2 = Constrain_Line.X1 = (System.Windows.Controls.Canvas.GetLeft(sender as UIElement) + (sender as UIElement).DesiredSize.Width/2);
                Constrain_Line.Y2 = Constrain_Line.Y1 = (System.Windows.Controls.Canvas.GetTop(sender as UIElement) + (sender as UIElement).DesiredSize.Height/2);
                Canvas.Children.Add(Constrain_Line);
            }
            else
            {
                _isAdding_Move = true;
                isAdding_Move_start = e.GetPosition(sender as UIElement);
                canvas_Propertis.LoadByUIElement(sender as UIElement);
            }
            canvas_Focus.SetFocus(sender as UIElement);
        }
        private void UserControl_Initialized(object sender, EventArgs e)
        {
            
            Canvas.MouseWheel           += Canvas_MouseWheel;
            Canvas.MouseLeftButtonDown  += Canvas_MouseLeftButtonDown;
            Canvas.MouseLeftButtonUp    += Canvas_MouseLeftButtonUp;
            Canvas.MouseMove            += Canvas_MouseMove;
            Canvas.MouseLeave           += Canvas_MouseLeave;

            CanvasViewer.KeyDown        += CanvasViewer_KeyDown;
            
            //CanvasViewer.MouseWheel         += CanvasViewer_MouseWheel;
            //CanvasViewer.PreviewMouseWheel  += CanvasViewer_MouseWheel;

            canvas_Cursor = new Canvas_Cursor(Canvas);
            canvas_Ruler = new Canvas_Ruler(Canvas,left_tape,top_tape,CanvasViewer);
            canvas_Propertis = new Canvas_Propertis(PropertisBar,propertisGrid,propertisGrid_nonData);
            constrain_Manager = new Canvas_Constrain_Manager(Canvas);
            canvas_Focus = new Canvas_Focus(this,Canvas);
            canvas_Object_Manager = new Canvas_Object_Manager(this,Canvas,object_manager_list);
            PropertisBar_close.MouseLeftButtonDown += (s, b) => { canvas_Propertis.close(); };
        }
        public  void Add_Constrain()
        {
            _isAdding_Add_Constrain = true;
            Console.WriteLine("_isAdding_Add_Constrain ACTIVATE");
        }
        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            canvas_Cursor.SetVisible(false);
        }
        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isAdding_Hover) return;
            if (_isAdding_Move) return;
            if (sender is Canvas)
                Canvas.ReleaseMouseCapture();
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
        }
        private void Corect_Size()
        {
            var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
            if (tt.X > 0) tt.X = 0;
            else if (tt.X < -(Canvas.ActualWidth - CanvasViewer.ActualWidth)) tt.X = -(Canvas.ActualWidth - CanvasViewer.ActualWidth);
            if (tt.Y > 0) tt.Y = 0;
            else if (tt.Y < -(Canvas.ActualHeight - CanvasViewer.ActualHeight)) tt.Y = -(Canvas.ActualHeight - CanvasViewer.ActualHeight);

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
                    canvas_Cursor.SetPosition(point);
                    canvas_Ruler.SetMousePosition(point);
                    if (!Canvas.IsMouseCaptured) return;
                    var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
                    Vector v = start - e.GetPosition(CanvasViewer);
                    tt.X = (origin.X - v.X);
                    tt.Y = (origin.Y - v.Y);
                    //Console.WriteLine($"MouseMove\nVector:{v.X},{v.Y}\nStart:{start.ToString()}\nOrigin:{origin.ToString()}\ntt:{tt.X},{tt.Y}");
                    Corect_Size();
                    canvas_Ruler.SetOffset(new Point(tt.X, tt.Y));
                }
            }
            else
            {
                Adding_Move(true,Adding,e.GetPosition(Canvas),e.GetPosition(CanvasViewer));
            }
        }
        public  void Adding_Move(bool first,UIElement obj,Point point,Point point_view)
        {
            canvas_Cursor.SetVisible(false);
            canvas_Ruler.SetMousePosition(point);
            canvas_Ruler.SetVisible(true);
            if (!first)
            {
                var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
                //Console.WriteLine($"point_view:{point_view}\npoint:{point}");
                //Console.WriteLine($"tt.XCC:{point_view.X - (obj.DesiredSize.Width / 2)}");
                if (point_view.X - (obj.DesiredSize.Width / 2) < 10)
                    tt.X += 5;
                if (point_view.Y - (obj.DesiredSize.Height / 2) < 10)
                    tt.Y += 5;
                if (point_view.X + (obj.DesiredSize.Width / 2) > CanvasViewer.ActualWidth - 10)
                    tt.X -= 5;
                if (point_view.Y + (obj.DesiredSize.Height / 2) > CanvasViewer.ActualHeight - 10)
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
                    //Console.WriteLine($"MouseLeftButtonDown\nStart:{start.ToString()}\nOrigin:{origin.ToString()}\ntt:{tt.X},{tt.Y}\ncanvas_origin:{canvas_origin.X},{canvas_origin.Y}");
                    Corect_Size();
                    canvas_Ruler.SetOffset(origin);
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
            
            if (_isAdding_Move) return;
            if (sender is Canvas)
            {
                var _scaleTransform = (ScaleTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is ScaleTransform);
                var _translateTransform = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
                var position = e.GetPosition(Canvas);
                
                double oldCenterX = _scaleTransform.CenterX;
                double oldCenterY = _scaleTransform.CenterY;

                _scaleTransform.CenterX = position.X;
                _scaleTransform.CenterY = position.Y;
               

                _translateTransform.X += (_scaleTransform.CenterX - oldCenterX) * (_scaleTransform.ScaleX - 1);
                _translateTransform.Y += (_scaleTransform.CenterY - oldCenterY) * (_scaleTransform.ScaleY - 1);

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
                    if (_scaleTransform.ScaleX > .5)
                    {
                        _scaleTransform.ScaleX-=.1;
                        _scaleTransform.ScaleY-=.1;
                    }
                }
                zoom_scale = _scaleTransform.ScaleX;
                Corect_Size();
                origin = new Point(_translateTransform.X, _translateTransform.Y);
                canvas_Ruler.SetOffset(origin);
                t_zoom_p.Content = $"{(int)(zoom_scale * 100)}%";
            }
        }
        /// <summary>
        /// Блокирует прокрутку для CanvasViewer.
        /// OpenSpace.xaml
        /// </summary>
        private void CanvasViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Corect_Size();
            canvas_Ruler.SetCoefficient(new Point(Canvas.ActualWidth, Canvas.ActualHeight), new Point(CanvasViewer.ActualWidth, CanvasViewer.ActualHeight));
            var tt = (TranslateTransform)((TransformGroup)Canvas.RenderTransform).Children.First(tr => tr is TranslateTransform);
            canvas_Ruler.SetOffset(new Point(tt.X, tt.Y));
        }
        private void Property_Action(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
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
                if (canvas_Focus.curent_Focus != null)
                    canvas_Focus.CleadFocus();
                constrain_Manager.Remove(element);
                canvas_Propertis.ClearProperty();
                canvas_Object_Manager.Remove(element);
            }
        }
        private void CanvasViewer_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    DeleteElement(canvas_Focus.curent_Focus);
                    break;
            }
        }
    }
}
