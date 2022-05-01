using System;
using Linearstar.Windows.RawInput;

namespace RawInput.Sharp.SimpleExample
{
    public class RawInputEventArgs : EventArgs
    {
        public RawInputEventArgs(RawInputData data)
        {
            Data = data;
        }

        public RawInputData Data { get; }
    }
}
