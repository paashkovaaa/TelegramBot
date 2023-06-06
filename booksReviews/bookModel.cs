namespace booksReviews
{
    public class BookModel
    {
        public List<Item>? Items { get; set; }

    }


    public class VolumeInfo
    {
        public string? Title { get; set; }
        public List<string>? Authors { get; set; }
        public string? PublishedDate { get; set; }
        public string? Description { get; set; }
        public List<string>? Categories { get; set; }
    }

    public class Item
    {
        public VolumeInfo? VolumeInfo { get; set; }
    }
}
