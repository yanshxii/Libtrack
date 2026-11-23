 
namespace LibraryManagementApp
{
    public class Book
    {
        public string BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        public Book(string bookID, string title,
            string author, string category,
            string description)
        {
            BookID = bookID;
            Title = title;
            Author = author;
            Category = category;
            Description = description;
        }
    }
}
