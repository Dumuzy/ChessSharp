﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using AwiUtils;

namespace ChessUI
{
    public partial class Form1 : Form
    {
        class PeckerIniFile
        {
            internal PeckerIniFile(Form1 form)
            {
                this.form = form;
                dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                ini = new IniFile(dir, filename);
            }

            public void Write()
            {
                if (!isReading)
                {
                    ini.WriteValue(Section, "PuzzleSet", form._currPuzzleSetName);
                    ini.WriteValue(Section, "Language", form.cbLanguage.SelectedItem.ToString());
                    WriteNumNext();
                    ini.Flush();
                }
            }

            public void WriteNumNext()
            {
                if (!isReading)
                {
                    var nnc = form.numClicks; // needed because of CS1690
                    ini.WriteValue(Section, "Next", nnc.ToString());
                }
            }

            public void Read()
            {
                isReading = true;
                form.cbPuzzleSets.SelectedItem = ini.ReadValue(Section, "PuzzleSet", "");
                form.cbLanguage.SelectedItem = ini.ReadValue(Section, "Language", "EN");
                form.numClicks = Helper.ToInt(ini.ReadValue(Section, "Next", "0"));
                form.iniDonated = ini.ReadValue(Section, "Donated", "Lasker");
                isReading = false;
            }

            bool isReading;
            readonly string dir;
            const string filename = "ChessPuzzlePecker.ini", Section = "A";
            readonly Form1 form;
            readonly IniFile ini;
        }
    }
}
