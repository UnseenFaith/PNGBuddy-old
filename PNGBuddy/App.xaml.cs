using AutoUpdaterDotNET;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PNGBuddy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AutoUpdater.Start("https://raw.githubusercontent.com/UnseenFaith/PNGBuddy/master/version.xml");

            if (Settings.Default.UPDATE)
            {
                Settings.Default.Upgrade();
                Settings.Default.UPDATE = false;
                Settings.Default.Save();
            }
        }
    }
}
