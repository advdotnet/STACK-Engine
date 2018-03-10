using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace STACK.Components
{
    /// <summary>
    /// AudioManager class which handles playback of songs and sound effects.
    /// </summary>
    [Serializable]
    public class AudioManager : Component, IContent, IUpdate, IInitialize
    {
        public bool Enabled { get; set; }
        public float UpdateOrder { get; set; }

        [NonSerialized]
        ContentLoader Content;

        [NonSerialized]
        Dictionary<string, SoundEffect> SoundEffects = new Dictionary<string, SoundEffect>();

        [NonSerialized]
        Dictionary<string, Song> Songs = new Dictionary<string, Song>();

        [NonSerialized]
        List<SoundEffectInstance> PlayingInstances = new List<SoundEffectInstance>();

        [NonSerialized]
        Song CurrentSong;

        string CurrentSongName;

        [NonSerialized]
        ISkipContent SkipContent = null;

        public void Initialize(bool restore)
        {
            SkipContent = ((World)Parent).Get<SkipContent>();
        }

        public AudioManager()
        {
            UpdateOrder = 500;
            Enabled = true;
        }

        public void LoadContent(ContentLoader content)
        {
            var Service = ((World)Parent).Get<ServiceProvider>();

            if (Service != null && Service.Provider != null)
            {
                Content = new ContentLoader(Service.Provider, EngineVariables.ContentPath);
            }

            SoundEffects = new Dictionary<string, SoundEffect>();
            Songs = new Dictionary<string, Song>();
            PlayingInstances = new List<SoundEffectInstance>();
        }

        /// <summary>
        /// Starts playing a song if not in fast forward mode.
        /// </summary>
        /// <param name="song"></param>
        public void PlaySong(string song)
        {
            if (null != SkipContent.SkipCutscene && SkipContent.SkipCutscene.Enabled)
            {
                return;
            }

            if (!Songs.ContainsKey(song))
            {
                Songs[song] = Content.Load<Song>(song);
            }

            CurrentSong = Songs[song];
            CurrentSongName = song;

            MediaPlayer.Play(CurrentSong);
        }

        public bool RepeatSong
        {
            get { return MediaPlayer.IsRepeating; }
            set { MediaPlayer.IsRepeating = value; }
        }

        public void PauseSong()
        {
            MediaPlayer.Pause();
        }

        public void ResumeSong()
        {
            MediaPlayer.Resume();
        }

        public void StopSong()
        {
            MediaPlayer.Stop();

            CurrentSong = null;
            CurrentSongName = null;
        }

        public void LoadSoundEffect(string soundEffect)
        {
            if (!SoundEffects.ContainsKey(soundEffect))
            {
                SoundEffects[soundEffect] = Content.Load<SoundEffect>(soundEffect);
            }
        }

        /// <summary>
        /// Starts playing a sound effect, if not in fast forward mode.
        /// </summary>
        /// <param name="soundEffect"></param>
        /// <returns>SoundEffectInstance, or null if in fast forward mode</returns>
        public SoundEffectInstance PlaySoundEffect(string soundEffect, bool looped = false)
        {
            if (null != SkipContent.SkipCutscene && SkipContent.SkipCutscene.Enabled)
            {
                return null;
            }

            LoadSoundEffect(soundEffect);
            var Instance = SoundEffects[soundEffect].CreateInstance();

            PlayingInstances.Add(Instance);

            Instance.Play();
            Instance.IsLooped = looped;
            return Instance;
        }

        public void UnloadContent()
        {
            StopSong();

            SoundEffects.Clear();
            Songs.Clear();

            if (Content != null)
            {
                Content.Unload();
            }
        }

        public static AudioManager Create(World addTo)
        {
            return addTo.Add<AudioManager>();
        }

        public void Update()
        {
            if (null != SkipContent.SkipCutscene && SkipContent.SkipCutscene.Enabled)
            {
                StopAll();
            }
        }

        /// <summary>
        /// Stops all sounds and the current song.
        /// </summary>
        public void StopAll()
        {
            if (CurrentSong != null && MediaPlayer.State == MediaState.Playing)
            {
                StopSong();
            }

            foreach (var Instance in PlayingInstances)
            {
                if (Instance.IsPlaying())
                {
                    Instance.Stop();
                }
            }
        }
    }
}
