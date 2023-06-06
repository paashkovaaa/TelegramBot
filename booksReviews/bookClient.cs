using Newtonsoft.Json;
using booksReviews.Db;


namespace booksReviews
{
    public class bookClient
    {
        private HttpClient _httpclient;
        public static string _adress;
        private static ApplicationDbContext _applicationDbContext = new ApplicationDbContext();

        public bookClient()
        {
            _httpclient = new HttpClient();
            _adress = Constants.adress;
            _httpclient.BaseAddress = new Uri(_adress);
        }

        public async Task<BookModel> SearchBooksAsync(string query)
        {
            var url = $"/books/v1/volumes?q={query}";

            var response = await _httpclient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BookModel>(content);

            return result;
        }
        public async Task<Entities> GetBookByTitle(string title)
        {
            var book = await _applicationDbContext.Books.FindAsync(title);
            if (book == null)
            {
                throw new KeyNotFoundException($"Unable to find entity with such key {title}");
            }
            return book;
        }
        public List<Entities> GetBooks()
        {
            var books = _applicationDbContext.Books.ToList();
            return books;
        }

        public async Task AddBook(string title)
        {
            if (await _applicationDbContext.Books.FindAsync(title) != null)
                throw new InvalidOperationException("Entity with such key already exists in database");
            var url = $"/books/v1/volumes?q={title}";
            var response = await _httpclient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            BookModel? bookModel = JsonConvert.DeserializeObject<BookModel>(content);
            if (bookModel.Items.Count > 0)
            {
                var book = bookModel.Items.FirstOrDefault(b => b.VolumeInfo.Title == title);
                if (book != null)
                {
                    Entities newBook = new Entities
                    {
                        Title = book.VolumeInfo.Title,
                        Authors = book.VolumeInfo.Authors != null ? string.Join(", ", book.VolumeInfo.Authors) : null,
                        PublishedDate = book.VolumeInfo.PublishedDate,
                        Description = book.VolumeInfo.Description,
                        Categories = book.VolumeInfo.Categories != null
                            ? string.Join(", ", book.VolumeInfo.Categories)
                            : null
                    };
                    _applicationDbContext.Books.Add(newBook);
                    _applicationDbContext.SaveChanges();
                }
            }
        }

        public async Task DeleteBook(string title)
        {
            var book = await _applicationDbContext.Books.FindAsync(title);
            if (book == null)
            {
                throw new KeyNotFoundException($"Unable to find entity with such key {title}");
            }
            if (book != null)
            {
                _applicationDbContext.Books.Remove(book);
                await _applicationDbContext.SaveChangesAsync();
            }
        }
        public async Task UpdateBook(string title, UpdateModel upBook)
        {

            var book = await _applicationDbContext.Books.FindAsync(title);
            if (book == null)
            {
                throw new KeyNotFoundException($"Unable to find entity with such key {title}");
            }
            book.Review = upBook.Review;
            book.Rating = upBook.Rating;

            _applicationDbContext.SaveChanges();
        }
        
    }
}


