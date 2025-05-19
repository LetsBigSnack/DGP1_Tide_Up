using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class NpcData
    {
        [Header("Current State")]
        [SerializeField] private NpcStates npcState = NpcStates.Intro;

        [Header("Npc Attributes")]
        [SerializeField] private NpcPersonalities npcPersonality;
        [SerializeField] private string npcName;
        [SerializeField] private string title;
        [SerializeField] private string homeDetails;
        [SerializeField] private string vibe;
        [SerializeField] private string birthday;
        [SerializeField] private int birthDay;
        [SerializeField] private Seasons birthSeason;
        [SerializeField] private string mbti;
        [SerializeField] private string description;
        [SerializeField] private string favColour;
        [SerializeField] private Color favColourCode;
        [SerializeField] private string favFood;
        [SerializeField] private string favAnimal;
        [SerializeField] private string favThing;
        [SerializeField] private Sprite portrait;
        [SerializeField] private Sprite portraitFriendbook;

        [Header("Npc Awareness")]
        [SerializeField] private NpcAwareness npcAwareness = NpcAwareness.Low;
        [SerializeField] private int completedQuests = 0;
        [SerializeField] private int maxCompletedQuests = 6;

        [Header("Npc Island")]
        [SerializeField] private int islandID;

        private Dialogue _currentDialogue;
        private Dialogue _finishedDialogue;
        private Quest _currentQuest;

        public NpcStates NpcState
        {
            get => npcState;
            set => npcState = value;
        }

        public NpcPersonalities NpcPersonality
        {
            get => npcPersonality;
            set => npcPersonality = value;
        }
        public string NpcName
        {
            get => npcName;
            set => npcName = value;
        }

        public string Title
        {
            get => title;
            set => title = value;
        }

        public string HomeDetails
        {
            get => homeDetails;
            set => homeDetails = value;
        }

        public string Vibe
        {
            get => vibe;
            set => vibe = value;
        }

        public string Birthday
        {
            get => birthday;
            set => birthday = value;
        }
        public int BirthDay
        {
            get => birthDay;
            set => birthDay = value;
        }
        public Seasons BirthSeason
        {
            get => birthSeason;
            set => birthSeason = value;
        }

        public string Mbti
        {
            get => mbti;
            set => mbti = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public string FavColour
        {
            get => favColour;
            set => favColour = value;
        }
        public Color FavColourCode
        {
            get => favColourCode;
            set => favColourCode = value;
        }

        public string FavFood
        {
            get => favFood;
            set => favFood = value;
        }

        public string FavAnimal
        {
            get => favAnimal;
            set => favAnimal = value;
        }

        public string FavThing
        {
            get => favThing;
            set => favThing = value;
        }

        public Sprite Portrait
        {
            get => portrait;
            set => portrait = value;
        }
        public Sprite PortraitFriendbook
        {
            get => portraitFriendbook;
            set => portraitFriendbook = value;
        }

        public NpcAwareness NpcAwareness
        {
            get => npcAwareness;
            set => npcAwareness = value;
        }

        public int CompletedQuests
        {
            get => completedQuests;
            set => completedQuests = value;
        }

        public int MaxCompletedQuests
        {
            get => maxCompletedQuests;
            set => maxCompletedQuests = value;
        }

        public int IslandID
        {
            get => islandID;
            set => islandID = value;
        }

        public Dialogue CurrentDialogue
        {
            get => _currentDialogue;
            set => _currentDialogue = value;
        }

        public Dialogue FinishedDialogue
        {
            get => _finishedDialogue;
            set => _finishedDialogue = value;
        }

        public Quest CurrentQuest
        {
            get => _currentQuest;
            set => _currentQuest = value;
        }
    }
}
