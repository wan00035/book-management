using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Lab6ServiceAPI.Controllers
{
    [EnableCors]
    [Route("[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly string connectionString;

        public BooksController()
        {
            // Initialize the database connection string
            string host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            string port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
            string user = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
            string pass = Environment.GetEnvironmentVariable("DB_PASS") ?? "root";
            string db = Environment.GetEnvironmentVariable("DB_NAME") ?? "Books";

            connectionString = $"server={host};port={port};user={user};password={pass};database={db}";
        }

        // ==========================================
        // GET: Retrieve all books
        // ==========================================
        [Authorize] // Only authenticated users can access this endpoint
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<Book> books = GetBooksFromDatabase();
                return Ok(books);
            }
            catch (Exception ex)
            {
                Console.WriteLine("System Error: " + ex.Message);
                return StatusCode(500, new { message = "Internal server error retrieving books." });
            }
        }

        // ==========================================
        // GET: Retrieve a specific book by ID
        // ==========================================
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try 
            {
                Book book = GetBookById(id);
                if (book != null)
                {
                    return Ok(book);
                }
                return NotFound(new { message = "Book not found." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("System Error: " + ex.Message);
                return StatusCode(500, new { message = "Internal server error retrieving the book." });
            }
        }

        // ==========================================
        // POST: Add a new book
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Post([FromBody] Book newBook)
        {
            try
            {
                AddBookToDatabase(newBook);
                return Ok(new { message = "Book added successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("System Error: " + ex.Message);
                return StatusCode(500, new { message = "Internal server error adding the book." });
            }
        }

        // ==========================================
        // PUT: Update an existing book
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Book book)
        {
            try
            {
                UpdateBookInDatabase(id, book);
                return Ok(new { message = "Book updated successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("System Error: " + ex.Message);
                return StatusCode(500, new { message = "Internal server error updating the book." });
            }
        }

        // ==========================================
        // DELETE: Remove a book
        // ==========================================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                DeleteBookFromDatabase(id);
                return Ok(new { message = "Book deleted successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("System Error: " + ex.Message);
                return StatusCode(500, new { message = "Internal server error deleting the book." });
            }
        }

        // ==========================================
        // Helper Methods (Database Operations)
        // ==========================================
        private List<Book> GetBooksFromDatabase()
        {
            List<Book> books = new List<Book>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // FIX: Changed 'books' to 'Books'
                string query = "SELECT BookID, Title, Author, PublicationYear, IsCheckedOut FROM Books";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Book book = new Book
                            {
                                BookID = reader.GetInt32("BookID"),
                                Title = reader.GetString("Title"),
                                Author = reader.GetString("Author"),
                                PublicationYear = reader.GetDateTime("PublicationYear"),
                                IsCheckedOut = reader.GetBoolean("IsCheckedOut")
                            };
                            books.Add(book);
                        }
                    }
                }
            }
            return books;
        }

        private Book GetBookById(int id)
        {
            Book book = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // FIX: Changed 'books' to 'Books' and implemented parameterization
                string query = "SELECT BookID, Title, Author, PublicationYear, IsCheckedOut FROM Books WHERE BookID = @BookID";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@BookID", id);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            book = new Book
                            {
                                BookID = reader.GetInt32("BookID"),
                                Title = reader.GetString("Title"),
                                Author = reader.GetString("Author"),
                                PublicationYear = reader.GetDateTime("PublicationYear"),
                                IsCheckedOut = reader.GetBoolean("IsCheckedOut")
                            };
                        }
                    }
                }
            }
            return book;
        }

        private void AddBookToDatabase(Book book)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // FIX: Changed 'books' to 'Books'
                string query = "INSERT INTO Books (Title, Author, PublicationYear, IsCheckedOut) VALUES (@Title, @Author, @PublicationYear, @IsCheckedOut)";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@PublicationYear", book.PublicationYear);
                    command.Parameters.AddWithValue("@IsCheckedOut", book.IsCheckedOut);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdateBookInDatabase(int id, Book book)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // FIX: Changed 'books' to 'Books'
                string query = "UPDATE Books SET Title = @Title, Author = @Author, PublicationYear = @PublicationYear, IsCheckedOut = @IsCheckedOut WHERE BookID = @BookID";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@Title", book.Title);
                    command.Parameters.AddWithValue("@Author", book.Author);
                    command.Parameters.AddWithValue("@PublicationYear", book.PublicationYear);
                    command.Parameters.AddWithValue("@IsCheckedOut", book.IsCheckedOut);
                    command.Parameters.AddWithValue("@BookID", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeleteBookFromDatabase(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // FIX: Changed 'books' to 'Books'
                string query = "DELETE FROM Books WHERE BookID = @BookID";
                using (MySqlCommand command = new MySqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@BookID", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}