namespace BoardGameTracker.ViewModels.Filters;

public class PlayerScoreFilterVM : AbstractFilterVM
{
    /// <summary>
    /// Only scores of this player
    /// </summary>
    public Guid? PlayerId { get; set; }

    /// <summary>
    /// Only scores of this play session
    /// </summary>
    public Guid? SessionId { get; set; }

    /// <summary>
    /// Only winning (true) or losing (false) scores
    /// </summary>
    public bool? Win { get; set; }
}
