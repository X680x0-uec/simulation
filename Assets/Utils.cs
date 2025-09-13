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
			var targetDistance = Vector3.Distance(origin.position, target.transform.position);
			if (!(targetDistance < minTargetDistance)) continue;
			minTargetDistance = targetDistance;
			result = target.transform.gameObject;
		}

		// 最後に記録されたオブジェクトを返す
		return result?.transform;
	}
}
