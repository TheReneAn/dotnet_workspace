using SQLite;

namespace EvernoteClone.Model
{
    public class Notebook : HasId
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
    }
}
