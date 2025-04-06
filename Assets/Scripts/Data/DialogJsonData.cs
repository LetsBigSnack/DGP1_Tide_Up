using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class DialogJsonData
    {
        public Dictionary<string, Dialogue> Intros;
        //TODO Refactor
        public Dictionary<NpcPersonalities, Dictionary<NpcAwareness, List<Dialogue>> > Quests;
        public Dictionary<NpcPersonalities, Dictionary<NpcAwareness, List<Dialogue>> > InProgress;
        public Dictionary<NpcPersonalities, Dictionary<NpcAwareness, List<Dialogue>> > Complete;
    }


}