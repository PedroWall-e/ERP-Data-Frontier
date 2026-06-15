using System.Net;
using System.Text.Json;
using DataFrontier.Domain.Exceptions;

namespace DataFrontier.API.Middleware;

/// <summary>
/// Middleware global de tratamento de exceções.
/// Captura exceções não tratadas e as converte em respostas HTTP padronizadas.
/// 
/// Mapeamento de exceções para status codes:
/// - UnauthorizedAccessException → 401
/// - TenantResolutionException → 403
/// - KeyNotFoundException → 404
/// - InvalidOperationException → 409 Conflict
/// - Exception genérica → 500
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;

        var (statusCode, errorMessage) = exception switch
        {
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Credenciais inválidas."),
            TenantResolutionException => (HttpStatusCode.Forbidden, "Falha na resolução do tenant."),
            KeyNotFoundException => (HttpStatusCode.NotFound, "Recurso não encontrado."),
            InvalidOperationException => (HttpStatusCode.Conflict, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "Erro interno do servidor.")
        };

        // Log com nível apropriado
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(
                exception,
                "Erro não tratado | TraceId: {TraceId} | Path: {Path}",
                traceId, context.Request.Path);
        }
        else
        {
            _logger.LogWarning(
                "Exceção tratada ({StatusCode}) | TraceId: {TraceId} | Path: {Path} | Mensagem: {Message}",
                (int)statusCode, traceId, context.Request.Path, exception.Message);
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = errorMessage,
            detail = statusCode == HttpStatusCode.InternalServerError
                ? "Um erro inesperado ocorreu. Contate o suporte."
                : exception.Message,
            traceId
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsJsonAsync(response, jsonOptions);
    }
}

/// <summary>
/// Métodos de extensão para registrar o ExceptionHandlingMiddleware.
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// Adiciona o middleware global de tratamento de exceções ao pipeline.
    /// Deve ser o PRIMEIRO middleware do pipeline.
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
