using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Skills
{
    public class SkillLevelToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int skillLevel = (int)value;

            switch (skillLevel)
            {
                case 1:
                    return "Grundkenntnisse";
                case 2:
                    return "Fortgeschrittene Kenntnisse";
                case 3:
                    return "Expertenkenntnisse";
                default:
                    return "Keine Kenntnisse";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
