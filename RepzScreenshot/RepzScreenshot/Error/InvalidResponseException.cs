using System;

namespace RepzScreenshot.Error
{
    [Serializable]
    class InvalidResponseException : ApiException
    {
        public InvalidResponseException() : base("Invalid response from server") { }
    }
}
