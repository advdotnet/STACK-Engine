using System;
using System.Collections;

namespace STACK.Components
{
    /// <summary>
    /// Base class for exits or entrances. 
    /// </summary>
    [Serializable]
    public abstract class Passage : Component 
    {        
		protected Script CurrentMergedScript;

        protected abstract IEnumerator DefaultScript(Entity gameObject);
        protected abstract IEnumerator MergedScript(Entity gameObject);

        public Func<Entity, IEnumerator> Script { get; set; }

		public bool Blocked { get; protected set; }

        public Passage()
        {
            Blocked = false;
            Script = DefaultScript;
        }

		public override void OnUpdate()
		{
			if (CurrentMergedScript == null)
			{
				Blocked = false;
			}

			if (CurrentMergedScript != null && CurrentMergedScript.Done)
			{
				Blocked = false;
				CurrentMergedScript = null;
			}
		}		
    }    
}
