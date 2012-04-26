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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Sparkle_Configuration
{
    /// <summary>
    /// Interaction logic for DatabaseConfig.xaml
    /// </summary>
    public partial class DatabaseConfig : Window
    {
        public DatabaseConfig()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            //Hide Window
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
            {
                Hide();
                return null;
            }, null);
            //Do not close application
            e.Cancel = true;
        }

        private void textBoxDatabase_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!this.IsVisible)
            {
                return;
            }

            // Fields changed, update Live Edit
            ConfigurationWindow p = (ConfigurationWindow)this.Owner;
            p.updateLiveEditfromFields();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Load the settings from parent
            ConfigurationWindow p = (ConfigurationWindow)this.Owner;
            p.updateFieldsFromConfiguration();
        }
    }
}
