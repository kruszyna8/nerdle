public interface IUniqueUserExpressionGuessing
{
    Dictionary<int, UserExpressionGuessing> Users { get; set; }
    string MakeResponse(string guess, int id);
    void StartGame(int id);
}

public class UniqueUserExpressionGuessing : IUniqueUserExpressionGuessing
{
    public Dictionary<int, UserExpressionGuessing> Users { get; set; }
    public UniqueUserExpressionGuessing()
    {
        Users = new Dictionary<int, UserExpressionGuessing>();
    }
    /// <summary>
    /// Starts game
    /// </summary>
    /// <param name="id">User id</param>
    public void StartGame(int id)
    {
        // Checking if user exists
        if(!Users.ContainsKey(id))
            Users.Add(id, new UserExpressionGuessing());
        
        // Starting new game for user
        Users[id].StartGame();
    }
    /// <summary>
    /// Says which characters in guess expression are correct, are not on good place and are not in expression
    /// </summary>
    /// <param name="guess">Expression provided by user</param>
    /// <param name="id">User id</param>
    /// <returns>Response string: g - character is on good place, v - character occurs in expression but is on wrong place, 
    /// b - character doesn't occur in expression</returns>
    public string MakeResponse(string guess, int id)
    {
        return Users[id].MakeResponse(guess);
    }
}