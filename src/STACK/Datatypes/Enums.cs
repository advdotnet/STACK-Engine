using System;

namespace STACK
{
    [Serializable]
    [Flags]
    public enum RenderStage
    {        
        PreBloom,
        Bloom,
        PostBloom
    }

    public enum MouseButton
    {
        Left, Right
    }   

    [Flags]
    public enum Alignment 
    { 
        Center = 0, 
        Left = 1, 
        Right = 2, 
        Top = 4, 
        Bottom = 8 
    }

}
