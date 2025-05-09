using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers
{
    public interface IErrorNotificationService
    {
        void NotifyAdministrators(Exception exception, string additionalInfo = null);
    }
}