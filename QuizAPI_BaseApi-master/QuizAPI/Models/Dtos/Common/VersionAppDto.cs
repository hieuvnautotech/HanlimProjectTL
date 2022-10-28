using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos.Common
{
    public class VersionAppDto
    {
        public int id_app { get; set; }
        public string name_file { get; set; } = string.Empty;
        public int? version { get; set; }
        public string change_date { get; set; } = string.Empty;
        public bool newVersion { get; set; }
        public IFormFile? file { get; set; }
        public long? createdBy { get; set; } = default;
    }
}
