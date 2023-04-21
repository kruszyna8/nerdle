using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace NerdleWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ExpressionValidatorController : ControllerBase
{
    private IExpressionValidator _expressionValidator;

    public ExpressionValidatorController(IExpressionValidator expressionValidator)
    {
        _expressionValidator = expressionValidator;
    }

    [HttpGet(Name = "GetExpressionValidation")]
    public ValidationResultMessage Get(string expression)
    {
        Log.Information($"GetExpressionValidation called with parameter expression: {expression}");
        Log.Information($"GetExpressionValidation called IsCorrectInput method with parameter: expression {expression}");
        var response = _expressionValidator.IsCorrectInput(expression); 
        Log.Information($"GetExpressionValidation got response from IsCorrectInput: IsCorrect{response.IsCorrect}, Message: {response.Message}");
        return response;
    }
}
