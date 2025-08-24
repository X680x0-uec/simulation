using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class UnitData
{
    public string unitName;
    public GameObject unitPrefab;
    public int health;
    public int cost;
    public Sprite icon;
}

[CreateAssetMenu(fileName = "UnitDatabase", menuName = "Scriptable Objects/UnitDatabase")]
public class UnitDatabase : ScriptableObject
{
    public List<UnitData> allUnits;
}
