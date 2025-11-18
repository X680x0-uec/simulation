using UnityEngine;
using System.Collections.Generic;
using System;

public class AllyIconManager : MonoBehaviour
{
    [SerializeField] private AllyManager allyManager;

	[SerializeField] private GameObject[] AllyIcons; // 0: left, 1: center, 2: right
	private UnityEngine.UI.Image[] AllyIconImages;
	private TMPro.TextMeshProUGUI[] AllyIconTexts;

	void Awake()
	{
		AllyIconImages = new UnityEngine.UI.Image[3];
		AllyIconTexts = new TMPro.TextMeshProUGUI[3];

		for (int i = 0; i < AllyIcons.Length; i++)
		{
			AllyIconImages[i] = AllyIcons[i].GetComponent<UnityEngine.UI.Image>();
			AllyIconTexts[i] = AllyIcons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();
		}

		SwitchIcon(AllyManager.index, allyManager.allyDatabase);
	}

	public void SwitchIcon(int newIndex, AllyDatabase allyDatabase)
	{
		for (int i = 0; i < AllyIcons.Length; i++)
		{
			if (AllyIconImages[i] != null)
			{
				int index = (newIndex - 1 + i + allyDatabase.allAllies.Count) % allyDatabase.allAllies.Count;
				AllyData ally = allyDatabase.allAllies[index];
				if (ally.icon != null)
				{
					AllyIconImages[i].sprite = ally.icon;
					AllyIconTexts[i].text = "";
				}
				else
				{
					AllyIconImages[i].sprite = null;
					AllyIconTexts[i].text = ally.name;
				}
			}
		}
	}
}
