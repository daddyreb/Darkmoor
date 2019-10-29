﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkmoor
{
    /// <summary>
    /// A collection of Hexes
    /// </summary>
    class HexDataIndex
    {
        private readonly Dice _dice;


        public GameTime TimeObj;
        public List<HexData> HexList = new List<HexData>();
        public int WorldWidth;
        public int WorldHeight;

        public HexDataIndex()
        {
            _dice = Dice.Instance;
            TimeObj = GameTime.Instance;
            TimeObj.Init(_dice);
        }

        /// <summary>
        /// After loading, need to set all the Home Hexes in every hex
        /// </summary>
        public void InitChildren()
        {
            foreach (var hex in HexList)
            {
                hex.InitChildren();
            }
        }

        public void ClearAllData()
        {
            TimeObj.Reset();
            HexList.Clear();
        }

        public void GenerateWorld(int width, int height)
        {
            WorldWidth = width;
            WorldHeight = height;

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    HexList.Add(CreateRandomHex(x, y));
                }
            }
        }

        public void ResizeWorld(int width, int height)
        {
            if (width < WorldWidth)
            {
                HexList.RemoveAll(hex => hex.XCoord > width);
                WorldWidth = width;
            }
            if (height < WorldHeight)
            {
                HexList.RemoveAll(hex => hex.YCoord > height);
                WorldHeight = height;
            }
            if (width > WorldWidth)
            {
                for (int x = WorldWidth; x < width; ++x)
                {
                    for (int y = 0; y < WorldHeight; ++y)
                    {
                        HexList.Add(CreateRandomHex(x, y));
                    }
                }
                WorldWidth = width;
            }
            if (height > WorldHeight)
            {
                for (int x = 0; x < WorldWidth; ++x)
                {
                    for (int y = WorldHeight; y < height; ++y)
                    {
                        HexList.Add(CreateRandomHex(x, y));
                    }
                }
                WorldHeight = height;
            }
        }

        public HexData CreateRandomHex(int x, int y)
        {
            HexData hex = new HexData(this);
            hex.InitializeAsRandomHex(x, y);
            return hex;
        }

        public HexData GetHexByCoordinates(int x, int y)
        {
            foreach(var hex in HexList)
            {
                if (hex.XCoord == x && hex.YCoord == y) { return hex; }
            }
            return null;
        }

        public HexData GetHexByCoordinates(Tuple<int, int> coords)
        {
            return GetHexByCoordinates(coords.Item1, coords.Item2);
        }

        /// <summary>
        /// after battles, we remove the civilizations that have no members
        /// left.
        /// </summary>
        public void CleanOutRuins()
        {
            foreach (var hex in HexList)
            {
                foreach (var lair in hex.LairList)
                {
                    if (lair.IsRuins()) { continue; }
                    lair.AbandonIfEmpty();
                }

            }
        }

        /// <summary>
        /// Determines the number of edges of the hex that fall outside the 
        /// existing map.
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public int NumberOutsideEdges(HexData hex)
        {
            int numOuterEdges = 0;
            for (int i = 0; i < 6; ++i)
            {
                int index = i + 1;
                var neighborCoords = hex.FindNeighborByIndex(index);
                var neighbor = GetHexByCoordinates(neighborCoords.Item1, 
                    neighborCoords.Item2);
                if (neighbor == null) { ++numOuterEdges; }
            }
            return numOuterEdges;
        }

        public void ResolveAllMigrations()
        {
            var migration = new Migration(this);

            // outside invaders
            foreach (var hex in HexList)
            {
                migration.ResolveOutsideSingleHexMigration(hex);
            }

            // internal migrations
            // todo: these should be resolved via date initiative settlement
            migration.ClearMigrationSchedule();
            foreach (var hex in HexList)
            {
                migration.QueueMigrations(hex);
            }
            migration.ResolveQueuedMigrations();
            
        }

        public void IncreaseAllPopulations()
        {
            foreach (var hex in HexList)
            {
                foreach(var lair in hex.LairList)
                {
                    if (lair.IsRuins()) { continue; }
                    lair.HomeCiv.IncreasePopulation();
                }
            }
        }
    }
}
