using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI pressEText;
    private Terminal terminalToReturn;
    public TextMeshProUGUI speedModifierText;
    public TextMeshProUGUI breathModeText;

    public void ShowPressE(Terminal terminal) {
        if (!pressEText.gameObject.activeSelf)
        {
            terminalToReturn = terminal;
            pressEText.gameObject.SetActive(true);
        }
    }

    public void HidePressE() {
        pressEText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (pressEText.gameObject.activeSelf && !terminalToReturn.IsLookingAt())
        {
            terminalToReturn = null;
            pressEText.gameObject.SetActive(false);
        }
    }

    public void RefreshSpeedModifier(float modifier)
    {
        speedModifierText.text = modifier.ToString();
    }

    public void RefreshBreathMode(BreathInput.BreathMode mode)
    {
        breathModeText.text = mode.ToString();
    }

}
