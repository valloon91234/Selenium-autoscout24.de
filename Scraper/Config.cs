using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoScout24
{
    static class Config
    {
        public const String FILE_SETTING = "config.ini";
        public const String KEY_PASSWORD = "password";
        public const String KEY_DIRECTORY = "dir";
        public const String KEY_DATA = "data";
        public const String KEY_PROXY_TYPE = "proxy_type";
        public const String KEY_PROXY_ADDRESS = "proxy_address";
        public const String KEY_PROXY_USERNAME = "proxy_username";
        public const String KEY_PROXY_PASSWORD = "proxy_password";

        public static String Get(String key)
        {
            try
            {
                String[] lines = File.ReadAllLines(FILE_SETTING, Encoding.UTF8);
                foreach (String line in lines)
                {
                    if (String.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith("//")) continue;
                    String[] array = line.Split(new Char[] { '=' }, 2);
                    String name = array[0].Trim().ToLower();
                    String value = array.Length > 1 ? array[1] : null;
                    if (name == key)
                    {
                        if (String.IsNullOrWhiteSpace(value))
                            return null;
                        return value.Trim();
                    }
                }
            }
            catch { }
            return null;
        }

        public static Boolean Write(String key, String value)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(FILE_SETTING);
                if (fileInfo.Exists)
                {
                    String[] lines = File.ReadAllLines(FILE_SETTING, Encoding.UTF8);
                    using (StreamWriter file = new StreamWriter(FILE_SETTING, false, Encoding.UTF8))
                    {
                        Boolean foundKey = false;
                        foreach (String line in lines)
                        {
                            if (String.IsNullOrWhiteSpace(line) || line.StartsWith("#") || line.StartsWith("//"))
                            {
                                file.WriteLine(line);
                                continue;
                            }
                            String[] array = line.Split(new Char[] { '=' }, 2);
                            String name = array[0].Trim().ToLower();
                            String value0 = array.Length > 1 ? array[1] : null;
                            if (name == key)
                            {
                                foundKey = true;
                                file.WriteLine(name + "=" + value);
                            }
                            else
                            {
                                file.WriteLine(line);
                            }
                        }
                        if (!foundKey) file.WriteLine(key + "=" + value);
                    }
                }
                else
                {
                    using (StreamWriter file = new StreamWriter(FILE_SETTING, false, Encoding.UTF8))
                    {
                        file.WriteLine(key + "=" + value);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
