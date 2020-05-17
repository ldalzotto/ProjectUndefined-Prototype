namespace Timelines
{
    public interface TimelineAction
    {
        bool Equals(object obj);
        int GetHashCode();
    }
}