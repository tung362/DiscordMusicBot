using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DiscordMusicBot
{
    public static class ConfigUtils
    {
        public static bool LoadConfig(string path, out Dictionary<string, string> entries)
        {
            if (File.Exists(path))
            {
                entries = new Dictionary<string, string>();
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    //If not empty
                    if (lines[i].Length != 0)
                    {
                        //If not a comment
                        if (lines[i][0] != '#')
                        {
                            int currentIndex = 0;
                            string entryKey = "";
                            string entryValue = "";

                            //Get entry key
                            for (int j = 0; j < lines[i].Length; j++)
                            {
                                currentIndex = j;
                                if (lines[i][j] != ' ' && lines[i][j] != '#') entryKey += lines[i][j];
                                else
                                {
                                    currentIndex += 3;
                                    break;
                                }
                            }

                            //Get entry value
                            for (int j = currentIndex; j < lines[i].Length; j++)
                            {
                                if (lines[i][j] != '#') entryValue += lines[i][j];
                                else break;
                            }

                            if (!string.IsNullOrEmpty(entryKey)) entries[entryKey] = entryValue;
                        }
                    }
                }
                return true;
            }
            entries = null;
            return false;
        }
    }
}
