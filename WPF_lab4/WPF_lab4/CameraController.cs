using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WPF_lab4
{
    internal class CameraController
    {
        // The minimum value of each conversion
        public double CameraR = 20;
        public const double CameraDTheta = Math.PI / 30;
        public const double CameraDPhi = Math.PI / 30;

        public PerspectiveCamera cm = null;
        private UIElement mainWindow = null;

        public Point3D cmPosition { get; set; } = new Point3D(4, 0.5, 5);
        public double CameraTheta = 44.75;
        public double CameraPhi = 45;

        private Point ptLast;
        private DirectionalLight light;

        public CameraController(PerspectiveCamera camera, Viewport3D viewport, UIElement mainWindow, DirectionalLight light)
        {
            cm = camera;
            this.light = light;
            viewport.Camera = cm;
            this.mainWindow = mainWindow;
            this.mainWindow.MouseRightButtonDown += mainWindow_RightDown;
            this.mainWindow.MouseWheel += mainWindow_Wheel;
            PositionCameraMouse();
        }

        private void mainWindow_RightDown(object sender, MouseButtonEventArgs e)
        {
            mainWindow.CaptureMouse();
            mainWindow.MouseMove += MainWindow_MouseMove;
            mainWindow.MouseUp += MainWindow_MouseUp;
            ptLast = e.GetPosition(mainWindow);
        }

        private void mainWindow_Wheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && CameraR < 28) CameraR *= 1.1f;
            else if (e.Delta < 0 && CameraR > 5) CameraR /= 1.1f;
            PositionCameraMouse();
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mainWindow.ReleaseMouseCapture();
            mainWindow.MouseMove -= MainWindow_MouseMove;
            mainWindow.MouseUp -= MainWindow_MouseUp;
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            const double xscale = 0.1;
            const double yscale = 0.1;

            Point newPoint = e.GetPosition(mainWindow);
            double dx = newPoint.X - ptLast.X;
            double dy = newPoint.Y - ptLast.Y;

            CameraTheta -= dx * CameraDTheta * xscale;
            CameraPhi -= dy * CameraDPhi * yscale;

            ptLast = newPoint;
            PositionCameraMouse();
        }

        private void PositionCameraMouse()
        {
            double x, y, z;

            y = CameraR * Math.Cos(CameraPhi);
            double h = CameraR * Math.Sin(CameraPhi);
            x = h * Math.Sin(CameraTheta);
            z = h * Math.Cos(CameraTheta);

            cm.Position = new Point3D(x, y, z);
            cm.LookDirection = new Vector3D(-x, -y, -z);
            cm.UpDirection = new Vector3D(0, 1, 0);
            
            light.Direction = new Vector3D(-x, -y, -z);
        }
    }
}
