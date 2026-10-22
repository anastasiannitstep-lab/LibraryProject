using LibraryProject.Models;
using LibraryProject.Service;

namespace LibraryProject
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Login l = new Login();
            bool b;
            do
            {
                Console.WriteLine("Put Name:");
                string name = Console.ReadLine();
                Console.WriteLine("Put Password:");
                string password = Console.ReadLine();
                b = l.ReturnTrueORFalse(name, password);
                if (b)
                {
                    Console.OutputEncoding = System.Text.Encoding.UTF8;
                    Console.InputEncoding = System.Text.Encoding.UTF8;

                    ServiceL s = new ServiceL();


                    Console.WriteLine("\nДодаємо нову тестову книгу...");
                    var newBook = new Book
                    {
                        title = "Тестове Фентезі Книга",
                        author = "Тестовий Автор",
                        publisher = "Тестове Видавництво",
                        genre = "Фентезі",
                        pages_count = 150,
                        publish_year = 2026,
                        cost_price = 100.00m,
                        retail_price = 180.00m,
                        stock_quantity = 5
                    };
                    s.AddBook(newBook);
                    Console.WriteLine("Книга надіслана в базу.");

                    Console.WriteLine("\nШукаємо щойно додану книгу...");
                    var foundBooks = s.SearchBooks("Тестове");
                    foreach (var book in foundBooks)
                    {
                        Console.WriteLine($"- Знайдено: {book.title} (Автор: {book.author}, Залишок: {book.stock_quantity} шт.)");
                    }

                    var targetBook = foundBooks.FirstOrDefault();
                    if (targetBook != null)
                    {
                        Console.WriteLine($"\nПродаємо 1 одиницю книги ID: {targetBook.book_id}...");
                        bool isSold = s.SellBook(targetBook.book_id);
                        Console.WriteLine($"Результат продажу: {isSold}");

                        Console.WriteLine($"\nВідкладаємо книгу ID: {targetBook.book_id} для покупця...");
                        bool isReserved = s.ReserveBook(targetBook.book_id, "Олексій Олексієнко");
                        Console.WriteLine($"Результат резервування: {isReserved}");
                    }

                    Console.WriteLine("\nЗапитуємо список новинок магазину...");
                    var newArrivals = s.GetNewBooks();
                    foreach (var book in newArrivals.Take(3))
                    {
                        Console.WriteLine($"- Новинка: {book.title} ({book.publish_year} рік)");
                    }

                    Console.WriteLine("\nЗапитуємо аналітику найпопулярніших авторів...");
                    var topAuthors = s.GetTopAuthors("week");
                    foreach (var item in topAuthors)
                    {
                        Console.WriteLine($"- Автор: {item.author}, Кількість резервувань: {item.popularity_score}");
                    }
                }
            } while (!b);
        }
    }
}
