using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
    [Serializable]
    public class RenderSettings : Component
    {
        BloomSettings _BloomSettings;
        bool _BloomEnabled;
        Point _VirtualResolution;

        public BloomSettings BloomSettings { get { return _BloomSettings; } set { _BloomSettings = value; } }
        public bool BloomEnabled { get { return _BloomEnabled; } set { _BloomEnabled = value; } }
        public Point VirtualResolution { get { return _VirtualResolution; } }

        public RenderSettings()
        {
            _BloomSettings = BloomSettings.PresetSettings[5];
            BloomEnabled = true;
        }

        public static RenderSettings Create(World addTo)
        {
            return addTo.Add<RenderSettings>();
        }

        public RenderSettings SetVirtualResolution(Point value) { _VirtualResolution = value; return this; }
    }
}
