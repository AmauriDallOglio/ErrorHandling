using ErrorHandling.Api.Util;
using FluentValidation;
using System.Net;
using System.Text.Json;
namespace ErrorHandling.Api.Middleware
{


    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly string _logFilePath = "logs/error_log.txt"; // Caminho do arquivo de log

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;

            if (!Directory.Exists("logs"))
            {
                Directory.CreateDirectory("logs");
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var requestPath = context.Request.Path;

             
                _logger.LogError(ex, "Ocorreu uma exceção não tratada no caminho {Path}.", requestPath);
                LogToFile(ex, requestPath, "Erro inesperado");
                await HandleExceptionAsync(context, ex);
         
            }
        }


        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var requestPath = context.Request.Path;
            var result = Resultado<object>.ComFalha(new Erro(500, "Erro inesperado"));

            var response = JsonSerializer.Serialize(new
            {
                result.Erro,
                error = exception.Message + $" / ocorreu um erro inesperado. Por favor, tente novamente mais tarde. / log: {requestPath}"
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(response);
        }

        public void LogToFile(Exception ex, string requestPath, string mensagem)
        {
            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | Path: {requestPath} | {mensagem}: {ex.Message} {Environment.NewLine}";
            File.AppendAllText(_logFilePath, logMessage);
        }
    }


}