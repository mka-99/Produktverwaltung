using Azure.Storage.Blobs;

namespace Produktverwaltung.Helper
{
    public class BlobService
    {
        private readonly IConfiguration Configuration;
        string connectionString;
        BlobServiceClient blobServiceClient;

        public BlobService(IConfiguration configuration)
        {
            Configuration = configuration;
            connectionString = Configuration["StorageConnectionString"];
            blobServiceClient = new BlobServiceClient(connectionString);
        }

        public BlobContainerClient GetOrCreateBlobContainer(string blobContainerName)
        {
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobContainerName);

            if (!blobContainerClient.Exists())
            {
                return blobServiceClient.CreateBlobContainer(blobContainerName);
            }
            else
            {
                return blobContainerClient;
            }
        }

        public void UploadDataToBlobContainer(string localPath, string fileName, string blobContainerName)
        {
            string localFilePath = Path.Combine(localPath, fileName + ".png");

            // Get a reference to a blob
            BlobClient blobClient = GetOrCreateBlobContainer(blobContainerName).GetBlobClient(fileName);

            // Upload data from the file
            try
            {
                blobClient.Upload(localFilePath, true);
            }
            catch (Exception e)
            {

            }
        }
    }
}
