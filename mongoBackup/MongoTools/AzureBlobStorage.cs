using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace mongoBackup.MongoTools {

    public class AzureBlobStorage {


        /// <summary>
        /// Uploads backup zip to azure blob
        /// </summary>
        /// <param name="path">file to path</param>
        /// <returns>success state</returns>
        public static async Task<bool> UploadAsync(string path) {

            var uploaded = false;

            if (!string.IsNullOrEmpty(path)) {
                try {
                    if (File.Exists(path)) {
                        
                        var blobservice_client = new BlobServiceClient(AppData.Settings.azureblob_settings.azureblob_endpoint);
                        var container_client = blobservice_client.GetBlobContainerClient(AppData.Settings.azureblob_settings.azureblob_name);
                        var blobclient = container_client.GetBlobClient(path);

                        Console.WriteLine($"Uploading: {path}");

                        using (var upload_stream = File.OpenRead(path)) {

                            await blobclient.UploadAsync(upload_stream, true);
                            upload_stream.Close();
                        }

                        Console.WriteLine("Finished upload");
                        uploaded = true;
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"!** Failed uploading backup: {ex.Message} **!");
                }
            }

            return uploaded;
        }
    }
}