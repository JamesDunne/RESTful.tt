using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WellDunne.REST.Pearson.LearningStudio
{
    [JsonObject()]
    public sealed class DocumentCategoryList
    {
        [JsonProperty("docSharingCategories")]
        public List<DocumentCategory> Categories { get; set; }
    }

    [JsonObject()]
    public sealed class DocumentCategory
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        // assignedGroup
    }
}
