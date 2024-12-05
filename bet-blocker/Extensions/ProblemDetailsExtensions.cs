using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Application.Extensions
{
    public static class ProblemDetailsExtensions
    {
        public static void UseProblemDetailsExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                    ILogger logger = loggerFactory.CreateLogger(typeof(ProblemDetailsExtensions));

                    if (exceptionHandlerFeature != null)
                    {
                        var exception = exceptionHandlerFeature.Error;
                        ProblemDetails problemDetails;

                        if (exception is BadHttpRequestException)
                        {
                            problemDetails = new ProblemDetails
                            {
                                Title = "The request is invalid.",
                                Status = StatusCodes.Status400BadRequest,
                                Detail = exception.Message
                            };

                            logger.LogWarning(exception, exception.Message);
                        }
                        else if (exception is InvalidOperationException)
                        {
                            problemDetails = new ProblemDetails
                            {
                                Title = "Invalid operation.",
                                Status = StatusCodes.Status408RequestTimeout,
                                Detail = exception.Message
                            };
                        }
                        else
                        {
                            problemDetails = new ProblemDetails
                            {
                                Title = "Internal server error.",
                                Status = StatusCodes.Status500InternalServerError,
                                Detail = "An error occurred while processing the request."
                            };

                            var body = "";

                            try
                            {
                                var bodyStream = new StreamReader(context.Request.Body);
                                bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                                body = await bodyStream.ReadToEndAsync();
                            }
                            catch
                            {
                                logger.LogError("It was not possible to obtain request body.");
                            }
                            finally
                            {
                                var path = context.Request.Path;
                                var message = $"Ocorreu um erro ao processar a requisição, path: {path}, body: {body}";

                                logger.LogError(exception, message);
                            }
                        }

                        problemDetails.Instance = context.Request.Path;
                        context.Response.StatusCode = problemDetails.Status.Value;
                        context.Response.ContentType = "application/problem+json";

                        await context.Response.WriteAsync(JsonSerializer.Serialize<object>(problemDetails));
                    }
                });
            });

        }
    }
}

