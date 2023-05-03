using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Neox.KnowHowTransfer.Models
{
    [Table("tb_employees")]
    public class Employee
    {
        /// <summary>
        /// Unique employee number
        /// </summary>
        /// <example>220901</example>
        [Key]
        [Column("id")]
        public string EmployeeId { get; set; }
        /// <summary>
        /// First name of the employee
        /// </summary>
        [Required]
        [Column("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the employee
        /// </summary>
        [Required]
        [Column("last_name")]
        public string LastName { get; set; }
    }
}
