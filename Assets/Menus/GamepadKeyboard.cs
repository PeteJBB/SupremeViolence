using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class GamepadKeyboard: MonoBehaviour 
{
    public GameObject TargetText;

    public void TypeCharacter(string character)
    {
        TargetText.GetComponent<Text>().text += character;
    }

    public void DeleteLastCharacter()
    {
        var tx = TargetText.GetComponent<Text>();
        if(tx.text.Length > 0)
            tx.text = tx.text.Substring(0, tx.text.Length - 1);
    }
}