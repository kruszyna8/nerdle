using System.Collections;
using NerdleWebApi;
using Serilog;

public interface IExpressionValidator
{
    ValidationResultMessage IsCorrectInput(string expression);
}

public class ExpressionValidator : IExpressionValidator
{
    /// <summary>
    /// Checks if expression contains one '=' sign, if yes if its left side can be computed and right side doesn't contain any operator signs.
    /// Expression can only contain numbers from 0 to 9 and +, -, *, /, = operators.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns> First returned value is string comment about expression. 
    /// Second returned value is index of expression to change while searching for next correct math expression. 
    /// If length of expression doesn't equal length paremater -1 is returned.
    /// Last is bool which indicates whether it is correct math expression or not. </returns>
    public static (string message, int index, bool isCorrect) IsCorrectMathExpression(char[] expression, int length = Constants.ExpressionLength)
    {
        // Checking if length is correct
        if (expression.Length != length)
            return ("Wrong expression length!", -1, false);

        // Checking if first character is a number
        if (!char.IsNumber(expression[0]))
            return ("First character is an operator sign!", 0, false);

        // Checking if there is any division by 0 or two math operators next to each other before '=' sign
        int index = 1;
        int lastOperation = 0; // 0 if last character is a digit, 1 when math sign other than /, 2 when /
        bool LastDivisionAndLast0 = false;
        while (index < expression.Length && expression[index] != '=')
        {
            // Character is a number
            if (char.IsNumber(expression[index]))
            {
                // Last operator is a division sign and checking for division by 0
                if (lastOperation == 2 && expression[index] == '0')
                    LastDivisionAndLast0 = true;
                if (expression[index] != '0')
                    LastDivisionAndLast0 = false;
                lastOperation = 0;
            }

            // Character is a math sign
            else
            {
                // Checking for division by 0
                if (LastDivisionAndLast0)
                    return ("Division by 0!", index - 1, false);

                // Checking for two operators next to each other
                if (lastOperation != 0)
                    return ("Two operator signs next to each other!", index, false);

                // Checking if operator is a division sign
                if (expression[index] == '/')
                    lastOperation = 2;
                else
                    lastOperation = 1;

                LastDivisionAndLast0 = false;
            }
            index++;
        }

        // Checking if equal sign was not in expression
        if (index > length - 1)
            return ("Equal sign not in expression!", length - 2, false);

        // Checking if equal sign was on last position
        if (index == length - 1)
            return ("Equal sign can't be on last position!", length - 2, false);

        // Checking if character before equal sign was a math operator
        if (lastOperation >= 1)
            return ("Operator right before equal sign!", index - 1, false);

        // Checking if there is a math operator after equal sign
        int equalSignIndex = index;
        index++;
        while (index < expression.Length)
        {
            if (!char.IsNumber(expression[index]))
                return ("Operator after equal sign", index, false);
            index++;
        }
        return ("Everything is good!", equalSignIndex, true);
    }

    /// <summary>
    /// Changes expression to reverse polish notation using basic principals. 
    /// Expression can only contain numbers from 0 to 9 and +, -, *, /, = operators.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>Expression in reverse polish notation where numbers are surrounded by '<' and '>' characters.</returns>
    /// <exception cref="WrongCharacterInExpression"></exception>
    /// <exception cref="ExpressionIsNotMathematicallyCorrect"></exception>
    public static string ChangeExpresisonToReversePolishNotation(string expression)
    {
        // Checking if expression contains legal characters
        foreach (char c in expression)
            if (!Char.IsDigit(c) && !Constants.AvailableOperators.Contains(c) || c == '=')
            {
                Log.Error($"WrongCharacterInExpression - Function: ChangeExpresisonToReversePolishNotation, character: {c} in expression: {expression}");
                throw new Exception("WrongCharacterInExpression");
            }

        // Checking if expression is mathematically correct
        if (!IsCorrectMathExpression((expression + "=0").ToCharArray(), expression.Length + 2).Item3)
        {
            Log.Error($"ExpressionIsNotMathematicallyCorrect - Function: ChangeExpresisonToReversePolishNotation, expression: {expression}");
            throw new Exception("ExpressionIsNotMathematicallyCorrect");
        }


        // Changing to reverse polsih notation
        string resultExpression = "", number = "";
        Stack<char> expressionStack = new Stack<char>();
        foreach (char c in expression)
        {
            if (Char.IsNumber(c))
                number += c;
            else
            {
                resultExpression += (Constants.NumberStart + number + Constants.NumberEnd);
                number = "";
                if (expressionStack.Count == 0)
                    expressionStack.Push(c);
                else if (c == '+' || c == '-')
                {
                    while (expressionStack.Count != 0)
                        resultExpression += expressionStack.Pop();
                    expressionStack.Push(c);
                }
                else
                {
                    char sign;
                    while (expressionStack.Count != 0)
                    {
                        sign = expressionStack.Pop();
                        if (sign == '+' || sign == '-')
                        {
                            expressionStack.Push(sign);
                            break;
                        }
                        else
                            resultExpression += sign;
                    }
                    expressionStack.Push(c);
                }
            }
        }
        resultExpression += (Constants.NumberStart + number + Constants.NumberEnd);
        while (expressionStack.Count != 0)
        {
            char sign = expressionStack.Pop();
            resultExpression += sign;
        }
        return resultExpression;
    }

    /// <summary>
    /// Counts value of reverse polish notation expression using basic principals.
    /// Expression can only contain numbers from 0 to 9 and +, -, *, /, =, ?, ? operators.
    /// Numbers must be surrounded with '<' and '>' characters.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns> Value of expression. </returns>
    /// <exception cref="WrongCharacterInExpression"></exception>
    /// <exception cref="ExpressionIsNotInReversePolishNotation"></exception>
    public static double CountReversePolishNotation(string expression)
    {
        // Checking if expression contains legal characters
        foreach (char c in expression)
            if (!Char.IsDigit(c) && !Constants.AvailableOperators.Contains(c) || c == '=')
                if (c != '<' && c != '>')
                {
                    Log.Error($"WrongCharacterInExpression - Function: CountReversePolishNotation, character: {c} in expression: {expression}");
                    throw new Exception("WrongCharacterInExpression");
                }
        // Counting value of expression
        Stack expressionStack = new Stack();
        string number = "";
        foreach (char c in expression)
        {
            if (c == Constants.NumberStart)
                number = "";
            else if (Char.IsNumber(c))
                number += c;
            else if (c == Constants.NumberEnd)
                expressionStack.Push(double.Parse(number));
            else
            {
                if(expressionStack.Count <= 1)
                {
                    Log.Error($"ExpressionIsNotInReversePolishNotation - Function: CountReversePolishNotation, expression: {expression}");
                    throw new Exception("ExpressionIsNotInReversePolishNotation");
                }
                var firstPop = expressionStack.Pop();
                var secondPop = expressionStack.Pop();
                if (firstPop == null || secondPop == null)
                {
                    Log.Error($"ExpressionIsNotInReversePolishNotation - Function: CountReversePolishNotation, expression: {expression}");
                    throw new Exception("ExpressionIsNotInReversePolishNotation");
                }
                
                double number1 = 0;
                double number2 = 0;

                if (firstPop.GetType() == typeof(double))
                    number1 = (double)firstPop;
                else
                {
                    Log.Error($"ExpressionIsNotInReversePolishNotation - Function: CountReversePolishNotation, expression: {expression}");
                    throw new Exception("ExpressionIsNotInReversePolishNotation");
                }
                if (secondPop.GetType() == typeof(double))
                    number2 = (double)secondPop;
                else
                {
                    Log.Error($"ExpressionIsNotInReversePolishNotation - Function: CountReversePolishNotation, expression: {expression}");
                    throw new Exception("ExpressionIsNotInReversePolishNotation");
                }
                double computedNumber = 0;
                switch (c)
                {
                    case '+':
                        computedNumber = number2 + number1;
                        break;
                    case '-':
                        computedNumber = number2 - number1;
                        break;
                    case '*':
                        computedNumber = number2 * number1;
                        break;
                    case '/':
                        computedNumber = number2 / number1;
                        break;
                    default:
                        break;
                }
                expressionStack.Push(computedNumber);
            }
        }

        var result = expressionStack.Pop();
        if (result == null)
        {
            Log.Error($"ExpressionIsNotInReversePolishNotation - Function: CountReversePolishNotation, expression: {expression}");
            throw new Exception("ExpressionIsNotInReversePolishNotation");
        }
        if (result.GetType() == typeof(double))
            return (double)result;
        else
        {
            Log.Error($"ExpressionIsNotInReversePolishNotation - Function: CountReversePolishNotation, expression: {expression}");
            throw new Exception("ExpressionIsNotInReversePolishNotation");
        }
    }

    /// <summary>
    /// Chekcing if whole expression is mathematically correct, where left side of equation equals right side.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns> True when it is valid expression, false when not with message according to error type.</returns>
    public ValidationResultMessage IsCorrectInput(string expression)
    {
        for (int i = 0; i < expression.Length; i++)
            if (!Constants.AvailableChars.Contains(expression[i]))
                return new ValidationResultMessage(false, $"Wrong character {expression[i]} at position {i}!");
        bool isCorrectMath;
        int equalSignIndex;
        string comment;
        (comment, equalSignIndex, isCorrectMath) = IsCorrectMathExpression(expression.ToCharArray());
        if (!isCorrectMath)
            return new ValidationResultMessage(false, comment);

        string leftExpression = expression.Substring(0, equalSignIndex);
        leftExpression = ChangeExpresisonToReversePolishNotation(leftExpression);
        double leftValue = CountReversePolishNotation(leftExpression);
        double rightValue = Double.Parse(expression.Substring(equalSignIndex + 1));
        if (leftValue != rightValue)
            return new ValidationResultMessage(false, "Left side of equation doesn't equal right side!");
        return new ValidationResultMessage(true, "Everything is good!");
    }
}