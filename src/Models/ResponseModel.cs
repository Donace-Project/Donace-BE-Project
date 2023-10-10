namespace Donace_BE_Project.Models
{
    public class ResponseModel<T>
    {
        public string Code { get; set; }
        public bool Success { get; set; }
        public T Result { get; set; }
        public ResponseModel(bool success, string code, T result)
        {
            Success = success;
            Result = result;
            Code = code;
        }
    }
}
