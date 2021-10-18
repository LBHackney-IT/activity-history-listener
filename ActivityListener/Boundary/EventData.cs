using System.Collections.Generic;

namespace ActivityListener.Boundary
{
    public class EventData
    {
        public Dictionary<string, object> OldData { get; set; }

        public Dictionary<string, object> NewData { get; set; }
    }
}
