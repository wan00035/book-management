using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LabClient.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}