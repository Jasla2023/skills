using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skills
{
  public class Skill
    {
        [Key]
        int skill_Id;
        string skillName;
        int skillLevel;
        int employee_Id;
     
      public int Skill_Id { get; set; }
      public string SkillName { get; set; }
      public int SkillLevel { get; set; }

    public int Employee_Id { get; set;}

    }
}
