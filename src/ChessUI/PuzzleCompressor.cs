﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using AwiUtils;

namespace PuzzlePecker
{
    static internal class PuzzleCompressor
    {
        static public void CreateManyCompressedFiles(string inputFile, string outputFileBase, bool shallGzipFile)
        {
            foreach (var part in new int[] { 750, 300, 150, 75, 30, 15, 3})
                Compress(inputFile, outputFileBase, NumPuzzlesInLichessDB / part, shallGzipFile);

        }

        static public void Compress(string inputFile, string outputFileBase, int numPuzzles, bool shallGzipFile)
        {
            if (numPuzzles <= 0 || numPuzzles * 3 > NumPuzzlesInLichessDB)
                throw new Exception($"numPuzzles={numPuzzles} unsupprted");
            Dictionary<string, string> takenPuzzles = new Dictionary<string, string>();
            do
            {
                foreach (string puzzle in File.ReadLines(inputFile))
                {
                    var parts = puzzle.Split(',').ToLi();
                    if (!PuzzleSet.IsAllowedByPopularityAndNbPlays(parts))
                        continue;
                    // Take it by chance which puzzles are taken, assuming some of the puzzles are not
                    // allowed by Popularity and NbPlays.
                    if (rand.NextDouble() < numPuzzles * 20.0 / NumPuzzlesInLichessDB)
                        if (!takenPuzzles.TryGetValue(parts[0], out string dummy))
                        {
                            // RemoveUnneededParts
                            foreach (var kill in new int[] { 4, 5, 6, 8 })
                                parts[kill] = "";
                            var p = string.Join(',', parts);
                            takenPuzzles.Add(parts[0], p);
                            if (takenPuzzles.Count >= numPuzzles)
                                break;
                        }
                }
            } while (takenPuzzles.Count < numPuzzles);

            var lines = takenPuzzles.Values.ToLi();
            lines.Insert(0, $"# Part of {inputFile} consisting of {numPuzzles} puzzles.");
            var fileName = $"{outputFileBase}-{numPuzzles}.csv";
            File.WriteAllLines(fileName, lines);
            if (shallGzipFile)
                StringCompressor.CompressFile(fileName, true);
        }

        static public void DecompressAllCsvGzFiles(string fileBase)
        {
            var files = Directory.EnumerateFiles(".", fileBase + "*.csv.gz");
            foreach (var f in files)
                StringCompressor.DecompressFile(f, true);
        }


        static Random rand = new Random();
        const int NumPuzzlesInLichessDB = 1500 * 1000;
    }
}
