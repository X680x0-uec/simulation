using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyData
{
    public string name;
    public GameObject prefab;
    public int health;
    public int damage;
    
}

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Scriptable Objects/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    public List<EnemyData> allEnemies = new List<EnemyData>();

}
