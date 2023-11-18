using System;

namespace STACK.Components
{
	[Serializable]
	public class ServiceProvider : Component
	{
		[NonSerialized]
		public IServiceProvider Provider;

		public static ServiceProvider Create(World addTo)
		{
			return addTo.Add<ServiceProvider>();
		}

		public ServiceProvider SetProvider(IServiceProvider value) { Provider = value; return this; }
	}
}
