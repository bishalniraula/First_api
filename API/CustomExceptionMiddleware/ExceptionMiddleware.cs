using API.Models;
using System.Net;
using System.Net.Mime;
using System.Text.Json;



namespace API.CustomExceptionMiddleware
{

    //middleware catches problem and generatenappropriate json responses
    //imiddleware yaha process ra request handle garna use
    //main use of Imiddleware is used to handle the exception in 
    // Task InvokeAsync(HttpContext context, RequestDelegate next);
    public class ExceptionMiddleware : IMiddleware
    {
        //generics types vayera generics 
        private readonly ILogger<ExceptionMiddleware> _logger;

        //ihost envrironment provides about the info abt hostin env
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }
        //program entry point 
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                //pass the context to next middleware 
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            //response type :it contain the type of response to json data 
            //media types means the format that is communicated between the client and the
            context.Response.ContentType = MediaTypeNames.Application.Json;
            //status code setup :which status code do we want to  setup it is set here
            ////500: internal server error
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //checks whether the app is running or not
            var response = _env.IsDevelopment()

                   // ?=choose the  value based on the two condition given 
                   // ? ToString: to avoid potential null reference exceptional
                   ? new CustomResponse(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                   : new CustomResponse(context.Response.StatusCode, "Internal Server Error");

            //setting for json serialization , properties haru add garne kam garchha
            //format change  camel case ma change garchh garchhan jastei {"name:john","address:kathmandu "}
            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            //serialize the responses 
            var json = JsonSerializer.Serialize(response, options);
            //serialized format ma dats respond garchha 
            await context.Response.WriteAsync(json);

        }
    }
}