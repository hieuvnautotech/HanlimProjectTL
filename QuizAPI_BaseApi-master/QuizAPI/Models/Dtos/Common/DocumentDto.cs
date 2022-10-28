using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos.Common
{
    public class DocumentDto : BaseModel
    {
        public long? documentId { get; set; }
        public string menuComponent { get; set; } = string.Empty;
        public string urlFile { get; set; } = string.Empty;
        public string language { get; set; } = string.Empty;
        public IFormFile? file { get; set; }
        public string menuName { get; set; } = string.Empty;
    }
}
