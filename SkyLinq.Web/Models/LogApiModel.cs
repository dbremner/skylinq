using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using SkyLinq.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.Diagnostics;

namespace SkyLinq.Web.Models
{
    public class LogApiModel
    {
        private IEnumerable<string> _lines;
        public LogApiModel()
        {
            //SetUpDirectory();
            SetUpAzureStorage();
        }

        private void SetUpDirectory()
        {
            string directory = @"G:\codecamp\Linq\LogFiles";
            _lines = Directory.EnumerateFiles(directory)
                .SelectMany(path => LingToText.EnumLines(File.OpenText(path)));
        }

        private void SetUpAzureStorage()
        {
            // Retrieve storage account from connection string.
            var connStr = ConfigurationManager.ConnectionStrings["StorageConnectionString"];
            if (connStr == null || string.IsNullOrEmpty(connStr.ConnectionString))
                throw new Exception("StorageConnectionString not configured.");

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                connStr.ConnectionString);

            Uri storageUri = storageAccount.BlobStorageUri.PrimaryUri;

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            CloudBlobContainer container = blobClient.GetContainerReference(storageUri.AbsoluteUri);
            int pathLength = container.Uri.AbsolutePath.Length;

            _lines = container.ListBlobs(null, true)
                .OfType<CloudBlockBlob>()
                .AsParallel()
                .SelectMany(item =>
            {
                string blobAddressUri = item.Uri.AbsolutePath.Substring(pathLength + 1);
                CloudBlockBlob blockBlob2 = container.GetBlockBlobReference(blobAddressUri);
                StreamReader sr = new StreamReader(blockBlob2.OpenRead());
                return LingToText.EnumLines(sr);
            });
        }

        private IEnumerable<W3SVCLogRecord> GetLogRecords()
        {
            return _lines.AsW3SVCLogRecords();
        }
        public IEnumerable<IDictionary<string, object>> RunReport(string report)
        {
            return (IEnumerable<IDictionary<string, object>>)typeof(BuildInW3SVCLogReports)
                .GetMethod("Get" + report, BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, new object[] { GetLogRecords() });
        }

    }
}