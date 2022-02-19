using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorpMessengerBackend.Models
{
    public class Department
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long DepartmentId { get; set; }

        [Required]
        public string DepartmentName { get; set; }
        public DateTime Modified { get; set; }
    }
}
