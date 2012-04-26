using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

[assembly: CLSCompliant(true)]

namespace Sparkle_Configuration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //ConfigurationWindow a  = new ConfigurationWindow();

        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            //a.Show();

            //BotCommunication b = new BotCommunication();
            //bool testconn = b.connect(textBoxIP.Text, textBoxPort.Text, textBoxPassword.Text);
            //Debug.WriteLine("Connection Attempt: " + testconn);
        }

        private void buttonEditLocal_Click(object sender, RoutedEventArgs e)
        {
            this.textBlockStatus.Text = "Loading Configuration...";
            this.progressBarStatus.Value = 25;
            ConfigurationWindow a = new ConfigurationWindow();
            //a.Owner = this;
            this.textBlockStatus.Text = "Parsing JSON...";
            this.progressBarStatus.Value = 100;
            a.ShowDialog();
            this.textBlockStatus.Text = "Not Connected";
            this.progressBarStatus.Value = 0;
        }
    }
}
