using Microsoft.Xna.Framework;
using System;

namespace STACK.Components
{
	[Serializable]
	public class Lightning : Component
	{
		public Vector3 LightPosition;
		public Vector3 LightColor, AmbientColor;
		public float DrawNormals, CellShading, EnableRotation;

		public Lightning()
		{
			DrawNormals = 0;
			CellShading = 0;
			EnableRotation = 1;
			LightPosition = Vector3.Zero;
			AmbientColor = new Vector3(0.6f, 0.6f, 0.65f);
			LightColor = new Vector3(1.5f, 1.5f, 1.55f);
		}

		public static Lightning Create(Entity addTo)
		{
			return addTo.Add<Lightning>();
		}

		public Lightning SetLightPosition(Vector3 value) { LightPosition = value; return this; }
		public Lightning SetLightColor(Vector3 value) { LightColor = value; return this; }
		public Lightning SetAmbientColor(Vector3 value) { AmbientColor = value; return this; }
		public Lightning SetDrawNormals(bool value) { DrawNormals = value ? 1 : 0; return this; }
		public Lightning SetCellShading(bool value) { CellShading = value ? 1 : 0; return this; }
		public Lightning SetEnableRotation(bool value) { EnableRotation = value ? 1 : 0; return this; }
	}
}
