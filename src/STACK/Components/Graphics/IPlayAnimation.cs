namespace STACK.Components
{
    public interface IPlayAnimation
    {
        void PlayAnimation(string animation, bool looped);
        bool Playing { get; }
    }
}
