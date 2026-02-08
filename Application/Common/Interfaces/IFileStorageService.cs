using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType,string folderPath,CancellationToken cancellationToken=default);
        Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
        Task<string> GetPreSignedUrlAsync(string fileKey,int expirationMinutes=60);
        Task<string> UploadImageAsync(
            IFormFile file,
            string folder = "posts",
            CancellationToken cancellationToken = default);
    }
}
