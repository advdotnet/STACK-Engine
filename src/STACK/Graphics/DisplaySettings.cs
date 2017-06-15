using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace STACK.Graphics
{

    public class DisplaySettings
    {
        public Viewport Viewport { get; private set; }
        public Point VirtualResolution { get; private set; }
        public Matrix ScaleMatrix { get { return _ScaleMatrix; } }
        public float AspectRatio { get; private set; }

        Matrix _ScaleMatrix = Matrix.Identity;
        float WindowScaleX, WindowScaleY;

        public DisplaySettings(Point virtualResolution, Point realResolution)
        {
            VirtualResolution = virtualResolution;
            AspectRatio = (float)VirtualResolution.X / VirtualResolution.Y;
            Viewport = new Viewport(0, 0, VirtualResolution.X, VirtualResolution.Y);
            OnClientSizeChanged(realResolution.X, realResolution.Y);            
        }

        private void CacheValues(float width, float height)
        {
            WindowScaleX = (float)Viewport.Width / VirtualResolution.X;
            WindowScaleY = (float)Viewport.Height / VirtualResolution.Y;
            var ViewportScaleX = (float)width / VirtualResolution.X;
            var ViewportScaleY = (float)height / VirtualResolution.Y;

            Matrix.CreateScale(ViewportScaleX, ViewportScaleY, 1.0f, out _ScaleMatrix);            
        }

        public Vector2 TransformPoint(float x, float y)
        {
            x -= Viewport.X;
            y -= Viewport.Y;

            var scaleX = (float)Viewport.Width / VirtualResolution.X;
            var scaleY = (float)Viewport.Height / VirtualResolution.Y;

            return new Vector2(x / WindowScaleX, y / WindowScaleY);                        
        }

        public void OnClientSizeChanged(int width, int height)
        {                        
            var Width = width;
            var Height = (int)(Width / AspectRatio + 0.5f);

            if (Height > height)
            {                
                Height = height;
                Width = (int)(Height * AspectRatio + 0.5f);
            }

            var X = (width / 2) - (Width / 2);
            var Y = (height / 2) - (Height / 2);

            Viewport = new Viewport(X, Y, Width, Height);

            CacheValues(width, height);            
        }
    }    
}
