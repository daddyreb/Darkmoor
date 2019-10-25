﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Darkmoor
{
    public partial class Form1 : Form
    {
        DataSaver _gameData;

        public Form1()
        {
            InitializeComponent();

            _gameData = new DataSaver();
            _gameData.ProgramData = new HexDataIndex();
            _gameData.ProgramData.GenerateWorld(3, 3);
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void bu_newYear_Click(object sender, EventArgs e)
        {
            ++_gameData.ProgramData.TimeObj.Year;
            _gameData.ProgramData.TimeObj.Month = 1;
            _gameData.ProgramData.TimeObj.Day = 1;
            _gameData.ProgramData.IncreaseAllPopulations();
            _gameData.ProgramData.ResolveAllMigrations();

        }

        private void bu_save_Click(object sender, EventArgs e)
        {
            _gameData.Save();

        }

        private void bu_load_Click(object sender, EventArgs e)
        {
            _gameData.Load();
        }
    }
}
