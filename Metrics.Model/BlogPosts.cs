namespace Metrics.Model
{
    public class BlogPosts : BasePosts
    {
        public bool Published { get; set; }
        public int Page_Views_Count { get; set; }
        public string Body_Mardown { get; set; }
    }
}
