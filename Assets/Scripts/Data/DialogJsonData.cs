using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class DialogJsonData
    {
        public Dictionary<string, Dialogue> Intros;
        public Dictionary<NpcPersonalities, Dictionary<NpcAwareness, List<Dialogue>> > Quests;
        public Dictionary<NpcPersonalities, Dictionary<NpcAwareness, List<Dialogue>> > InProgress;
        public Dictionary<NpcPersonalities, Dictionary<NpcAwareness, List<Dialogue>> > Complete;
        public Dictionary<string, List<Dialogue>> Finished;
    }


}