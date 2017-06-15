using System;
using System.Collections;

namespace STACK.Components
{
    /// <summary>
    /// Represents an entrance to a scene.
    /// </summary>
    [Serializable]
    public class Entrance : Passage
    {
        protected override IEnumerator DefaultScript(Entity gameObject)
        {
            yield return 0;
        }

        protected override IEnumerator MergedScript(Entity gameObject)
        {
            Blocked = true;

            yield return gameObject.Get<Scripts>().Start(Script(gameObject), "EntranceScript");

            Blocked = false;
        }

        public Script Use(Entity gameObject)
        {
            CurrentMergedScript = gameObject.Get<Scripts>().Start(MergedScript(gameObject), "MergedEntranceScript");
            return CurrentMergedScript;
        }

        public static Entrance Create(Entity addTo)
        {
            return addTo.Add<Entrance>();
        }

        public Entrance SetScript(Func<Entity, IEnumerator> value) { Script = value; return this; }
        public Entrance SetBlocked(bool value) { Blocked = value; return this; }
    }
}
