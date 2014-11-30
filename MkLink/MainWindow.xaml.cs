using MkLink.Code;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace MkLink
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click_Link(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            bt.IsEnabled = false;

            //开始执行
            string targetPath = this.Text_Target.Text.ToString();
            string linkPath = this.Text_Link.Text.ToString();
            string option = (this.OptionComboBox.SelectedItem as ComboBoxItem).Tag.ToString();
            string mode = (this.ModeComboBox.SelectedItem as ComboBoxItem).Tag.ToString();

            Result result = MkLink.Code.MkLink.Start(mode, option, linkPath, targetPath);
            System.Windows.MessageBox.Show(result.Info, "提示");
            //执行结束

            bt.IsEnabled = true;
        }

        private void Button_Click_FileDialog(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.DefaultExt = ".xml";
            ofd.Filter = "文本文件|*.txt";
            if (ofd.ShowDialog() == true)
            {
                this.Text_File.Text = ofd.FileName;
            }
        }

        private void Button_Click_Batch(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            bt.IsEnabled = false;
            Result result = MkLink.Code.MkLink.BatchStart(this.Text_File.Text);
            System.Windows.MessageBox.Show(result.Info, "提示");
            bt.IsEnabled = true;
        }
    }
}
