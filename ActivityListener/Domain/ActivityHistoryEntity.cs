using System;

namespace ActivityListener.Domain
{
    public class ActivityHistoryEntity
    {
        public Guid Id { get; set; }

        public ActivityType Type { get; set; }

        public string SourceDomain { get; set; }

        public TargetType TargetType { get; set; }

        public Guid TargetId { get; set; }

        public DateTime CreatedAt { get; set; }

        public int TimetoLiveForRecord { get; set; }

        public object OldData { get; set; }

        public object NewData { get; set; }

        public AuthorDetails AuthorDetails { get; set; }
    }
}
