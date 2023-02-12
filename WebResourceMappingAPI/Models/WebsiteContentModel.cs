using HtmlAgilityPack;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;

namespace WebResourceMappingAPI.Models
{
    /// <summary>
    /// A list of all images, a count of all words, and a count of each word within the content.
    /// </summary>
    public class WebsiteContentModel
    {
        public int WordCountAll { get; set; }
        public int WordCountContent { get; set; }
        public string[] Images { get; set; }
        public Dictionary<string, int> AllWordCounters { get; set; }
        public Dictionary<string, int> ContentWordCounters { get; set; }
        public string ErrorMessages { get; set; }

        public WebsiteContentModel()
        {
            Images = Array.Empty<string>();
            AllWordCounters = new Dictionary<string, int>();
            ContentWordCounters = new Dictionary<string, int>();
            ErrorMessages = string.Empty;
        }

    }
    public static class HttpContentExtensions
    {
        public static WebsiteContentModel ProcessContent(this HttpContent content)
        {
            WebsiteContentModel result =
                 ProcessHttpContent(content);

            return result;
        }

        private static WebsiteContentModel ProcessHttpContent(HttpContent content)
        {
            WebsiteContentModel model = new WebsiteContentModel();
            using (TextReader textReader = new StreamReader(content.ReadAsStreamAsync().Result))
            {
                var d = new HtmlDocument();
                string html = textReader.ReadToEnd();
                var allWordMatches = Regex.Matches(html, @"\b\w+\b");
                model.WordCountAll = allWordMatches.Count;

                foreach(var wordCountMatch in 
                                allWordMatches
                                .GroupBy(x=> x.Value)
                                .Select(x => new { x.Key, Value = x.Count() }).OrderByDescending(x=>x.Value))
                {
                    model.AllWordCounters.Add(wordCountMatch.Key, wordCountMatch.Value);
                }

                try
                {
                    d.LoadHtml(html);
                    d.DocumentNode.CountWordsInAllContent(ref model);
                    d.DocumentNode.ExtractAllImages(ref model);
                }
                catch(Exception ex) {
                    //throw new ArgumentException("Error while parsing the html.", ex);
                    //better than throw an error, continue with the stats properly counted and add an error log.
                    model.ErrorMessages = $"Error while parsing the html." +
                        $"{Environment.NewLine}HTML:" +
                        $"{Environment.NewLine}{html}" +
                        $"{Environment.NewLine}{ex.Message}";
                }
            }
            return model;
        }
        static void ExtractAllImages(this HtmlNode node, ref WebsiteContentModel model)
        {
            IEnumerable<HtmlNode> imageNodes = node.HasChildNodes ? node.
                    ChildNodes.
                    Where(x =>
                        "img".Equals(x.OriginalName, StringComparison.OrdinalIgnoreCase))
                    : Array.Empty<HtmlNode>();
            IEnumerable<HtmlNode> otherNodes = node.HasChildNodes ? node.
                    ChildNodes.
                    Where(x =>
                        "img".Equals(x.OriginalName, StringComparison.OrdinalIgnoreCase) 
                        == false)
                    : Array.Empty<HtmlNode>();

            foreach (var img in imageNodes)
            {
                string src = img.Attributes["src"]?.Value ?? string.Empty;
                model.Images = model.Images.Append(src).ToArray();
            }
            foreach (var inner in otherNodes)
            {
                inner.ExtractAllImages(ref model);
            }
        }
        static void CountWordsInAllContent(this HtmlNode node, ref WebsiteContentModel model)
        {
            foreach (var inner in node.ChildNodes)
            {
                if (inner.HasChildNodes == false)
                {
                    var wordMatches = Regex.Matches(inner.InnerHtml, @"\b\w+\b", RegexOptions.Multiline);

                    model.WordCountContent += wordMatches.Count;

                    foreach (var wordCountMatch in
                                    wordMatches
                                    .GroupBy(x => x.Value)
                                    .Select(x => new { x.Key, Value = x.Count() }).OrderByDescending(x => x.Value))
                    {
                        if (model.ContentWordCounters.ContainsKey(wordCountMatch.Key))
                        {
                            model.ContentWordCounters[wordCountMatch.Key] += wordCountMatch.Value;
                        }
                        else
                        {
                            model.ContentWordCounters.Add(wordCountMatch.Key, wordCountMatch.Value);
                        }
                    }
                }
                else
                    inner.CountWordsInAllContent(ref model);
            }
        }
    }
}
