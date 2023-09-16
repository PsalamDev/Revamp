using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extensions
{
    public static class EnumExtensionMethods
    {
        public static string GenerateCasesForEnum(Type type, string columnName)
        {
            var array = GetEnumProperties(type);

            var resultBuilder = new StringBuilder("CASE");
            foreach (var item in array)
            {
                AppendCaseForEnumProperty(columnName, resultBuilder, item);
            }

            return resultBuilder.Append(" END").ToString();
        }

        public static string GetDescription(this Enum genericEnum)
        {
            return GetEnumAttributeProperty(genericEnum,
                                            genericEnum.ToString(),
                                            (DescriptionAttribute attr) => attr.Description);
        }

        // mini factory for getting the value converter
        private static TProperty GetEnumAttributeProperty<TProperty, TAttribute>(
            Enum genericEnum,
            TProperty defaultValue,
            Func<TAttribute, TProperty> map) where TAttribute : Attribute
        {
            var genericEnumType = genericEnum.GetType();
            var memberInfo = genericEnumType.GetMember(genericEnum.ToString());
            if (memberInfo.Length <= 0)
            {
                return defaultValue;
            }

            var attributes = memberInfo[0].GetCustomAttributes(typeof(TAttribute), false);
            return attributes.Any()
                       ? map((TAttribute)attributes.ElementAt(0))
                       : defaultValue;
        }



        private static void AppendCaseForEnumProperty(string columnName, StringBuilder resultBuilder, Enum item)
        {
            resultBuilder.Append(" WHEN ")
                         .Append(columnName)
                         .Append(" = ")
                         .Append(item.ToString("D"))
                         .Append(" THEN '")
                         .Append(GetDescription(item))
                         .Append("'");
        }



        private static IEnumerable<Enum> GetEnumProperties(Type type)
        {
            var array = Enum.GetValues(type).OfType<Enum>();
            return array;
        }
    }
}
