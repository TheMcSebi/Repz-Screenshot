using RepzScreenshot.Model;
using System;

namespace RepzScreenshot.Error
{
    [Serializable]
    class UserOfflineException : ApiException
    {
        public UserOfflineException(Player p) : base(String.Format("{0} is offline", p.Name)) { }
    }
}
