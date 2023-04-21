public interface IUniqueComputerExpressionGuessing
{
    Dictionary<int, ComputerExpressionGuessing> Users { get; set; }
    ComputerGameResult MakeGuess(int id);
    void StartGame(int id);
}

public class UniqueComputerExpressionGuessing : IUniqueComputerExpressionGuessing
{
    public Dictionary<int, ComputerExpressionGuessing> Users { get; set; }
    public UniqueComputerExpressionGuessing()
    {
        Users = new Dictionary<int, ComputerExpressionGuessing>();
    }
    /// <summary>
    /// Starts game
    /// </summary>
    /// <param name="id">User id</param>
    public void StartGame(int id)
    {
        // Checking if user exists
        if (!Users.ContainsKey(id))
            Users.Add(id, new ComputerExpressionGuessing());

        // Starting new game for user
        Users[id].StartGame();
    }
    /// <summary>
    /// Tries to guess correct expression
    /// </summary>
    /// <param name="id">User id</param>
    /// <returns>Computer guess</returns>
    public ComputerGameResult MakeGuess(int id)
    {
        return Users[id].MakeGuess();
    }

}