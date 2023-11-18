using Microsoft.Xna.Framework;
using StarFinder;
using System;
using System.Diagnostics;

namespace STACK.Components
{
	/// <summary>
	/// A hotspot described by a triangle mesh.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{Mesh}")]
	public class HotspotMesh : Hotspot
	{
		public Mesh<TriangleVertexData> Mesh { get; protected set; }

		public override bool IsHit(Vector2 mouse)
		{
			return Mesh.Contains(mouse);
		}

		public static HotspotMesh Create(Entity addTo)
		{
			return addTo.Add<HotspotMesh>();
		}

		public HotspotMesh SetMesh(Mesh<TriangleVertexData> value) { Mesh = value; return this; }
		public HotspotMesh SetCaption(string value) { Caption = value; return this; }
	}
}
