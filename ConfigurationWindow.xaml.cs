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
using System.Web.Script.Serialization;
using System.IO;
using System.Net;


namespace Sparkle_Configuration
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public ConfigurationWindow()
        {
            InitializeComponent();

            
        }

        public dynamic JSONConfiguration;
        public JavaScriptSerializer serializer;
        public string defaultJSONConfiguration = "";

        public string configurationFilename = "";

        public DatabaseConfig dbconfig;

        public void updateFieldsFromConfiguration(dynamic config)
        {
            this.textBoxAuth.Text = config.botinfo.auth;
            this.textBoxUserID.Text = config.botinfo.userid;
            this.textBoxBotName.Text = config.botinfo.botname;
            this.textBoxRoomID.Text = config.roomid;
            this.textBoxAdmin.Text = config.admin;

            this.checkBoxUseDatabase.IsChecked = config.database.usedb;
            this.checkBoxLogChat.IsChecked = config.database.logchat;
            this.textBoxDBUser.Text = config.database.login.user;
            this.passwordBoxDBPassword.Password = config.database.login.password;
            this.checkBoxLogToConsole.IsChecked = config.consolelog;

            this.checkBoxUseLastfm.IsChecked = config.lastfm.useapi;
            this.textBoxLastfmAPIKey.Text = config.lastfm.lastfmkey;
            this.checkBoxUseTCP.IsChecked = config.tcp.usetcp;
            this.textBoxTCPPort.Text = config.tcp.port.ToString();
            this.textBoxTCPHostname.Text = config.tcp.host;
            this.checkBoxUseHTTP.IsChecked = config.http.usehttp;
            this.textBoxHTTPPort.Text = config.http.port.ToString();
            this.textBoxHTTPHostname.Text = config.http.host;

            this.checkBoxRespond.IsChecked = config.responses.respond;

            //if (this.dbconfig.IsVisible)
            //{
                this.dbconfig.textBoxDatabase.Text = config.database.dbname;
                this.dbconfig.textBoxSong.Text = config.database.tablenames.song;
                this.dbconfig.textBoxChat.Text = config.database.tablenames.chat;
                this.dbconfig.textBoxUser.Text = config.database.tablenames.user;
                this.dbconfig.textBoxHoliday.Text = config.database.tablenames.holiday;

            //}
            //------------------
        }

        public void updateFieldsFromConfiguration()
        {
            this.updateFieldsFromConfiguration(this.JSONConfiguration);
        }

        public void updateLiveEditfromFields()
        {
            // something is calling this before the form even appears, sanity check
            if (this.textBoxTCPPort.Text.Equals("") || this.textBoxHTTPPort.Text.Equals("") || (JSONConfiguration == null))
            {
                return;
            }
            try
            {
            dynamic config = JSONConfiguration;

                config.botinfo.auth = this.textBoxAuth.Text;
                config.botinfo.userid = this.textBoxUserID.Text;
                config.botinfo.botname = this.textBoxBotName.Text;
                config.roomid = this.textBoxRoomID.Text;
                config.admin = this.textBoxAdmin.Text;

                config.database.usedb = this.checkBoxUseDatabase.IsChecked;
                config.database.logchat = this.checkBoxLogChat.IsChecked;
                config.database.login.user = this.textBoxDBUser.Text;
                config.database.login.password = this.passwordBoxDBPassword.Password;
                config.consolelog = this.checkBoxLogToConsole.IsChecked;

                config.lastfm.useapi = this.checkBoxUseLastfm.IsChecked;
                config.lastfm.lastfmkey = this.textBoxLastfmAPIKey.Text;
                config.tcp.usetcp = this.checkBoxUseTCP.IsChecked;
                config.tcp.port = Convert.ToInt32(this.textBoxTCPPort.Text);
                config.tcp.host = this.textBoxTCPHostname.Text;
                config.http.usehttp = this.checkBoxUseHTTP.IsChecked;
                config.http.port = Convert.ToInt32(this.textBoxHTTPPort.Text);
                config.http.host = this.textBoxHTTPHostname.Text;

                config.responses.respond = this.checkBoxRespond.IsChecked;

                if (this.dbconfig.IsVisible)
                {
                    config.database.dbname = this.dbconfig.textBoxDatabase.Text;
                    config.database.tablenames.song = this.dbconfig.textBoxSong.Text;
                    config.database.tablenames.chat = this.dbconfig.textBoxChat.Text;
                    config.database.tablenames.user = this.dbconfig.textBoxUser.Text;
                    config.database.tablenames.holiday = this.dbconfig.textBoxHoliday.Text;
                }
                //------------------

                this.textBoxJSON.Text = config.ToString();
                //JSONConfiguration = config;
            }
            catch (Exception)
            {
                //MessageBox.Show("Error: Invalid JSON, reverting to last valid settings!");
                //throw;
            }
            //this.updateFieldsFromConfiguration();
        }

        private void checkBoxUseDatabase_Checked(object sender, RoutedEventArgs e)
        {
            this.checkBoxLogChat.IsEnabled = true;
            this.textBoxDBUser.IsEnabled = true;
            this.passwordBoxDBPassword.IsEnabled = true;
            this.buttonAdvancedDBOptions.IsEnabled = true;
            this.updateLiveEdit_Changed(sender, e);
        }

        private void checkBoxUseDatabase_Unchecked(object sender, RoutedEventArgs e)
        {
            this.checkBoxLogChat.IsEnabled = false;
            this.textBoxDBUser.IsEnabled = false;
            this.passwordBoxDBPassword.IsEnabled = false;
            this.buttonAdvancedDBOptions.IsEnabled = false;
            this.updateLiveEdit_Changed(sender, e);
        }

        private void textBoxJSON_LostFocus(object sender, RoutedEventArgs e)
        {

            try
            {
                dynamic newConfiguration = serializer.Deserialize(this.textBoxJSON.Text, typeof(object));
                this.textBoxJSON.Text = newConfiguration.ToString();
                JSONConfiguration = newConfiguration;
                this.updateFieldsFromConfiguration();
            }
            catch (Exception)
            {
                MessageBox.Show("Error: Invalid JSON, reverting to last valid settings!");
                this.textBoxJSON.Text = JSONConfiguration.ToString();
            }
        }

        

        private void checkBoxUseLastfm_Checked(object sender, RoutedEventArgs e)
        {
            this.textBoxLastfmAPIKey.IsEnabled = true;
            this.updateLiveEdit_Changed(sender, e);
        }

        private void checkBoxUseLastfm_Unchecked(object sender, RoutedEventArgs e)
        {
            this.textBoxLastfmAPIKey.IsEnabled = false;
            this.updateLiveEdit_Changed(sender, e);
        }

        private void checkBoxUseTCP_Checked(object sender, RoutedEventArgs e)
        {
            this.textBoxTCPPort.IsEnabled = true;
            this.textBoxTCPHostname.IsEnabled = true;
            this.updateLiveEdit_Changed(sender, e);
        }

        private void checkBoxUseTCP_Unchecked(object sender, RoutedEventArgs e)
        {
            this.textBoxTCPPort.IsEnabled = false;
            this.textBoxTCPHostname.IsEnabled = false;
            this.updateLiveEdit_Changed(sender, e);
        }

        private void checkBoxUseHTTP_Checked(object sender, RoutedEventArgs e)
        {
            this.textBoxHTTPPort.IsEnabled = true;
            this.textBoxHTTPHostname.IsEnabled = true;
            this.updateLiveEdit_Changed(sender, e);
        }

        private void checkBoxUseHTTP_Unchecked(object sender, RoutedEventArgs e)
        {
            this.textBoxHTTPPort.IsEnabled = false;
            this.textBoxHTTPHostname.IsEnabled = false;
            this.updateLiveEdit_Changed(sender, e);
        }

        private void updateLiveEdit_Changed(object sender, RoutedEventArgs e)
        {
            this.updateLiveEditfromFields();
        }

        private void updateLiveEdit_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.updateLiveEditfromFields();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            serializer = new JavaScriptSerializer();

            dbconfig = new DatabaseConfig();
            

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "config.json"; // Default file name
            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "JSON data (.json)|*.json"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            string json = "";
            //string filename = "";

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                configurationFilename = dlg.FileName;

                StreamReader streamReader = new StreamReader(configurationFilename);
                json = streamReader.ReadToEnd();
                streamReader.Close();
            }
            else
            {
                configurationFilename = "http://raw.github.com/sharedferret/Sparkle-Turntable-Bot/master/config.sample.json";

                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(configurationFilename);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                json = sr.ReadToEnd();
                sr.Close();
                myResponse.Close();
            }

            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

            JSONConfiguration = serializer.Deserialize(json, typeof(object));

            this.textBoxJSON.Text = JSONConfiguration.ToString();

            this.updateFieldsFromConfiguration();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter streamwriter = new StreamWriter(configurationFilename);
            streamwriter.Write(JSONConfiguration.ToString());
            streamwriter.Close();
        }

        private void buttonSaveAs_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "config.json"; // Default file name
            dlg.DefaultExt = ".json"; // Default file extension
            dlg.Filter = "JSON data (.json)|*.json"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                StreamWriter streamwriter = new StreamWriter(filename);
                streamwriter.Write(JSONConfiguration.ToString());
                streamwriter.Close();
            }
        }

        private void buttonRevert_Click(object sender, RoutedEventArgs e)
        {
            StreamReader streamReader = new StreamReader(configurationFilename);
            string json = streamReader.ReadToEnd();
            streamReader.Close();

            JSONConfiguration = serializer.Deserialize(json, typeof(object));
            this.textBoxJSON.Text = JSONConfiguration.ToString();
            this.updateFieldsFromConfiguration();
        }

        private void buttonAdvancedDBOptions_Click(object sender, RoutedEventArgs e)
        {
            dbconfig.Owner = this;
            dbconfig.Show();
            //this.updateFieldsFromConfiguration();
        }

        
     

    }
}
