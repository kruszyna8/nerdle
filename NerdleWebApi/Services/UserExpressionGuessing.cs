using NerdleWebApi;
using Serilog;

public interface IUserExpressionGuessing
{
    string CorrectExpression { get; set; }
    bool IsGameOn { get; set; }
    int GuessNumber { get; set; }
    string MakeResponse(string guess);
    void StartGame();
}

public class UserExpressionGuessing : IUserExpressionGuessing
{
    public string CorrectExpression { get; set; }
    public bool IsGameOn { get; set; }
    public int GuessNumber { get; set; }
    public UserExpressionGuessing()
    {
        CorrectExpression = "";//42-13=29
        IsGameOn = false;
        GuessNumber = 0;
    }
    /// <summary>
    /// Says which characters in guess expression are correct, are not on good place and are not in expression.
    /// </summary>
    /// <param name="guess"> Users guess </param>
    /// <returns> Response string: g - character is on good place, v - character occurs in expression but is on wrong place, 
    /// b - character doesn't occur in expression
    /// </returns>
    public string MakeResponse(string guess)
    {
        int[] occurrenceNumber = new int[Constants.AvailableChars.Length];
        char[] response = new char[Constants.ExpressionLength];

        // Initialize response
        for (int i = 0; i < Constants.ExpressionLength; i++)
            response[i] = '.';

        if (!IsGameOn)
        {
            Log.Error($"Game didn't start - Function: MakeResponse");
            throw new Exception("Game didn't start");
        }
            

        // Checking for good guesses
        for (int i = 0; i < Constants.ExpressionLength; i++)
        {
            if (guess[i].Equals(CorrectExpression[i]))
                response[i] = 'g';
            else
                occurrenceNumber[Constants.AvailableChars.IndexOf(CorrectExpression[i])]++;
        }

        // Checking for bad and possible guesses
        for (int i = 0; i < Constants.ExpressionLength; i++)
        {
            if (response[i] == '.')
            {
                if (occurrenceNumber[Constants.AvailableChars.IndexOf(guess[i])] > 0)
                {
                    response[i] = 'v';
                    occurrenceNumber[Constants.AvailableChars.IndexOf(guess[i])]--;
                }
                else
                    response[i] = 'b';
            }
        }
        string finalResponse = new string(response);
        return finalResponse;
    }
    /// <summary>
    /// Starts new game
    /// </summary>
    public void StartGame()
    {
        CorrectExpression = ExpressionFileHelper.GetRandomExpression();
        IsGameOn = true;
        GuessNumber = 0;
    }
}