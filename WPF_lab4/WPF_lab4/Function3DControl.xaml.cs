using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Windows.Media.Color;

namespace WPF_lab4
{
    public partial class Function3DControl : UserControl
    {
        private PerspectiveCamera TheCamera = null;
        private CameraController camControl = null;

        private int funcNum = 0;
        private IFunction function = null;

        private UIElement mainWindow;

        private System.Windows.Forms.Timer timer;

        private int fallingIndex = -1;
        private double h;
        private double time = -1;
        private double lastX;
        private double lastY;
        private double lastZ;
        private List<Point3D> ballPoints = new List<Point3D>();
        private MeshGeometry3D deformationGeometry;

        public Function3DControl(int funcNum, UIElement recievedElement)
        {
            InitializeComponent();
            this.funcNum = funcNum;

            if (funcNum == 1) function = new SinCos();
            else if (funcNum == 2) function = new Linear();
            else if (funcNum == 3) function = new Arctan();
            else if (funcNum == 4) function = new Abs();
            RecreateModels();
            this.Loaded += Function3DControl_Loaded;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 10; 
            timer.Tick += Timer_Tick;
            mainWindow = recievedElement;

        }

        private void Function3DControl_Loaded(object sender, RoutedEventArgs e)
        {
            DefineCamera(viewport3D);
            mainWindow.MouseLeftButtonDown += LeftClick;
        }

        #region definers
        private void RecreateModels()
        {
            visual.Children.Clear();
            ModelsAdder(Cube(new Vector3D(0.02, 20, 0.02), new Vector3D(0, 0, 0), Colors.Black));
            ModelsAdder(Cube(new Vector3D(20, 0.02, 0.02), new Vector3D(0, 0, 0), Colors.Black));
            ModelsAdder(Cube(new Vector3D(0.02, 0.02, 20), new Vector3D(0, 0, 0), Colors.Black));
            ModelsAdder(Cube(new Vector3D(60, 60, 60), new Vector3D(0, 0, 0), Colors.White));
            ModelsAdder(Surface(-10, 10, -10, 10, 150, 150));
        }

        private void DefineCamera(Viewport3D viewport)
        {
            TheCamera = new PerspectiveCamera();
            TheCamera.FieldOfView = 60;
            camControl = new CameraController(TheCamera, viewport, mainWindow, light);
        }

        private DiffuseMaterial GetImage(int choice)
        {
            Random random = new Random();
            ImageBrush imageBrush = new ImageBrush();
            Bitmap bmp;
            int grad = random.Next(0, 3);
            //if (choice == -1) bmp = new Bitmap(Properties.Resources.Pepe);
            if(grad == 0) bmp = new Bitmap(Properties.Resources.Grad);
            else if (grad == 1) bmp = new Bitmap(Properties.Resources.Grad1);
            else bmp = new Bitmap(Properties.Resources.Grad2);
            imageBrush.ImageSource = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            //imageBrush.ImageSource = new BitmapImage(new Uri("F:\\Egor\\foto\\24454.jpg"));
            //if (imageBrush.ImageSource == null) MessageBox.Show("blin");
            imageBrush.Stretch = Stretch.Fill;
            imageBrush.TileMode = TileMode.Tile;
            return new DiffuseMaterial(imageBrush);
        }
        #endregion

        #region event handlers
        private void Timer_Tick(object sender, EventArgs e)
        {
            time+=0.01;
            bool flag = false;
            double newZ = h - (9.8 * time * time / 2);
            if (newZ - 1  <= function.calc(lastX, lastY))
            {
                for (int i = 0; i < deformationGeometry.Positions.Count; i++)
                {
                    if (CheckSphere(new Point3D(lastX, lastZ, lastY),
                        new Point3D(deformationGeometry.Positions[i].X, deformationGeometry.Positions[i].Y, deformationGeometry.Positions[i].Z), 1.2))
                    {
                        deformationGeometry.Positions[i] = new Point3D(deformationGeometry.Positions[i].X, deformationGeometry.Positions[i].Y - lastZ  + newZ, deformationGeometry.Positions[i].Z);
                    }
                }
            }
            foreach (Point3D pnt in ballPoints)
            {
                if (CheckDistanseBetweenSpheres(new Point3D(lastX, lastZ, lastY), 1.4, pnt, 1.4))
                {
                    visual.Children.RemoveAt(fallingIndex);
                    fallingIndex = -1;
                    time = -1;
                    timer.Stop();
                    ModelsAdder(Sphere(1, 30, 30, new Vector3D(lastX, lastZ, lastY), Colors.Red));
                    ballPoints.Add(new Point3D(lastX, lastZ, lastY));
                    while (newZ - 1 <= function.calc(lastX, lastY) && !flag)
                    {
                        flag = true;
                        for (int i = 0; i < deformationGeometry.Positions.Count; i++)
                        {
                            if (CheckSphere(new Point3D(lastX, lastZ, lastY),
                                new Point3D(deformationGeometry.Positions[i].X, deformationGeometry.Positions[i].Y, deformationGeometry.Positions[i].Z), 1.2))
                            {
                                deformationGeometry.Positions[i] = new Point3D(deformationGeometry.Positions[i].X, deformationGeometry.Positions[i].Y - 0.05, deformationGeometry.Positions[i].Z);
                                flag = false;
                            }
                        }
                    }
                    return;
                }
            }
            if (newZ < function.calc(lastX, lastY))
            {
                visual.Children.RemoveAt(fallingIndex);
                fallingIndex = -1;
                time = -1;
                timer.Stop();
                ModelsAdder(Sphere(1, 30, 30, new Vector3D(lastX, function.calc(lastX, lastY), lastY), Colors.Red));
                ballPoints.Add(new Point3D(lastX, function.calc(lastX, lastY), lastY));
                while (newZ - 1 <= function.calc(lastX, lastY) && !flag)
                {
                    flag = true;
                    for (int i = 0; i < deformationGeometry.Positions.Count; i++)
                    {
                        if (CheckSphere(new Point3D(lastX, lastZ, lastY),
                            new Point3D(deformationGeometry.Positions[i].X, deformationGeometry.Positions[i].Y, deformationGeometry.Positions[i].Z), 1.2))
                        {
                            deformationGeometry.Positions[i] = new Point3D(deformationGeometry.Positions[i].X, deformationGeometry.Positions[i].Y - 0.05, deformationGeometry.Positions[i].Z);
                            flag = false;
                        }
                    }
                }
                return;
            }
            visual.Children.RemoveAt(fallingIndex);
            ModelsAdder(Sphere(1, 30, 30, new Vector3D(lastX, newZ, lastY), Colors.Red));
            lastZ = newZ;
        }

        private void LeftClick(object sender, MouseButtonEventArgs e)
        {
            if (fallingIndex == -1)
            {
                double x, y, z;
                x = TheCamera.Position.X;
                y = TheCamera.Position.Y;
                z = TheCamera.Position.Z;
                fallingIndex = ModelsAdder(Sphere(1, 30, 30, new Vector3D(x / 2, y / 2, z / 2), Colors.Red));
                h = y / 2;
                if (h > function.calc(x / 2, z / 2))
                {
                    timer.Start();
                    lastX = x / 2;
                    lastY = z / 2;
                    lastZ = y / 2;
                    time = 0;
                }
                else
                {
                    fallingIndex = -1;
                    ballPoints.Add(new Point3D(x / 2, y / 2, z / 2));
                }
                deformationGeometry = ((visual.Children[4] as ModelVisual3D).Content as GeometryModel3D).Geometry as MeshGeometry3D; 
            }
        }

        private int ModelsAdder(GeometryModel3D recievedModel)
        {
            ModelVisual3D model = new ModelVisual3D();
            model.Content = recievedModel;
            visual.Children.Add(model);
            return (visual.Children.Count - 1);
        }
        #endregion

        #region 3D models
        public GeometryModel3D Cube(Vector3D LWH, Vector3D center, Color recievedColor)
        {

            MeshGeometry3D meshGeometry = new MeshGeometry3D();
            DiffuseMaterial diffuseMaterial = new DiffuseMaterial();
            //PointCollection textureCoordinates = new PointCollection();

            diffuseMaterial.Brush = new SolidColorBrush(recievedColor);

            meshGeometry.Positions.Add(new Point3D(center.X - LWH.X / 2, center.Y - LWH.Y / 2, center.Z - LWH.Z / 2));
            meshGeometry.Positions.Add(new Point3D(center.X + LWH.X / 2, center.Y - LWH.Y / 2, center.Z - LWH.Z / 2));
            meshGeometry.Positions.Add(new Point3D(center.X - LWH.X / 2, center.Y + LWH.Y / 2, center.Z - LWH.Z / 2));
            meshGeometry.Positions.Add(new Point3D(center.X + LWH.X / 2, center.Y + LWH.Y / 2, center.Z - LWH.Z / 2));
            meshGeometry.Positions.Add(new Point3D(center.X - LWH.X / 2, center.Y - LWH.Y / 2, center.Z + LWH.Z / 2));
            meshGeometry.Positions.Add(new Point3D(center.X + LWH.X / 2, center.Y - LWH.Y / 2, center.Z + LWH.Z / 2));
            meshGeometry.Positions.Add(new Point3D(center.X - LWH.X / 2, center.Y + LWH.Y / 2, center.Z + LWH.Z / 2));
            meshGeometry.Positions.Add(new Point3D(center.X + LWH.X / 2, center.Y + LWH.Y / 2, center.Z + LWH.Z / 2));

            meshGeometry.TriangleIndices.Add(0);
            meshGeometry.TriangleIndices.Add(2);
            meshGeometry.TriangleIndices.Add(1);

            meshGeometry.TriangleIndices.Add(1);
            meshGeometry.TriangleIndices.Add(2);
            meshGeometry.TriangleIndices.Add(3);

            meshGeometry.TriangleIndices.Add(0);
            meshGeometry.TriangleIndices.Add(4);
            meshGeometry.TriangleIndices.Add(2);

            meshGeometry.TriangleIndices.Add(2);
            meshGeometry.TriangleIndices.Add(4);
            meshGeometry.TriangleIndices.Add(6);

            meshGeometry.TriangleIndices.Add(0);
            meshGeometry.TriangleIndices.Add(1);
            meshGeometry.TriangleIndices.Add(4);

            meshGeometry.TriangleIndices.Add(1);
            meshGeometry.TriangleIndices.Add(5);
            meshGeometry.TriangleIndices.Add(4);

            meshGeometry.TriangleIndices.Add(1);
            meshGeometry.TriangleIndices.Add(7);
            meshGeometry.TriangleIndices.Add(5);

            meshGeometry.TriangleIndices.Add(1);
            meshGeometry.TriangleIndices.Add(3);
            meshGeometry.TriangleIndices.Add(7);

            meshGeometry.TriangleIndices.Add(4);
            meshGeometry.TriangleIndices.Add(5);
            meshGeometry.TriangleIndices.Add(6);

            meshGeometry.TriangleIndices.Add(7);
            meshGeometry.TriangleIndices.Add(6);
            meshGeometry.TriangleIndices.Add(5);

            meshGeometry.TriangleIndices.Add(2);
            meshGeometry.TriangleIndices.Add(6);
            meshGeometry.TriangleIndices.Add(3);

            meshGeometry.TriangleIndices.Add(3);
            meshGeometry.TriangleIndices.Add(6);
            meshGeometry.TriangleIndices.Add(7);

            /*textureCoordinates.Add(new System.Windows.Point(0, 0));
            textureCoordinates.Add(new System.Windows.Point(1, 0));
            textureCoordinates.Add(new System.Windows.Point(1, 1));
            textureCoordinates.Add(new System.Windows.Point(0, 1));
            textureCoordinates.Add(new System.Windows.Point(0, 0));
            textureCoordinates.Add(new System.Windows.Point(1, 0));
            textureCoordinates.Add(new System.Windows.Point(1, 1));
            textureCoordinates.Add(new System.Windows.Point(0, 1));

            meshGeometry.TextureCoordinates = textureCoordinates;*/

            GeometryModel3D model = new GeometryModel3D(meshGeometry, diffuseMaterial);
            model.BackMaterial = diffuseMaterial;
            //DiffuseMaterial dfmt = GetImage(-1);
            //GeometryModel3D model = new GeometryModel3D(meshGeometry, dfmt);
            //model.BackMaterial = dfmt;
            return model;
        }

        public GeometryModel3D Sphere(double radius, int uDiv, int vDiv, Vector3D center, Color recievedColor)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            for (int i = 0; i <= uDiv; ++i)
            {
                double theta = i * Math.PI / uDiv;
                for (int j = 0; j <= vDiv; ++j)
                {
                    double phi = j * 2 * Math.PI / vDiv;
                    Point3D point = new Point3D(
                        center.X + radius * Math.Sin(theta) * Math.Cos(phi),
                        center.Y + radius * Math.Cos(theta),
                        center.Z + radius * Math.Sin(theta) * Math.Sin(phi));
                    mesh.Positions.Add(point);
                }
            }

            for (int i = 0; i < uDiv; ++i)
            {
                for (int j = 0; j < vDiv; ++j)
                {
                    int i1 = i * (vDiv + 1) + j;
                    int i2 = i * (vDiv + 1) + j + 1;
                    int i3 = (i + 1) * (vDiv + 1) + j;
                    int i4 = (i + 1) * (vDiv + 1) + j + 1;

                    mesh.TriangleIndices.Add(i1);
                    mesh.TriangleIndices.Add(i2);
                    mesh.TriangleIndices.Add(i3);

                    mesh.TriangleIndices.Add(i2);
                    mesh.TriangleIndices.Add(i4);
                    mesh.TriangleIndices.Add(i3);
                }
            }
            return new GeometryModel3D(mesh, new DiffuseMaterial(new SolidColorBrush(recievedColor)));
        }

        public GeometryModel3D Cylinder(double radius, double height, int uDiv, int variant)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            double halfHeight = height / 2;

            // Верхняя и нижняя крышки
            if (variant == 1)
            {
                for (int i = 0; i <= uDiv; i++)
                {
                    double angle = i * 2 * Math.PI / uDiv;
                    mesh.Positions.Add(new Point3D(radius * Math.Cos(angle), halfHeight, radius * Math.Sin(angle))); //верх
                    mesh.Positions.Add(new Point3D(radius * Math.Cos(angle), -halfHeight, radius * Math.Sin(angle)));//низ
                }
            }
            if (variant == 2)
            {
                for (int i = 0; i <= uDiv; i++)
                {
                    double angle = i * 2 * Math.PI / uDiv;
                    mesh.Positions.Add(new Point3D(radius * Math.Sin(angle), radius * Math.Cos(angle), halfHeight)); //верх
                    mesh.Positions.Add(new Point3D(radius * Math.Sin(angle), radius * Math.Cos(angle), -halfHeight));//низ
                }
            }
            if (variant == 3)
            {
                for (int i = 0; i <= uDiv; i++)
                {
                    double angle = i * 2 * Math.PI / uDiv;
                    mesh.Positions.Add(new Point3D(halfHeight, radius * Math.Sin(angle), radius * Math.Cos(angle))); //верх
                    mesh.Positions.Add(new Point3D(-halfHeight, radius * Math.Sin(angle), radius * Math.Cos(angle)));//низ
                }
            }

            // Боковая поверхность
            for (int i = 0; i < uDiv; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int p1 = i * 2 + j;
                    int p2 = (i + 1) * 2 + j;
                    mesh.TriangleIndices.Add(p1);
                    mesh.TriangleIndices.Add(p2);
                    mesh.TriangleIndices.Add(p1 + 1);
                }
            }

            return new GeometryModel3D(mesh, new DiffuseMaterial(new SolidColorBrush(Colors.Black)));
        }

        public GeometryModel3D Surface(double minX, double maxX, double minY, double maxY, int xDiv, int yDiv)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            PointCollection textureCoordinates = new PointCollection();

            double dx = (maxX - minX) / xDiv;
            double dy = (maxY - minY) / yDiv;
            double maxZ = 0;
            double minZ = 0;

            for (int i = 0; i <= xDiv; i++)
            {
                for (int j = 0; j <= yDiv; j++)
                {
                    double x = minX + i * dx;
                    double y = minY + j * dy;
                    double z = function.calc(x, y);
                    if (i == 0 && j == 0) maxZ = minZ = z;
                    else if (z > maxZ) maxZ = z;
                    else if (z < minZ) minZ = z;
                }
            }

            for (int i = 0; i <= xDiv; i++)
            {
                for (int j = 0; j <= yDiv; j++)
                {
                    double x = minX + i * dx;
                    double y = minY + j * dy;
                    double z = function.calc(x, y);
                    mesh.Positions.Add(new Point3D(x, z, y));
                    double v = (z - minZ) / (maxZ - minZ);
                    //double u = (x + y - minX + minY) / (maxX + maxY - minX + minY);
                    double u = (x - minX) / (maxX - minX);
                    textureCoordinates.Add(new System.Windows.Point(u, -v));
                }
            }

            for (int i = 0; i < xDiv; i++)
            {
                for (int j = 0; j < yDiv; j++)
                {
                    int p1 = i * (yDiv + 1) + j;
                    int p2 = i * (yDiv + 1) + j + 1;
                    int p3 = (i + 1) * (yDiv + 1) + j;
                    int p4 = (i + 1) * (yDiv + 1) + j + 1;

                    mesh.TriangleIndices.Add(p1);
                    mesh.TriangleIndices.Add(p2);
                    mesh.TriangleIndices.Add(p3);

                    mesh.TriangleIndices.Add(p2);
                    mesh.TriangleIndices.Add(p4);
                    mesh.TriangleIndices.Add(p3);
                }
            }

            mesh.TextureCoordinates = textureCoordinates;

            //GeometryModel3D surfaceModel = new GeometryModel3D(mesh, new DiffuseMaterial(new SolidColorBrush(Colors.Yellow)));
            //surfaceModel.BackMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Yellow));
            DiffuseMaterial gradientMaterial = GetImage(0);
            GeometryModel3D surfaceModel = new GeometryModel3D(mesh, gradientMaterial);
            surfaceModel.BackMaterial = gradientMaterial;
            return surfaceModel;
        }
        #endregion

        #region checkers
        private bool CheckCube(Point3D cubeCenter, Point3D pnt, double length)
        {
            if ((pnt.X >= cubeCenter.X - length/2 || pnt.X <= cubeCenter.X + length / 2) &&
                (pnt.Y >= cubeCenter.Y - length / 2 || pnt.Y <= cubeCenter.Y + length / 2) &&
                (pnt.Z >= cubeCenter.Z - length / 2 || pnt.Z <= cubeCenter.Z + length / 2)) return true;
            return false;
        }

        private bool CheckSphere(Point3D roundCenter, Point3D pnt, double radius)
        {
            if (((roundCenter.X - pnt.X) * (roundCenter.X - pnt.X)
                + (roundCenter.Y - pnt.Y) * (roundCenter.Y - pnt.Y)
                + (roundCenter.Z - pnt.Z) * (roundCenter.Z - pnt.Z)) <= radius * radius) return true;
            return false;
        }

        private bool CheckDistanseBetweenSpheres(Point3D roundCenter1, double radius1, Point3D roundCenter2, double radius2)
        {
            if (((roundCenter1.X - roundCenter2.X) * (roundCenter1.X - roundCenter2.X)
                + (roundCenter1.Y - roundCenter2.Y) * (roundCenter1.Y - roundCenter2.Y)
                + (roundCenter1.Z - roundCenter2.Z) * (roundCenter1.Z - roundCenter2.Z)) <= radius1 * radius1 + radius2 * radius2) return true;
            return false;
        }
        #endregion
    }
}