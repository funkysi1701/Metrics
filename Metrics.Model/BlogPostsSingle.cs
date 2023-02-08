namespace Metrics.Model
{
    public class BlogPostsSingle : BasePosts
    {
        public string Readable_Publish_Date { get; set; }
        public int? Collection_Id { get; set; }
        public string Social_Image { get; set; }
        public DateTime? Created_At { get; set; }
        public DateTime? Edited_At { get; set; }
        public string Crossposted_At { get; set; }
        public DateTime? Last_Comment_At { get; set; }
        public List<string> Tags { get; set; }
        public string Body_Html { get; set; }
    }
}
