using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WellDunne.REST.Pearson.LearningStudio
{
    [JsonObject()]
    public sealed class CourseListExpanded
    {
        [JsonProperty("courses")]
        public List<ExpandableLinks<CourseLinkExpanded>> Courses { get; set; }
    }

    [JsonObject()]
    public sealed class CourseLinkExpanded : LinkEntry
    {
        [JsonProperty("course")]
        public CourseEntry Course { get; set; }
    }

    [JsonObject()]
    public sealed class CourseEntry
    {
        [JsonProperty("instructors")]
        public ExpandableLinks<LinkEntry> Instructors { get; set; }
        [JsonProperty("teachingAssistants")]
        public ExpandableLinks<LinkEntry> TeachingAssistants { get; set; }
        [JsonProperty("students")]
        public ExpandableLinks<LinkEntry> Students { get; set; }
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("displayCourseCode")]
        public string DisplayCourseCode { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("callNumbers")]
        public string[] CallNumbers { get; set; }
        [JsonProperty("links")]
        public List<LinkEntry> Links { get; set; }
    }
}
