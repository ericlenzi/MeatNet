using System.ComponentModel;

namespace Meat.Application.Shared
{
    public class RequestListBase : RequestBase
    {
        public string Filter { get; set; }

        [DefaultValue(0)]
        public int PageIndex { get; set; } = 0;

        [DefaultValue(int.MaxValue)]
        public int PageSize { get; set; } = int.MaxValue;
    }
}
