using System;

namespace RepzScreenshot.Error
{
    [Serializable]
    class ExceptionBase : Exception
    {
        public ExceptionBase(string msg) : base(msg) { }
        public ExceptionBase() { }
    }
}
