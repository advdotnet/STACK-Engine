using Microsoft.Xna.Framework;
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

        [NonSerialized]
        bool _IsEnginePaused;

        string CurrentSongName;
        string NextSongName;

        float _SoundEffectVolume = 1;
        float _MusicVolume = 1;

        float _MaxSoundEffectVolume = 1;
        float _MaxMusicVolume = 1;

        MediaState _lastMediaState;
        bool IsRepeating = false;

        [NonSerialized]
        ISkipContent SkipContent = null;

        public void Initialize(bool restore)
        {
            SkipContent = ((World)Parent).Get<SkipContent>();

            if (null != CurrentSongName)
            {
                LoadSong(CurrentSongName);
                if (MediaState.Playing == _lastMediaState)
                {
                    PlaySong(CurrentSongName);
                    RepeatSong = IsRepeating;

                    return;
                }
            }

            if (restore)
            {
                StopSong();
            }
        }

        public AudioManager()
        {
            UpdateOrder = 500;
            Enabled = true;
        }

        public bool IsEnginePaused
        {
            get
            {
                return _IsEnginePaused;
            }
            set
            {
                _IsEnginePaused = value;
            }
        }

        public float MusicVolume
        {
            get
            {
                return MediaPlayer.Volume;
            }
            set
            {
                _MusicVolume = MathHelper.Clamp(value, 0.0f, 1.0f);
                MediaPlayer.Volume = value * MaxMusicVolume;
            }
        }

        public float SoundEffectVolume
        {
            get
            {
                return _SoundEffectVolume;
            }
            set
            {
                _SoundEffectVolume = MathHelper.Clamp(value, 0.0f, 1.0f);
            }
        }

        public float MaxMusicVolume
        {
            get
            {
                return _MaxMusicVolume;
            }
            set
            {
                _MaxMusicVolume = MathHelper.Clamp(value, 0.0f, 1.0f);
                MediaPlayer.Volume = MusicVolume * value;
            }
        }

        public float MaxSoundEffectVolume
        {
            get
            {
                return _MaxSoundEffectVolume;
            }
            set
            {
                _MaxSoundEffectVolume = MathHelper.Clamp(value, 0.0f, 1.0f);
            }
        }

        public float EffectiveSoundEffectVolume
        {
            get
            {
                return _MaxSoundEffectVolume * _SoundEffectVolume;
            }
        }

        public MediaState LastMediaState
        {
            get
            {
                return _lastMediaState;
            }
        }

        public void ApplyGameSettingsVolume(GameSettings gameSettings)
        {
            if (null != gameSettings)
            {
                MaxSoundEffectVolume = gameSettings.SoundEffectVolume;
                MaxMusicVolume = gameSettings.MusicVolume;
            }
        }

        private void OnMediaStateChanged(object sender, EventArgs e)
        {
            // if the media player state was paused due to an engine pause, don't 
            // store the last state.

            if (MediaPlayer.State == MediaState.Paused && IsEnginePaused)
            {
                return;
            }

            _lastMediaState = MediaPlayer.State;
            if (MediaState.Stopped == _lastMediaState)
            {
                if (string.IsNullOrEmpty(NextSongName))
                {
                    CurrentSongName = null;
                    CurrentSong = null;
                }
                else
                {
                    PlaySong(NextSongName);
                    NextSongName = null;
                }
            }
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

            MediaPlayer.MediaStateChanged += this.OnMediaStateChanged;
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

            LoadSong(song);

            if (!SoundDisabled)
            {
                MediaPlayer.Play(CurrentSong);
            }
            else
            {
                StopSong();
            }
        }

        public void EnqueueSong(string song)
        {
            if (string.IsNullOrEmpty(CurrentSongName))
            {
                PlaySong(song);
            }
            else
            {
                NextSongName = song;
            }
        }

        private void LoadSong(string song)
        {
            if (!Songs.ContainsKey(song))
            {
                Songs[song] = Content.Load<Song>(song);
            }

            CurrentSong = Songs[song];
            CurrentSongName = song;
        }

        public bool RepeatSong
        {
            get
            {
                return MediaPlayer.IsRepeating;
            }
            set
            {
                MediaPlayer.IsRepeating = value;
                IsRepeating = value;
            }
        }

        public void PauseSong()
        {
            MediaPlayer.Pause();
        }

        public void ResumeSong()
        {
            if (CurrentSongName != null && MediaPlayer.State == MediaState.Stopped)
            {
                PlaySong(CurrentSongName);
            }
            else
            {
                MediaPlayer.Resume();
            }
        }

        public void StopSong()
        {
            MediaPlayer.Stop();

            CurrentSong = null;
            CurrentSongName = null;
            NextSongName = null;
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
        public SoundEffectInstance PlaySoundEffect(string soundEffect, bool looped = false, AudioEmitter emitter = null, AudioListener listener = null)
        {
            if (null != SkipContent.SkipCutscene && SkipContent.SkipCutscene.Enabled)
            {
                return null;
            }

            LoadSoundEffect(soundEffect);
            var Instance = SoundEffects[soundEffect].CreateInstance();

            PlayingInstances.Add(Instance);

            Instance.Volume = EffectiveSoundEffectVolume;
            Instance.IsLooped = looped;
            if (null != emitter && null != listener)
            {
                try
                {
                    Instance.Apply3D(listener.Listener, emitter.Emitter);
                }
                catch (AccessViolationException)
                {

                }
            }


            if (!SoundDisabled)
            {
                Instance.Play();
            }
            else
            {
                Instance.Stop();
            }

            return Instance;
        }

        private const string SOUND_DISABLED_VALUE = "1";
        private const string SOUND_DISABLED_KEY = "FNA_AUDIO_DISABLE_SOUND";

        public static bool SoundDisabled
        {
            get
            {
                return SOUND_DISABLED_VALUE == Environment.GetEnvironmentVariable(SOUND_DISABLED_KEY);
            }
        }

        public static void DisableSound()
        {
            Environment.SetEnvironmentVariable(SOUND_DISABLED_KEY, SOUND_DISABLED_VALUE);
        }

        public void UnloadContent()
        {
            MediaPlayer.MediaStateChanged -= this.OnMediaStateChanged;
            StopSong();

            SoundEffects.Clear();
            Songs.Clear();

            if (Content != null)
            {
                Content.Unload();
            }
        }

        public void Update()
        {
            if (null != SkipContent.SkipCutscene && SkipContent.SkipCutscene.Enabled)
            {
                StopAll();
            }

            if (PlayingInstances.Count > 0)
            {
                for (int i = PlayingInstances.Count - 1; i >= 0; i--)
                {
                    if (SoundState.Stopped == PlayingInstances[i].State)
                    {
                        PlayingInstances.RemoveAt(i);
                    }
                }
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

        public static AudioManager Create(World addTo)
        {
            return addTo.Add<AudioManager>();
        }

        public AudioManager SetSongVolume(float val) { MusicVolume = val; return this; }
        public AudioManager SetSoundEffectVolume(float val) { SoundEffectVolume = val; return this; }
        public AudioManager SetMaxSongVolume(float val) { MaxMusicVolume = val; return this; }
        public AudioManager SetMaxSoundEffectVolume(float val) { MaxSoundEffectVolume = val; return this; }
    }
}
