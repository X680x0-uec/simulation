using UnityEngine;

// Global static counters for simple game statistics
public static class GameStats
{
    // Enemy kills
    public static int KilledEnemies = 0;

    // Ally deaths by type: 0 = Attackers, 1 = Defencers, 2 = Archers
    public static int[] AllyDeaths = new int[3] { 0, 0, 0 };

    public static void IncrementEnemyKill(int amount = 1)
    {
        KilledEnemies += amount;
    }

    public static void IncrementAllyDeath(int typeIndex, int amount = 1)
    {
        if (typeIndex < 0 || typeIndex >= AllyDeaths.Length) return;
        AllyDeaths[typeIndex] += amount;
    }

    public static void ResetAll()
    {
        KilledEnemies = 0;
        for (int i = 0; i < AllyDeaths.Length; i++) AllyDeaths[i] = 0;
    }
}
