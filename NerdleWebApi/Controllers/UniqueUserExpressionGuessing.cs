using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace NerdleWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UniqueUserExpressionGuessingController : ControllerBase
{
    private IUniqueUserExpressionGuessing _uniqueUserExpressionGuessing;

    public UniqueUserExpressionGuessingController(IUniqueUserExpressionGuessing uniqueUserExpressionGuessing)
    {
        _uniqueUserExpressionGuessing = uniqueUserExpressionGuessing;
    }

    [HttpGet(Name = "GetExpressionResponse")]
    public UserGameResultMessage Get(string expression, int id)
    {
        Log.Information($"GetExpressionResponse id: {id} called with parameters: expression: {expression}, id: {id}");
        if (!_uniqueUserExpressionGuessing.Users.ContainsKey(id))
        {
            Log.Error($"UserNotInBase - Function: GetExpressionResponse, expression: {expression}, id: {id}");
            throw new Exception("UserNotInBase");
        }
        else
        {
            if (!_uniqueUserExpressionGuessing.Users[id].IsGameOn)
            {
                Log.Error($"GameIsNotOn - Function: GetExpressionResponse, expression: {expression}, id: {id}");
                throw new Exception("GameIsNotOn");
            }
            else
            {
                _uniqueUserExpressionGuessing.Users[id].GuessNumber++;
                Log.Debug($"GetExpressionResponse id: {id} called MakeResponse method with parameters: expression {expression}, id: {id}");
                string response = _uniqueUserExpressionGuessing.MakeResponse(expression, id);
                Log.Debug($"GetExpressionResponse id: {id} got response from MakeResponse: response: {response}");
                if (response == new string('g', Constants.ExpressionLength))
                    _uniqueUserExpressionGuessing.Users[id].IsGameOn = false;
                return new UserGameResultMessage(!_uniqueUserExpressionGuessing.Users[id].IsGameOn, response, _uniqueUserExpressionGuessing.Users[id].GuessNumber);
            }
        }
    }
    [HttpPost(Name = "StartNewUserGame")]
    public void Post(int id)
    {
        Log.Information($"StartNewUserGame id: {id} called with parameter: id: {id}");
        Log.Debug($"StartNewUserGame id: {id} called StartGame method with parameter: id: {id}");
        _uniqueUserExpressionGuessing.StartGame(id);
    }
}