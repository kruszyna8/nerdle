public class UserGameResultMessage
{
    public bool IsGameFinished { get; set; }
    public string ColoredExpression { get; set; }
    public int GuessNumber { get; set; }
    public UserGameResultMessage(bool isGameFinished, string coloredExpression, int guessNumber)
    {
        IsGameFinished = isGameFinished;
        ColoredExpression = coloredExpression;
        GuessNumber = guessNumber;
    }
}