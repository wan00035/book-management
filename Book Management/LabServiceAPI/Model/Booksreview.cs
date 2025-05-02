using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


public class Book
{
    public int BookID { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public DateTime PublicationYear { get; set; }
    public bool IsCheckedOut { get; set; }
}
