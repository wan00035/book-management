using System;

namespace LabClient.Models 
{
    public class AdminBorrowRecordDto
    {
        public int RecordID { get; set; }
        public string Username { get; set; }
        public string Title { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; } // Nullable, because it might not be returned yet
    }
}