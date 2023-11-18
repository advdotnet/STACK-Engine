using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
	/// <summary>
	/// Camera class which handles translation, rotation and scaling.
	/// </summary>
	[Serializable]
	public class Camera : Component
	{
		private float _zoom;
		private Vector2 _position;
		private float _rotation;
		private Matrix _transformation;
		private Matrix _transformationInverse;

		/// <summary>
		/// The transformation matrix for the camera.
		/// </summary>
		public Matrix Transformation => _transformation;

		/// <summary>
		/// The inverse of the transformation matrix.
		/// </summary>
		public Matrix TransformationInverse => _transformationInverse;

		public Camera() : this(Vector2.Zero, 1f) { }

		public Camera(Vector2 position, float zoom = 1.0f)
		{
			_zoom = zoom;
			_rotation = 0.0f;
			_position = position;
			UpdateTransformation();
		}

		public float Zoom
		{
			get => _zoom;

			set
			{
				_zoom = value;
				UpdateTransformation();
			}
		}

		public float Rotation
		{
			get => _rotation;

			set
			{
				_rotation = value;
				UpdateTransformation();
			}
		}

		public void Move(Vector2 delta)
		{
			Position += delta;
		}

		public Vector2 Position
		{
			get => _position;

			set
			{
				_position = value;
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

		private void UpdateTransformation()
		{
			_transformation = Matrix.CreateRotationZ(Rotation)
				* Matrix.CreateScale(new Vector3(Zoom, Zoom, 1))
				* Matrix.CreateTranslation(new Vector3(-_position.X, -_position.Y, 0).ToInt());

			_transformationInverse = Matrix.Invert(Transformation);
		}

		public static Camera Create(BaseEntityCollection addTo)
		{
			return addTo.Add<Camera>();
		}
	}
}
