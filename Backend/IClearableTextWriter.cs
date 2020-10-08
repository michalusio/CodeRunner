namespace Backend
{
    public interface IClearableTextWriter
    {
        bool Open { get; set; }
        void Clear();
    }
}
