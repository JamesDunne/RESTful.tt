using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WellDunne.REST
{
    [JsonObject()]
    public sealed class ExpandableLinks<TLinkType> where TLinkType : class, new()
    {
        [JsonProperty("links")]
        public List<TLinkType> Links { get; set; }
    }

    [JsonObject()]
    public class LinkEntry
    {
        [JsonProperty("href")]
        public string Href { get; set; }
        [JsonProperty("rel")]
        public string Rel { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
