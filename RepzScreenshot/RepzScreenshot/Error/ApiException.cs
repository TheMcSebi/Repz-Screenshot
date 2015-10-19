using System;

namespace RepzScreenshot.Error
{
    [Serializable]
    class ApiException : ExceptionBase
    {
        public ApiException(string msg) : base(msg) { }
    }
}
