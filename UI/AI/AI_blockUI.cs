using CaseManager.RecordSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
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

namespace CaseManager.UI.AI
{
    /// <summary>
    /// Логика взаимодействия для AI_blockUI.xaml
    /// </summary>
    public partial class AI_blockUI : FrameworkElement, IElement
    {
        public List<Canvas_Propertis.Property> properties { get; set; }
        public bool CanDelite { get; set; }

        public List<List<AI_NodeUI>> nodes;
        public List<Line> lines;
        private Canvas canvas;
        private Point offset;
        private OpenSpace os;
        private bool _isAdding = false;
        private int max = 0,layers_count = 0;
        private bool _isMove=false;
        public AI_blockUI()
        {
            CanDelite = true;
            this.os = IOCore.openSpace;

            this.MouseLeftButtonUp += Rectangle_MouseLeftButtonUp;
            this.MouseLeftButtonDown += Rectangle_MouseLeftButtonDown;
            this.MouseMove += Rectangle_MouseMove;

            this.canvas = this.os.Canvas;
            offset = new Point(0, 0);
            nodes = new List<List<AI_NodeUI>>();
            lines = new List<Line>();
            _isAdding = true;
            Margin = new Thickness(5, 15, 5, 5);
            Width = 100;
            Height = 60;
            RenderSize = new Size(Width, Height);
            properties = new List<Canvas_Propertis.Property>
            {
                new Canvas_Propertis.Property("Основное","Данные", "1,2,2,1", "string",ReGenerate)
            };
        }
        private void Rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isAdding)
            {
                _isAdding = false;
                ReGenerate(properties[0].Value);
            }    
            _isMove = false;
            UpdateOffset(new Point(Canvas.GetLeft(this), Canvas.GetTop(this)));
        }
        private void Rectangle_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMove)
                UpdateOffset(new Point(Canvas.GetLeft(sender as UIElement), Canvas.GetTop(sender as UIElement)));
        }
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isMove = true;
        }
        internal void ReGenerate(object v)
        {
            if (!_isAdding)
            {
                Clear();
                List<int> result = (v as string).Split(',').Where(x => x != "").ToArray().Select(int.Parse).ToList();
                Generate(result);
                Point new_offset = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
                if (offset != new_offset)
                    UpdateOffset(new_offset);
            }
        }
        internal void UpdateOffset(Point new_offset)
        {
            Vector diff = new_offset-offset;
            offset = new_offset;
            foreach (List<AI_NodeUI> layer_i in nodes)
            {
                foreach (AI_NodeUI node in layer_i)
                {
                    Point off = new Point(Canvas.GetLeft(node), Canvas.GetTop(node));
                    Canvas.SetLeft(node, diff.X + off.X);
                    Canvas.SetTop(node, diff.Y + off.Y);
                }
            }
            foreach (Line item in lines)
            {
                item.X1 += diff.X;
                item.Y1 += diff.Y;
                item.X2 += diff.X;
                item.Y2 += diff.Y;
            }
        }
        public void GenerateConstrain()
        {
            for (int i = 1; i < nodes.Count; i++)
            {
                for (int k = 0; k < nodes[i-1].Count; k++)
                {
                    AI_NodeUI aI_Node_from = nodes[i-1][k];
                    for (int j = 0; j < nodes[i].Count; j++)
                    {
                        os.constrain_Manager.Add_New(aI_Node_from, nodes[i][j]);
                    }
                }
            }
        }
        public void GenerateLine()
        {
            for (int i = 1; i < nodes.Count; i++)
            {
                for (int k = 0; k < nodes[i - 1].Count; k++)
                {
                    AI_NodeUI aI_Node_from = nodes[i - 1][k];
                    for (int j = 0; j < nodes[i].Count; j++)
                    {
                        Line line = new Line()
                        {
                            X1 = Canvas.GetLeft(aI_Node_from) + offset.X + 30,
                            Y1 = Canvas.GetTop(aI_Node_from) + offset.Y + 40,
                            X2 = Canvas.GetLeft(nodes[i][j]) + offset.X + 30,
                            Y2 = Canvas.GetTop(nodes[i][j]) + offset.Y + 40,
                            Stroke = new SolidColorBrush(Colors.White),
                            IsHitTestVisible = false,
                            StrokeThickness = 1
                        };
                        Panel.SetZIndex(line,0);
                        lines.Add(line);
                        canvas.Children.Add(line);
                    }
                }
            }
        }
        public void Generate(List<int> a_layers)
        {
            Clear();
            layers_count = a_layers.Count;
            for (int i = 0; i < layers_count; i++)
                if(a_layers[i]>max)max=a_layers[i];
            for (int i = 0; i < layers_count; i++)
                CreateLayer(i, a_layers[i]);
            GenerateLine();
        }
        public void CreateLayer(int index,int cout)
        {
            List<AI_NodeUI> aI_NodeUIs= new List<AI_NodeUI>();
            for (int i = 0; i < cout; i++)
            {
                AI_NodeUI node = new AI_NodeUI();
                node.CanDelite = false;
                Canvas.SetLeft(node,(offset.X-(layers_count*40))+(index*120));
                Canvas.SetTop(node,offset.Y+((max-cout)*35)+(i*60)+70);
                aI_NodeUIs.Add(node);
                Panel.SetZIndex(node, 1);
                os.canvas_Object_Manager.Add(node,this,false);
            }
            nodes.Add(aI_NodeUIs);
        }
        public void Clear()
        {
            max = 0; layers_count = 0;
            offset = new Point(0, 0);
            foreach (List<AI_NodeUI> layer_i in nodes)
            {
                foreach (AI_NodeUI node in layer_i)
                {
                    os.canvas_Object_Manager.Remove(node);
                }
            }
            foreach (Line line in lines)
            {
                canvas.Children.Remove(line);
            }
            lines.Clear();
            nodes.Clear();
            UpdateOffset(offset);
        }
        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRoundedRectangle(
                new SolidColorBrush(Colors.White),
                new Pen(new SolidColorBrush(Colors.Gray), 1),
                new Rect(new Point(0, 0), new Point(Width, Height)), 5, 5);
            drawingContext.DrawText(
                new FormattedText("AI Блок",
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface("normal"),
                10, new SolidColorBrush(Colors.Black), 1.5d), new Point(Width / 2 - 20, Height / 2 - 5));
        }
    }
}
