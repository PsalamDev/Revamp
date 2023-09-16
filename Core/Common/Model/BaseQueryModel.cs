namespace Core.Common.Model
{
    public class BaseQueryModel
    {
        public string? Keyword { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}