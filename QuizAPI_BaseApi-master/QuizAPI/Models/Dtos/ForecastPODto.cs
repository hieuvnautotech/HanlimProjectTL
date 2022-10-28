using QuizAPI.Models.Dtos.Common;
namespace QuizAPI.Models.Dtos
{
    public class ForecastPODto : BaseModel
    {
        public long? FPOId { get; set; }
        public long? MaterialId { get; set; }
        public long? LineId { get; set; }
        public int? Week { get; set; }
        public int? Year { get; set; }
        public int? Amount { get; set; }
        public string LineName { get; set; }
        public string MaterialCode { get; set; }
        public float Inch { get; set; }

        public string Description { get; set; }
        public string DescriptionMaterial { get; set; }
    }
    public class SelectMaterial
    {
        public long MaterialId { get; set; }
        public string MaterialCode { get; set; }

    }
    public class SelectLine
    {
        public long LineId { get; set; }
        public string LineName { get; set; }

    }
    public class SelectYear
    {
        public long YearId { get; set; }
        public string YearName { get; set; }

    }
}
