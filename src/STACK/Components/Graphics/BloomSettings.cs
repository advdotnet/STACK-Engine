using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
    [Serializable]
    public class RenderSettings : Component
    {
        public BloomSettings BloomSettings { get; set; }
        public bool BloomEnabled { get; set; }
        public Point VirtualResolution { get; private set; }

        public RenderSettings()
        {
             BloomSettings = BloomSettings.PresetSettings[5];
             BloomEnabled = true;
             Visible = false;
        }

        public static RenderSettings Create(World addTo)
        {
            return addTo.Add<RenderSettings>();            
        }

        public RenderSettings SetVirtualResolution(Point value) { VirtualResolution = value; return this; }
    }  
}
