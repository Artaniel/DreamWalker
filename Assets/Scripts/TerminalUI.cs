using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalUI : MonoBehaviour
{
    public Terminal terminal;

    public void CloseButton()
    {
        terminal.CloseUI();
    }
}
