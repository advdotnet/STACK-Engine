namespace STACK.Components
{
    interface IPlayAnimation
    {
        void PlayAnimation(string animation, bool looped);
        bool Playing { get; }
    }
}
