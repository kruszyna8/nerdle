public class ValidationResultMessage
{
    public bool IsCorrect { get; set; }
    public string Message { get; set; }
    public ValidationResultMessage(bool isCorrect, string message)
    {
        IsCorrect = isCorrect;
        Message = message;
    }
}