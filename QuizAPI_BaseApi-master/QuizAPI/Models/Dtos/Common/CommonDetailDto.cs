using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Models.Dtos.Common
{
    public class CommonDetailDto : BaseModel
    {
        //public long id
        //{
        //    get
        //    {
        //        return commonDetailId;
        //    }

        //}
        public long commonDetailId { get; set; }

        public long? commonMasterId { get; set; } = default;

        public string? commonDetailName { get; set; } = default;

    }
}
