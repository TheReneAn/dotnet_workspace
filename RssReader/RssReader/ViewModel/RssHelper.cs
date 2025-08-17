using System.IO;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using RssReader.Model;

namespace RssReader.ViewModel
{
    public class RssHelper : IRssHelper
    {
        public List<Item> GetPosts()
        {
            var posts = new List<Item>();

            var xmlSerializer = new XmlSerializer(typeof(FinZenBlog));

            using (var client = new WebClient())
            {
                var xml = Encoding.Default.GetString(client.DownloadData("https://www.latimes.com/local/rss2.0.xml"));

                using (Stream reader = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                {
                    var blog = (FinZenBlog)xmlSerializer.Deserialize(reader);

                    posts = blog.Channel.Item;
                }
            }

            return posts;
        }
    }
}
