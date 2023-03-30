using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skills
{
    class Skill
    {
        string skillName;
        int skillLevel;
     
        public string SkillName
        {
            get
            {
                return skillName;
            }

            set
            {
                skillName = value;
            }
        }

        public int SkillLevel
        {
            get
            {
                return skillLevel;
            }

            set
            {
                skillLevel = value;
            }
        }

    }
}
