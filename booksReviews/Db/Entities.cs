using System.ComponentModel.DataAnnotations;

namespace booksReviews.Db;

public class Entities
{
    [Key]
    public string? Title { get; set; }
    public string? Authors { get; set; }
    public string? PublishedDate { get; set; }
    public string? Description { get; set; }
    public string? Categories { get; set; }
    public string? Review { get; set; }
    public double? Rating { get; set; }
}