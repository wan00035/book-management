using System;

namespace LabServiceAPI.Models
{
    public class BorrowRecord
    {
        public int RecordID { get; set; }
        

        public int UserID { get; set; }
        

        public int BookID { get; set; }
        

        public DateTime BorrowDate { get; set; }
        

        public DateTime DueDate { get; set; }
        

        public DateTime? ReturnDate { get; set; } 
    }
}