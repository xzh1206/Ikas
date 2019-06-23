﻿using System;
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
using System.Windows.Shapes;

using System.Text.RegularExpressions;
using System.Windows.Media.Animation;

using ClassLib;

namespace Ikas
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private MessageWindow messageWindow;

        public string SelectionForeground
        {
            get
            {
                return "#7F" + Design.NeonOrange;
            }
        }

        public string GreenForeground
        {
            get
            {
                return "#FF" + Design.NeonGreen;
            }
        }

        public string OrangeForeground
        {
            get
            {
                return "#FF" + Design.NeonOrange;
            }
        }

        public string RedForeground
        {
            get
            {
                return "#FF" + Design.NeonRed;
            }
        }

        private bool useProxy = false;
        private string language = "en-US";

        public SettingsWindow()
        {
            // Load language
            if (Depot.Language != null)
            {
                try
                {
                    ResourceDictionary lang = (ResourceDictionary)Application.LoadComponent(new Uri(@"assets/lang/" + Depot.Language + ".xaml", UriKind.Relative));
                    if (Resources.MergedDictionaries.Count > 0)
                    {
                        Resources.MergedDictionaries.Clear();
                    }
                    Resources.MergedDictionaries.Add(lang);
                }
                catch { }
            }
            // Initialize component
            InitializeComponent();
            // Add handler for global member
            Depot.LanguageChanged += new LanguageChangedEventHandler(LanguageChanged);
            // Prepare windows
            messageWindow = new MessageWindow();
            messageWindow.Opacity = 0;
            messageWindow.Visibility = Visibility.Hidden;
        }

        #region Control Event

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Activate animation, maybe WPF bug
            ((Storyboard)FindResource("fore_to_green")).Begin(lbOk);
            ((Storyboard)FindResource("fore_to_orange")).Begin(lbOk);
            ((Storyboard)FindResource("fore_to_red")).Begin(lbOk);
            ((Storyboard)FindResource("fore_to_white")).Begin(lbOk);
            // Load configuration
            txtSessionToken.Text = Depot.SessionToken;
            txtCookie.Text = Depot.Cookie;
            if (Depot.UseProxy)
            {
                LbUseProxyTrue_MouseDown(null, null);
                // Animation in former method may not be activated, maybe WPF bug
                lbUseProxyTrue.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF" + Design.NeonOrange));
            }
            else
            {
                LbUseProxyFalse_MouseDown(null, null);
                lbUseProxyFalse.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF" + Design.NeonOrange));
            }
            txtProxyHost.Text = Depot.ProxyHost;
            if (Depot.ProxyPort > 0)
            {
                txtProxyPort.Text = Depot.ProxyPort.ToString();
            }
            else
            {
                txtProxyPort.Text = "";
            }
            switch (Depot.Language)
            {
                case "en-US":
                    LbLanguageEnUs_MouseDown(null, null);
                    lbLanguageEnUs.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF" + Design.NeonOrange));
                    break;
                case "ja-JP":
                    LbLanguageJaJp_MouseDown(null, null);
                    lbLanguageJaJp.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF" + Design.NeonOrange));
                    break;
                default:
                    LbLanguageEnUs_MouseDown(null, null);
                    lbLanguageEnUs.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF" + Design.NeonOrange));
                    break;
            }
        }

        private void LbOk_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fore_to_green")).Begin(lbOk);
        }

        private void LbOk_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fore_to_white")).Begin(lbOk);
        }

        private void LbOk_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (txtCookie.Text.Trim() == "")
            {
                MessageBox.Show(Translate("You may enter a valid Cookie before closing the settings.", true), "Ikas", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!int.TryParse(txtProxyPort.Text, out _))
            {
                MessageBox.Show(Translate("You may enter a valid proxy port before closing the settings.", true), "Ikas", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int port = int.Parse(txtProxyPort.Text);
            if (port< 1 || port> 65535)
            {
                MessageBox.Show(Translate("You may enter a valid proxy port before closing the settings.", true), "Ikas", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Depot.SessionToken = txtSessionToken.Text;
            Depot.Cookie = txtCookie.Text;
            Depot.UseProxy = useProxy;
            Depot.ProxyHost = txtProxyHost.Text;
            Depot.ProxyPort = port;
            Depot.Language = language;
            // Clear failed counter
            Depot.ScheduleFailedCount = 0;
            Depot.BattleFailedCount = 0;
            ((Storyboard)FindResource("window_fade_out")).Begin(this);
        }

        private void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // DragMove();
        }

        private void LbWhatIsSessionToken_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowMessage(Translate("Session Token is..", true),
                string.Format(Translate("{0}\n{1}", true),
                Translate("A Session Token is a small piece of data used for automatic cookie generation.", true),
                Translate("Automatic cookie generation sends minimal data including the Session Token to Nintendo servers, and get cookie back for accessing SplatNet.", true)),
                e.GetPosition(this));
        }

        private void LbWhatIsSessionToken_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("window_fade_out")).Begin(messageWindow);
        }

        private void LbGetSessionToken_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fore_to_red")).Begin(lbGetSessionToken);
        }

        private void LbGetSessionToken_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fore_to_white")).Begin(lbGetSessionToken);
        }

        private void LbGetSessionToken_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void LbWhatIsCookie_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowMessage(Translate("Cookie is..", true),
                string.Format(Translate("{0}\n{1}\n{2}", true),
                Translate("A Cookie is a small piece of data sent from a website and stored on the user's computer.", true),
                Translate("Ikas uses cookies to access SplatNet and get schedule and battle data.", true),
                Translate("If you don't know how to get a cookie, you may fill out the Session Token, and the system will get one automatically.", true)),
                e.GetPosition(this));
        }

        private void LbWhatIsCookie_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("window_fade_out")).Begin(messageWindow);
        }

        private void LbUpdateCookie_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fore_to_red")).Begin(lbUpdateCookie);
        }

        private void LbUpdateCookie_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fore_to_white")).Begin(lbUpdateCookie);
        }

        private void LbUpdateCookie_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void LbUseProxyTrue_MouseDown(object sender, MouseButtonEventArgs e)
        {
            useProxy = true;
            ((Storyboard)FindResource("fore_to_orange")).Begin(lbUseProxyTrue);
            ((Storyboard)FindResource("fore_to_white")).Begin(lbUseProxyFalse);
        }

        private void LbUseProxyFalse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            useProxy = false;
            ((Storyboard)FindResource("fore_to_white")).Begin(lbUseProxyTrue);
            ((Storyboard)FindResource("fore_to_orange")).Begin(lbUseProxyFalse);
        }

        private void LbLanguageEnUs_MouseDown(object sender, MouseButtonEventArgs e)
        {
            language = "en-US";
            ((Storyboard)FindResource("fore_to_orange")).Begin(lbLanguageEnUs);
            ((Storyboard)FindResource("fore_to_white")).Begin(lbLanguageJaJp);
            ResourceDictionary lang = (ResourceDictionary)Application.LoadComponent(new Uri(@"assets/lang/" + language + ".xaml", UriKind.Relative));
            if (Resources.MergedDictionaries.Count > 0)
            {
                Resources.MergedDictionaries.Clear();
            }
            Resources.MergedDictionaries.Add(lang);
        }

        private void LbLanguageJaJp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            language = "ja-JP";
            ((Storyboard)FindResource("fore_to_white")).Begin(lbLanguageEnUs);
            ((Storyboard)FindResource("fore_to_orange")).Begin(lbLanguageJaJp);
            ResourceDictionary lang = (ResourceDictionary)Application.LoadComponent(new Uri(@"assets/lang/" + language + ".xaml", UriKind.Relative));
            if (Resources.MergedDictionaries.Count > 0)
            {
                Resources.MergedDictionaries.Clear();
            }
            Resources.MergedDictionaries.Add(lang);
        }

        private void LbClearCache_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fore_to_red")).Begin(lbClearCache);
        }

        private void LbClearCache_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("fore_to_white")).Begin(lbClearCache);
        }

        private void LbClearCache_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(Translate("Currently, we can not clear cache automatically, but we can guide you to the application data directory.", true), "Ikas", MessageBoxButton.OK, MessageBoxImage.Information);
            System.Diagnostics.Process.Start("explorer.exe", FileFolderUrl.ApplicationData);
        }

        private void LbWhatIsClearCache_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowMessage(Translate("Clear Cache is..", true),
                string.Format(Translate("{0}\n{1}\n{2}", true),
                Translate("When Ikas gets schedule and battle data, it will cache images including user icons, stages, weapons and gears.", true),
                Translate("Sometimes, due to network or other reasons, Ikas may not get images properly, which may cause wrong cached images.", true),
                Translate("You can help Ikas get back to normal operation by clearing the cached data.", true)),
                e.GetPosition(this));
        }

        private void LbWhatIsClearCache_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Storyboard)FindResource("window_fade_out")).Begin(messageWindow);
        }

        #endregion

        private void LanguageChanged()
        {
            ResourceDictionary lang = (ResourceDictionary)Application.LoadComponent(new Uri(@"assets/lang/" + Depot.Language + ".xaml", UriKind.Relative));
            if (Resources.MergedDictionaries.Count > 0)
            {
                Resources.MergedDictionaries.Clear();
            }
            Resources.MergedDictionaries.Add(lang);
        }

        private void ShowMessage(string title, string content, Point point)
        {
            messageWindow.SetContent(title, content);
            messageWindow.Top = point.Y + Top - messageWindow.Height / 2;
            messageWindow.Left = point.X + Left + 10;
            ((Storyboard)FindResource("window_fade_in")).Begin(messageWindow);
        }

        private string Translate(string s, bool isLocal = false)
        {
            try
            {
                if (isLocal)
                {
                    return (string)FindResource("settings_window-" + s);
                }
                else
                {
                    return (string)FindResource(s);
                }
            }
            catch
            {
                return s;
            }
        }
    }
}
