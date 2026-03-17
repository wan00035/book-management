using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims; // <-- Added this mandatory reference!

namespace LabServiceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class BorrowController : ControllerBase
    {
        private readonly string _connectionString;

        public BorrowController(IConfiguration config)
        {
            string host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            string port = Environment.GetEnvironmentVariable("DB_PORT") ?? "3306";
            string user = Environment.GetEnvironmentVariable("DB_USER") ?? "root";
            string pass = Environment.GetEnvironmentVariable("DB_PASS") ?? "root";
            string db = Environment.GetEnvironmentVariable("DB_NAME") ?? "Books";
            _connectionString = $"server={host};port={port};user={user};password={pass};database={db}";
        }

        // ==========================================
        // API: Borrow a Book
        // ==========================================
        [HttpPost("{bookId}/borrow")]
        public IActionResult BorrowBook(int bookId)
        {
            // Extract the username from the JWT Token
            string username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                              ?? User.FindFirst("sub")?.Value;
                              
            if (string.IsNullOrEmpty(username)) return Unauthorized(new { message = "Could not identify user from token." });

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                // Retrieve the UserID
                int userId = GetUserIdByUsername(conn, username);
                if (userId == 0) return BadRequest(new { message = "User not found in database." });

                // Check if book is available
                if (!IsBookAvailable(conn, bookId)) return BadRequest(new { message = "This book is currently not available for borrowing." });

                // Start Transaction
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Update book status
                        string updateBookQuery = "UPDATE Books SET IsCheckedOut = 1 WHERE BookID = @BookID";
                        using (MySqlCommand cmd1 = new MySqlCommand(updateBookQuery, conn, transaction))
                        {
                            cmd1.Parameters.AddWithValue("@BookID", bookId);
                            cmd1.ExecuteNonQuery();
                        }

                        // Insert borrow record
                        string insertRecordQuery = @"
                            INSERT INTO BorrowRecords (UserID, BookID, BorrowDate, DueDate, ReturnDate) 
                            VALUES (@UserID, @BookID, NOW(), DATE_ADD(NOW(), INTERVAL 14 DAY), NULL)";
                        using (MySqlCommand cmd2 = new MySqlCommand(insertRecordQuery, conn, transaction))
                        {
                            cmd2.Parameters.AddWithValue("@UserID", userId);
                            cmd2.Parameters.AddWithValue("@BookID", bookId);
                            cmd2.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return Ok(new { message = "Book borrowing successful! Please return it within 14 days." });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return StatusCode(500, new { message = "Failed to borrow the book. System error: " + ex.Message });
                    }
                }
            }
        }

        // ==========================================
        // API: Return a Book
        // ==========================================
        [HttpPost("{bookId}/return")]
        public IActionResult ReturnBook(int bookId)
        {
            // Extract the username from the JWT Token
            string username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "Could not identify user from token." });
            }

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                // Retrieve the UserID
                int userId = GetUserIdByUsername(conn, username);
                if (userId == 0)
                {
                    return BadRequest(new { message = "User does not exist." });
                }

                // Verify if this user actually borrowed this book and hasn't returned it yet
                if (!HasActiveBorrowRecord(conn, userId, bookId))
                {
                    return BadRequest(new { message = "You do not have an active borrow record for this book." });
                }

                // Start Transaction
                using (MySqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // Mark the book as available again
                        string updateBookQuery = "UPDATE Books SET IsCheckedOut = 0 WHERE BookID = @BookID";
                        using (MySqlCommand cmd1 = new MySqlCommand(updateBookQuery, conn, transaction))
                        {
                            cmd1.Parameters.AddWithValue("@BookID", bookId);
                            cmd1.ExecuteNonQuery();
                        }

                        // Record the exact return time
                        string updateRecordQuery = @"
                            UPDATE BorrowRecords 
                            SET ReturnDate = NOW() 
                            WHERE UserID = @UserID AND BookID = @BookID AND ReturnDate IS NULL";
                        using (MySqlCommand cmd2 = new MySqlCommand(updateRecordQuery, conn, transaction))
                        {
                            cmd2.Parameters.AddWithValue("@UserID", userId);
                            cmd2.Parameters.AddWithValue("@BookID", bookId);
                            cmd2.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return Ok(new { message = "Book returned successfully! Thank you." });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        return StatusCode(500, new { message = "Failed to return the book. System error: " + ex.Message });
                    }
                }
            }
        }

        // ==========================================
        // Helper Methods (Private)
        // ==========================================
        private int GetUserIdByUsername(MySqlConnection conn, string username)
        {
            string query = "SELECT UserID FROM Users WHERE Username = @Username";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                var result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        private bool IsBookAvailable(MySqlConnection conn, int bookId)
        {
            string query = "SELECT IsCheckedOut FROM Books WHERE BookID = @BookID";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@BookID", bookId);
                var result = cmd.ExecuteScalar();
                return result != null && Convert.ToBoolean(result) == false;
            }
        }

        private bool HasActiveBorrowRecord(MySqlConnection conn, int userId, int bookId)
        {
            string query = "SELECT COUNT(1) FROM BorrowRecords WHERE UserID = @UserID AND BookID = @BookID AND ReturnDate IS NULL";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@BookID", bookId);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }
    }
}