using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Plant_HexArquitecture_API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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


        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            var problem = ex switch
            {
                BadRequestException bre => new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Solicitud inválida",
                    Detail = bre.Message
                },

                NotFoundException nfe => new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Title = "Recurso no encontrado",
                    Detail = nfe.Message
                },

                ConflictException ce => new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflicto",
                    Detail = ce.Message
                },

                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Error interno del servidor",
                    Detail = "Ocurrió un error inesperado"
                }
            };
            context.Response.StatusCode = problem.Status!.Value;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
