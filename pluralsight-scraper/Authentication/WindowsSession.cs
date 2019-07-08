using System;

namespace VH.PluralsightScraper.Authentication
{
    internal class WindowsSession : ISession
    {
        #region Implementation of ISession

        public string CurrentUser => Environment.UserName;

        #endregion
    }
}
