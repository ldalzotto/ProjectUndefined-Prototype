using System;
using System.Collections.Generic;
using Persistence;

namespace Timelines
{
    public class ATimelinePersister<T> : AbstractGamePersister<T>
    {
        public const string TimelinePersisterFolderName = "Timelines";
        public const string TimelineFileExtension = ".tim";

        public ATimelinePersister(Type timelineType) : base(TimelinePersisterFolderName, TimelineFileExtension, timelineType.Name)
        {
        }
    }
}

namespace Persistence
{
    [Serializable]
    public struct ATimelinePersistedNodes<NODE_KEY>
    {
        public HashSet<NODE_KEY> Nodes;
        public static ATimelinePersistedNodes<NODE_KEY> Empty => new ATimelinePersistedNodes<NODE_KEY>() {Nodes = new HashSet<NODE_KEY>()};
    }
}