namespace Backend
{
    public sealed class Singleton<T> where T: new()
    {
        public static T Instance { get; } = new T();
    }
}
