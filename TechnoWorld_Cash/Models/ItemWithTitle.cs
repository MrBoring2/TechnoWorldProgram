namespace TechnoWorld_API.Models
{
    public class ItemWithTitle<T>
    {
        public ItemWithTitle(T item, string title)
        {
            Item = item;
            Title = title;
        }

        public T Item { get; set; }
        public string Title { get; set; }
    }
}
