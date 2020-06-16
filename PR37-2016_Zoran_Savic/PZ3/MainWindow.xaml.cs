using PZ2;
using PZ2.ELEMENTS;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PZ3
{
    public partial class MainWindow : Window
    {
        public double centerx { get; set; }
        private Point start = new Point();
        private Point diffOffset = new Point();
        private int zoomMax = 100;
        private int zoomCurent = 1;
        private System.Windows.Point startRotation = new System.Windows.Point();
        public Point3DCollection Positions { get; private set; }
        public Int32Collection Indicies { get; private set; }
        public XmlParser xml = new XmlParser();
        private double cubeSize = 0.005;
        private double lineSize = 0.003;
        private double mapSize = 1;
        private double minX = 45.2325;
        private double maxX = 45.277031;
        private double minY = 19.793909;
        private double maxY = 19.894459;
        private int[,] mappp = new int[1000,1000];//gde se sta nalazi na mapi
        List<NodeEntity> nodes;
        List<SwitchEntity> switches;
        List<SubstationEntity> substationEntities;
        List<LineEntity> lines;
        private Dictionary<long, Tuple<GeometryModel3D, int>> models = new Dictionary<long, Tuple<GeometryModel3D, int>>();
        private List<GeometryModel3D> linesmodel = new List<GeometryModel3D>();
        private List<GeometryModel3D> node = new List<GeometryModel3D>();
        private List<GeometryModel3D> substation = new List<GeometryModel3D>();
        private List<GeometryModel3D> switchc = new List<GeometryModel3D>();
        private GeometryModel3D hitgeo;//predstavlja mapu na koju se sve stavlja

        public MainWindow()
        {
            centerx = 0.5;
            xml.ParseXML("../../Geographic.xml", out substationEntities, out nodes, out switches, out lines);
            InitializeComponent();
            InitializeMapppp();
            DrawNodes();
            DrawSubstations();
            DrawSwitches();
            DrawLines();
        }

        private void InitializeMapppp()//napravi mapu praznu
        {
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    mappp[i, j] = 0;
                }
            }
        }

        private void viewport1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            viewport1.CaptureMouse();
            start = e.GetPosition(this);
            diffOffset.X = translacija.OffsetX;
            diffOffset.Y = translacija.OffsetY;
        }

        private void viewport1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewport1.ReleaseMouseCapture();
        }

        private void viewport1_MouseMove(object sender, MouseEventArgs e)
        {
            if (viewport1.IsMouseCaptured)
            {
                Point end = e.GetPosition(this);
                double offsetX = end.X - start.X;
                double offsetY = end.Y - start.Y;
                double w = this.Width;
                double h = this.Height;
                double translateX = (offsetX * 100) / w;
                double translateY = -(offsetY * 100) / h;
                translacija.OffsetX = diffOffset.X + (translateX / (100 * skaliranje.ScaleX));
                translacija.OffsetY = diffOffset.Y + (translateY / (100 * skaliranje.ScaleX));                        
            }
            else if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Point end = e.GetPosition(this);
                if (end.X > start.X)
                {
                    myHorizontalRotation.Angle += 1;
                }
                else if (end.X < start.X)
                {
                    myHorizontalRotation.Angle -= 1;
                }
                if (end.Y > start.Y)
                {
                    if (myVerticalRotation.Angle < 65)
                    {
                        myVerticalRotation.Angle += 1;
                    }
                }
                else if (end.Y < start.Y)
                {
                    if (myVerticalRotation.Angle > -30)
                    {
                        myVerticalRotation.Angle -= 1;
                    }
                }
                start = end;
            }
        }

        private void viewport1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point p = e.MouseDevice.GetPosition(this);
            double scaleX = 1;
            double scaleY = 1;
            double scaleZ = 1;
            if (e.Delta > 0 && zoomCurent < zoomMax)
            {
                scaleX = skaliranje.ScaleX + 0.1;
                scaleY = skaliranje.ScaleY + 0.1;
                scaleZ = skaliranje.ScaleZ + 0.1;
                zoomCurent++;
                skaliranje.ScaleX = scaleX;
                skaliranje.ScaleY = scaleY;
                skaliranje.ScaleZ = scaleZ;
            }
            else if (e.Delta <= 0 && zoomCurent > -zoomMax)
            {
                scaleX = skaliranje.ScaleX - 0.1;
                scaleY = skaliranje.ScaleY - 0.1;
                scaleZ = skaliranje.ScaleZ - 0.1;
                zoomCurent--;
                skaliranje.ScaleX = scaleX;
                skaliranje.ScaleY = scaleY;
                skaliranje.ScaleZ = scaleZ;
            }
        }

        private void MouseDown(object sender, MouseButtonEventArgs e)
        {
            startRotation = e.GetPosition(this);
        }
        private void DrawSubstations()
        {
            foreach (var substation in substationEntities)
            {
                double latitude = substation.X;
                double longitude = substation.Y;

                if (latitude < 45.2325 || latitude > 45.277031 || longitude < 19.793909 || longitude > 19.894459)
                    continue;

                System.Windows.Point point = ApproximateFunction(latitude, longitude);//Odredjuje gde se crta(nalazi)
                double x = 0, y = 0;
                funkcija(latitude, longitude, out x, out y);//ograniceno 1000*1000
                for (int i = 0; i < 5; i++)//velicina kockice
                {
                    for (int j = 0; j < 5; j++)
                    {
                        mappp[(int)x + i, (int)y + j]++;//zauzima na mapi
                    }
                }
                ToolTip tt = new System.Windows.Controls.ToolTip();//samo dodaj tekst
                string str = String.Format("ID: {0}, Type: {1} Name: {2}", substation.ID, "Substation", substation.Name);
                tt.Content = str;

                DrawSub(point, tt, substation.ID, x, y);
            }
        }

        private void DrawSub(System.Windows.Point point, ToolTip tt, long id, double x, double y)
        {
            GeometryModel3D obj = new GeometryModel3D();
            obj.Material = new DiffuseMaterial(Brushes.Red);
            int getup = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (getup < mappp[(int)x + i, (int)y + j])//nalazenje maks i dizanje u vis
                    {
                        getup = mappp[(int)x + i, (int)y + j];
                    }
                }
            }
            getup -= 1;
            Positions = new Point3DCollection()
            {
                new Point3D(point.X, point.Y,getup*cubeSize),
                new Point3D(point.X + cubeSize, point.Y, getup*cubeSize),
                new Point3D(point.X, point.Y + cubeSize,getup*cubeSize),
                new Point3D(point.X + cubeSize, point.Y + cubeSize, getup*cubeSize),
                new Point3D(point.X, point.Y, cubeSize +  getup *cubeSize),
                new Point3D(point.X + cubeSize, point.Y, cubeSize+ getup*cubeSize),
                new Point3D(point.X , point.Y + cubeSize, cubeSize+  getup*cubeSize),
                new Point3D(point.X + cubeSize, point.Y + cubeSize, cubeSize+  getup*cubeSize),
            };

            Indicies = new Int32Collection()
            {
                2,3,1,  2,1,0,  7,1,3,  7,5,1,  6,5,7,  6,4,5,  6,2,4,  2,0,4,  2,7,3,  2,6,7,  0,1,5,  0,5,4
            };

            obj.Geometry = new MeshGeometry3D() { Positions = Positions, TriangleIndices = Indicies };//od pozicija objekat
            obj.SetValue(IsToolTip, tt);//postavi vrednosti
            substation.Add(obj);
            map.Children.Add(obj);//doda na kanvas
            try
            {
                models.Add(id, new Tuple<GeometryModel3D, int>(obj, 0));
            }
            catch (Exception)
            {
                MessageBox.Show("Sub problem");
            }
        }

        private void DrawNodes()
        {
            foreach (var node in nodes)
            {
                double latitude = node.X;
                double longitude = node.Y;

                if (latitude < 45.2325 || latitude > 45.277031 || longitude < 19.793909 || longitude > 19.894459)
                    continue;

                System.Windows.Point point = ApproximateFunction(latitude, longitude);
                double x=0, y=0;
                funkcija(latitude, longitude,out x, out y);
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        mappp[(int) x + i, (int)y+j]++;
                    }              
                }          
                ToolTip tt = new System.Windows.Controls.ToolTip();
                string str = String.Format("ID: {0}, Type: {1} Name: {2}", node.ID, "Node", node.Name);
                tt.Content = str;
                DrawNode(point, tt, node.ID,x,y);
            }
        }

        private void DrawNode(System.Windows.Point point, ToolTip tt, long id,double x,double y)
        {
            GeometryModel3D obj = new GeometryModel3D();
            obj.Material = new DiffuseMaterial(Brushes.Blue);
            int getup = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (getup < mappp[(int)x + i, (int)y + j])
                    {
                        getup = mappp[(int)x + i, (int)y + j];
                    }
                }
            }
            getup -= 1;
           
            Positions = new Point3DCollection()
            {
                new Point3D(point.X, point.Y,getup*cubeSize),
                new Point3D(point.X + cubeSize, point.Y, getup*cubeSize),
                new Point3D(point.X, point.Y + cubeSize,getup*cubeSize),
                new Point3D(point.X + cubeSize, point.Y + cubeSize, getup*cubeSize),
                new Point3D(point.X, point.Y, cubeSize +  getup *cubeSize),
                new Point3D(point.X + cubeSize, point.Y, cubeSize+ getup*cubeSize),
                new Point3D(point.X , point.Y + cubeSize, cubeSize+  getup*cubeSize),
                new Point3D(point.X + cubeSize, point.Y + cubeSize, cubeSize+  getup*cubeSize),
            };

            Indicies = new Int32Collection()
            {
                2,3,1,  2,1,0,  7,1,3,  7,5,1,  6,5,7,  6,4,5,  6,2,4,  2,0,4,  2,7,3,  2,6,7,  0,1,5,  0,5,4
            };

            obj.Geometry = new MeshGeometry3D() { Positions = Positions, TriangleIndices = Indicies };
            obj.SetValue(IsToolTip, tt);
            node.Add(obj);

            map.Children.Add(obj);
            try
            {
                models.Add(id, new Tuple<GeometryModel3D, int>(obj, 0));
            }
            catch (Exception)
            {
                MessageBox.Show("Node problem");
            }
        }
        private void DrawSwitches()
        {
            foreach (var switchh in switches)
            {
                double latitude = switchh.X;
                double longitude = switchh.Y;

                if (latitude < 45.2325 || latitude > 45.277031 || longitude < 19.793909 || longitude > 19.894459)
                    continue;

                System.Windows.Point point = ApproximateFunction(latitude, longitude);
                double x = 0, y = 0;
                funkcija(latitude, longitude, out x, out y);
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        mappp[(int)x + i, (int)y + j]++;
                    }
                }
                ToolTip tt = new System.Windows.Controls.ToolTip();
                string str = String.Format("ID: {0}, Type: {1} Name: {2}, State {3}", switchh.ID, "Switch", switchh.Name, switchh.Status.ToString());
                tt.Content = str;

                DrawSwitch(point, tt, switchh.ID,x,y);
            }
        }

        private void DrawSwitch(System.Windows.Point point, ToolTip tt, long id,double x,double y)
        {
            GeometryModel3D obj = new GeometryModel3D();
            DiffuseMaterial material = new DiffuseMaterial();
            int getup = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (getup < mappp[(int)x + i, (int)y + j])
                    {
                        getup = mappp[(int)x + i, (int)y + j];
                    }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      
                }
            }
            getup -= 1;
            material = new DiffuseMaterial(Brushes.Green);

            obj.Material = material;

            Positions = new Point3DCollection()
                {
                  new Point3D(point.X, point.Y,getup*cubeSize),
                new Point3D(point.X + cubeSize, point.Y, getup*cubeSize),
                new Point3D(point.X, point.Y + cubeSize,getup*cubeSize),
                new Point3D(point.X + cubeSize, point.Y + cubeSize, getup*cubeSize),
                new Point3D(point.X, point.Y, cubeSize +  getup *cubeSize),
                new Point3D(point.X + cubeSize, point.Y, cubeSize+ getup*cubeSize),
                new Point3D(point.X , point.Y + cubeSize, cubeSize+  getup*cubeSize),
                new Point3D(point.X + cubeSize, point.Y + cubeSize, cubeSize+  getup*cubeSize),
                };

            Indicies = new Int32Collection()
            {
                2,3,1,  2,1,0,  7,1,3,  7,5,1,  6,5,7,  6,4,5,  6,2,4,  2,0,4,  2,7,3,  2,6,7,  0,1,5,  0,5,4
            };

            obj.Geometry = new MeshGeometry3D() { Positions = Positions, TriangleIndices = Indicies };
            obj.SetValue(IsToolTip, tt);

            switchc.Add(obj);
            map.Children.Add(obj);
            try
            {
                models.Add(id, new Tuple<GeometryModel3D, int>(obj, 0));
            }
            catch (Exception)
            {
                MessageBox.Show("switch problem");
            }
        }

        private void DrawLines()
        {
            foreach (var line in lines)
            {
                double latitude;
                double longitude;
                List<System.Windows.Point> points = new List<System.Windows.Point>();

                foreach (var dot in line.Vertices)
                {
                    latitude = dot.X;
                    longitude = dot.Y;
                    if (latitude < 45.2325 || latitude > 45.277031 || longitude < 19.793909 || longitude > 19.894459)
                        continue;

                    System.Windows.Point point = ApproximateFunction(latitude, longitude);
                    points.Add(point);
                }
                try
                {
                    if (models.ContainsKey((long)line.FirstEnd) && models.ContainsKey((long)line.SecondEnd))
                    {
                        models[(long)line.FirstEnd] = new Tuple<GeometryModel3D, int>(models[(long)line.FirstEnd].Item1, models[(long)line.FirstEnd].Item2 + 1);
                        models[(long)line.SecondEnd] = new Tuple<GeometryModel3D, int>(models[(long)line.SecondEnd].Item1, models[(long)line.SecondEnd].Item2 + 1);
                    }
                }
                catch (Exception)
                {
                }

                ToolTip tt = new System.Windows.Controls.ToolTip();
                string str = String.Format("ID: {0}, Type: {1} Name: {2}, LineType: {3}", line.ID, "Line", line.Name, line.LineType);
                tt.Content = str;
                DrawLine(points, tt, line.ID);
            }
        }

        private void DrawLine(List<System.Windows.Point> points, ToolTip tt, long id)
        {
            DiffuseMaterial material = new DiffuseMaterial();
            material = new DiffuseMaterial(Brushes.Gold);

            Indicies = new Int32Collection()
            {
                0,1,2,  1,3,2,  0,4,2,  4,6,2,  5,1,7,  1,3,7,  6,7,2,  7,3,2,  4,5,0,  5,1,0,  4,5,6,  5,7,6
            };

            for (int i = 0; i < points.Count - 1; i++)
            {
                Positions = new Point3DCollection()
                {
                    new Point3D(points[i].X, points[i].Y, 0),
                    new Point3D(points[i].X + lineSize, points[i].Y, 0),
                    new Point3D(points[i].X, points[i].Y + lineSize, 0),
                    new Point3D(points[i].X + lineSize, points[i].Y + lineSize, 0),
                    new Point3D(points[i + 1].X, points[i + 1].Y, lineSize),
                    new Point3D(points[i + 1].X + lineSize, points[i + 1].Y, lineSize),
                    new Point3D(points[i + 1].X , points[i + 1].Y + lineSize, lineSize),
                    new Point3D(points[i + 1].X + lineSize, points[i + 1].Y + lineSize, lineSize),
                 };

                GeometryModel3D obj = new GeometryModel3D();
                obj.Material = material;
                obj.Geometry = new MeshGeometry3D() { Positions = Positions, TriangleIndices = Indicies };
                obj.SetValue(IsToolTip, tt);

                map.Children.Add(obj);
                try
                {
                    linesmodel.Add(obj);
                }
                catch (Exception)
                {

                    MessageBox.Show("line problem");
                }
            }
        }

        public void funkcija( double x,double y,out double pozicijax,out double pozicijay)
        {       
                pozicijax = (x - minX) / ((maxX - minX) / 995);
                pozicijay = (y - minY) / ((maxY - minY) / 995);
        }

        public static readonly DependencyProperty IsToolTip = DependencyProperty.RegisterAttached(
                 "ToolTip",
                 typeof(ToolTip),
                 typeof(GeometryModel3D)
        );

        public static void SetIsToolTip(GeometryModel3D element, ToolTip value)
        {
            element.SetValue(IsToolTip, value);
        }
        public static ToolTip GetIsToolTip(GeometryModel3D element)
        {
            return (ToolTip)element.GetValue(IsToolTip);
        }

        private System.Windows.Point ApproximateFunction(double X, double Y)
        {
            System.Windows.Point point = new System.Windows.Point();
            point.Y = (X - minX) / (maxX - minX) * (mapSize - cubeSize);
            point.X = (Y - minY) / (maxY - minY) * (mapSize - cubeSize);
            return point;
        }
        GeometryModel3D f1=null;
        GeometryModel3D f2=null;
        long id1 = -1, id2 = -1;

        private HitTestResultBehavior HTResult(System.Windows.Media.HitTestResult rawresult)
        {
            RayHitTestResult rayResult = rawresult as RayHitTestResult;
            ToolTip tt = new ToolTip();
            if (f1 != null || f2 != null)
            {
                map.Children.Remove(f1);
                map.Children.Remove(f2);
                map.Children.Add(models[id1].Item1);
                map.Children.Add(models[id2].Item1);
            }
            if (rayResult != null)
            {
                bool gasit = false;
                foreach (var gm in models.Values)
                {
                    if (gm.Item1 == rayResult.ModelHit)
                    {
                        hitgeo = (GeometryModel3D)rayResult.ModelHit;
                        gasit = true;
                        tt = GetIsToolTip(gm.Item1);
                        MessageBox.Show(tt.Content.ToString());
                    }
                }
                foreach (var gm in linesmodel)
                {
                    if (gm == rayResult.ModelHit)
                    {
                        hitgeo = (GeometryModel3D)rayResult.ModelHit;
                        gasit = true;
                        tt = GetIsToolTip(gm);
                        MessageBox.Show(tt.Content.ToString());
                        string id = tt.Content.ToString().Split(',')[0].Split(':')[1].Substring(1);
                        foreach (var item in lines)
                        {
                            long idd=long.Parse(id);
                            if(item.ID==idd)
                            {
                                id1 = (long)item.FirstEnd;
                                id2 = (long)item.SecondEnd;
                                map.Children.Remove(models[(long)item.FirstEnd].Item1);
                                map.Children.Remove(models[(long)item.SecondEnd].Item1);
                                f1 = new GeometryModel3D(models[(long)item.FirstEnd].Item1.Geometry, models[(long)item.FirstEnd].Item1.Material);
                                f1.Material = new DiffuseMaterial(Brushes.Pink);
                                f2 = new GeometryModel3D(models[(long)item.SecondEnd].Item1.Geometry, models[(long)item.SecondEnd].Item1.Material);
                                f2.Material = new DiffuseMaterial(Brushes.Pink);
                                map.Children.Add(f1);
                                map.Children.Add(f2);
                            }
                        }
                    }
                }
                if (!gasit)
                {
                    hitgeo = null;
                }
            }
            return HitTestResultBehavior.Stop;
        }

        private void viewport1_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            startRotation = e.GetPosition(this);
            System.Windows.Point mouseposition = e.GetPosition(viewport1);
            Point3D testpoint3D = new Point3D(mouseposition.X, mouseposition.Y, 0);
            Vector3D testdirection = new Vector3D(mouseposition.X, mouseposition.Y, 10);
            PointHitTestParameters pointparams =
                     new PointHitTestParameters(mouseposition);
            RayHitTestParameters rayparams =
                     new RayHitTestParameters(testpoint3D, testdirection);
            
            hitgeo = null;
            VisualTreeHelper.HitTest(viewport1, null, HTResult, pointparams);
        }

        private void viewport1_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            viewport1.ReleaseMouseCapture();
        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in substation)
            {
                map.Children.Add(item);
            }
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var item in substation)
            {
                map.Children.Remove(item);
            }
        }

        private void MenuItem_Checked_1(object sender, RoutedEventArgs e)
        {
            foreach (var item in node)
            {
                map.Children.Add(item);
            }
        }

        private void MenuItem_Unchecked_1(object sender, RoutedEventArgs e)
        {
            foreach (var item in node)
            {
                map.Children.Remove(item);
            }
        }

        private void MenuItem_Checked_2(object sender, RoutedEventArgs e)
        {
            foreach (var item in switchc)
            {
                map.Children.Add(item);
            }
        }

        private void MenuItem_Unchecked_2(object sender, RoutedEventArgs e)
        {
            foreach (var item in switchc)
            {
                map.Children.Remove(item);
            }
        }
    }
}