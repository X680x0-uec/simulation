using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
	// １番近いオブジェクトを取得する
	public static Transform FetchNearObjectWithTag(Transform origin, string tagName)
	{
		var targets = GameObject.FindGameObjectsWithTag(tagName);

		// 該当タグが1つしか無い場合はそれを返す
		if (targets.Length == 1) return targets[0].transform;

		GameObject result = null;
		var minTargetDistance = float.MaxValue;
		foreach (var target in targets)
		{
			// 前回計測したオブジェクトよりも近くにあれば記録
			var targetDistance = Vector2.Distance(origin.position, target.transform.position);
			if (!(targetDistance < minTargetDistance)) continue;
			minTargetDistance = targetDistance;
			result = target.transform.gameObject;
		}

		// 最後に記録されたオブジェクトを返す
		return result?.transform;
	}

	public static List<Transform> FetchInRangeObjectsWithTag(Transform origin, string tagName, float range)
	{
		var targets = GameObject.FindGameObjectsWithTag(tagName);
		var result = new List<Transform>();
		foreach (var target in targets)
		{
			if (Vector2.Distance(origin.position, target.transform.position) <= range)
			{
				result.Add(target.transform);
			}
		}
		return result;
	}

	public static Vector2 GetVelocityOfGameObject2D(GameObject obj)
	{
		var rb = obj.GetComponent<Rigidbody2D>();
		if (rb != null)
		{
			return rb.linearVelocity;
		}
		Debug.LogWarning(obj.name + "にはRigidbody2Dがアタッチされていません");
		return Vector2.zero;
	}
}
