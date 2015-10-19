using System;

namespace RepzScreenshot.Error
{
    [Serializable]
    class UserNotFoundException : ApiException
    {

        public UserNotFoundException():base("User could not be found") { }

    }
}
