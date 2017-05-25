using System;

namespace STACK.Components
{
    [Serializable]
    public class SkipContent : Component, ISkipContent
    {
        [NonSerialized]
        ISkipContent Interface;
        
        public SkipContent()
        {
            Enabled = false;
            Visible = false;
            Interactive = false;
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

        public SkipContent SetInterface(ISkipContent value) { Interface = value; return this; }
        public SkipContent SetInterfaceFromServiceProvider(IServiceProvider value) { if (null != value) Interface = value.GetService(typeof(ISkipContent)) as ISkipContent; return this; }        
    }  
}
