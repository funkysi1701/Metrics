namespace Metrics.Core.Model
{
    public class BlogPostsSingle
    {
        public string Type_of { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Readable_Publish_Date { get; set; }
        public string Slug { get; set; }
        public string Path { get; set; }
        public string Url { get; set; }
        public int Comments_Count { get; set; }
        public int Public_Reactions_Count { get; set; }
        public int? Collection_Id { get; set; }
        public DateTime? Published_Timestamp { get; set; }
        public int Positive_Reactions_Count { get; set; }
        public string Cover_Image { get; set; }
        public string Social_Image { get; set; }
        public string Canonical_Url { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Edited_At { get; set; }
        public string Crossposted_At { get; set; }
        public DateTime? Published_At { get; set; }
        public DateTime? Last_Comment_At { get; set; }
        public string Tag_List { get; set; }
        public List<string> Tags { get; set; }
        public string Body_Html { get; set; }
        public string Body_Markdown { get; set; }
        public User User { get; set; }
    }
}
