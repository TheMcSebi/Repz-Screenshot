using System;

namespace RepzScreenshot.Error
{
    [Serializable]
    class ScreenshotException : ApiException
    {
        public ScreenshotException(string msg) : base(msg) { }

    }
}
