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

                // Grava o erro no arquivo .txt
                LogToFile(ex, requestPath, "");

                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var requestPath = context.Request.Path;
            var statusCode = HttpStatusCode.InternalServerError;
            string error = "";
            var result = JsonSerializer.Serialize(new
            {
                error = exception.Message + $" / ocorreu um erro inesperado. Por favor, tente novamente mais tarde. / log: {requestPath}"
            });

            LogToFile(exception, requestPath, error);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(result);
        }

        public void LogToFile(Exception ex, string requestPath, string mensagem)
        {
            if (string.IsNullOrEmpty(mensagem))
            {
                mensagem = $"Erro: {ex.Message} + / + {Environment.NewLine}";
            }
            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | Path: {requestPath} | Erro: {ex.Message} {Environment.NewLine}";


            File.AppendAllText(_logFilePath, logMessage);
        }
    }
}
