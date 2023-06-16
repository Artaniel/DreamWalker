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
	public SunManager sun;
	public float sunDPS = 5f;

	private void Update()
	{
		Regen();
		SunHeatCkeck();
		UpdateUI();
	}

	private void Regen() {
		if (regenDelayTimer < regenDelay)
		{
			regenDelayTimer += Time.deltaTime;
		}
		else
			HP = Mathf.Clamp(HP + regenSpeed * Time.deltaTime, 0, maxHP);
	}

	private void SunHeatCkeck() {
		if (sun.IsUnderLight(transform.position))
			Damage(sunDPS * Time.deltaTime);
	}

	public void Damage(float value) {
		if (!isImmune)
		{
			HP = Mathf.Clamp(HP - value, 0, maxHP); 
			UpdateUI();
			if (HP <= 0)
				Death();
			regenDelayTimer = 0;
		}
	}

	private void UpdateUI() {
		HeatMeterUI.RefreshHeatMeter(1- (HP / maxHP));
		if (hpText != null)
			hpText.text = $"{(int)HP}%hp";
	}

	private void Death() {
		Debug.Log("Game over");
	}

}
