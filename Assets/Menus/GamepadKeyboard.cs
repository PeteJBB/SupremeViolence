using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class GamepadKeyboard: MonoBehaviour 
{
    public GameObject TargetText;
    public int PlayerIndex = 0;

    private GamePadState lastGamepadState;

    void Start()
    {
        var cmi = Helper.GetComponentInParentsRecursive<CustomMenuInputController>(transform);
        if (cmi != null)
            this.PlayerIndex = cmi.PlayerIndex;
    }

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

    void Update()
    {
        var state = Helper.GetGamePadInput(PlayerIndex);

        if(state.Buttons.X == ButtonState.Pressed && lastGamepadState.Buttons.X == ButtonState.Released)
        {
            DeleteLastCharacter();

            var elem = transform.FindChild("DEL");
            if (elem != null)
                elem.GetComponent<CustomButton>().FlashBackground();
        }
        else if(state.Buttons.Y == ButtonState.Pressed && lastGamepadState.Buttons.Y == ButtonState.Released)
        {
            TypeCharacter(" ");

            var elem = transform.FindChild("SPACE");
            if (elem != null)
                elem.GetComponent<CustomButton>().FlashBackground();
        }

        lastGamepadState = state;
    }
}