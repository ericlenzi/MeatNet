using Meat.Queries.SincronizacionTablasOffline.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meat.Queries.SincronizacionTablasOffline.Response
{
    public class GiftcardsResponse
    {
        public IEnumerable<GiftcardDto> Giftcards { get; set; }
    }
}
