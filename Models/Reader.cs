
using System.Text.Json.Serialization;

namespace Book_Bro.Models
{
    public class Reader
    {
        public Reader(string fullName)
        {
            FullName = fullName;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";

        public bool Active { get; set; } = false;
        public Guid? ActiveBookId { get; set; } = null;

        [JsonIgnore]
        public Book? ActiveBook { get; set; } = null;

        public void GiveBook(Book? book)
        {
            if (book == null) return;
            
            ActiveBook = book;
            ActiveBookId = book.Id;
            Active = true;
        }

        public void TakeBookBack()
        {
            ActiveBook = null;
            ActiveBookId = null;
            Active = false;
        }
    }
}