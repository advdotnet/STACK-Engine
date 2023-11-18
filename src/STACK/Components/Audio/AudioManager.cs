using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace STACK.Components
{
	/// <summary>
	/// AudioManager class which handles playback of songs and sound effects.
	/// </summary>
	[Serializable]
	public class AudioManager : Component, IContent, IUpdate, IInitialize
	{
		private const string _soundDisabledValue = "1";
		private const string _soundDisabledKey = "FNA_AUDIO_DISABLE_SOUND";

		public bool Enabled { get; set; }
		public float UpdateOrder { get; set; }

		[NonSerialized]
		private ContentLoader _content;

		[NonSerialized]
		private Dictionary<string, SoundEffect> _soundEffects = new Dictionary<string, SoundEffect>();

		[NonSerialized]
		private Dictionary<string, Song> _songs = new Dictionary<string, Song>();

		[NonSerialized]
		private List<SoundEffectInstance> _playingInstances = new List<SoundEffectInstance>();

		[NonSerialized]
		private Song _currentSong;

		[NonSerialized]
		private bool _isEnginePaused;
		private string _currentSongName;
		private string _nextSongName;
		private float _soundEffectVolume = 1;
		private float _musicVolume = 1;
		private float _maxSoundEffectVolume = 1;
		private float _maxMusicVolume = 1;
		private MediaState _lastMediaState;
		private bool _isRepeating = false;

		[NonSerialized]
		private ISkipContent _skipContent = null;

		public void Initialize(bool restore)
		{
			_skipContent = ((World)Parent).Get<SkipContent>();

			if (null != _currentSongName)
			{
				LoadSong(_currentSongName);
				if (MediaState.Playing == _lastMediaState)
				{
					PlaySong(_currentSongName);
					RepeatSong = _isRepeating;

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
			get => _isEnginePaused;
			set => _isEnginePaused = value;
		}

		public float MusicVolume
		{
			get => _musicVolume;
			set
			{
				_musicVolume = MathHelper.Clamp(value, 0.0f, 1.0f);
				MediaPlayer.Volume = value * MaxMusicVolume;
			}
		}

		public float SoundEffectVolume
		{
			get => _soundEffectVolume;
			set => _soundEffectVolume = MathHelper.Clamp(value, 0.0f, 1.0f);
		}

		public float MaxMusicVolume
		{
			get => _maxMusicVolume;
			set
			{
				_maxMusicVolume = MathHelper.Clamp(value, 0.0f, 1.0f);
				MediaPlayer.Volume = MusicVolume * value;
			}
		}

		public float MaxSoundEffectVolume
		{
			get => _maxSoundEffectVolume;
			set => _maxSoundEffectVolume = MathHelper.Clamp(value, 0.0f, 1.0f);
		}

		public float EffectiveSoundEffectVolume => _maxSoundEffectVolume * _soundEffectVolume;

		public MediaState LastMediaState => _lastMediaState;

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
				if (string.IsNullOrEmpty(_nextSongName))
				{
					_currentSongName = null;
					_currentSong = null;
				}
				else
				{
					PlaySong(_nextSongName);
					_nextSongName = null;
				}
			}
		}

		public void LoadContent(ContentLoader content)
		{
			var service = ((World)Parent).Get<ServiceProvider>();

			if (service != null && service.Provider != null)
			{
				_content = new ContentLoader(service.Provider, EngineVariables.ContentPath);
			}

			_soundEffects = new Dictionary<string, SoundEffect>();
			_songs = new Dictionary<string, Song>();
			_playingInstances = new List<SoundEffectInstance>();

			MediaPlayer.MediaStateChanged += OnMediaStateChanged;
		}

		/// <summary>
		/// Starts playing a song if not in fast forward mode.
		/// </summary>
		/// <param name="song"></param>
		public void PlaySong(string song)
		{
			if (null != _skipContent.SkipCutscene && _skipContent.SkipCutscene.Enabled)
			{
				return;
			}

			LoadSong(song);

			if (!SoundDisabled)
			{
				MediaPlayer.Play(_currentSong);
			}
			else
			{
				StopSong();
			}
		}

		public void EnqueueSong(string song)
		{
			if (string.IsNullOrEmpty(_currentSongName))
			{
				PlaySong(song);
			}
			else
			{
				_nextSongName = song;
			}
		}

		private void LoadSong(string song)
		{
			if (!_songs.ContainsKey(song))
			{
				_songs[song] = _content.Load<Song>(song);
			}

			_currentSong = _songs[song];
			_currentSongName = song;
		}

		public bool RepeatSong
		{
			get => MediaPlayer.IsRepeating;
			set
			{
				MediaPlayer.IsRepeating = value;
				_isRepeating = value;
			}
		}

		public void PauseSong()
		{
			MediaPlayer.Pause();
		}

		public void ResumeSong()
		{
			if (_currentSongName != null && MediaPlayer.State == MediaState.Stopped)
			{
				PlaySong(_currentSongName);
			}
			else
			{
				MediaPlayer.Resume();
			}
		}

		public void StopSong()
		{
			MediaPlayer.Stop();

			_currentSong = null;
			_currentSongName = null;
			_nextSongName = null;
		}

		public void LoadSoundEffect(string soundEffect)
		{
			if (!_soundEffects.ContainsKey(soundEffect))
			{
				_soundEffects[soundEffect] = _content.Load<SoundEffect>(soundEffect);
			}
		}

		/// <summary>
		/// Starts playing a sound effect, if not in fast forward mode.
		/// </summary>
		/// <param name="soundEffect"></param>
		/// <returns>SoundEffectInstance, or null if in fast forward mode</returns>
		[HandleProcessCorruptedStateExceptions]
		public SoundEffectInstance PlaySoundEffect(string soundEffect, bool looped = false, AudioEmitter emitter = null, AudioListener listener = null)
		{
			if (null != _skipContent.SkipCutscene && _skipContent.SkipCutscene.Enabled)
			{
				return null;
			}

			LoadSoundEffect(soundEffect);
			var instance = _soundEffects[soundEffect].CreateInstance();

			instance.Volume = EffectiveSoundEffectVolume;
			instance.IsLooped = looped;
			if (null != emitter && null != listener)
			{
				try
				{
					instance.Apply3D(listener.Listener, emitter.Emitter);
				}
				catch (AccessViolationException)
				{

				}
			}

			_playingInstances.Add(instance);

			if (!SoundDisabled)
			{
				instance.Play();
			}
			else
			{
				instance.Stop();
			}

			return instance;
		}

		public static bool SoundDisabled => _soundDisabledValue == Environment.GetEnvironmentVariable(_soundDisabledKey);

		public static void DisableSound()
		{
			Environment.SetEnvironmentVariable(_soundDisabledKey, _soundDisabledValue);
		}

		public void UnloadContent()
		{
			MediaPlayer.MediaStateChanged -= OnMediaStateChanged;
			StopSong();

			_soundEffects.Clear();
			_songs.Clear();
			_content?.Unload();			
		}

		public void Update()
		{
			if (null != _skipContent.SkipCutscene && _skipContent.SkipCutscene.Enabled)
			{
				StopAll();
			}

			if (_playingInstances.Count > 0)
			{
				for (var i = _playingInstances.Count - 1; i >= 0; i--)
				{
					if (SoundState.Stopped == _playingInstances[i].State)
					{
						_playingInstances.RemoveAt(i);
					}
				}
			}
		}

		/// <summary>
		/// Stops all sounds and the current song.
		/// </summary>
		public void StopAll()
		{
			if (_currentSong != null && MediaPlayer.State == MediaState.Playing)
			{
				StopSong();
			}

			foreach (var instance in _playingInstances)
			{
				if (instance.IsPlaying())
				{
					instance.Stop();
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
