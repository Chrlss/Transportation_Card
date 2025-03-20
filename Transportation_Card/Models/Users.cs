using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Transportation_Card.Models
{
    [Index(nameof(CardNumber), IsUnique = true)]
    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required, StringLength(14)]
        public string CardNumber { get; set; }

        [Required, StringLength(255)]
        public string FullName { get; set; }

        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Address { get; set; }

        [Required, StringLength(15)]
        public string MobileNumber { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal InitialLoad { get; set; }

        public DateTime? LastUsedDate { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? ExpirationDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string SeniorCitizenCard { get; set; }

        public string PwdId { get; set; }
   
        public string GetCardType()
        {
            if (!string.IsNullOrEmpty(SeniorCitizenCard) || !string.IsNullOrEmpty(PwdId))
            {
                return "Discounted";
            }
            else
            {
                return "Regular";
            }
        }
    }
}
