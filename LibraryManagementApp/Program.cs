
namespace LibraryManagementApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Initialize database
            Database.Initialize();

            // Open Book Management
            Application.Run(new BookForm());
        }
    }
}
