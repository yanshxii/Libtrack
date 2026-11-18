
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace LibraryManagementApp
{
    public class BookForm : Form
    {
        TextBox txtTitle = new TextBox();
        TextBox txtAuthor = new TextBox();
        TextBox txtCategory = new TextBox();
        TextBox txtQuantity = new TextBox();
        TextBox txtSearch = new TextBox();

        DataGridView dgvBooks = new DataGridView();

        Button btnAdd = new Button();
        Button btnUpdate = new Button();
        Button btnDelete = new Button();
        Button btnClear = new Button();
        Button btnSearch = new Button();

        int selectedBookID = 0;

        public BookForm()
        {
            Text = "Library Management System - Book Management";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;

            CreateInterface();
            LoadBooks();
        }

        void CreateInterface()
        {
            Label heading = new Label
            {
                Text = "BOOK MANAGEMENT",
                Font = new Font("Arial", 20, FontStyle.Bold),
                Location = new Point(30, 20),
                AutoSize = true
            };
            Controls.Add(heading);

            AddLabel("Book Title:", 30, 85);
            txtTitle.Location = new Point(150, 85);
            txtTitle.Width = 250;
            Controls.Add(txtTitle);

            AddLabel("Author:", 30, 125);
            txtAuthor.Location = new Point(150, 125);
            txtAuthor.Width = 250;
            Controls.Add(txtAuthor);

            AddLabel("Category:", 30, 165);
            txtCategory.Location = new Point(150, 165);
            txtCategory.Width = 250;
            Controls.Add(txtCategory);

            AddLabel("Quantity:", 30, 205);
            txtQuantity.Location = new Point(150, 205);
            txtQuantity.Width = 250;
            Controls.Add(txtQuantity);

            ConfigureButton(btnAdd, "Add Book", 450, 85);
            btnAdd.Click += (s, e) => AddBook();

            ConfigureButton(btnUpdate, "Update", 450, 130);
            btnUpdate.Click += (s, e) => UpdateBook();

            ConfigureButton(btnDelete, "Delete", 450, 175);
            btnDelete.Click += (s, e) => DeleteBook();

            ConfigureButton(btnClear, "Clear", 450, 220);
            btnClear.Click += (s, e) => ClearFields();

            txtSearch.Location = new Point(30, 285);
            txtSearch.Width = 250;
            Controls.Add(txtSearch);

            ConfigureButton(btnSearch, "Search", 300, 280);
            btnSearch.Click += (s, e) => SearchBooks();

            dgvBooks.Location = new Point(30, 330);
            dgvBooks.Size = new Size(910, 250);
            dgvBooks.ReadOnly = true;
            dgvBooks.AllowUserToAddRows = false;
            dgvBooks.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvBooks.MultiSelect = false;
            dgvBooks.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvBooks.CellClick += DgvBooks_CellClick;

            Controls.Add(dgvBooks);
        }

        void AddLabel(string text, int x, int y)
        {
            Label label = new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true
            };

            Controls.Add(label);
        }

        void ConfigureButton(Button button, string text, int x, int y)
        {
            button.Text = text;
            button.Location = new Point(x, y);
            button.Size = new Size(100, 32);

            Controls.Add(button);
        }

        void LoadBooks()
        {
            using var connection = Database.GetConnection();
            connection.Open();

            string sql = "SELECT * FROM Books ORDER BY BookID DESC";

            using var command = new SqliteCommand(sql, connection);
            using var reader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            dgvBooks.DataSource = table;
        }

        void AddBook()
        {
            if (!ValidateFields())
                return;

            using var connection = Database.GetConnection();
            connection.Open();

            string sql = @"INSERT INTO Books
                (Title, Author, Category, Quantity)
                VALUES (@title, @author, @category, @quantity)";

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
            command.Parameters.AddWithValue("@author", txtAuthor.Text.Trim());
            command.Parameters.AddWithValue("@category", txtCategory.Text.Trim());
            command.Parameters.AddWithValue("@quantity", int.Parse(txtQuantity.Text));

            command.ExecuteNonQuery();

            MessageBox.Show("Book added successfully!");

            ClearFields();
            LoadBooks();
        }

        void UpdateBook()
        {
            if (selectedBookID == 0)
            {
                MessageBox.Show("Please select a book first.");
                return;
            }

            if (!ValidateFields())
                return;

            using var connection = Database.GetConnection();
            connection.Open();

            string sql = @"UPDATE Books SET
                Title = @title,
                Author = @author,
                Category = @category,
                Quantity = @quantity
                WHERE BookID = @id";

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
            command.Parameters.AddWithValue("@author", txtAuthor.Text.Trim());
            command.Parameters.AddWithValue("@category", txtCategory.Text.Trim());
            command.Parameters.AddWithValue("@quantity", int.Parse(txtQuantity.Text));
            command.Parameters.AddWithValue("@id", selectedBookID);

            command.ExecuteNonQuery();

            MessageBox.Show("Book updated successfully!");

            ClearFields();
            LoadBooks();
        }

        void DeleteBook()
        {
            if (selectedBookID == 0)
            {
                MessageBox.Show("Please select a book first.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this book?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            using var connection = Database.GetConnection();
            connection.Open();

            string sql = "DELETE FROM Books WHERE BookID = @id";

            using var command = new SqliteCommand(sql, connection);
            command.Parameters.AddWithValue("@id", selectedBookID);

            command.ExecuteNonQuery();

            MessageBox.Show("Book deleted successfully!");

            ClearFields();
            LoadBooks();
        }

        void SearchBooks()
        {
            using var connection = Database.GetConnection();
            connection.Open();

            string sql = @"SELECT * FROM Books
                WHERE Title LIKE @search
                OR Author LIKE @search
                OR Category LIKE @search";

            using var command = new SqliteCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@search", "%" + txtSearch.Text.Trim() + "%"
            );

            using var reader = command.ExecuteReader();

            DataTable table = new DataTable();
            table.Load(reader);

            dgvBooks.DataSource = table;
        }

        void DgvBooks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgvBooks.Rows[e.RowIndex];

            selectedBookID = Convert.ToInt32(row.Cells["BookID"].Value);
            txtTitle.Text = row.Cells["Title"].Value.ToString();
            txtAuthor.Text = row.Cells["Author"].Value.ToString();
            txtCategory.Text = row.Cells["Category"].Value.ToString();
            txtQuantity.Text = row.Cells["Quantity"].Value.ToString();
        }

        bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                string.IsNullOrWhiteSpace(txtAuthor.Text) ||
                string.IsNullOrWhiteSpace(txtCategory.Text) ||
                string.IsNullOrWhiteSpace(txtQuantity.Text))
            {
                MessageBox.Show("Please fill in all fields.");
                return false;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Quantity must be a valid non-negative number.");
                return false;
            }

            return true;
        }

        void ClearFields()
        {
            txtTitle.Clear();
            txtAuthor.Clear();
            txtCategory.Clear();
            txtQuantity.Clear();
            selectedBookID = 0;
            dgvBooks.ClearSelection();
        }
    }
}
