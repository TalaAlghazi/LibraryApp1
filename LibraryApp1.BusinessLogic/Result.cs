namespace LibraryApp1.BusinessLogic
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public int Code { get; set; }
        public string Description { get; set; } = "";
        public T? Data { get; set; }

        public static Result<T> Success(T data, string description = "")
            => new Result<T> { IsSuccess = true, Code = 200, Data = data, Description = description };

        public static Result<T> Failure(int code, string description)
            => new Result<T> { IsSuccess = false, Code = code, Description = description };
    }
}