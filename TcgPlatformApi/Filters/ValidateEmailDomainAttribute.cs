using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Models;

namespace TcgPlatformApi.Filters
{
    public class ValidateEmailDomainAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.ActionArguments.Values.OfType<EmailRequest>().FirstOrDefault();

            if (request == null)
            {
                context.Result = new BadRequestObjectResult("Invalid request");
                return;
            }

            var allowedDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "gmail.com",
                    "yahoo.com",
                    "outlook.com",
                    "hotmail.com",
                    "mail.ru",
                    "yandex.ru",
                    "rambler.ru",
                };

            var domain = request.ToEmail.Split('@').LastOrDefault();

            if (string.IsNullOrWhiteSpace(domain) || !allowedDomains.Contains(domain))
            {
                context.Result = new BadRequestObjectResult("Email domain is not allowed");
            }
        }
    }
}
