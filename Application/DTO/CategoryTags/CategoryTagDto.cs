using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.CategoryTags
{
    public class CategoryTagDto
    {
        public int CategoryTagId { get; set; }
        public string Name { get; set; } = null!;
        public string? Icon { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
