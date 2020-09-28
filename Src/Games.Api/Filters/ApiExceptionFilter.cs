using System;
using System.Collections.Generic;
using System.Security;
using Games.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Games.Api.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {

        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

        public ApiExceptionFilter()
        {
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                {typeof(GuardValidationException), HandleValidationException },
                {typeof(NotFoundException), HandleNotFoundException },
                {typeof(SecurityException), HandleForbidException },
                {typeof(DbUpdateConcurrencyException), HandleConflictException }

            };
        }

        public override void OnException(ExceptionContext context)
        {
            HandleException(context);

            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);

                return;
            }

            HandleUnknownException(context);
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Detail = context.Exception.Message + " " + context.Exception.InnerException?.Message                
            };

            context.Result = new ObjectResult(details)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

            context.ExceptionHandled = true;
        }

        private void HandleValidationException(ExceptionContext context)
        {
            var exception = context.Exception as GuardValidationException;

            var errors = exception?.Errors ?? new Dictionary<string, string[]>() { { "validate", new[] { exception?.Message } } };
            var details = new ValidationProblemDetails(errors)
            {
                Type = "https://tools.ietf.org/html/rfc4918#section-11.2",
                Title = "The specified resource was invalid.",
                Detail = exception?.Message
            };

            context.Result = new UnprocessableEntityObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleNotFoundException(ExceptionContext context)
        {
            var exception = context.Exception as NotFoundException;

            var details = new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Detail = exception?.Message
            };

            context.Result = new NotFoundObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleForbidException(ExceptionContext context)
        {
            context.Result = new ForbidResult();

            context.ExceptionHandled = true;
        }

        private void HandleConflictException(ExceptionContext context)
        {
            var exception = context.Exception as DbUpdateConcurrencyException;

            var details = new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                Title = "The specified resource was updated.",
                Detail = exception?.Message
            };

            context.Result = new ConflictObjectResult(details);

            context.ExceptionHandled = true;
        }
    }
}