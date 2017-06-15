using System;
using System.Collections;
using System.Diagnostics;

namespace STACK.Components
{
    /// <summary>
    /// Represents an exit of a scene.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{TargetEntrance}")]
    public class Exit : Passage
    {
        public string TargetEntrance { get; set; }

        protected override IEnumerator DefaultScript(Entity gameObject)
        {
            Entity.World.Interactive = false;
            yield return 0;
        }

        protected override IEnumerator MergedScript(Entity gameObject)
        {
            var TargetEntity = Entity.World.GetGameObject(TargetEntrance);

            if (TargetEntity == null)
            {
                throw new NullReferenceException("Exit's TargetEntity");
            }

            var Entrance = TargetEntity.Get<Entrance>();

            if (Entrance == null)
            {
                throw new NullReferenceException("Entrance needs an Entrance component!");
            }

            while (Blocked || Entrance.Blocked)
            {
                yield return false;
            }

            Blocked = true;

            yield return gameObject.Get<Scripts>().Start(Script(gameObject), "ExitScript");

            gameObject.EnterScene(TargetEntity.DrawScene.ID);

            yield return Entrance.Use(gameObject);

            Blocked = false;

            yield return true;
        }

        public static Exit Create(Entity addTo)
        {
            return addTo.Add<Exit>();
        }

        public Exit SetTargetEntrance(string value) { TargetEntrance = value; return this; }
        public Exit SetScript(Func<Entity, IEnumerator> value) { Script = value; return this; }
        public Exit SetBlocked(bool value) { Blocked = value; return this; }

        public Script Use(Entity gameObject)
        {
            CurrentMergedScript = gameObject.Get<Scripts>().Start(MergedScript(gameObject), "MergedExitScript");
            return CurrentMergedScript;
        }


    }
}
