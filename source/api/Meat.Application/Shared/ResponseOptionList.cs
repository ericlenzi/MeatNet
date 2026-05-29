using System;
using System.Collections.Generic;

namespace Meat.Application.Shared
{
    public class ResponseOptionList
    {
        public IEnumerable<OptionList> Data { get; set; }
    }

    public class OptionList
    {
        public Guid Id { get; set; }

        public string Descripcion { get; set; }
    }
}
