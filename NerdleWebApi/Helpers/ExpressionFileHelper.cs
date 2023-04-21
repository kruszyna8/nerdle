using NerdleWebApi;

public static class ExpressionFileHelper
{
    private static List<string> AllExpressions;
    private static string filePath;

    /// <summary>
    /// Loads all expressions from a file and saves it to list AllExpressions.
    /// </summary>
    static ExpressionFileHelper()
    {
        //filePath = @"Resources\expressions.txt";
        filePath = @"E:\Projects\Nerdle\NerdleWebApi\Resources\expressions.txt";
        string[] fileLines = File.ReadAllLines(filePath);
        AllExpressions = fileLines.ToList();
    }
    /// <summary>
    /// Changes file path
    /// </summary>
    /// <param name="fPath"> Path to file </param>
    public static void ChangeFilePath(string fPath)
    {
        filePath = fPath;
    }

    /// <summary>
    /// Copies list AllExpressions.
    /// </summary>
    /// <returns> List of strings that is a copy of AllExpressions.</returns>
    public static List<string> GetAllExpresions()
    {
        return new List<string>(AllExpressions);
    }

    /// <summary>
    /// Randomly chooses expression from AllExpresisons.
    /// </summary>
    /// <returns> Random math expression. </returns>
    public static string GetRandomExpression()
    {
        Random rnd = new Random();
        return AllExpressions[rnd.Next(0, AllExpressions.Count)];
    }

    /// <summary>
    /// Searchs for next math expression while searching for one.
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="index"></param>
    /// <returns> Math expression which may be incorrect mathematically. </returns>
    public static char[] NextExpression(char[] expression, int index)
    {
        // Setting each char after index char to first available char
        for (int i = index + 1; i < Constants.ExpressionLength; i++)
            expression[i] = Constants.AvailableChars[0];

        // Setting char on index to next available char. If there is none available chars sets index to 
        // first available char and and tries to set next for index - 1 etc.
        for (int i = index; i >= 0; i--)
        {
            int charIndex = Constants.AvailableChars.IndexOf(expression[i]);
            if (charIndex == Constants.AvailableChars.Length - 1)
                expression[i] = Constants.AvailableChars[0];
            else
            {
                expression[i] = Constants.AvailableChars[charIndex + 1];
                break;
            }
        }
        return expression;
    }

    /// <summary>
    /// Writes to file all correct math expressions which are length 8 and contains digits from 0 to 9 and operators +, -, *, /, =.
    /// File is located in FilePath and it is called expressions.
    /// </summary>
    public static void SaveAllExpressionsToFile()
    {
        char[] guess = new char[Constants.ExpressionLength];
        Array.Fill(guess, Constants.AvailableChars[0]);
        var exitArray = new char[Constants.ExpressionLength];
        Array.Fill(exitArray, Constants.AvailableChars[0]);
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            while (true)
            {
                // Checking if it is correct math expression
                (_, int index, bool isMathExpression) = ExpressionValidator.IsCorrectMathExpression(guess);
                if (isMathExpression)
                {
                    //Checking if left and right side of expression is equal
                    string polishNotation = ExpressionValidator.ChangeExpresisonToReversePolishNotation(new string(guess).Substring(0, index));
                    double leftValue = ExpressionValidator.CountReversePolishNotation(polishNotation);
                    double rightValue = int.Parse(new string(guess).Substring(index + 1));
                    if (leftValue == rightValue)
                    {
                        // Saving to file
                        foreach (var c in guess)
                        {
                            writer.Write(c);
                            Console.Write(c);
                        }
                        Console.WriteLine();
                        writer.WriteLine();
                    }
                    guess = NextExpression(guess, Constants.ExpressionLength - 1);
                }
                else
                    guess = NextExpression(guess, index);

                // Checking exit condition
                int equalCharCount = 0;
                for (int i = 0; i < Constants.ExpressionLength; i++)
                {
                    if (exitArray[i] == guess[i])
                        equalCharCount++;
                    else
                        break;
                }
                if (equalCharCount == Constants.ExpressionLength)
                    break;
            }
        }
    }
}