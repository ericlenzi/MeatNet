namespace Meat.Domain.Enums
{
    using System;
    using System.Reflection;

    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();

            string name = Enum.GetName(type, value);

            FieldInfo field = type.GetField(name);

            DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attr.Description;
        }

        //public static string GetSAPDescription(this Enum value)
        //{
        //    Type type = value.GetType();

        //    string name = Enum.GetName(type, value);

        //    FieldInfo field = type.GetField(name);

        //    SAPDescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(SAPDescriptionAttribute)) as SAPDescriptionAttribute;

        //    return attr.SAPDescription;
        //}

        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }

                //if (Attribute.GetCustomAttribute(field,
                //typeof(SAPDescriptionAttribute)) is SAPDescriptionAttribute sapAttribute)
                //{
                //    if (sapAttribute.SAPDescription == description)
                //        return (T)field.GetValue(null);
                //}
                //else
                //{
                //    if (field.Name == description)
                //        return (T)field.GetValue(null);
                //}
            }

            throw new ArgumentException("Not found.", nameof(description));
            // Or return default(T);
        }
    }
}