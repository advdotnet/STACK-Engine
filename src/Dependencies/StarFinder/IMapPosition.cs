namespace StarFinder
{
	public interface IMapPosition
	{
		float Cost(IMapPosition parent);
		bool Equals(IMapPosition b);
	}
}
