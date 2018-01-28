using Microsoft.Xna.Framework;
using STACK.Components;
using STACK.Graphics;
using STACK.Input;
using System;
using System.Collections.Generic;

namespace STACK
{
    /// <summary>
    /// The root of the object hierarchy.
    /// Manages a collection of scenes, render settings and user input.
    /// </summary>
    [Serializable]
    public class World : SceneCollection
    {
        public World(InputProvider input, Point? resolution = null)
        {
            Mouse
                .Create(this);

            Camera
                .Create(this);

            Randomizer
                .Create(this);

            RenderSettings
                .Create(this)
                .SetVirtualResolution(resolution.HasValue ? resolution.Value : new Point());

            AudioManager
                .Create(this);

            if (input != null)
            {
                input.Handler += HandleInputEvent;
            }
        }

        public World(IServiceProvider services, InputProvider input = null, Point? resolution = null)
            : this(input, resolution)
        {
            ServiceProvider
                .Create(this)
                .SetProvider(services);

            SkipContent
                .Create(this)
                .SetInterfaceFromServiceProvider(services);
        }

        public World(IServiceProvider services, InputProvider input, Point resolution, List<Scene> scenes)
            : this(services, input, resolution)
        {
            Push(scenes.ToArray());
        }

        public void Unsubscribe(InputProvider input)
        {
            input.Handler -= HandleInputEvent;
        }

        public override void OnDraw(Renderer renderer)
        {
            // Draw the scenes in reverse order

            for (int i = Items.Count - 1; i >= 0; i--)
            {
                Items[i].Draw(renderer);
            }
        }

        public override bool Push(BaseEntity scene)
        {
            if (base.Push(scene))
            {
                if (scene is Scene)
                {
                    ((Scene)scene).World = this;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles input events.
        /// </summary>
        public void HandleInputEvent(InputEvent inputEvent)
        {
            var SkipInputEvent = ((!Interactive && inputEvent.Type != InputEventType.MouseMove) ||
                inputEvent.Handled ||
                inputEvent.Paused);

            if (SkipInputEvent)
            {
                return;
            }

            base.OnHandleInputEvent(Get<Mouse>().Position, inputEvent);
        }

        /// <summary>
        /// Restores a snapshot of a list of scenes from the given file.
        /// </summary>        
        public void RestoreState(World state, IServiceProvider provider, ContentLoader content)
        {
            RestoreState(state.Items, provider, content);
            Interactive = state.Interactive;
            state = null;
        }
    }
}
