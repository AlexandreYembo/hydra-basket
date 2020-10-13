using Microsoft.AspNetCore.Mvc.Filters;

namespace Hydra.Basket.Function.Helper
{
    public class ErrorHandlingFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context){
            var exception = context.Exception;
            //log your exception here
            context.ExceptionHandled = true;
        }
    }
}