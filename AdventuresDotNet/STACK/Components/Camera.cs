﻿using System;
using Microsoft.Xna.Framework;

namespace STACK.Components
{
    /// <summary>
    /// Camera class which handles translation, rotation and scaling.
    /// </summary>
    [Serializable]
    public class Camera : Component
    {
        float _Zoom;        
        Vector2 _Position;
        float _Rotation;

        /// <summary>
        /// The transformation matrix for the camera.
        /// </summary>
        public Matrix Transformation { get; private set; }

        /// <summary>
        /// The inverse of the transformation matrix.
        /// </summary>
        public Matrix TransformationInverse { get; private set; }       

        public Camera() : this(Vector2.Zero, 1f) { }

        public Camera(Vector2 position, float zoom = 1.0f)
        {
            _Zoom = zoom;
            _Rotation = 0.0f;
            _Position = position;
            Visible = false;
            UpdateTransformation();
        }        

        public float Zoom
        {
            get 
            { 
                return _Zoom; 
            }

            set
            {
                _Zoom = value;
                UpdateTransformation();
            }
        }

        public float Rotation
        {
            get 
            { 
                return _Rotation; 
            }

            set 
            { 
                _Rotation = value;
                UpdateTransformation();
            }
        }

        public void Move(Vector2 delta)
        {
            Position += delta;            
        }

        public Vector2 Position
        {
            get 
            { 
                return _Position; 
            }

            set 
            { 
                _Position = value;
                UpdateTransformation();
            }
        }

        public Vector2 Transform(Vector2 position)
        {
            return Vector2.Transform(position, Transformation);
        }

        public Vector2 TransformInverse(Vector2 position)
        {
            return Vector2.Transform(position, TransformationInverse);
        }

        void UpdateTransformation() 
        {
            Transformation = Matrix.CreateRotationZ(Rotation) 
                * Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) 
                * Matrix.CreateTranslation(new Vector3(-_Position.X, -_Position.Y, 0)) ;

            TransformationInverse = Matrix.Invert(Transformation);
        }

        public static Camera Create(BaseEntityCollection addTo)
        {
            return addTo.Add<Camera>();
        }
    }
}
