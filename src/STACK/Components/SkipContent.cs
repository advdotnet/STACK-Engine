using System;
using System.Runtime.Serialization;

namespace STACK.Components
{
    [Serializable]
    public class SkipContent : Component, ISkipContent
    {
        [NonSerialized]
        ISkipContent Interface;

        /// <summary>
        /// Serialize possible states in savegames.
        /// </summary>
        bool SkipTextPossible;
        bool SkipCutscenePossible;

        [OnSerializing]
        void OnSerializingMethod(StreamingContext context)
        {
            if (null != SkipText)
            {
                SkipTextPossible = SkipText.Possible;
            }
            else
            {
                SkipTextPossible = true;
            }

            if (null != SkipCutscene)
            {
                SkipCutscenePossible = SkipCutscene.Possible;
            }
        }

        void PropagatePossibleStates()
        {
            if (null != SkipText)
            {
                SkipText.Possible = SkipTextPossible;
            }

            if (null != SkipCutscene)
            {
                SkipCutscene.Possible = SkipCutscenePossible;
            }
        }

        public SkipContent()
        {

        }

        public SkipCutscene SkipCutscene
        {
            get
            {
                return Interface?.SkipCutscene;
            }
        }

        public SkipText SkipText
        {
            get
            {
                return Interface?.SkipText;
            }
        }

        public static SkipContent Create(World addTo)
        {
            return addTo.Add<SkipContent>();
        }

        public SkipContent SetInterface(ISkipContent value) { Interface = value; PropagatePossibleStates(); return this; }
        public SkipContent SetInterfaceFromServiceProvider(IServiceProvider value) { if (null != value) Interface = value.GetService(typeof(ISkipContent)) as ISkipContent; PropagatePossibleStates(); return this; }
    }
}
