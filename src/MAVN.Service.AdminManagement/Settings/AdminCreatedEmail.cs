using System;
using JetBrains.Annotations;

namespace MAVN.Service.AdminManagement.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AdminCreatedEmail
    {
        public string EmailTemplateId { set; get; }
        public string SubjectTemplateId { set; get; }
        public TimeSpan VerificationLinkExpirePeriod { get; set; }
        public string VerificationLinkPath { set; get; }
    }
}
