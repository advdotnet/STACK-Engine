namespace STACK
{
	/// <summary>
	/// Interface to control skipping of text lines or whole cutscenes
	/// </summary>
	public interface ISkipContent
	{
		SkipCutscene SkipCutscene { get; }
		SkipText SkipText { get; }
	}
}
