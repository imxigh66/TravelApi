using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CategoryTagLink
    {
        public int PlaceId { get; set; }
        public int CategoryTagId { get; set; }

        public Place Place { get; set; } = null!;
        public CategoryTag CategoryTag { get; set; } = null!;
    }
}
