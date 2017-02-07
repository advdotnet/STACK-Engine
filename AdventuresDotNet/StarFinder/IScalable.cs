namespace StarFinder
{
    /// <summary>
    /// To make extrapolation of triangle vertex data on any given point inside the
    /// triangle possible, the vertex data uses this interface.
    /// </summary>    
    public interface IScalable<T>
    {
        T Multiply(float scale);
        T Add(T t);
        T Default();
    }

}
