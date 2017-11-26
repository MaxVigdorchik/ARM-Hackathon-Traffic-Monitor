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

namespace ARM_Hackathon_Traffic_Monitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        public MainWindow()
        {
            InitializeComponent();
            Retrieve.Initialise();
            //Display.Nodes(Map);
        }

        public class Display
        {
            private static Dictionary<int, Ellipse> NodeEllipses = new Dictionary<int, Ellipse>();

            //Nodes
            public static void Nodes(Canvas c)
            {
                double width = c.ActualWidth;
                double height = c.ActualHeight;
                int nodeID;

                Ellipse e;
                Position p;

                Position.SetUpExtremes();
                var nodes = Dictionaries.Nodes.Values;

                foreach (var node in nodes)
                {
                    nodeID = node.NodeID;
                    e = CreateNewEllipse();

                    NodeEllipses.Add(nodeID, e);
                    c.Children.Add(e);

                    p = Position.ConvertToFraction(node);
                    Canvas.SetLeft(e, c.ActualWidth * p.X - e.Width/2);
                    Canvas.SetTop(e, c.ActualHeight * p.Y - e.Height/2);

                    e.MouseEnter += E_MouseEnter;
                }
            }

            private static void E_MouseEnter(object sender, MouseEventArgs e)
            {
                throw new NotImplementedException();
            }

            public static Ellipse CreateNewEllipse()
            {
                Ellipse E = new Ellipse();
                int Radius = 5;
                E.Stroke = Brushes.Black;
                E.StrokeThickness = 1;
                E.Width = Radius * 2;
                E.Height = Radius * 2;
                E.Fill = Brushes.White;
                return E;
            }

            //Edges
            public static void Edges(Canvas c)
            {
                double width = c.ActualWidth;
                double height = c.ActualHeight;

                Line l;
                Position a;
                Position b;

                var edges = Dictionaries.Edges.Values;

                foreach (var edge in edges)
                {
                    a = Position.ConvertToFraction(edge.NodeA);
                    b = Position.ConvertToFraction(edge.NodeB);

                }
            }

            public static Line CreateNewLine()
            {

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

                double x = (longitude - BotLeft.X) * (TopRight.X - BotLeft.X);
                double y = 1 - ((latitude - BotLeft.Y) * (TopRight.Y = BotLeft.Y));
                return new Position(x, y);
            }
        }
    }
}
