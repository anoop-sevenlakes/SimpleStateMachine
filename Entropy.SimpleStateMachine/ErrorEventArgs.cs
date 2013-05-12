using System;

namespace Entropy.SimpleStateMachine
{
    public class ErrorEventArgs : EventArgs
    {
        private readonly Exception _error;

        public ErrorEventArgs(Exception error)
        {
            _error = error;
        }

        public Exception Error
        {
            get { return _error; }
        }
    }
}