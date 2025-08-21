using AddressManager.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AddressManager.API.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is FluentValidation.ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());

            var result = new ObjectResult(new { Errors = errors })
            {
                StatusCode = 400, 
            };

            context.Result = result;
            context.ExceptionHandled = true;
        }
        else if (context.Exception is AddressNotFoundException)
        {
            context.Result = new NotFoundObjectResult(new {Error = context.Exception.Message});
            context.ExceptionHandled = true;
        }
        else if (context.Exception is DomainValidationException)
        {
            context.Result = new BadRequestObjectResult(new { Error = context.Exception.Message });
            context.ExceptionHandled = true;
        }
    }
}
