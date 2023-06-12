using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpiderHealth : MonoBehaviour
{
	public float maxHP = 100f;
	public float HP = 100f; // in %
	public float regenDelay = 1f; // in second
	public float regenSpeed = 10f; // hp/sec
	private float regenDelayTimer;
	public TextMeshProUGUI hpText;

	public bool isImmune = false;

	private void Update()
	{
		if (regenDelayTimer < regenDelay)
		{
			regenDelayTimer += Time.deltaTime;
		}
		else
			HP = Mathf.Clamp(HP + regenSpeed * Time.deltaTime, 0 , maxHP);
		UpdateUI();
	}

	public void Damage(float value) {
		if (!isImmune)
		{
			HP = Mathf.Clamp(HP - value, 0, maxHP); 
			UpdateUI();
			if (HP <= 0)
				Death();
		}
	}

	private void UpdateUI() {
		if (hpText != null)
			hpText.text = $"{(int)HP}%hp";
	}

	private void Death() {
		Debug.Log("Game over");
	}

}
