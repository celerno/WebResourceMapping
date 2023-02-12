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
        public IList<string> Images { get; set; }

        public string ErrorMessages { get; set; }

        public WebsiteContentModel()
        {
            Images = Array.Empty<string>();
        }

    }
    public static class HttpContentExtensions
    {
        public static WebsiteContentModel ProcessContent(this HttpContent content)
        {
            (IList<string> images, int contentWords, int allWords, string ErrorMessage) stats =
                 ProcessHttpContent(content);

            return new WebsiteContentModel
            {
                Images = stats.images,
                WordCountAll = stats.allWords,
                WordCountContent = stats.contentWords
            };
        }

        private static (IList<string> images, int contentWords, int allWords, string ErrorMessage) ProcessHttpContent(HttpContent content)
        {
            (IList<string> images, int contentWords, int allWords, string ErrorMessage) stats = (Array.Empty<string>(), 0, 0, string.Empty);

            using (TextReader textReader = new StreamReader(content.ReadAsStreamAsync().Result))
            {
                var d = new HtmlDocument();
                string html = textReader.ReadToEnd();
                stats.allWords = Regex.Matches(html, @"\b\w+\b").Count;
                try
                {
                    d.LoadHtml(html);
                    d.DocumentNode.CountWordsInAllContent(ref stats.contentWords);
                    d.DocumentNode.ExtractAllImages(ref stats.images);
                }
                catch(Exception ex) {
                    //throw new ArgumentException("Error while parsing the html.", ex);
                    //better than throw an error, continue with the stats properly counted and add an error log.
                    stats.ErrorMessage = $"Error while parsing the html." +
                        $"{Environment.NewLine}HTML:" +
                        $"{Environment.NewLine}{html}" +
                        $"{Environment.NewLine}{ex.Message}";
                }
            }
            return stats;
        }
        static void ExtractAllImages(this HtmlNode node, ref IList<string> images)
        {
            IEnumerable<HtmlNode> imageNodes = node.HasChildNodes ? node.
                    ChildNodes.
                    Where(x =>
                        "img".Equals(node.OriginalName, StringComparison.OrdinalIgnoreCase))
                    : Array.Empty<HtmlNode>();
            IEnumerable<HtmlNode> otherNodes = node.HasChildNodes ? node.
                    ChildNodes.
                    Where(x =>
                        "img".Equals(node.OriginalName, StringComparison.OrdinalIgnoreCase) 
                        == false)
                    : Array.Empty<HtmlNode>();

            foreach (var img in imageNodes)
            {
                string src = img.Attributes["src"]?.Value ?? string.Empty;
                images.Add(src);
            }
            foreach (var inner in otherNodes)
            {
                inner.ExtractAllImages(ref images);
            }
        }
        static void CountWordsInAllContent(this HtmlNode node, ref int wordCount)
        {
            foreach (var inner in node.ChildNodes)
            {
                if (inner.HasChildNodes == false)
                {
                    wordCount += Regex.Matches(inner.InnerHtml, @"\b\w+\b", RegexOptions.Multiline).Count;
                }
                else
                    inner.CountWordsInAllContent(ref wordCount);
            }
        }
    }
}
