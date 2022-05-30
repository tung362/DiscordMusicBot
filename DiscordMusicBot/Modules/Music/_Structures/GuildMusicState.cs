using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Victoria;

namespace DiscordMusicBot.Modules.Music
{
    public class GuildMusicState
    {
        /*Enum*/
        public enum LoopType { None, LoopTrack, LoopPlaylist }

        /*Cache*/
        public LoopType LoopState { get; set; }
        public bool PendingIgnoreStop { get; set; }
        public Tuple<bool, int> PendingSeek { get; set; }
        public int Volume { get; set; }
        public int PlaylistPos { get; private set; }
        public List<LavaTrack> Playlist { get; private set; }
        public LavaTrack CurrentTrack { get; private set; }
        public Dictionary<string, IUserMessage> Messages { get; set; }

        public GuildMusicState()
        {
            LoopState = LoopType.None;
            PendingIgnoreStop = false;
            PendingSeek = Tuple.Create(false, -1);
            Volume = 100;
            PlaylistPos = -1;
            Playlist = new List<LavaTrack>();
            CurrentTrack = null;
            Messages = new Dictionary<string, IUserMessage>();
        }

        public void Rewind()
        {
            if (Playlist.Count == 0)
            {
                PlaylistPos = -1;
                CurrentTrack = null;
                return;
            }

            PlaylistPos--;
            if (PlaylistPos < 0) PlaylistPos = Playlist.Count - 1;
            CurrentTrack = Playlist[PlaylistPos];
        }

        public void Forward()
        {
            if (Playlist.Count == 0)
            {
                PlaylistPos = -1;
                CurrentTrack = null;
                return;
            }

            PlaylistPos++;
            if (PlaylistPos >= Playlist.Count) PlaylistPos = 0;
            CurrentTrack = Playlist[PlaylistPos];
        }

        public void Seek(int pos)
        {
            if (Playlist.Count == 0)
            {
                PlaylistPos = -1;
                CurrentTrack = null;
                return;
            }

            PlaylistPos = pos;
            if (PlaylistPos < 0) PlaylistPos = Playlist.Count - 1;
            else if (PlaylistPos >= Playlist.Count) PlaylistPos = 0;
            CurrentTrack = Playlist[PlaylistPos];
        }

        public bool RemoveTrack(int index)
        {
            if (index < 0 || index >= Playlist.Count) return false;

            Playlist.RemoveAt(index);
            if (index <= PlaylistPos) Rewind();
            return true;
        }

        public void ClearPlaylist()
        {
            Playlist.Clear();
            CurrentTrack = null;
        }

        public async Task DeleteMessage(string messageKey)
        {
            if (!Messages.ContainsKey(messageKey)) return;

            IMessage message = await Messages[messageKey].Channel.GetMessageAsync(Messages[messageKey].Id);
            if (message != null) await Messages[messageKey].DeleteAsync();
        }

        public async Task DeleteAllMessages()
        {
            foreach (KeyValuePair<string, IUserMessage> kv in Messages) await DeleteMessage(kv.Key);
            Messages.Clear();
        }

        public async Task Reset()
        {
            LoopState = LoopType.None;
            PendingIgnoreStop = false;
            PendingSeek = Tuple.Create(false, -1);
            Volume = 100;
            PlaylistPos = -1;
            ClearPlaylist();
            await DeleteAllMessages();
        }
    }
}
