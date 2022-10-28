using System.Security.Cryptography;
using System.Text;

namespace QuizAPI.Extensions
{
    public static class MD5Encryptor
    {
        public static string MD5Hash(string? text)
        {
            MD5 md5 = MD5.Create();

            //compute hash from the bytes of text
            md5.ComputeHash(Encoding.ASCII.GetBytes(text));

            //get hash result after compute
            byte[]? result = md5.Hash;

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < result?.Length; i++)
            {
                //change it into 2 hexadecimal digits
                //for each byte
                stringBuilder.Append(result[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }
}
