using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PostImage
    {
        public int PostImageId { get; set; }
        public int PostId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int SortOrder { get; set; }  // Для сортировки (первое фото = обложка)
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Post Post { get; set; } = null!;
    }
}
