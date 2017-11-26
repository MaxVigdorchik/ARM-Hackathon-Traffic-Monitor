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

using DataAnalysis;
using System.Media;

namespace ARM_Hackathon_Traffic_Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool SmallGraph = false; // toggle to change between demo modes
        private bool ReadFromFile = false;
           
        public MainWindow()
        {
            InitializeComponent();
            WriteToFile.Write();
            Retrieve.Initialise(PacketBox, SmallGraph);           
            Display.Initialise(NodeIDBox, EdgeBox, WeightBox, Map);
            Display.Edges();
            Display.Nodes();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (ReadFromFile)
            {
                Retrieve.ReadFromFile(SmallGraph);
            }
        }

        public class Display
        {
            private static TextBox NodeIDBox { get; set; }
            private static TextBox EdgeBox { get; set; }
            private static TextBox WeightBox { get; set; }
            private static Canvas C { get; set; }

            public static void Initialise(TextBox nodeIDBox, TextBox edgeBox, TextBox weightBox, Canvas c)
            {
                NodeIDBox = nodeIDBox;
                EdgeBox = edgeBox;
                WeightBox = weightBox;
                C = c;
                Position.SetUpExtremes();
            }

            //Nodes
            public static void Nodes()
            {
                double width = C.Width;
                double height = C.Height;
                int nodeID;

                Ellipse e;
                Position p;

                var nodes = Dictionaries.Nodes.Values;

                foreach (var node in nodes)
                {
                    nodeID = node.NodeID;
                    e = CreateNewEllipse();

                    Dictionaries.NodeEllipses.Add(nodeID, e);
                    C.Children.Add(e);

                    p = Position.ConvertToFraction(node);
                    Canvas.SetLeft(e, width * p.X - e.Width/2);
                    Canvas.SetTop(e, height * p.Y - e.Height/2);

                    e.MouseEnter += E_MouseEnter;
                }
            }

            private static void E_MouseEnter(object sender, MouseEventArgs e)
            {
                Ellipse ell = sender as Ellipse;
                int key = 21;

                foreach (var item in Dictionaries.NodeEllipses)
                {
                    if (item.Value == ell)
                    {
                        key = item.Key;
                    }
                }
                NodeIDBox.Text = Convert.ToString(key);

                //SystemSounds.Beep.Play();
            }

            public static Ellipse CreateNewEllipse()
            {
                Ellipse E = new Ellipse();
                int Radius = 10;
                E.Stroke = Brushes.Black;
                E.StrokeThickness = 1;
                E.Width = Radius * 2;
                E.Height = Radius * 2;
                E.Fill = Brushes.White;
                return E;
            }

            //Edges
            public static void Edges()
            {
                double width = C.Width;
                double height = C.Height;

                Line L;
                Position a;
                Position b;

                var edgeItems = Dictionaries.Edges;

                foreach (var edgeItem in edgeItems)
                {
                    var value = edgeItem.Value;
                    var key = edgeItem.Key;

                    a = Position.ConvertToFraction(value.NodeA);
                    b = Position.ConvertToFraction(value.NodeB);

                    L = CreateNewLine();

                    Dictionaries.EdgeLines.Add(key, L);
                    C.Children.Add(L);

                    L.X1 = a.X * width;
                    L.X2 = ((b.X + a.X)/2) * width;

                    L.Y1 = a.Y * height;
                    L.Y2 = ((b.Y + a.Y)/2) * height;

                    L.MouseEnter += L_MouseEnter;
                }
            }

            private static void L_MouseEnter(object sender, MouseEventArgs e)
            {
                Line L = sender as Line;
                int key = 21;

                foreach (var item in Dictionaries.EdgeLines)
                {
                    if (item.Value == L)
                    {
                        key = item.Key;
                    }
                }

                IEdge edge = Dictionaries.Edges[key];
                int A = edge.NodeA.NodeID;
                int B = edge.NodeB.NodeID;

                EdgeBox.Text = A + " --> " + B;
                WeightBox.Text = edge.ToString();
                //WeightBox.Text = Convert.ToString(edge.GetWeight());
                //SystemSounds.Beep.Play();
            }

            public static Line CreateNewLine()
            {
                Line L = new Line();
                L.StrokeThickness = 5;
                L.Stroke = new SolidColorBrush(Colors.Black);
                L.StrokeLineJoin = PenLineJoin.Round;
                L.StrokeDashCap = PenLineCap.Triangle;
                L.Opacity = 0.5;
                return L;
            }
        }

        public class Position
        {
            private static Position TopRight { get; set; }
            private static Position BotLeft { get; set; }

            public double X { get; set; }
            public double Y { get; set; }

            public Position(double x, double y)
            {
                this.X = x;
                this.Y = y;
            }

            public static void SetUpExtremes()
            {
                double x; // origin bottom left
                double y;

                bool set = false;

                var nodesDict = Dictionaries.Nodes;

                foreach (var node in nodesDict.Values)
                {
                    x = node.Longitude;
                    y = node.Latitude;

                    if(!set)
                    {
                        set = true;
                        TopRight = new Position(x, y);
                        BotLeft = new Position(x, y);
                    }

                    if (x > TopRight.X)
                    {
                        TopRight.X = x;
                    }
                    if (y > TopRight.Y)
                    {
                        TopRight.Y = y;
                    }

                    if (x < BotLeft.X)
                    {
                        BotLeft.X = x;
                    }
                    if(y < BotLeft.Y)
                    {
                        BotLeft.Y = y;
                    }
                }
            }

            public static Position ConvertToFraction(INode n)
            {
                double longitude = n.Longitude;
                double latitude = n.Latitude;

                double x = (longitude - BotLeft.X) / (TopRight.X - BotLeft.X);
                double y = 1 - ((latitude - BotLeft.Y) / (TopRight.Y - BotLeft.Y));
                return new Position(x, y);
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }
    }
}
