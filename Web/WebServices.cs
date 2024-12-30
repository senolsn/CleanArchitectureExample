using Web.Middleware;

namespace Web
{
    public static class WebServices
    {
        public static void AddWebServices(this IServiceCollection services)
        {
            services.AddTransient<ExceptionHandlingMiddleware>();
        }
    }
}
