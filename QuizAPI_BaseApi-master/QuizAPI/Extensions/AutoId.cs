namespace QuizAPI.Extensions
{
    public static class AutoId
    {
        public static long AutoGenerate()
        {
            //var d = DateTime.UtcNow;
            //var toStr = d.ToString("yyMMddHHmmssf");
            //var rd = new Random().Next(100, 999).ToString();
            //return Int64.Parse(string.Concat(toStr, rd));

            var now = DateTime.UtcNow;
            return now.Ticks / 10000;
        }
    }
}
