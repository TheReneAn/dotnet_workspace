using System.Collections.ObjectModel;
using RssReader.Model;

namespace RssReader.ViewModel
{
    public class MainVM
    {
        IRssHelper rssHelper;

        public ObservableCollection<Item> Items { get; set; }
        public MainVM(IRssHelper rssHelper)
        {
            this.rssHelper = rssHelper;

            Items = new ObservableCollection<Item>();

            ReadRss();
        }

        private void ReadRss()
        {
            var posts = rssHelper.GetPosts();

            Items.Clear();

            foreach (var post in posts)
            {
                Items.Add(post);
            }
        }
    }
}
