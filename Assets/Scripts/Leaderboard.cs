﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Leaderboard
{
    public const int EntryCount = 5;

    public struct ScoreEntry
    {
        public int score;

        public ScoreEntry(int score)
        {
            this.score = score;
        }
    }

    private static List<ScoreEntry> s_Entries;

    private static List<ScoreEntry> Entries
    {
        get
        {
            if (s_Entries == null)
            {
                s_Entries = new List<ScoreEntry>();
                LoadScores();
            }
            return s_Entries;
        }
    }

    private const string PlayerPrefsBaseKey = "leaderboard";

    private static void SortScores()
    {
        s_Entries.Sort((a, b) => b.score.CompareTo(a.score));
    }

    private static void LoadScores()
    {
        s_Entries.Clear();

        for (int i = 0; i < EntryCount; ++i)
        {
            ScoreEntry entry;
            entry.score = PlayerPrefs.GetInt(PlayerPrefsBaseKey + "[" + i + "].score", 0);
            s_Entries.Add(entry);
        }

        SortScores();
    }

    private static void SaveScores()
    {
        for (int i = 0; i < EntryCount; ++i)
        {
            var entry = s_Entries[i];
            PlayerPrefs.SetInt(PlayerPrefsBaseKey + "[" + i + "].score", entry.score);
        }
    }

    public static ScoreEntry GetEntry(int index)
    {
        return Entries[index];
    }

    public static void Record(int score)
    {
        Entries.Add(new ScoreEntry(score));
        SortScores();
        Entries.RemoveAt(Entries.Count - 1);
        SaveScores();
    }


    public static bool LowestScore(int currentScore)
    {
        //as the list is already sorted
        if (currentScore < Entries[EntryCount - 1].score)
            return true;
        else
            return false;
    }
    public static bool HighestScore(int currentScore)
    {
        //as the list is already sorted
        if (currentScore > Entries[0].score)
            return true;
        else
            return false;
    }
    public static void Clear()
    {
        for (int i = 0; i < EntryCount; ++i)
            s_Entries[i] = new ScoreEntry(0);
        SaveScores();
    }

}