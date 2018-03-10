namespace STACK
{
    public interface INotify
    {
        void Notify<T>(string message, T data);
    }
}
