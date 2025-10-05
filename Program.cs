/*
    * BookBro - Console Library Manager
    * ---------------------------------
    * A simple console-based library management system written in C# (.NET).
    * 
    * Features:
    *  - Add, remove, and view books
    *  - Register and manage readers
    *  - Give and return books
    *  - Save and load data from JSON files
    *  - Track most popular books
    * 
    * Code style:
    *  - Semi-OOP: logic split between models (Book, Reader) and main Program
    *  - Uses System.Text.Json for data persistence
    *  - Console-based numeric navigation (menu sections)
    *  - Clean structure, easy to extend or refactor
    * 
    * Authors:
    *  - Alisher
    *  - Amir
    *  - Adil
    *
    * Created: October 2025
*/

using System.Text.Json;
using Book_Bro.Models;

namespace Book_Bro
{
    public class Program
    {
        static List<Book> books = new List<Book>();
        static List<Reader> readers = new List<Reader>();

        private static readonly string booksFile = "books.json";
        private static readonly string readersFile = "readers.json";

        // Flags
        private static bool confirmReturnAsk = true;

        static int section = 0;

        #region Main

        static void Main(string[] args)
        {
            LoadAll();

            bool isRunning = true;

            while (isRunning)
            {
                #region Draw UI
                switch (section)
                {
                    case 0:
                        DrawMainMenu(); break;
                    case 1:
                        DrawBooksMenu(); break;
                    case 2:
                        DrawReadersMenu(); break;
                    case 3:
                        DrawReaderBookMenu(); break;
                    case 4:
                        DrawFilesMenu(); break;
                    case 5:
                        DrawAppMenu(); break;
                }

                #endregion

                #region Read Actions

                int action = ReadAction();

                switch (section)
                {
                    case 0:
                        {
                            switch (action)
                            {
                                case 1: section = 1; break; // Book Manager
                                case 2: section = 2; break; // Reader Manager
                                case 3: section = 3; break; // Reader & Book Manager
                                case 4: section = 4; break; // File Manager
                                case 5: section = 5; break; // App
                                case 0: isRunning = false; break;
                            }
                            break;
                        }
                    case 1:
                        {
                            switch (action)
                            {
                                case 1: AddBook(); break;
                                case 2: ShowBooks(false); break;
                                case 3: ShowBooks(true); break;
                                case 4: ShowPopularBooks(); break;
                                case 0: section = 0; break;
                            }
                            break;
                        }
                    case 2:
                        {
                            switch (action)
                            {
                                case 1: AddReader(); break;
                                case 2: ShowReaders(); break;
                                case 3: RemoveReader(); break;
                                case 0: section = 0; break;
                            }
                            break;
                        }
                    case 3:
                        {
                            switch (action)
                            {
                                case 1: GiveBookToReader(); break;
                                case 2: ReturnBookFromReader(); break;
                                case 3: FindReaderByName(); break;
                                case 4: FindReaderByBookTitle(); break;
                                case 5: FindBooksByTitle(); break;
                                case 6: FindBooksByAuthor(); break;
                                case 0: section = 0; break;
                            }
                            break;
                        }
                    case 4:
                        {
                            switch (action)
                            {
                                case 1: SaveBooks(); break;
                                case 2: SaveReaders(); break;
                                case 3: LoadAll(); break;
                                case 0: section = 0; break;
                            }
                            break;
                        }
                    case 5:
                        {
                            switch (action)
                            {
                                case 1:
                                    SaveAll();
                                    Console.WriteLine("All data saved. Exiting...");
                                    Thread.Sleep(1000);
                                    isRunning = false;
                                    break;
                                case 0: section = 0; break;
                            }
                            break;
                        }
                }
            }

            #endregion

            // Save all after exiting

            SaveAll();
            Console.Clear();
        }

        #endregion

        #region UI Input (Read Actions)

        private static int ReadAction()
        {
            while (true)
            {
                var key = Console.ReadKey(intercept: true);

                if (char.IsDigit(key.KeyChar))
                {
                    int number = key.KeyChar - '0';
                    Console.WriteLine(number);
                    return number;
                }
            }
        }

        private static void RequireBackAction(string? actionName = "Back")
        {
            Console.WriteLine();
            Console.WriteLine($"0. {actionName}");
            int action = ReadAction();

            while (action != 0)
            {
                action = ReadAction();
            }
        }

        #endregion

        #region Drawers

        private static void DrawMainMenu()
        {
            Console.Clear();
            Console.WriteLine("BookBro");
            Console.WriteLine();
            Console.WriteLine("1. Book Manager");
            Console.WriteLine("2. Reader Manager");
            Console.WriteLine("3. Reader & Book Manager");
            Console.WriteLine("4. File Manager");
            Console.WriteLine("5. App");
            Console.WriteLine("0. Exit");
        }

        private static void DrawBooksMenu()
        {
            Console.Clear();
            Console.WriteLine("Book Manager");
            Console.WriteLine();
            Console.WriteLine("1. Add Book");
            Console.WriteLine("2. Show Books");
            Console.WriteLine("3. Show Avaliable Books");
            Console.WriteLine("4. Most Popular Books");
            Console.WriteLine("0. Back");
        }

        private static void DrawReadersMenu()
        {
            Console.Clear();
            Console.WriteLine("Readers Manager");
            Console.WriteLine();
            Console.WriteLine("1. Add Reader");
            Console.WriteLine("2. Show Readers");
            Console.WriteLine("3. Remove Reader (by Id)");
            Console.WriteLine("0. Back");
        }

        private static void DrawReaderBookMenu()
        {
            Console.Clear();
            Console.WriteLine("Reader & Book Manager");
            Console.WriteLine();
            Console.WriteLine("1. Give Book to Reader");
            Console.WriteLine("2. Return Book from Reader");
            Console.WriteLine();
            Console.WriteLine("3. Find Reader (by Name)");
            Console.WriteLine("4. Find Reader (by Book Title)");
            Console.WriteLine();
            Console.WriteLine("5. Find Book (by Title)");
            Console.WriteLine("6. Find Book (by Author)");
            Console.WriteLine("0. Back");
        }

        private static void DrawFilesMenu()
        {
            Console.Clear();
            Console.WriteLine("File Manager");
            Console.WriteLine();
            Console.WriteLine("1. Save All Books");
            Console.WriteLine("2. Save All Readers");
            Console.WriteLine("3. Load All Data");
            Console.WriteLine("0. Back");
        }

        private static void DrawAppMenu()
        {
            Console.Clear();
            Console.WriteLine("App");
            Console.WriteLine();
            Console.WriteLine("1. Save & Exit");
            Console.WriteLine("0. Back");
        }

        #endregion

        #region Book Manager
        private static void AddBook()
        {
            string? title = "";
            string? author = "";
            int year = 0;

            while (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(author) || year == 0)
            {
                Console.Clear();

                if (string.IsNullOrEmpty(title))
                {
                    Console.Write("Title: ");
                    title = Console.ReadLine();
                }

                if (string.IsNullOrEmpty(author))
                {
                    Console.Write("Author: ");
                    author = Console.ReadLine();
                }

                if (year == 0)
                {
                    Console.Write("Year: ");
                    _ = int.TryParse(Console.ReadLine(), out year);
                }
            }

            Book newBook = new Book(title, author, year);
            books.Add(newBook);

            Console.WriteLine($"Book {title} by {author} was added to library. Id: {newBook.Id}");
            RequireBackAction();
        }

        private static void ShowBooks(bool onlyAvaliable = false)
        {
            Console.Clear();

            int found = 0;

            if (books.Count > 0)
            {
                foreach (var book in books)
                {
                    if (onlyAvaliable && !book.IsAvaliable) continue;

                    string taken = !book.IsAvaliable ? "(Taken)" : "(Avaliable)";
                    string universalBoom = onlyAvaliable ? "" : taken;

                    Console.WriteLine($"{book.Id} - {book.Title} ({book.Year}) by {book.Author} {universalBoom}");
                    found++;
                }
            }

            if (found == 0)
            {
                string universalBoom = onlyAvaliable ? "No avaliable books found" : "No books found";
                Console.WriteLine(universalBoom);
            }

            RequireBackAction();
        }

        private static void FindBooksByTitle()
        {
            Console.Clear();
            Console.Write("Enter title part: ");
            string? query = Console.ReadLine();

            var results = books.Where(b => b.Title.Contains(query ?? "", StringComparison.OrdinalIgnoreCase)).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("No books found by title");
            }
            else
            {
                foreach (var book in results)
                    Console.WriteLine($"{book.Id} - {book.Title} by {book.Author}");
            }

            RequireBackAction();
        }

        private static void FindBooksByAuthor()
        {
            Console.Clear();
            Console.Write("Enter author part: ");
            string? query = Console.ReadLine();

            var results = books.Where(b => b.Author.Contains(query ?? "", StringComparison.OrdinalIgnoreCase)).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("No books found by author");
            }
            else
            {
                foreach (var book in results)
                    Console.WriteLine($"{book.Id} - {book.Title} by {book.Author}");
            }

            RequireBackAction();
        }

        private static void ShowPopularBooks()
        {
            Console.Clear();
            Console.WriteLine("Popular Books Report\n");

            var popular = books
                .OrderByDescending(b => b.TimesTaken)
                .Take(5)
                .ToList();

            if (popular.Count == 0)
            {
                Console.WriteLine("No data available.");
            }
            else
            {
                int index = 1;
                foreach (var b in popular)
                {
                    Console.WriteLine($"{index}. {b.Title} ({b.Year}) by {b.Author} - taken {b.TimesTaken} times");
                    index++;
                }
            }

            RequireBackAction();
        }

        #endregion

        #region Reader Manager

        private static void AddReader()
        {
            Console.Clear();
            Console.Write("Reader Full Name: ");
            string? name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Reader name cannot be empty");
                RequireBackAction();
                return;
            }

            Reader newReader = new Reader(name);
            readers.Add(newReader);

            Console.WriteLine($"Reader {name} was added. Id: {newReader.Id}");
            RequireBackAction();
        }

        private static void RemoveReader()
        {
            Console.Clear();
            Console.Write("Enter Reader Id: ");
            string? input = Console.ReadLine();

            if (Guid.TryParse(input, out Guid id))
            {
                var reader = GetReaderById(id);
                if (reader != null)
                {
                    readers.Remove(reader);
                    Console.WriteLine($"Reader {reader.FullName} removed");
                }
                else
                    Console.WriteLine("Reader not found");
            }
            else
            {
                Console.WriteLine("Invalid Id format");
            }

            RequireBackAction();
        }

        private static void ShowReaders()
        {
            Console.Clear();

            if (readers.Count == 0)
            {
                Console.WriteLine("No readers found");
                RequireBackAction();
                return;
            }

            foreach (var r in readers)
            {
                string status = r.Active ? $"(Reading: {r.ActiveBook?.Title})" : "(Free)";
                Console.WriteLine($"{r.Id} - {r.FullName} {status}");
            }

            RequireBackAction();
        }

        private static void FindReaderByName()
        {
            Console.Clear();
            Console.Write("Enter name part: ");
            string? query = Console.ReadLine();

            var results = readers.Where(r => r.FullName.Contains(query ?? "", StringComparison.OrdinalIgnoreCase)).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("No readers found by name");
            }
            else
            {
                foreach (var r in results)
                {
                    string status = r.Active ? $"(Reading: {r.ActiveBook?.Title})" : "(Free)";
                    Console.WriteLine($"{r.Id} - {r.FullName} {status}");
                }
            }

            RequireBackAction();
        }

        private static void FindReaderByBookTitle()
        {
            Console.Clear();
            Console.Write("Enter book title part: ");
            string? query = Console.ReadLine();

            var results = readers.Where(r => r.ActiveBook != null && r.ActiveBook.Title.Contains(query ?? "", StringComparison.OrdinalIgnoreCase)).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("No readers found with that book");
            }
            else
            {
                foreach (var r in results)
                    Console.WriteLine($"{r.Id} - {r.FullName} (Reading: {r.ActiveBook?.Title})");
            }

            RequireBackAction();
        }

        #endregion

        #region Reader & Book Manager

        private static void GiveBookToReader()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Give Book to Reader");
                Console.WriteLine("----------------------");

                var freeReaders = readers.Where(r => r.ActiveBook == null).ToList();
                var availableBooks = books.Where(b => b.IsAvaliable).ToList();

                if (readers.Count == 0)
                {
                    Console.WriteLine("No readers found.");
                    RequireBackAction();
                    return;
                }

                if (availableBooks.Count == 0)
                {
                    Console.WriteLine("No available books to give.");
                    RequireBackAction();
                    return;
                }

                if (freeReaders.Count == 0)
                {
                    Console.WriteLine("No free readers available.");
                    RequireBackAction();
                    return;
                }

                Console.WriteLine("\nAvailable Readers:");
                for (int i = 0; i < freeReaders.Count; i++)
                    Console.WriteLine($"{i + 1}. {freeReaders[i].FullName}");

                Console.Write("\nSelect reader by number (0 = back): ");
                if (!int.TryParse(Console.ReadLine(), out int readerIndex) || readerIndex < 0 || readerIndex > freeReaders.Count)
                {
                    Console.WriteLine("Invalid reader selection.");
                    RequireBackAction("Try Again");
                    continue;
                }

                if (readerIndex == 0)
                    return;

                var reader = freeReaders[readerIndex - 1];

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"Available Books for {reader.FullName}:");

                    for (int i = 0; i < availableBooks.Count; i++)
                        Console.WriteLine($"{i + 1}. {availableBooks[i].Title} by {availableBooks[i].Author}");

                    Console.Write("\nSelect book by number (0 = back): ");
                    if (!int.TryParse(Console.ReadLine(), out int bookIndex) || bookIndex < 0 || bookIndex > availableBooks.Count)
                    {
                        Console.WriteLine("Invalid book selection.");
                        RequireBackAction("Try Again");
                        continue;
                    }

                    if (bookIndex == 0)
                        break;

                    var book = availableBooks[bookIndex - 1];

                    reader.GiveBook(book);
                    book.MarkAsTaken();

                    Console.WriteLine($"\nBook \"{book.Title}\" was successfully given to {reader.FullName}!");
                    RequireBackAction("Done");
                    return;
                }
            }
        }

        private static void ReturnBookFromReader()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Return Book from Reader");
                Console.WriteLine("--------------------------");

                var activeReaders = readers.Where(r => r.Active && r.ActiveBook != null).ToList();

                if (activeReaders.Count == 0)
                {
                    Console.WriteLine("No readers currently have books.");
                    RequireBackAction();
                    return;
                }

                Console.WriteLine("\nReaders with books:");
                for (int i = 0; i < activeReaders.Count; i++)
                {
                    var r = activeReaders[i];
                    Console.WriteLine($"{i + 1}. {r.FullName} - {r.ActiveBook?.Title}");
                }

                Console.Write("\nSelect reader by number (0 = back): ");
                if (!int.TryParse(Console.ReadLine(), out int readerIndex) || readerIndex < 0 || readerIndex > activeReaders.Count)
                {
                    Console.WriteLine("Invalid selection.");
                    RequireBackAction("Try Again");
                    continue;
                }

                if (readerIndex == 0)
                    return;

                var reader = activeReaders[readerIndex - 1];
                var book = reader.ActiveBook!;

                if (confirmReturnAsk)
                {
                    Console.Clear();
                    Console.WriteLine($"You are about to return \"{book.Title}\" from {reader.FullName}.");
                    Console.WriteLine("Are you sure?");
                    Console.WriteLine();
                    Console.WriteLine("1. OK");
                    Console.WriteLine("2. No");
                    Console.WriteLine("3. OK, but never ask again");
                    Console.Write("\nChoose action: ");

                    var key = ReadAction();

                    if (key == 2)
                    {
                        Console.WriteLine("\nCancelled.");
                        Thread.Sleep(800);
                        continue;
                    }
                    else if (key == 3)
                    {
                        confirmReturnAsk = false;
                        Console.WriteLine("\nConfirmation disabled for this session.");
                        Thread.Sleep(800);
                    }
                    else if (key != 1)
                    {
                        Console.WriteLine("\nInvalid choice.");
                        RequireBackAction("Try Again");
                        continue;
                    }
                }

                reader.TakeBookBack();
                book.MarkAsAvaliable();

                Console.WriteLine($"\nBook \"{book.Title}\" successfully returned from {reader.FullName}.");
                Thread.Sleep(800);
            }
        }

        #endregion

        #region File Manager

        private static void SaveBooks()
        {
            Console.Clear();
            File.WriteAllText(booksFile, JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine("Books saved");
            RequireBackAction("Okay");
        }

        private static void SaveReaders()
        {
            Console.Clear();
            File.WriteAllText(readersFile, JsonSerializer.Serialize(readers, new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine("Readers saved");
            RequireBackAction("Okay");
        }

        private static void SaveAll()
        {
            File.WriteAllText(booksFile, JsonSerializer.Serialize(books, new JsonSerializerOptions { WriteIndented = true }));
            File.WriteAllText(readersFile, JsonSerializer.Serialize(readers, new JsonSerializerOptions { WriteIndented = true }));
        }

        private static void LoadAll()
        {
            if (File.Exists(booksFile))
            {
                string json = File.ReadAllText(booksFile);
                books = JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
            }

            if (File.Exists(readersFile))
            {
                string json = File.ReadAllText(readersFile);
                readers = JsonSerializer.Deserialize<List<Reader>>(json) ?? new List<Reader>();
            }

            foreach (var reader in readers)
            {
                if (reader.ActiveBookId.HasValue)
                {
                    reader.ActiveBook = books.FirstOrDefault(b => b.Id == reader.ActiveBookId.Value);
                }
            }
        }

        #endregion

        public static Reader? GetReaderById(Guid Id)
        {
            return readers.FirstOrDefault(r => r.Id == Id);
        }

        public static Book? GetBookById(Guid Id)
        {
            return books.FirstOrDefault(b => b.Id == Id);
        }
    }
}
