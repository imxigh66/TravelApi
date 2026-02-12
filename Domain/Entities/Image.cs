using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Image
    {
        public int ImageId { get; set; }

        public ImageEntityType EntityType { get; set; }
        public int EntityId { get; set; }

      
        public string ImageUrl { get; set; } = null!;
        public string? ThumbnailUrl { get; set; }

        public int? Width { get; set; }
        public int? Height { get; set; }
        public long? FileSize { get; set; }
        public string? MimeType { get; set; }
        public string? OriginalFileName { get; set; }

      
        public int SortOrder { get; set; }
        public bool IsCover { get; set; }
        public bool IsActive { get; set; } = true;

       
        public int? UploadedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public User? Uploader { get; set; }
    }
}
