using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace TcgPlatformApi.Filters
{
    public class ValidateAvatarFileAttribute : ActionFilterAttribute
    {
        private readonly string _parameterName;

        public ValidateAvatarFileAttribute(string parameterName = "file")
        {
            _parameterName = parameterName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ActionArguments.TryGetValue(_parameterName, out var fileObj) || fileObj is not IFormFile file)
            {
                context.Result = new BadRequestObjectResult("Invalid file");
                return;
            }

            if (file.Length == 0)
            {
                context.Result = new BadRequestObjectResult("Invalid file");
                return;
            }

            var allowedTypes = new[] { "image/jpeg", "image/pjpeg", "image/jpg" };
            if (!allowedTypes.Contains(file.ContentType))
            {
                context.Result = new BadRequestObjectResult("File must be .jpg");
            }
        }
    }

}
