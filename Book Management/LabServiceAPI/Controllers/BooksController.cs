using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Lab6ServiceAPI.Controllers
{
    [EnableCors]
    [Route("[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private string connectionString;

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
                
                Console.WriteLine("Error: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        //GET api/Books/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            Book book = GetBookById(id);
            if (book != null)
            {
                return Ok(book);
            }
            else
            {
                return NotFound();
            }
        }

        private List<Book> GetBooksFromDatabase()
        {
            List<Book> books = new List<Book>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

              
                string query = "SELECT BookID, Title, Author, PublicationYear, IsCheckedOut FROM books";
                MySqlCommand command = new MySqlCommand(query, conn);

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

            return books;
        }


        private Book GetBookById(int id)
        {
            Book book = null;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = $"SELECT BookID, Title, Author, PublicationYear, IsCheckedOut FROM books WHERE BookID = {id}";
                MySqlCommand command = new MySqlCommand(query, conn);

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

            return book;
        }

        // POST api/Books
        [HttpPost]
        public IActionResult Post([FromBody] Book newBook)
        {
            try
            {
                AddBookToDatabase(newBook);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        private void AddBookToDatabase(Book book)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO books (Title, Author, PublicationYear, IsCheckedOut) VALUES (@Title, @Author, @PublicationYear, @IsCheckedOut)";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@Title", book.Title);
                command.Parameters.AddWithValue("@Author", book.Author);
                command.Parameters.AddWithValue("@PublicationYear", book.PublicationYear);
                command.Parameters.AddWithValue("@IsCheckedOut", book.IsCheckedOut);

                command.ExecuteNonQuery();
            }
        }


        //// PUT api/Books/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Book book)
        {
            try
            {
                UpdateBookInDatabase(id, book);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        private void UpdateBookInDatabase(int id, Book book)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "UPDATE books SET Title = @Title, Author = @Author, PublicationYear = @PublicationYear, IsCheckedOut = @IsCheckedOut WHERE BookID = @BookID";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@Title", book.Title);
                command.Parameters.AddWithValue("@Author", book.Author);
                command.Parameters.AddWithValue("@PublicationYear", book.PublicationYear);
                command.Parameters.AddWithValue("@IsCheckedOut", book.IsCheckedOut);
                command.Parameters.AddWithValue("@BookID", id);

                command.ExecuteNonQuery();
            }
        }

        // DELETE api/Books/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                DeleteBookFromDatabase(id);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        private void DeleteBookFromDatabase(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "DELETE FROM books WHERE BookID = @BookID";
                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.AddWithValue("@BookID", id);

                command.ExecuteNonQuery();
            }
        }
    }

}