﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkmoor
{
    /// <summary>
    /// Populations are derived from Ancestry objexts.
    /// A Population is informative about an Ancestry. 
    /// The Ancestry is the blueprint of a Population.
    /// Civilizatons are made up of populations.
    /// </summary>
    class Population
    {
        public Ancestry BaseAncestry;
        public int Members;
        public int AcMod = 0;
        public int ToHitMod = 0;
        public int BonusAttacks = 0;
        public int HitDiceBonus = 0;
        public int MoraleBonus = 0;
        public string Rank = "Patrician";
        public HistoryLog History = new HistoryLog();

        readonly Dice _dice;

        public Population()
        {
            _dice = Dice.Instance;
        }

        public void InitializeAsRandomPop(int tier)
        {
            var ancestryGen = AncestryIndex.Instance;
            ancestryGen.LoadAllSources();
            BaseAncestry = ancestryGen.GetRandomAncestry(tier);
            Members = _dice.RandomNumber(BaseAncestry.MinAppearing, 
                BaseAncestry.MaxAppearing);
            if (Members < 1) { Members = 1; }
            string record = Members + " " 
                + RandomName.Pluralize(BaseAncestry.Name)
                + " have entered the world.";
            History.addRecord(record);
        }
    }
}
