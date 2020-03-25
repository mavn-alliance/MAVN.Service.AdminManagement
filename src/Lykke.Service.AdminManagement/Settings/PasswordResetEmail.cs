using JetBrains.Annotations;

namespace Lykke.Service.AdminManagement.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class PasswordResetEmail
    {
        public string EmailTemplateId { set; get; }
        public string SubjectTemplateId { set; get; }
    }
}