using System;

namespace MAVN.Service.AdminManagement.Domain.Models
{
    public class CallRateLimitSettingsDto
    {
        public int EmailVerificationMaxAllowedRequestsNumber { get; set; }
        public TimeSpan EmailVerificationCallsMonitoredPeriod { get; set; }
    }
}
