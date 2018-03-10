namespace STACK.Components
{
    public static class StateExtensions
    {
        public static string ToAnimationName(this State state)
        {
            switch (state)
            {
                case State.Idle: return "idle";
                case State.Talking: return "talk";
                case State.Walking: return "walk";
                case State.Custom: return "custom";
                case State.Talking | State.Walking: return "walktalk";
            }

            return string.Empty;
        }

        public static bool Has(this State state, State value)
        {
            // do not use enum.HasFlag
            return (state & value) == value;
        }

        public static bool Is(this State state, State value)
        {
            return state == value;
        }

        public static State Add(this State state, State value)
        {
            state |= value;
            return state;
        }

        public static State Remove(this State state, State value)
        {
            state &= ~value;
            return state;
        }
    }
}
