using System.Text.Json;
using API;
using API.Controllers;


namespace API.Models
{
    public class CustomResponse
    {
        public CustomResponse(int statusCode, string message, string details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
        }
        //status code pathaucha
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}
