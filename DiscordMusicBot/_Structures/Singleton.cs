using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordMusicBot
{
    public abstract class Singleton<T> where T : class
    {
        public static T instance { get; set; }

        public Singleton()
        {
            if (instance == null)
            {
                instance = this as T;
                return;
            }
        }
    }
}
