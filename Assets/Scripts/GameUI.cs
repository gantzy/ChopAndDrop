using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{

    public TextMeshProUGUI goldText;

    public static GameUI instance;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateGoldText(int gold)
    {
        goldText.text = "<b>Gold:</b> " + gold;
    }
}
