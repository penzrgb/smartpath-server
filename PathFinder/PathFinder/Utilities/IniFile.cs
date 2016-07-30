using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PathFinder.Utilities
{
    class IniFile
    {
        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW"
    , SetLastError = true
    , CharSet = CharSet.Unicode
    , ExactSpelling = true
    , CallingConvention = CallingConvention.StdCall)]

        public static extern int GetPrivateProfileString(
            string IpAppName,
            string IpKeyName,
            string IpDefault,
            string IpReturnString,
            int nSize,
            string IpFilename);

        public static List<string> GetCategories(string iniFile)
        {
            string returnString = new string(' ', 65536);
            GetPrivateProfileString(null, null, null, returnString, 65536, iniFile);
            List<string> result = new List<string>(returnString.Split('\0'));
            result.RemoveRange(result.Count - 2, 2);
            return result;
        }
        public static List<string> GetKeys(string iniFile, string Category)
        {
            string returnString = new string(' ', 32768);
            GetPrivateProfileString(null, null, null, returnString, 32768, iniFile);
            List<string> result = new List<string>(returnString.Split('\0'));
            result.RemoveRange(result.Count - 2, 2);
            return result;
        }

        public static string GetIniFileString(string iniFile, string category, string key, string defaultValue)
        {
            string returnString = new string(' ', 1024);
            string[] lines = System.IO.File.ReadAllLines(iniFile);
            int index = -1;
            for(int i = 0; i < lines.Length; i++)
            {
                if (index != -1)
                {
                    if (lines[i].Contains(key + "="))
                    {
                        return lines[i].Substring(key.Length + 1);
                    }
                }
                if (lines[i] == "["+category+"]")
                {
                    index = i;
                }
            }
            return defaultValue;
        }
    }
}
