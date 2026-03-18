using System;

namespace LabClient.Models
{
    public class MyBookDto
    {
        public int RecordID { get; set; }
        public int BookID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
    }
}