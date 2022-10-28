namespace QuizAPI.Models.Dtos.Common
{
    public class BaseModel: PageModel
    {
        public bool? isActived { get; set; } = true;
        public DateTime? createdDate { get; set; } = DateTime.UtcNow;
        public long? createdBy { get; set; } = default;
        public DateTime? modifiedDate { get; set; } = default;
        public long? modifiedBy { get; set; } = default;
        public byte[]? row_version { get; set; } = default;
        public string? createdName { get; set; } = default;
        public string? modifiedName { get; set; } = default;
    }
}
