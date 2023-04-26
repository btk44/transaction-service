namespace TransactionService.Application;

public static class ConfigureServices {
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration){
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}