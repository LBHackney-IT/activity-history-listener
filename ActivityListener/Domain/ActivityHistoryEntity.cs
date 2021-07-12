using System;
using System.Collections.Generic;

namespace ActivityListener.Domain
{
    public class ActivityHistoryEntity
    {
        public Guid Id { get; set; }

        public ActivityType Type { get; set; }

        public TargetType TargetType { get; set; }

        public Guid TargetId { get; set; }

        public DateTime CreatedAt { get; set; }

        public int TimetoLiveForRecord { get; set; }

        public /*Dictionary<string, object>*/ object OldData { get; set; }

        public /*Dictionary<string, object>*/ object NewData { get; set; }

        public AuthorDetails AuthorDetails { get; set; }
    }
}
