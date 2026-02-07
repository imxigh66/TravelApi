using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ExternalServices.S3
{
    public class S3Options
    {
        public string BucketName { get; set; } = null!;
        public string Region { get; set; } = "eu-central-1";
        public string? AccessKeyId { get; set; }
        public string? SecretAccessKey { get; set; }
        public string? CloudFrontUrl { get; set; }

        /// <summary>
        /// Если true, использует CloudFront URL вместо прямого S3
        /// </summary>
        public bool UseCloudFront { get; set; } = false;
    }
}
