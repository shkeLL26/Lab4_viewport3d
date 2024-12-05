using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Windows.Forms.AxHost;

namespace WPF_lab4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event Action<int> dataSender;
        private int selectedIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            MyInit();
            tabControl.SelectionChanged += TabControl_SelectionChanged;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedIndex = tabControl.SelectedIndex;
            dataSender?.Invoke(selectedIndex + 1);
        }

        private void MyInit()
        {
            TabItem tabItem1 = new TabItem();
            //WPF_lab4.Function3DControl myControl1 = new WPF_lab4.Function3DControl(1, this);
            WPF_lab4.Function3DControl myControl1 = new WPF_lab4.Function3DControl(1, tabItem1);
            tabItem1.Header = "Sinx + Cosy";
            tabItem1.Content = myControl1;
            tabControl.Items.Add(tabItem1);

            TabItem tabItem2 = new TabItem();
            //WPF_lab4.Function3DControl myControl2 = new WPF_lab4.Function3DControl(2, this);
            WPF_lab4.Function3DControl myControl2 = new WPF_lab4.Function3DControl(2, tabItem2);
            tabItem2.Header = "-0.3x + 0.1y";
            tabItem2.Content = myControl2;
            tabControl.Items.Add(tabItem2);

            TabItem tabItem3 = new TabItem();
            //WPF_lab4.Function3DControl myControl3 = new WPF_lab4.Function3DControl(3, this);
            WPF_lab4.Function3DControl myControl3 = new WPF_lab4.Function3DControl(3, tabItem3);
            tabItem3.Header = "arctg(x - y)";
            tabItem3.Content = myControl3;
            tabControl.Items.Add(tabItem3);

            TabItem tabItem4 = new TabItem();
            WPF_lab4.Function3DControl myControl4 = new WPF_lab4.Function3DControl(4, tabItem4);
            tabItem4.Header = "-|x/4 + y/4|";
            tabItem4.Content = myControl4;
            tabControl.Items.Add(tabItem4);

        }
    }
}
