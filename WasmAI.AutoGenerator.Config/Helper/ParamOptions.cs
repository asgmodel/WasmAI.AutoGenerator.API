namespace AutoGenerator.Helper
{
    public class ParamOptions
    {
        public ParamOptions()
        {

        }
        public ParamOptions(List<string> includes)
        {
            Includes = includes;
        }

        public ParamOptions(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public ParamOptions(List<string> includes, int pageNumber, int pageSize)
        {
            Includes = includes;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public List<string> Includes { get; set; } = [];

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;

        public void AddInclude(params string[] extends)
        {
            Includes.AddRange(extends);
        }
    }


}
