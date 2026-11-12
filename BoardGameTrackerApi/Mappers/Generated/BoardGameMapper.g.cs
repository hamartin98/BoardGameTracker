using System;
using System.Collections.Generic;
using BoardGameTracker.Api.ViewModels;
using BoardGameTracker.Api.ViewModels.Enums;
using BoardGameTracker.Mappers;
using BoardGameTracker.Models.Entities;
using BoardGameTracker.Models.Enums;
using Mapster;

namespace BoardGameTracker.Mappers
{
    public partial class BoardGameMapper : IBoardGameMapper
    {
        public BoardGame MapToEntity(BoardGameCreateVM p1)
        {
            return p1 == null ? null : new BoardGame()
            {
                Name = p1.Name,
                MinPlayTime = p1.MinPlayTime,
                MaxPlayTime = p1.MaxPlayTime,
                MinPlayerCount = p1.MinPlayerCount,
                MaxPlayerCount = p1.MaxPlayerCount,
                BggUrl = p1.BggUrl,
                Type = (BoardGameType)p1.Type,
                Categories = BoardGameMapperConfig.MapBoardGameCategory(p1.Categories),
                ExpansionOfId = p1.ExpansionOfId == null ? null : (Guid?)(Guid)p1.ExpansionOfId
            };
        }
        public BoardGame MapToEntity(BoardGameUpdateVM p2)
        {
            return p2 == null ? null : new BoardGame()
            {
                Name = p2.Name,
                MinPlayTime = p2.MinPlayTime,
                MaxPlayTime = p2.MaxPlayTime,
                MinPlayerCount = p2.MinPlayerCount,
                MaxPlayerCount = p2.MaxPlayerCount,
                BggUrl = p2.BggUrl,
                Type = (BoardGameType)p2.Type,
                Categories = BoardGameMapperConfig.MapBoardGameCategory(((BoardGameCreateVM)p2).Categories),
                ExpansionOfId = p2.ExpansionOfId == null ? null : (Guid?)(Guid)p2.ExpansionOfId
            };
        }
        public BoardGameResponseVM MapToVM(BoardGame p3)
        {
            return p3 == null ? null : new BoardGameResponseVM()
            {
                MetaData = p3.Adapt<ResponseMetaDataVM>(),
                ExpansionOf = TypeAdapter<BoardGame, BoardGameResponseVM>.Map.Invoke(p3.ExpansionOf),
                Expansions = funcMain1(p3.Expansions),
                Name = p3.Name,
                MinPlayTime = p3.MinPlayTime,
                MaxPlayTime = p3.MaxPlayTime,
                MinPlayerCount = p3.MinPlayerCount,
                MaxPlayerCount = p3.MaxPlayerCount,
                BggUrl = p3.BggUrl,
                Type = (BoardGameTypeVM)p3.Type,
                Categories = new List<BoardGameCategoryVM>() {},
                ExpansionOfId = p3.ExpansionOfId == null ? null : (Guid?)(Guid)p3.ExpansionOfId
            };
        }
        
        private List<BoardGameResponseVM> funcMain1(ICollection<BoardGame> p4)
        {
            if (p4 == null)
            {
                return null;
            }
            List<BoardGameResponseVM> result = new List<BoardGameResponseVM>(p4.Count);
            
            IEnumerator<BoardGame> enumerator = p4.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                BoardGame item = enumerator.Current;
                result.Add(TypeAdapter<BoardGame, BoardGameResponseVM>.Map.Invoke(item));
            }
            return result;
            
        }
    }
}