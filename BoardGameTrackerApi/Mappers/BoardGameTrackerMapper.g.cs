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
    public partial class BoardGameTrackerMapper : IBoardGameTrackerMapper
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
                MetaData = p3 == null ? null : new ResponseMetaDataVM()
                {
                    Id = p3.Id,
                    CreatedAt = p3.CreatedAt,
                    UpdatedAt = p3.UpdatedAt,
                    Version = p3.Version,
                    IsDeleted = p3.IsDeleted
                },
                ExpansionOf = TypeAdapter<BoardGame, BoardGameResponseVM>.Map.Invoke(p3.ExpansionOf),
                Name = p3.Name,
                MinPlayTime = p3.MinPlayTime,
                MaxPlayTime = p3.MaxPlayTime,
                MinPlayerCount = p3.MinPlayerCount,
                MaxPlayerCount = p3.MaxPlayerCount,
                BggUrl = p3.BggUrl,
                Type = (BoardGameTypeVM)p3.Type,
                Categories = funcMain1(BoardGameMapperConfig.MapBoardGameCategories(p3.Categories)),
                ExpansionOfId = p3.ExpansionOfId == null ? null : (Guid?)(Guid)p3.ExpansionOfId
            };
        }
        public Player MapToEntity(PlayerCreateVM p5)
        {
            return p5 == null ? null : new Player() {Name = p5.Name};
        }
        public Player MapToEntity(PlayerUpdateVM p6)
        {
            return p6 == null ? null : new Player() {Name = p6.Name};
        }
        public PlayerResponseVM MapToVM(Player p7)
        {
            return p7 == null ? null : new PlayerResponseVM()
            {
                MetaData = funcMain2(p7.Adapt<ResponseMetaDataVM>()),
                Name = p7.Name
            };
        }
        public PlayerScore MapToEntity(PlayerScoreCreateVM p9)
        {
            return p9 == null ? null : new PlayerScore()
            {
                PlayerId = p9.PlayerId,
                SessionId = p9.SessionId,
                Score = p9.Score == null ? null : (int?)(int)p9.Score,
                Win = p9.Win,
                Notes = p9.Notes
            };
        }
        public PlayerScore MapToEntity(PlayerScoreUpdateVM p10)
        {
            return p10 == null ? null : new PlayerScore()
            {
                PlayerId = p10.PlayerId,
                SessionId = p10.SessionId,
                Score = p10.Score == null ? null : (int?)(int)p10.Score,
                Win = p10.Win,
                Notes = p10.Notes
            };
        }
        public PlayerScoreResponseVM MapToVM(PlayerScore p11)
        {
            return p11 == null ? null : new PlayerScoreResponseVM()
            {
                MetaData = funcMain3(p11.Adapt<ResponseMetaDataVM>()),
                PlayerId = p11.PlayerId,
                SessionId = p11.SessionId,
                Score = p11.Score == null ? null : (int?)(int)p11.Score,
                Win = p11.Win,
                Notes = p11.Notes
            };
        }
        public PlaySession MapToEntity(PlaySessionCreateVM p13)
        {
            return p13 == null ? null : new PlaySession()
            {
                BoardGameId = p13.BoardGameId,
                UsedExpansionIds = funcMain4(p13.UsedExpansionIds),
                StartTime = p13.StartTime == null ? null : (DateTime?)(DateTime)p13.StartTime,
                Duration = p13.Duration == null ? null : (TimeSpan?)(TimeSpan)p13.Duration,
                PlayerScoreIds = funcMain5(p13.PlayerScoreIds),
                SessionType = (PlaySessionType)p13.SessionType,
                Notes = p13.Notes
            };
        }
        public PlaySession MapToEntity(PlaySessionUpdateVM p16)
        {
            return p16 == null ? null : new PlaySession()
            {
                BoardGameId = p16.BoardGameId,
                UsedExpansionIds = funcMain6(p16.UsedExpansionIds),
                StartTime = p16.StartTime == null ? null : (DateTime?)(DateTime)p16.StartTime,
                Duration = p16.Duration == null ? null : (TimeSpan?)(TimeSpan)p16.Duration,
                PlayerScoreIds = funcMain7(p16.PlayerScoreIds),
                SessionType = (PlaySessionType)p16.SessionType,
                Notes = p16.Notes
            };
        }
        public PlaySessionResponseVM MapToVM(PlaySession p19)
        {
            return p19 == null ? null : new PlaySessionResponseVM()
            {
                MetaData = funcMain8(p19.Adapt<ResponseMetaDataVM>()),
                BoardGameId = p19.BoardGameId,
                UsedExpansionIds = funcMain9(p19.UsedExpansionIds),
                StartTime = p19.StartTime == null ? null : (DateTime?)(DateTime)p19.StartTime,
                Duration = p19.Duration == null ? null : (TimeSpan?)(TimeSpan)p19.Duration,
                PlayerScoreIds = funcMain10(p19.PlayerScoreIds),
                SessionType = (PlaySessionTypeVM)p19.SessionType,
                Notes = p19.Notes
            };
        }
        
        private List<BoardGameCategoryVM> funcMain1(List<BoardGameCategoryVM> p4)
        {
            if (p4 == null)
            {
                return null;
            }
            List<BoardGameCategoryVM> result = new List<BoardGameCategoryVM>(p4.Count);
            
            int i = 0;
            int len = p4.Count;
            
            while (i < len)
            {
                BoardGameCategoryVM item = p4[i];
                result.Add(item);
                i++;
            }
            return result;
            
        }
        
        private ResponseMetaDataVM funcMain2(ResponseMetaDataVM p8)
        {
            return p8 == null ? null : new ResponseMetaDataVM()
            {
                Id = p8.Id,
                CreatedAt = p8.CreatedAt,
                UpdatedAt = p8.UpdatedAt,
                Version = p8.Version,
                IsDeleted = p8.IsDeleted
            };
        }
        
        private ResponseMetaDataVM funcMain3(ResponseMetaDataVM p12)
        {
            return p12 == null ? null : new ResponseMetaDataVM()
            {
                Id = p12.Id,
                CreatedAt = p12.CreatedAt,
                UpdatedAt = p12.UpdatedAt,
                Version = p12.Version,
                IsDeleted = p12.IsDeleted
            };
        }
        
        private ICollection<Guid> funcMain4(ICollection<Guid> p14)
        {
            if (p14 == null)
            {
                return default(ICollection<Guid>);
            }
            
            if (p14 == null)
            {
                return null;
            }
            ICollection<Guid> result = new List<Guid>(p14.Count);
            
            IEnumerator<Guid> enumerator = p14.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Guid item = enumerator.Current;
                result.Add(item);
            }
            return result;
            
        }
        
        private ICollection<Guid> funcMain5(ICollection<Guid> p15)
        {
            if (p15 == null)
            {
                return default(ICollection<Guid>);
            }
            
            if (p15 == null)
            {
                return null;
            }
            ICollection<Guid> result = new List<Guid>(p15.Count);
            
            IEnumerator<Guid> enumerator = p15.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Guid item = enumerator.Current;
                result.Add(item);
            }
            return result;
            
        }
        
        private ICollection<Guid> funcMain6(ICollection<Guid> p17)
        {
            if (p17 == null)
            {
                return default(ICollection<Guid>);
            }
            
            if (p17 == null)
            {
                return null;
            }
            ICollection<Guid> result = new List<Guid>(p17.Count);
            
            IEnumerator<Guid> enumerator = p17.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Guid item = enumerator.Current;
                result.Add(item);
            }
            return result;
            
        }
        
        private ICollection<Guid> funcMain7(ICollection<Guid> p18)
        {
            if (p18 == null)
            {
                return default(ICollection<Guid>);
            }
            
            if (p18 == null)
            {
                return null;
            }
            ICollection<Guid> result = new List<Guid>(p18.Count);
            
            IEnumerator<Guid> enumerator = p18.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Guid item = enumerator.Current;
                result.Add(item);
            }
            return result;
            
        }
        
        private ResponseMetaDataVM funcMain8(ResponseMetaDataVM p20)
        {
            return p20 == null ? null : new ResponseMetaDataVM()
            {
                Id = p20.Id,
                CreatedAt = p20.CreatedAt,
                UpdatedAt = p20.UpdatedAt,
                Version = p20.Version,
                IsDeleted = p20.IsDeleted
            };
        }
        
        private ICollection<Guid> funcMain9(ICollection<Guid> p21)
        {
            if (p21 == null)
            {
                return default(ICollection<Guid>);
            }
            
            if (p21 == null)
            {
                return null;
            }
            ICollection<Guid> result = new List<Guid>(p21.Count);
            
            IEnumerator<Guid> enumerator = p21.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Guid item = enumerator.Current;
                result.Add(item);
            }
            return result;
            
        }
        
        private ICollection<Guid> funcMain10(ICollection<Guid> p22)
        {
            if (p22 == null)
            {
                return default(ICollection<Guid>);
            }
            
            if (p22 == null)
            {
                return null;
            }
            ICollection<Guid> result = new List<Guid>(p22.Count);
            
            IEnumerator<Guid> enumerator = p22.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Guid item = enumerator.Current;
                result.Add(item);
            }
            return result;
            
        }
    }
}