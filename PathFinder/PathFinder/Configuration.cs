using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PathFinder.Utilities;

namespace PathFinder
{
    class Configuration
    {
        public static string HostIP;
        public static string HostUser;
        public static string HostPass;
        public static string HostDBName;
        public void Initialize()
        {
            HostIP = Utilities.IniFile.GetIniFileString(Application.StartupPath + "/config/setup.ini", "CONFIGURATION", "HostIP", "127.0.0.1");
            HostUser = Utilities.IniFile.GetIniFileString(Application.StartupPath + "/config/setup.ini", "CONFIGURATION", "HostUser", "osm");
            HostPass = Utilities.IniFile.GetIniFileString(Application.StartupPath + "/config/setup.ini", "CONFIGURATION", "HostPass", "");
            HostDBName = Utilities.IniFile.GetIniFileString(Application.StartupPath + "/config/setup.ini", "CONFIGURATION", "HostDBName", "australia");

        }
    }
}
