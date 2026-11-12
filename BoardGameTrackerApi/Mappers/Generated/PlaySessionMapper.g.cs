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
    public partial class PlaySessionMapper : IPlaySessionMapper
    {
        public PlaySession MapToEntity(PlaySessionCreateVM p1)
        {
            return p1 == null ? null : new PlaySession()
            {
                BoardGameId = p1.BoardGameId,
                UsedExpansionIds = funcMain1(p1.UsedExpansionIds),
                StartTime = p1.StartTime == null ? null : (DateTime?)(DateTime)p1.StartTime,
                Duration = p1.Duration == null ? null : (TimeSpan?)(TimeSpan)p1.Duration,
                PlayerScoreIds = funcMain2(p1.PlayerScoreIds),
                SessionType = (PlaySessionType)p1.SessionType,
                Notes = p1.Notes
            };
        }
        public PlaySession MapToEntity(PlaySessionUpdateVM p4)
        {
            return p4 == null ? null : new PlaySession()
            {
                BoardGameId = p4.BoardGameId,
                UsedExpansionIds = funcMain3(p4.UsedExpansionIds),
                StartTime = p4.StartTime == null ? null : (DateTime?)(DateTime)p4.StartTime,
                Duration = p4.Duration == null ? null : (TimeSpan?)(TimeSpan)p4.Duration,
                PlayerScoreIds = funcMain4(p4.PlayerScoreIds),
                SessionType = (PlaySessionType)p4.SessionType,
                Notes = p4.Notes
            };
        }
        public PlaySessionResponseVM MapToVM(PlaySession p7)
        {
            return p7 == null ? null : new PlaySessionResponseVM()
            {
                MetaData = p7.Adapt<ResponseMetaDataVM>(),
                BoardGameId = p7.BoardGameId,
                UsedExpansionIds = funcMain5(p7.UsedExpansionIds),
                StartTime = p7.StartTime == null ? null : (DateTime?)(DateTime)p7.StartTime,
                Duration = p7.Duration == null ? null : (TimeSpan?)(TimeSpan)p7.Duration,
                PlayerScoreIds = funcMain6(p7.PlayerScoreIds),
                SessionType = (PlaySessionTypeVM)p7.SessionType,
                Notes = p7.Notes
            };
        }
        
        private ICollection<Guid> funcMain1(ICollection<Guid> p2)
        {
            if (p2 == null)
            {
                return default(ICollection<Guid>);
            }
            
            if (p2 == null)
            {
                return null;
            }
            ICollection<Guid> result = new List<Guid>(p2.Count);
            
            IEnumerator<Guid> enumerator = p2.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Guid item = enumerator.Current;
                result.Add(item);
            }
            return result;
            
        }
        
        private ICollection<Guid> funcMain2(ICollection<Guid> p3)
        {
            if (p3 == null)
            {
                return default(ICollection<Guid>);
            }
            
            if (p3 == null)
            {
                return null;
            }
            ICollection<Guid> result = new List<Guid>(p3.Count);
            
            IEnumerator<Guid> enumerator = p3.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Guid item = enumerator.Current;
                result.Add(item);
            }
            return result;
            
        }
        
        private ICollection<Guid> funcMain3(ICollection<Guid> p5)
        {
            if (p5 == null)
            {
                return default(ICollection<Guid>);
            }
            
            if (p5 == null)
            {
                return null;
            }
            ICollection<Guid> result = new List<Guid>(p5.Count);
            
            IEnumerator<Guid> enumerator = p5.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Guid item = enumerator.Current;
                result.Add(item);
            }
            return result;
            
        }
        
        private ICollection<Guid> funcMain4(ICollection<Guid> p6)
        {
            if (p6 == null)
            {
                return default(ICollection<Guid>);
            }
            
            if (p6 == null)
            {
                return null;
            }
            ICollection<Guid> result = new List<Guid>(p6.Count);
            
            IEnumerator<Guid> enumerator = p6.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Guid item = enumerator.Current;
                result.Add(item);
            }
            return result;
            
        }
        
        private ICollection<Guid> funcMain5(ICollection<Guid> p8)
        {
            if (p8 == null)
            {
                return default(ICollection<Guid>);
            }
            
            if (p8 == null)
            {
                return null;
            }
            ICollection<Guid> result = new List<Guid>(p8.Count);
            
            IEnumerator<Guid> enumerator = p8.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Guid item = enumerator.Current;
                result.Add(item);
            }
            return result;
            
        }
        
        private ICollection<Guid> funcMain6(ICollection<Guid> p9)
        {
            if (p9 == null)
            {
                return default(ICollection<Guid>);
            }
            
            if (p9 == null)
            {
                return null;
            }
            ICollection<Guid> result = new List<Guid>(p9.Count);
            
            IEnumerator<Guid> enumerator = p9.GetEnumerator();
            
            while (enumerator.MoveNext())
            {
                Guid item = enumerator.Current;
                result.Add(item);
            }
            return result;
            
        }
    }
}