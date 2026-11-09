using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDayAPI.Services
{
    public class BlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName = "raceday-images";

        public BlobService(IConfiguration configuration)
        {
            
            _connectionString = configuration["AzureBlobStorage"] ?? "UseDevelopmentStorage=true";
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            return blobClient.Uri.ToString();
        }
    }
}