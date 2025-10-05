
namespace Book_Bro.Models
{
    public class Book
    {
        public Book(string title, string author, int year)
        {
            Title = title;
            Author = author;
            Year = year;
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = "";
        public string Author { get; set; } = "";

        public int Year { get; set; }
        public int TimesTaken { get; set; }

        public bool IsAvaliable { get; set; } = true;

        public void MarkAsTaken()
        {
            IsAvaliable = false;
            TimesTaken++;
        }

        public void MarkAsAvaliable()
        {
            IsAvaliable = true;
        }
    }
}