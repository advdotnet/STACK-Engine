namespace STACK
{
	public interface IUpdate
	{
		bool Enabled { get; set; }
		void Update();
		float UpdateOrder { get; set; }
	}
}
