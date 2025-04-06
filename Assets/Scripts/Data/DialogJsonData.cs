using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class DialogJsonData
    {
        public Dictionary<string, Dialogue> Intros;
        //TODO: refactor into own classes
        public Dictionary<NpcPersonalities, Dictionary<NpcAwareness, List<Dialogue>> > Quests;
        public Dictionary<NpcPersonalities, Dictionary<NpcAwareness, List<Dialogue>> > InProgress;
        public Dictionary<NpcPersonalities, Dictionary<NpcAwareness, List<Dialogue>> > Complete;
        public Dictionary<string, List<Dialogue>> Finished;
    }


}