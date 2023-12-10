namespace Donace_BE_Project.Models
{
    public class ResponseModel<T>
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public T Result { get; set; }
        public PageInfoModel PageInfo { get; set; }

        public ResponseModel(bool success, string code)
        {
            Success = success;
            Code = code;
        }

        public ResponseModel(bool success, string code, T result)
        {
            Success = success;
            Result = result;
            Code = code;
        }

        public ResponseModel(bool success, string code, T result, PageInfoModel pageInfo)
        {
            Success = success;
            Result = result;
            Code = code;
            PageInfo = pageInfo;
        }
        public ResponseModel()
        {
            
        }
    }
}
