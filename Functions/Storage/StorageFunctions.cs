using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace HeroServer
{
    public static class StorageFunctions
    {
        private static BlobServiceClient blobServiceClient;

        public static void Initialize()
        {
            if (String.IsNullOrEmpty(WebEnvConfig.StorageString))
                throw new Exception("ERROR : No StorageString Defined");

            blobServiceClient = new BlobServiceClient(WebEnvConfig.StorageString);
        }

        // CONTAINER
        public static async Task CreateContainer(String containerName)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            await container.CreateIfNotExistsAsync();
        }

        public static async Task<bool> ExistContainer(String containerName)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            return await container.ExistsAsync();
        }

        public static async Task<bool> DeleteContainer(String containerName)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            return await container.DeleteIfExistsAsync();
        }

        // READ
        public static async Task<byte[]> ReadFile(String containerName, String fileName, String fileExt)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blob = container.GetBlobClient(fileName + "." + fileExt);

            if (!blob.Exists())
                return null;

            BlobDownloadInfo downloadInfo = await blob.DownloadAsync();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await downloadInfo.Content.CopyToAsync(memoryStream);
                memoryStream.Close();

                return memoryStream.ToArray();
            }
        }

        // CREATE
        public static async Task CreateFile(String containerName, String fileName, String fileExt, byte[] content)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blob = container.GetBlobClient(fileName + "." + fileExt);

            using (MemoryStream stream = new MemoryStream(content))
            {
                BlobUploadOptions blobUploadOptions = new BlobUploadOptions();
                await blob.UploadAsync(stream, true);
            }
        }

        // UPDATE
        public static async Task UpdateFile(String containerName, String fileName, String fileExt, byte[] content, bool backup = true)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blob = container.GetBlobClient(fileName + "." + fileExt);

            if (backup && await blob.ExistsAsync())
            {
                Azure.Response<BlobProperties> response = await blob.GetPropertiesAsync();
                if (!response.HasValue)
                    throw new Exception("Update failed : Blob properties not Found");

                await CopyFile(container, blob, $"{fileName}_{response.Value.LastModified:yyyyMMdd-HHmmss}.{fileExt}");
            }

            using (MemoryStream stream = new MemoryStream(content))
            {
                BlobUploadOptions blobUploadOptions = new BlobUploadOptions();
                await blob.UploadAsync(stream, true);
            }
        }

        public static async Task UpdateCFile(String containerName, String fileName, String fileExt, byte[] content, bool backup = true)
        {
            await CreateContainer(containerName);
            await UpdateFile(containerName, fileName, fileExt, content, backup);
        }

        // COPY
        public static async Task CopyFile(String containerName, String sourceName, String targetName)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient source = container.GetBlobClient(sourceName);

            await CopyFile(container, source, targetName);
        }

        public static async Task<BlobClient> CopyFile(BlobContainerClient container, BlobClient source, String targetName)
        {
            BlobClient target = container.GetBlobClient(targetName);

            CopyFromUriOperation op = await target.StartCopyFromUriAsync(source.Uri);
            await op.WaitForCompletionAsync();

            Azure.Response<BlobProperties> response = await target.GetPropertiesAsync();
            if (!response.HasValue || response.Value.BlobCopyStatus != CopyStatus.Success)
                throw new Exception("Copy failed : " + (response.HasValue ? response.Value.BlobCopyStatus.ToString() : "Target properties not Found"));

            return target;
        }

        // DELETE
        public static async Task<bool> DeleteSoftFile(String containerName, String fileName, String fileExt)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blob = container.GetBlobClient(fileName + "." + fileExt);

            if (!blob.Exists())
                return false;

            BlobDownloadInfo downloadInfo = await blob.DownloadAsync();
            byte[] content = null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await downloadInfo.Content.CopyToAsync(memoryStream);
                memoryStream.Close();

                content = memoryStream.ToArray();
            }

            Azure.Response<BlobProperties> response = await blob.GetPropertiesAsync();
            if (!response.HasValue)
                throw new Exception("DeleteSoft failed : Blob properties not Found");

            await CopyFile(container, blob, $"{fileName}_{response.Value.LastModified:yyyyMMdd-HHmmss}.{fileExt}");

            await blob.DeleteAsync();
            return true;
        }

        public static async Task<bool> DeleteFile(String containerName, String fileName)
        {
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blob = container.GetBlobClient(fileName);

            return await blob.DeleteIfExistsAsync();
        }
    }
}
