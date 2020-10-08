using System;

namespace Backend
{
    public interface ILogger
    {
        void Clear();
        void InnerLevelWrite(string label, Action actions);
        void OuterLevelWrite(string label, Action actions);
        void Log(object obj);
    }
}
