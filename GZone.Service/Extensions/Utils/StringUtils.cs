using System.Security.Cryptography;
using System.Text;

namespace GZone.Service.Extensions.Utils
{
    public static class StringUtils
    {
        public static string HashStringSHA256(string input)//SHA-256 Algorithm (1 way)
        {
            using SHA256 sha256Hasher = SHA256.Create();
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Extract username from email.
        /// </summary>
        public static string GetUsername(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;

            // Tìm vị trí của ký tự @
            int atIndex = email.IndexOf('@');

            // Nếu không tìm thấy @ hoặc @ ở đầu chuỗi, trả về giá trị phù hợp
            if (atIndex <= 0)
                return string.Empty;

            // Sử dụng Span để thao tác trên bộ nhớ mà không tạo chuỗi tạm
            return email.AsSpan(0, atIndex).ToString();
        }
    }
}
