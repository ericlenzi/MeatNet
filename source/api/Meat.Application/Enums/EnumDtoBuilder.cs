namespace Meat.Application.Enums
{
    using Meat.Domain.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    public static class EnumDtoBuilder
    {
        public static IEnumerable<EnumDto> EnumToEnumDto<TEnum>()
            where TEnum : Enum
        {
            List<EnumDto> enums = new List<EnumDto>();

            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                enums.Add(
                    new EnumDto()
                    {
                        Code = (int)Enum.Parse(typeof(TEnum), enumValue.ToString()),
                        Name = enumValue.ToString(),
                        Description = enumValue.GetDescription()
                    }
                );
            }

            return enums;
        }

        public static IEnumerable<EnumCategoryDto> EnumToEnumCategoryDto<TEnum>()
            where TEnum : Enum
        {
            List<EnumCategoryDto> enums = new List<EnumCategoryDto>();

            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                enums.Add(
                    new EnumCategoryDto()
                    {
                        Code = (int)Enum.Parse(typeof(TEnum), enumValue.ToString()),
                        Name = enumValue.ToString(),
                        Description = enumValue.GetDescription(),
                        Categoria = enumValue.GetCategory()
                    }
                );
            }

            return enums;
        }

        private static string GetCategory<TEnum>(this TEnum source)
        {
            FieldInfo fieldInfo = source.GetType().GetField(source.ToString());

            CategoryAttribute attribute = (CategoryAttribute)fieldInfo.GetCustomAttribute(typeof(CategoryAttribute), false);

            return attribute.Category;
        }
    }
}
