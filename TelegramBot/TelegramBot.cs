using System.Text;
using booksReviews;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;
using booksReviews.Db;
namespace BookShelfBot;

    class TelegramBot
    {
        public static ITelegramBotClient _bot = new TelegramBotClient("6166533107:AAFPE2icpdXVUmxwwnJgGwh1uRA_3ZNL7CA");
        private static Dictionary<long, string> userState = new Dictionary<long, string>();
        private static ApplicationDbContext _applicationDbContext = new ApplicationDbContext();
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Telegram.Bot.Types.Update update, CancellationToken cancellationToken)
        {
            Console.WriteLine(JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                IReplyMarkup keyboard = Keyboard.GetButtons();
                IReplyMarkup keyboardUp = Keyboard.GetButtonsUpdate();
                var message = update.Message;
                if (userState.ContainsKey(message!.Chat.Id) && userState[message.Chat.Id] == "Search Books")
                {
                    var httpClient = new HttpClient();
                    var url = $"https://www.googleapis.com/books/v1/volumes?q={Uri.EscapeDataString(message.Text)}";
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var bookModel = JsonConvert.DeserializeObject<BookModel>(responseContent);
                        var books = bookModel.Items.Select(b => b).ToList();
                        string messageText = "Books\n";
                        foreach (var item in books)
                        {
                            var volumeInfo = item.VolumeInfo;
                            string title = volumeInfo.Title ?? "Unknown Title";
                            string authors = volumeInfo.Authors != null
                                ? string.Join(", ", volumeInfo.Authors)
                                : "Unknown Authors";
                            string publishedDate = volumeInfo.PublishedDate ?? "Unknown Published Date";
                            string description = volumeInfo.Description ?? "No Description";
                            string categories = volumeInfo.Categories != null
                                ? string.Join(", ", volumeInfo.Categories)
                                : "No Categories";

                            messageText = $"📖 {title}\nAuthors: {authors}\n" +
                                          $"Published Date: {publishedDate}\nDescription: {description}\n" +
                                          $"Categories: {categories}\n";
                            await botClient.SendTextMessageAsync(message.Chat, text: messageText,
                                replyMarkup: keyboardUp);
                        }

                        userState.Remove(message.Chat.Id);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat,
                            text: "😔Sorry, but this book is not find");
                        userState.Remove(message.Chat.Id);
                    }
                }
                else if (userState.ContainsKey(message!.Chat.Id) && userState[message.Chat.Id] == "Add Book")
                {
                    var title = message.Text;
                    var data = new { title = title };
                    var httpClient = new HttpClient();
                    var url = $"https://localhost:7210/Book/addBook?title={title}";
                    var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8,
                        "application/json");
                    var response = await httpClient.PostAsync(url, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var book = JsonConvert.DeserializeObject<Entities>(responseContent);
                        await botClient.SendTextMessageAsync(message.Chat, text:
                          $"📖{book.Title}\nAuthors:{book.Authors}\nDate:{book.PublishedDate}\nDescription:{book.Description}\nCategories:{book.Categories}\nReview:{book.Review}\nRating:{book.Rating}\n");
                        userState.Remove(message.Chat.Id);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat,
                            text: "😔Sorry, but this book is not find");
                        userState.Remove(message.Chat.Id);
                    }
                }
                else if (userState.ContainsKey(message!.Chat.Id) && userState[message.Chat.Id] == "Delete Book")
                {
                    var title = message.Text;
                    var book = await _applicationDbContext.Books.FindAsync(title);
                    if (book == null)
                    {
                        await botClient.SendTextMessageAsync(message.Chat,
                            text: "😔Sorry, we could not find that in our database.");
                        userState.Remove(message.Chat.Id);
                    }
                    else
                    {
                        var httpClient = new HttpClient();
                        var url = $"https://localhost:7210/Book/Book?query={title}";
                        var response = await httpClient.DeleteAsync(url);
                        response.EnsureSuccessStatusCode();
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<Entities>(responseContent);
                        userState.Remove(message.Chat.Id);
                        await botClient.SendTextMessageAsync(message.Chat,
                            text: "Fantastic! You've just deleted your book🫶", replyMarkup: keyboard);
                    }
                }
                else if (userState.ContainsKey(message!.Chat.Id) && userState[message.Chat.Id] == "Review")
                {
                    string[] parameters = message.Text.Split(';');
                    string title = parameters[0];
                    string review = parameters[1];
                    double rating = double.Parse(parameters[2]);
                    var books = await _applicationDbContext.Books.FindAsync(title);
                    if (books != null)
                    {
                        var data = new
                        {
                            Review = review,
                            Rating = rating
                        };

                        var httpClient = new HttpClient();
                        var url = $"https://localhost:7210/Book/Update?title={title}";
                        var jsonContent = JsonConvert.SerializeObject(data);
                        var requestContent =
                            new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                        var response = await httpClient.PutAsync(url, requestContent);
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var book = JsonConvert.DeserializeObject<Entities>(responseContent);
                        await botClient.SendTextMessageAsync(message.Chat, text:
                            $"📖{book.Title}\nAuthors:{book.Authors}\nDate:{book.PublishedDate}\nDescription:{book.Description}\nCategories:{book.Categories}\nReview:{book.Review}\nRating:{book.Rating}\n");
                        userState.Remove(message.Chat.Id);
                        await botClient.SendTextMessageAsync(message.Chat,
                            text: "Fantastic! You've just added review🫶", replyMarkup: keyboard);

                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat,
                            text: "😔Sorry, we could not find that in our database.");
                        userState.Remove(message.Chat.Id);
                    }
                }
                else
                {

                    if (message.Text.ToLower() == "/start")
                    {
                        await botClient.SendTextMessageAsync(message.Chat,
                            "📚 Welcome to BookReviews! 🤖\n\nI'm your personal book assistant, here to help you explore, discover, and organize your favorite books. Whether you're an avid reader or just starting your literary journey, I've got you covered!\n\nWith BookShelf, you can:\n\n🔍 Search for books by title.\n📖 Get detailed information about a book, including synopsis, ratings, and reviews.\n📚 Create your own virtual bookshelf.\n📝 Leave reviews and ratings for the books you've read.\n\nJust type in any book-related query, and I'll do my best to provide you with the information you need. Let's embark on a literary adventure together! Happy reading! 📖✨");
                        await botClient.SendTextMessageAsync(message.Chat, text: "Choose options:",
                            replyMarkup: keyboard);
                    }
                    else if (message.Text == "My Book Shelf")
                    {
                        var httpClient = new HttpClient();
                        var url = "https://localhost:7210/Book/AllBook";
                        var response = await httpClient.GetAsync(url);
                        response.EnsureSuccessStatusCode();
                        var responseContent = await response.Content.ReadAsStringAsync();
                        List<Entities> Books = JsonConvert.DeserializeObject<List<Entities>>(responseContent)!;
                        string messageText = "Shelf Book:\n";
                        foreach (Entities book in Books)
                        {
                            messageText +=
                                $"📖{book.Title}\nAuthors:{book.Authors}\nDate:{book.PublishedDate}\nDescription:{book.Description}\nCategories:{book.Categories}\nReview:{book.Review}\nRating:{book.Rating}\n";
                        }

                        await botClient.SendTextMessageAsync(message.Chat, text: messageText,
                            replyMarkup: keyboardUp);
                    }
                    else if (message.Text == "Search Books")
                    {
                        await botClient.SendTextMessageAsync(message.Chat,
                            text: "If you want to find book input keyword");
                        userState[message.Chat.Id] = "Search Books";
                    }
                    else if (message.Text == "Add Book")
                    {
                        await botClient.SendTextMessageAsync(message.Chat,
                            text: "If you want to add book input title");
                        userState[message.Chat.Id] = "Add Book";
                    }
                    else if (message.Text == "Delete Book")
                    {
                        await botClient.SendTextMessageAsync(message.Chat,
                            text: "If you want to delete book input title");
                        userState[message.Chat.Id] = "Delete Book";
                    }
                    else if (message.Text == "Review")
                    {
                        await botClient.SendTextMessageAsync(message.Chat,
                            text: "If you want to add review input (for example: title;review;rating)");
                        userState[message.Chat.Id] = "Review";
                    }
                    else if (message.Text == "Exit")
                    {
                        await botClient.SendTextMessageAsync(message.Chat, text: "Choose options:",
                            replyMarkup: keyboard);
                    }
                }
            }
            
        }
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
    }
    
