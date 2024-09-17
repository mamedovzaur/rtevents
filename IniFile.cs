using System.Runtime.InteropServices;
using System.Text;

namespace Ini
{
    public class IniFile
    {
        public string path;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public IniFile(string INIPath)
        {
            this.path = INIPath;
        }

        public void IniWriteValue(string Section, string Key, string Value)
        {
            IniFile.WritePrivateProfileString(Section, Key, Value, this.path);
        }

        public string IniReadValue(string Section, string Key)
        {
            StringBuilder retVal = new StringBuilder((int)byte.MaxValue);
            IniFile.GetPrivateProfileString(Section, Key, "", retVal, (int)byte.MaxValue, this.path);
            return retVal.ToString();
        }
    }
}