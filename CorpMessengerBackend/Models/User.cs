using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorpMessengerBackend.Models
{
    public class User
    {
        [Key]
        public long UserId { get; set; }

        [Required]
        public long DepartmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string SecondName { get; set; }

        [StringLength(50)]
        public string Patronymic { get; set; }

        [StringLength(255)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        public string Email { get; set; }

        public DateTime Modified { get; set; }
        public bool Deleted { get; set; }


        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }
    }
}
