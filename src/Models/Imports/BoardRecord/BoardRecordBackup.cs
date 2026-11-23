namespace BoardGameTracker.Models.Imports.BoardRecord;

/// <summary>
/// Backup file of the BoardRecord app; only the imported fields are listed
/// </summary>
public class BoardRecordBackup
{
    /// <summary>
    /// Time the backup was made
    /// </summary>
    public DateTimeOffset? BackupDate { get; set; }

    public List<BoardRecordPlayer> Players { get; set; } = [];

    public List<BoardRecordGame> Games { get; set; } = [];

    public List<BoardRecordPlay> Plays { get; set; } = [];

    /// <summary>
    /// Groups of games; a group of an expansion lists the expansion and its base games
    /// </summary>
    public List<BoardRecordObjectGroup> ObjectGroups { get; set; } = [];

    public List<BoardRecordCampaign> Campaigns { get; set; } = [];
}

public class BoardRecordPlayer
{
    public long Id { get; set; }

    public Guid Uuid { get; set; }

    public string Name { get; set; } = "";
}

public class BoardRecordGame
{
    public long Id { get; set; }

    public Guid Uuid { get; set; }

    public string Title { get; set; } = "";

    public int? BggId { get; set; }

    /// <summary>
    /// 2 for an expansion
    /// </summary>
    public int? GameType { get; set; }

    public int? MinPlayers { get; set; }

    public int? MaxPlayers { get; set; }

    /// <summary>
    /// Play time in seconds
    /// </summary>
    public int? PlayingTime { get; set; }

    public List<BoardRecordScoreSheet> ScoreSheets { get; set; } = [];
}

public class BoardRecordScoreSheet
{
    public long Id { get; set; }

    public bool HasPoints { get; set; } = true;

    public bool IsCoop { get; set; }

    public List<BoardRecordRound> Rounds { get; set; } = [];
}

/// <summary>
/// Scoring row of a score sheet
/// </summary>
public class BoardRecordRound
{
    public long Id { get; set; }

    public Guid Uuid { get; set; }

    public string Name { get; set; } = "";

    /// <summary>
    /// Kind of value; empty for points, 1 round win, 6 text, 8 global choice
    /// </summary>
    public int? RoundType { get; set; }

    public bool IgnoreScore { get; set; }

    public bool IsGlobal { get; set; }
}

public class BoardRecordPlay
{
    public long Id { get; set; }

    public Guid Uuid { get; set; }

    /// <summary>
    /// Local start time (yyyy-MM-dd HH:mm)
    /// </summary>
    public string? Date { get; set; }

    /// <summary>
    /// Local creation time (yyyy-MM-dd HH:mm)
    /// </summary>
    public string? CreateTime { get; set; }

    public long GameId { get; set; }

    public long ScoreSheetId { get; set; }

    /// <summary>
    /// Duration in seconds
    /// </summary>
    public int? Duration { get; set; }

    public bool CoopVictory { get; set; }

    public string? Comments { get; set; }

    public long? CampaignId { get; set; }

    public string? CampaignScenario { get; set; }

    public List<BoardRecordPlayerScore> PlayerScores { get; set; } = [];
}

public class BoardRecordPlayerScore
{
    public long PlayerId { get; set; }

    public decimal? TotalScore { get; set; }

    public bool Victory { get; set; }

    public bool StartPlayer { get; set; }

    public string? Team { get; set; }

    public List<BoardRecordRoundScore> RoundScores { get; set; } = [];
}

public class BoardRecordRoundScore
{
    public long RoundId { get; set; }

    /// <summary>
    /// Points, already multiplied by the round's modifier
    /// </summary>
    public decimal? Score { get; set; }

    public string? TextValue { get; set; }
}

public class BoardRecordObjectGroup
{
    public long GroupId { get; set; }

    public long ObjectId { get; set; }
}

public class BoardRecordCampaign
{
    public long Id { get; set; }

    public string Name { get; set; } = "";
}
