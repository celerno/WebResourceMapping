namespace WebResourceMappingAPI.Models
{
    public class WebsiteContentModel
    {
        public string TargetSite { get; set; }
        public Dictionary<string, int> AllWordCounters { get; set; }
        public Dictionary<string, int> ContentWordCounters { get; set; }
        public string ErrorMessages { get; set; }
        public WebsiteContentModel(string targetSite)
        {
            TargetSite = targetSite;
            ErrorMessages = string.Empty;
            Images = Array.Empty<string>();
            AllWordCounters = new Dictionary<string, int>();
            ContentWordCounters = new Dictionary<string, int>();
        }

        public string[] Images { get; set; }
        public int WordCountAll { get; set; }
        public int WordCountContent { get; set; }
    }
}