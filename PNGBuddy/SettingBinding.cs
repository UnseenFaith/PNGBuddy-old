using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PNGBuddy
{
    public class SettingBindingExtension : Binding
    {
        public SettingBindingExtension()
        {
            Initialize();
        }

        public SettingBindingExtension(string path) : base(path)
        {
            Initialize();
        }

        private void Initialize()
        {
            this.Source = Settings.Default;
            this.Mode = BindingMode.TwoWay;
            this.NotifyOnSourceUpdated = true;
            this.NotifyOnTargetUpdated = true;
        }
    }
}
