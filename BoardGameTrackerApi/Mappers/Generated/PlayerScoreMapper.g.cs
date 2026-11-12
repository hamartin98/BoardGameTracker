using BoardGameTracker.Api.ViewModels;
using BoardGameTracker.Mappers;
using BoardGameTracker.Models.Entities;
using Mapster;

namespace BoardGameTracker.Mappers
{
    public partial class PlayerScoreMapper : IPlayerScoreMapper
    {
        public PlayerScore MapToEntity(PlayerScoreCreateVM p1)
        {
            return p1 == null ? null : new PlayerScore()
            {
                PlayerId = p1.PlayerId,
                SessionId = p1.SessionId,
                Score = p1.Score == null ? null : (int?)(int)p1.Score,
                Win = p1.Win,
                Notes = p1.Notes
            };
        }
        public PlayerScore MapToEntity(PlayerScoreUpdateVM p2)
        {
            return p2 == null ? null : new PlayerScore()
            {
                PlayerId = p2.PlayerId,
                SessionId = p2.SessionId,
                Score = p2.Score == null ? null : (int?)(int)p2.Score,
                Win = p2.Win,
                Notes = p2.Notes
            };
        }
        public PlayerScoreResponseVM MapToVM(PlayerScore p3)
        {
            return p3 == null ? null : new PlayerScoreResponseVM()
            {
                MetaData = p3.Adapt<ResponseMetaDataVM>(),
                PlayerId = p3.PlayerId,
                SessionId = p3.SessionId,
                Score = p3.Score == null ? null : (int?)(int)p3.Score,
                Win = p3.Win,
                Notes = p3.Notes
            };
        }
    }
}