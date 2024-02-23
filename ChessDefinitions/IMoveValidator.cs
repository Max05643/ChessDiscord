namespace ChessDefinitions
{
    /// <summary>
    /// Provdies a way to validate moves inputted by players
    /// </summary>
    public interface IMoveValidator
    {
        bool Validate(string move);
    }
}
