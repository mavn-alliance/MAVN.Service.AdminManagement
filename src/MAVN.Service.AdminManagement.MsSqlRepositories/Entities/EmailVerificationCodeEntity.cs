using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MAVN.Service.AdminManagement.Domain.Models.Verification;

namespace MAVN.Service.AdminManagement.MsSqlRepositories.Entities
{
    [Table("email_verification_codes")]
    public class EmailVerificationCodeEntity : IVerificationCode
    {
        [Key]
        [Required]
        [Column("admin_id")]
        public string AdminId { get; set; }

        [Required]
        [Column("code")]
        public string VerificationCode { get; set; }

        [Required]
        [Column("is_verified")]
        public bool IsVerified { get; set; }

        [Required]
        [Column("expire_date")]
        public DateTime ExpireDate { get; set; }

        internal static EmailVerificationCodeEntity Create(string adminId, string code, TimeSpan expirePeriod)
        {
            return new EmailVerificationCodeEntity
            {
                AdminId = adminId,
                VerificationCode = code,
                ExpireDate = DateTime.UtcNow.Add(expirePeriod),
                IsVerified = false
            };
        }
    }
}
