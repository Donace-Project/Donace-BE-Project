namespace Donace_BE_Project.Models
{
    public class PageInfoModel
    {
        public int PageNumber { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public PageInfoModel(long totalCount, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPage = (int)Math.Ceiling(totalCount/(decimal) pageSize);
        }
        public PageInfoModel()
        {
            
        }
    }
}
