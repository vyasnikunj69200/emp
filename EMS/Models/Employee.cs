using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models
{

    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Employee Name")]
        public string Name { get; set; }
        public string Gender { get; set; }
        public DesignationList Designation { get; set; }
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }
        public bool Dotnet { get; set; }
        public bool CSharp { get; set; }
        public bool Angular { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string? ImagePath { get; set; }
        public DateTime? RecordCreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public enum DesignationList
        {
            Select = 0,
            Intern = 1,
            Trainee = 2,
            SoftwareDeveloper = 3,
            TeamLeader = 4,
        }
    }


}
