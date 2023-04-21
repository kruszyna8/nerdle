using NerdleWebApi;
using Serilog;

public class CharQuantity
{
    public int Quantity { get; set; }
    public bool IsEqual { get; set; }
    public CharQuantity()
    {
        Quantity = 0;
        IsEqual = false;
    }
}

public interface IComputerExpressionGuessing
{
    List<string> Expressions { get; set; }
    Dictionary<char, CharQuantity> Quantities { get; set; }
    string FirstGuess { get; set; }
    string LastGuess { get; set; }
    List<List<char>> PossibleChars { get; set; }
    bool IsGameOn { get; set; }

    ComputerGameResult MakeGuess();
    void StartGame();
    void UpdateData(string response, string myGuess);
}

public class ComputerExpressionGuessing : IComputerExpressionGuessing
{
    public List<string> Expressions { get; set; }
    public Dictionary<char, CharQuantity> Quantities { get; set; }
    public string FirstGuess { get; set; }
    public string LastGuess { get; set; }
    public List<List<char>> PossibleChars { get; set; }
    public bool IsGameOn { get; set; }
    public ComputerExpressionGuessing()
    {
        Expressions = new List<string>();
        Quantities = new Dictionary<char, CharQuantity>();
        FirstGuess = Constants.FirstGuess;
        LastGuess = FirstGuess;
        PossibleChars = new List<List<char>>();
        IsGameOn = false;
    }
    /// <summary>
    /// Starts new game
    /// </summary>
    public void StartGame()
    {
        FirstGuess = Constants.FirstGuess;
        LastGuess = FirstGuess;
        Expressions = ExpressionFileHelper.GetAllExpresions();
        IsGameOn = true;

        // Amount of each char at each possition
        Quantities = new Dictionary<char, CharQuantity>();
        for (int i = 0; i < Constants.AvailableChars.Length; i++)
            Quantities.Add(Constants.AvailableChars[i], new CharQuantity());
        Quantities['='].Quantity = 1;
        Quantities['='].IsEqual = true;

        // Possible chars at each possition
        PossibleChars = new List<List<char>>();
        PossibleChars.Add(new List<char>(Constants.AvailableChars.Substring(0, 10)));
        for (int i = 0; i < Constants.ExpressionLength - 2; i++)
            PossibleChars.Add(new List<char>(Constants.AvailableChars));
        PossibleChars.Add(new List<char>(Constants.AvailableChars.Substring(0, 10)));
    }
    /// <summary>
    /// Updates variables which are responsible for making guesses
    /// </summary>
    /// <param name="response"> Users response on computer guess </param>
    /// <param name="myGuess"> Computer guess </param>
    public void UpdateData(string response, string myGuess)
    {
        char[] answer = new char[Constants.ExpressionLength];

        // Checking length of response
        if (response.Length != Constants.ExpressionLength)
        {
            Log.Error($"WrongResponseLength - Function: UpdateData, response: {response}");
            throw new Exception("WrongResponseLength");
        }
        
        // Checking for illegal char in response
        foreach (var c in response)
        {
            if (!"gvb".Contains(c))
            {
                Log.Error($"WrongCharInResponse - Function: UpdateData, response: {response}, char: {c}");
                throw new Exception("WrongCharInResponse");
            }
        }

        // Making copy of Quantities
        Dictionary<char, CharQuantity> currentQuantities = new Dictionary<char, CharQuantity>();
        foreach (var item in Quantities)
        {
            currentQuantities.Add(item.Key, new CharQuantity());
        }

        // Updating data according to response
        int index = 0;
        foreach (char c in response)
        {
            // Green
            if (c.Equals('g'))
            {
                currentQuantities[myGuess[index]].Quantity++;
                answer[index] = myGuess[index];
                PossibleChars[index] = new List<char> { myGuess[index] };
            }
            
            // Violet
            else if (c.Equals('v'))
            {
                currentQuantities[myGuess[index]].Quantity++;
                PossibleChars[index].Remove(myGuess[index]);
            }

            // Black
            else
            {
                currentQuantities[myGuess[index]].IsEqual = true;
                PossibleChars[index].Remove(myGuess[index]);
                
                // Checking if there is any char c in violet color 
                int newIndex = 0;
                bool removeAll = true;
                foreach (char cc in response)
                {
                    if (myGuess[newIndex].Equals(myGuess[index]) && cc.Equals('v'))
                    {
                        removeAll = false;
                        break;
                    }
                    newIndex++;
                }

                // Removing char c from data if there is no more char c in expression
                if (removeAll)
                {
                    newIndex = 0;
                    foreach (var list in PossibleChars)
                    {
                        if (answer[newIndex] != myGuess[index] && (myGuess[index] != myGuess[newIndex] || response[newIndex] != 'g'))
                            list.Remove(myGuess[index]);
                        newIndex++;
                    }
                }
            }
            index++;
        }

        // Updating Quantities
        foreach (var cq in currentQuantities)
        {
            if (Quantities[cq.Key].Quantity < cq.Value.Quantity)
            {
                Quantities[cq.Key].Quantity = cq.Value.Quantity;
                Quantities[cq.Key].IsEqual = cq.Value.IsEqual;
            }
            else if (!Quantities[cq.Key].IsEqual)
                Quantities[cq.Key].IsEqual = cq.Value.IsEqual;
        }

        // Updating possible expression list
        for (int i = Expressions.Count - 1; i >= 0; i--)
        {
            string expression = Expressions[i];
            bool ifBreak = false;

            // Checking if expression contains not correct characters
            for (int j = 0; j < 8; j++)
            {
                if (!PossibleChars[j].Contains(expression[j]))
                {
                    Expressions.RemoveAt(i);
                    ifBreak = true;
                    break;
                }
            }
            if (ifBreak)
                continue;

            // Removing expression from expression list
            foreach (var item in Quantities)
            {
                int charCount = expression.Where(x => x.Equals(item.Key)).Count();
                if (item.Value.IsEqual)
                {
                    if (charCount != item.Value.Quantity)
                    {
                        Expressions.RemoveAt(i);
                        break;
                    }
                }
                else
                {
                    if (charCount < item.Value.Quantity)
                    {
                        Expressions.RemoveAt(i);
                        break;
                    }
                }
            }
        }

    }
    /// <summary>
    /// Makes a guess according to current state of guessing
    /// </summary>
    /// <returns> New guess. When there is no possible guess ExpressionFound is set to null. </returns>
    public ComputerGameResult MakeGuess()
    {
        // Checking if there is any possible guess
        if (Expressions.Count == 0)
            return new ComputerGameResult(false, "I believe there is no correct answer");
        
        List<string> guessExpressions = new List<string>(Expressions);
        List<int> rowsToSkip = new List<int>();
        char[] guess = new char[Constants.ExpressionLength];

        // Choosing every char of new guess
        for (int i = 0; i < Constants.ExpressionLength; i++)
        {
            // Checking if only one expression is left
            if (guessExpressions.Count == 1)
            {
                LastGuess = guessExpressions[0];
                return new ComputerGameResult(true, LastGuess);
            }

            int[,] charCount = new int[Constants.ExpressionLength, Constants.AvailableChars.Length];

            // Count occurences of chars
            foreach (string expression in guessExpressions)
            {
                for (int j = 0; j < Constants.ExpressionLength; j++)
                {
                    charCount[j, Constants.AvailableChars.IndexOf(expression[j])]++;
                }
            }

            // Deciding which char will be in guess
            int closestChar = -1, closestColumn = -1;
            double closestValue = double.MaxValue;
            for (int j = 0; j < Constants.ExpressionLength; j++)
            {
                // Char already added on this position
                if (rowsToSkip.Contains(j))
                    continue;

                // Serching for char with occurence near 50%
                for (int k = 0; k < Constants.AvailableChars.Length; k++)
                {
                    double currentValue = Math.Abs(0.5 - (double)charCount[j, k] / (double)guessExpressions.Count);
                    if (currentValue < closestValue)
                    {
                        closestValue = currentValue;
                        closestColumn = j;
                        closestChar = k;
                    }
                }
            }

            // Adding new char
            guess[closestColumn] = Constants.AvailableChars[closestChar];
            rowsToSkip.Add(closestColumn);

            // Delete wrong expressions
            char c = Constants.AvailableChars[closestChar];
            for (int j = guessExpressions.Count - 1; j >= 0; j--)
            {
                if (guessExpressions[j][closestColumn] != c)
                    guessExpressions.RemoveAt(j);
            }
        }

        LastGuess = new string(guess);
        return new ComputerGameResult(true, LastGuess);
    }
}