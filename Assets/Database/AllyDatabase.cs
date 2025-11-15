using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AllyData
{
    public string name;
    public GameObject prefab;
    public int health;
    public int cost;
    public Sprite icon;
}

[CreateAssetMenu(fileName = "AllyDatabase", menuName = "Scriptable Objects/AllyDatabase")]
public class AllyDatabase : ScriptableObject
{
    public List<AllyData> allAllies;
}
