namespace Meat.Domain.Enums
{
    using System;

    public class DescriptionAttribute : Attribute
    {
        private string description;

        public DescriptionAttribute(string description)
        {
            this.description = description;
        }

        public string Description { get => this.description; }
    }
}
