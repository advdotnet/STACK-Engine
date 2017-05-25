using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using STACK.Graphics;
using STACK.Input;
using STACK.Components;
using StarFinder;

namespace STACK
{    
    /// <summary>
    /// A scene represents a base node in the game hierarchy containing game objects.
    /// </summary>   
    [Serializable]
    public class VisibleObjectList : List<Entity>
    {
		private Scene Scene { get; set; }

		public VisibleObjectList(Scene scene) : base(20)
		{
			Scene = scene;
			Cache();
		}
       
        public void Cache()
        {			
            Clear();

			if (Scene.World != null)
            {
				for (int i = 0; i < Scene.World.Scenes.Count; i++)
                {
					var CurrentScene = Scene.World.Scenes[i];                    

                    for (int j = 0; j < CurrentScene.Entities.Count; j++)
                    {
                        var Entity = CurrentScene.Entities[j];
						if (Entity.DrawScene == Scene && Entity.Visible) // || (Entity.DrawScene == null && Entity.UpdateScene == this)
                        {
                            Add(Entity);
                        }
                    }                    
                }                
            }
            else
            {
				for (int i = 0; i < Scene.Entities.Count; i++)
                {
					var entity = Scene.Entities[i];

					if (entity.Visible && (entity.DrawScene == null || entity.DrawScene == Scene))
                    {
						Add(entity);
                    }
                }
            }

            Sort(BaseEntityCollection.PrioritySorter);
        }               
    }
}
