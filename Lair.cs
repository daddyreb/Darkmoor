﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkmoor
{
    /// <summary>
    /// The home of a civilization.
    /// Only one Civilization can live in a lair at a time.
    /// If another one comes to live here, they must defeat and drive out the
    /// existing occupants, assimilate them, or be assimilated in order to 
    /// stay.
    /// </summary>
    class Lair
    {
        const int SUB_HEXES = 13;

        public string Name = "Unnamed";
        public string Type = "Cave";
        public Civilization HomeCiv;
        public int SubhexIndex = 0;
        public HistoryLog History = new HistoryLog();

        private readonly Dice _dice;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dice"></param>
        public Lair(Dice dice)
        {
            _dice = dice;
        }

        /// <summary>
        /// Builds a random civ as founders, and creates the lair
        /// </summary>
        public void InitializeAsRandomLair()
        {
            // Name
            var nameGen = new RandomName(_dice);
            Name = nameGen.CreateWord();

            // Type
            var lairTypeList = new List<string>()
            {
                "Cave", "Dungeon", "Castle", "Keep", "Village", "Town", "City",
                "Hamlet"
            };
            int lairIndex = _dice.RandomNumber(0, lairTypeList.Count - 1);
            Type = lairTypeList[lairIndex];

            // Civ
            var civ = new Civilization(_dice);
            civ.InitializeAsRandomCiv();
            HomeCiv = civ;

            // Location in hex
            SubhexIndex = _dice.RandomNumber(1, SUB_HEXES);

            string record = Name + " " + Type + " has been founded by the " 
                + RandomName.Pluralize(HomeCiv.GetFullName()) + " in area "
                + SubhexIndex;
            History.addRecord(record);
        }

        /// <summary>
        /// Name and type make up the full name.
        /// </summary>
        /// <returns></returns>
        public string GetFullName()
        {
            string name = "";
            if (IsRuins())
            {
                name = "The Ruins of ";
            }
            name += Name + " " + Type;
            return name;
        }

        public bool IsRuins()
        {
            return HomeCiv is null;
        }

        public void AbandonIfEmpty()
        {
            if (IsRuins()) { return; }
            if (HomeCiv.Patricians.Members > 0) { return; } 
            HomeCiv = null;
            string record = GetFullName() + " has fallen into ruin.";
            History.addRecord(record);
        }

        public void ForceAbandon()
        {
            string record = GetFullName() + " has been abandoned by "
                + HomeCiv.GetFullName()
                + " and fallen into ruin.";
            History.addRecord(record);
            HomeCiv.History.addRecord(record, isLogged: false);
            HomeCiv = null;
        }

        public void MoveCivIn(Civilization civ)
        {
            HomeCiv = civ;
            string record = civ.GetFullName() + " have taken control of "
                + GetFullName();
            History.addRecord(record);
            civ.History.addRecord(record, isLogged: false);
        }
    }
}
