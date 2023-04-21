using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace NerdleWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ComputerExpressionGuessingController : ControllerBase
{
    private IUniqueComputerExpressionGuessing _computerExpressionGuessing;

    public ComputerExpressionGuessingController(IUniqueComputerExpressionGuessing computerExpressionGuessing)
    {
        _computerExpressionGuessing = computerExpressionGuessing;
    }
    [HttpGet(Name = "GetResponse")]
    public ComputerGameResult Get(string response, int id)
    {
        Log.Information($"GetResponse called with parameters: response: {response}, id: {id}");
        if (!_computerExpressionGuessing.Users.ContainsKey(id))
        {
            Log.Error($"UserNotInBase - Function: GetResponse, response: {response}, id: {id}");
            throw new Exception("UserNotInBase");
        }
        else
        {
            if (!_computerExpressionGuessing.Users[id].IsGameOn)
            {
                Log.Error($"GameIsNotOn - Function GetResponse, response: {response}, id: {id}");
                throw new Exception("GameIsNotOn");
            }
            else
            {
                Log.Debug($"GetResponse id: {id} called UpdateData with parameters: response:{response}, lastGuess{_computerExpressionGuessing.Users[id].LastGuess}");
                _computerExpressionGuessing.Users[id].UpdateData(response, _computerExpressionGuessing.Users[id].LastGuess);
                Log.Debug($"GetResponse id: {id} called MakeGuess with paremater: id {id}");
                var a = _computerExpressionGuessing.MakeGuess(id);
                Log.Debug($"GetResponse id: {id} get response from MakeGuess: ExpressionFound: {a.ExpressionFound}, Response: {a.Response}");
                return a;
            }
        }
    }
    [HttpPost(Name = "StartNewComputerGame")]
    public void Post(int id)
    {
        Log.Information($"StartNewComputerGame id: {id} called with parameter: id: {id}");
        Log.Debug($"StartNewComputerGame id: {id} called StartGame method with parameter: id: {id}");
        _computerExpressionGuessing.StartGame(id);
    }
}
