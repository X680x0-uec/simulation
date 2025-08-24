using System.Collections.Generic;
using UnityEngine;

public class UnlockedUnitManager : MonoBehaviour
{
	[SerializeField] private UnitDatabase database;
	[SerializeField] private int initialUnlockedUnits = 16;
	static public List<UnitData> unlockedUnits = new List<UnitData>();

	void Start()
	{
		// 初期状態：データベースの先頭から解禁する
		for (int i = 0; i < initialUnlockedUnits; i++)
		{
			if (i < database.allUnits.Count)
			{
				UnlockUnit(database.allUnits[i]);
			}
		}
	}

	static public void UnlockUnit(UnitData newUnit)
	{
		if (!unlockedUnits.Contains(newUnit))
		{
			unlockedUnits.Add(newUnit);
			Debug.Log(newUnit.unitName + " を解禁しました！");
		}
	}
}
