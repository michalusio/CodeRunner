namespace Reactive
{
    public sealed class Unit
    {
        private Unit() { }
        public static Unit Instance { get; } = new Unit();
    }
}
