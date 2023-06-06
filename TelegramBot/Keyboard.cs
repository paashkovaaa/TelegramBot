using Telegram.Bot.Types.ReplyMarkups;

namespace BookShelfBot
{
    public class Keyboard
    {
        public static IReplyMarkup GetButtons()
        {
            KeyboardButton buttonMyBookShelf = new KeyboardButton("My Book Shelf");
            KeyboardButton buttonSearchBooks = new KeyboardButton("Search Books");


            KeyboardButton[][] buttons = new KeyboardButton[][]
            {
                new KeyboardButton[] { buttonMyBookShelf},
                new KeyboardButton[] { buttonSearchBooks},
            };

            ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(buttons);

            return keyboard;
        }
        public static IReplyMarkup GetButtonsUpdate()
        {
            KeyboardButton buttonAdd = new KeyboardButton("Add Book");
            KeyboardButton buttonDelete = new KeyboardButton("Delete Book");
            KeyboardButton buttonReview = new KeyboardButton("Review");
            KeyboardButton buttonExit = new KeyboardButton("Exit");



            KeyboardButton[][] buttons = new KeyboardButton[][]
            {
                new KeyboardButton[] { buttonAdd },
                new KeyboardButton[] { buttonDelete, buttonReview },
                new KeyboardButton[] { buttonExit}
            };

            ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(buttons);

            return keyboard;
        }
    }
}