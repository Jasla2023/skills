using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Skills
{
    [Table("Skills")]
    public class Skill 
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}


        [Key]
        int skill_Id;
        string skillName;
        string skillLevel;
        int employee_Id;
       

        //public string SkillLevelString { get;  set; }
       

        public int Skill_Id { get; set; }
        [Column("SkillName")]
        public string SkillName { get; set; }
        [Column("SkillLevel")]
        public string SkillLevel
        {
            get; set;
            //get { return skillLevel; }
            //set
            //{
            //    if (skillLevel != value)
            //    {
            //        skillLevel = value;
            //        SkillLevelString = GetConvertedSkillLevelIntoString(skillLevel);
            //        OnPropertyChanged(nameof(SkillLevel));
            //        OnPropertyChanged(nameof(SkillLevelString));
            //    }
            //}
          
        }
        [Column("Employee_Id")]
        public int Employee_Id { get; set;}

        // <summary>
        /// Converts a digit-based level to its description
        /// </summary>
        /// <param name="l">A digit within the range [1;4] representing the skill level</param>
        /// <returns>A skill description of the corresponding skill level</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thtows an ArgumentOutOfRangeException if l is not within the range [1;4]</exception>
        private string Level_DigitToString(int l)
        {
            switch (l)
            {
                case 1: return "Grundkenntnisse";
                case 2: return "Fortgeschrittene Kenntnisse";
                case 3: return "Bereits in Projekt eingesetzt";
                case 4: return "Umfangreiche Projekterfahrungen";
                default: throw new ArgumentOutOfRangeException();
            }
        }









        //private static string GetConvertedSkillLevelIntoString(int skillLevel)
        //{
        //    switch (skillLevel)
        //    {
        //        case 1:
        //            return "Grundkenntnisse";
        //        case 2:
        //            return "Fortgeschrittene Kenntnisse";
        //        case 3:
        //            return "Bereits in Projekt eingesetzt";
        //        case 4:
        //            return "Umfangreiche Kenntnisse";
        //        default:
        //            return "";
        //    }
        //}















    }
}
