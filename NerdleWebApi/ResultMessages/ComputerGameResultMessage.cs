public class ComputerGameResult
{
    public bool ExpressionFound { get; set; }
    public string Response { get; set; }
    public ComputerGameResult(bool expressionFound, string response)
    {
        ExpressionFound = expressionFound;
        Response = response;
    }
}