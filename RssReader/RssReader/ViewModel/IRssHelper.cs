using RssReader.Model;

namespace RssReader.ViewModel
{
    public interface IRssHelper
    {
        public List<Item> GetPosts();
    }
}
