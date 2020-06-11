using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hydra.Basket.Function
{
    public static class TryCatchAction
    {
        public static async Task<IActionResult> TryCatch<T>(this T obj, Func<Task<IActionResult>> action, ILogger logger){
            try
            {
                return await action();
            }
            catch (UnauthorizedAccessException ex)
            {
                return new UnauthorizedObjectResult(ex);
            }
            catch(Exception ex){
                 logger.LogError($"Exception thrown: {ex.Message}");
                 return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}