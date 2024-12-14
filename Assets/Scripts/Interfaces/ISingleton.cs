public interface ISingleton<T> where T : new()
{
    private static T _singletonInstance;
    private static readonly object _padlock = new object();
    public static T GetInstance()
    {
        lock (_padlock)
        {
            if (_singletonInstance == null)
            {
                _singletonInstance = new T();
            }
            return _singletonInstance;
        }
    }
}