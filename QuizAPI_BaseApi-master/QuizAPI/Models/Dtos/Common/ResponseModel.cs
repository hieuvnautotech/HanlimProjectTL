namespace QuizAPI.Models.Dtos.Common
{
    public class ResponseModel<T>
    {
        public int HttpResponseCode { get; set; } = 200;
        public T? Data { get; set; } = default;
        public int? TotalRow { get; set; } = 0;
        public string? ResponseMessage { get; set; } = string.Empty;
        

        //public ResponseModel()
        //{
        //    HttpResponseCode = 200;
        //    ResponseMessage = $"";
        //    Data = default;
        //}
    }
}
