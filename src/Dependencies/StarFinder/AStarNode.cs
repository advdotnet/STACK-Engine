namespace StarFinder
{
	/// <summary>
	/// An intermediate structure which is used for searching.
	/// </summary>    
	public struct AStarNode<S> where S : IMapPosition
	{
		public float F, G, H;
		public S Pos, ParentPos;

		public AStarNode(float g, float h, S pos, S parentPos)
		{
			G = g;
			H = h;
			F = g + h;
			ParentPos = parentPos;
			Pos = pos;
		}
	}
}
