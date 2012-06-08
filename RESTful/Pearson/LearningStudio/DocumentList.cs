using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WellDunne.REST.Pearson.LearningStudio
{
    [JsonObject()]
    public sealed class DocumentList
    {
        [JsonProperty("docSharingDocuments")]
        public List<Document> Documents { get; set; }
    }

    [JsonObject()]
    public sealed class Document
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("fileName")]
        public string FileName { get; set; }
        [JsonProperty("fileDescription")]
        public string FileDescription { get; set; }
        [JsonProperty("fileSize")]
        public long FileSize { get; set; }
        [JsonProperty("uploadedTime")]
        public DateTimeOffset UploadedTime { get; set; }
        [JsonProperty("downloadCount")]
        public int DownloadCount { get; set; }
        // submitter
    }
}
