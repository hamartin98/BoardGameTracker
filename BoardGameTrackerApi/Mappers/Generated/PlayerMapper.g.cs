using BoardGameTracker.Api.ViewModels;
using BoardGameTracker.Mappers;
using BoardGameTracker.Models.Entities;
using Mapster;

namespace BoardGameTracker.Mappers
{
    public partial class PlayerMapper : IPlayerMapper
    {
        public Player MapToEntity(PlayerCreateVM p1)
        {
            return p1 == null ? null : new Player() {Name = p1.Name};
        }
        public Player MapToEntity(PlayerUpdateVM p2)
        {
            return p2 == null ? null : new Player() {Name = p2.Name};
        }
        public PlayerResponseVM MapToVM(Player p3)
        {
            return p3 == null ? null : new PlayerResponseVM()
            {
                MetaData = p3.Adapt<ResponseMetaDataVM>(),
                Name = p3.Name
            };
        }
    }
}