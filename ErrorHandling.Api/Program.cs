using ErrorHandling.Api.Middleware;
using ErrorHandling.Api.Util;
using FluentValidation.AspNetCore;
using Serilog;

namespace ErrorHandling.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            // Configuração do Serilog para log em arquivo
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) // Cria um novo arquivo por dia
                .CreateLogger();

            // Registra todos os validadores do FluentValidation automaticamente
            builder.Services.AddControllers().AddFluentValidation(config =>
            {
                config.RegisterValidatorsFromAssemblyContaining<ProdutoDTOValidator>();
            });

            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

       

    }
}
