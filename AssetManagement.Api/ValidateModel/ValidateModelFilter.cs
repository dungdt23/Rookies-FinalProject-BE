using AssetManagement.Application.ApiResponses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AssetManagement.Api.ValidateModel;

public class ValidateModelFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }


    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    );

            var apiResponse = new ApiResponse
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Validation Failed",
                Data = errors
            };

            context.Result = new BadRequestObjectResult(apiResponse);
        }
    }
}
