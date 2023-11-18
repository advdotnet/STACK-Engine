using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
	[Serializable]
	public class RenderSettings : Component
	{
		private BloomSettings _bloomSettings;
		private bool _bloomEnabled;
		private Point _virtualResolution;

		public BloomSettings BloomSettings { get => _bloomSettings; set => _bloomSettings = value; }
		public bool BloomEnabled { get => _bloomEnabled; set => _bloomEnabled = value; }
		public Point VirtualResolution => _virtualResolution;

		public RenderSettings()
		{
			_bloomSettings = BloomSettings.PresetSettings[5];
			BloomEnabled = true;
		}

		public static RenderSettings Create(World addTo)
		{
			return addTo.Add<RenderSettings>();
		}

		public RenderSettings SetVirtualResolution(Point value) { _virtualResolution = value; return this; }
	}
}
