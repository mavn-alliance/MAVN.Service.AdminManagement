using System;

namespace MAVN.Service.AdminManagement.Settings
{
    public class LimitationSettings
    {
        public int EmailVerificationMaxAllowedRequestsNumber { get; set; }
        public TimeSpan EmailVerificationCallsMonitoredPeriod { get; set; }
    }
}
