namespace Carbon.Demo.WebApplication.Application.Dtos.Redis
{
    public static class CacheKey
    {
        public const string CompanyById = "Company:{0}";
        public const string CompanyByLocation = "Company:{0}:Location:{1}";
    }
}
