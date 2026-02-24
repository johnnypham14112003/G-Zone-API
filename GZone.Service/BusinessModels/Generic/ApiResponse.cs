namespace GZone.Service.BusinessModels.Generic
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public T? Data { get; set; } // Dữ liệu trả về (Token, UserInfo, v.v.)

        // Các hàm helper để tạo response nhanh (Optional)
        public static ApiResponse<T> Success(T data, string message = "Success")
        {
            return new ApiResponse<T> { StatusCode = 200, Message = message, Data = data };
        }

        public static ApiResponse<T> Failure(string message, int statusCode = 400)
        {
            return new ApiResponse<T> { StatusCode = statusCode, Message = message, Data = default };
        }
    }
}
