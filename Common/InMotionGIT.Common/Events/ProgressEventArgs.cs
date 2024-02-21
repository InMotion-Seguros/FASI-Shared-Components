using System;

namespace InMotionGIT.Common.Events
{

    public class ProgressEventArgs : EventArgs
    {

        public string Message { get; set; }
        public int Level { get; set; }
        public int Index { get; set; }

        public ProgressEventArgs(string message, int level, int index)
        {
            Message = message;
            Level = level;
            Index = index;
        }

    }

}