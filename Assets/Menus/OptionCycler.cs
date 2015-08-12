using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Linq;

public class OptionCycler: MonoBehaviour, IMoveHandler
{
    Text valText;
    public string SettingName;
    public string DisplayFormat;

    void Start()
    {
        valText = GetComponent<Text>();
        var val = GameSettings.GetValue(SettingName);
        if(val != null)
            valText.text = val.ToString();
    }

    void Update()
    {
        var val = GameSettings.GetValue(SettingName);
        if(val == null)
            valText.text = string.Empty;

        if(string.IsNullOrEmpty(DisplayFormat))
            valText.text = val.ToString();
        else
            valText.text = string.Format(DisplayFormat, val);
    }

    public void OnMove(AxisEventData eventData)
    {
        if(eventData.moveDir == MoveDirection.Left)
        {
            GameSettings.IncrementValue(SettingName, -1).ToString();
        }
        if(eventData.moveDir == MoveDirection.Right)
        {
            GameSettings.IncrementValue(SettingName).ToString();
        }
//        var currentIndex = Array.IndexOf(s.AllowedValues, s.Value);
//
//        if(eventData.moveDir == MoveDirection.Left)
//        {
//            var newIndex = (currentIndex + s.AllowedValues.Length - 1) % s.AllowedValues.Length;
//            var val = s.AllowedValues[newIndex];
//            GameSettings.Settings[SettingName].Value = val;
//        }
//        if(eventData.moveDir == MoveDirection.Right)
//        {
//            var newIndex = (currentIndex + s.AllowedValues.Length + 1) % s.AllowedValues.Length;
//            var val = s.AllowedValues[newIndex];
//            GameSettings.Settings[SettingName].Value = val;
//        }



//        var vals = GameSettings.GetAllowedValues(SettingName);
//        if(vals != null)
//        {
//            var currentIndex = vals.IndexOf(GetCurrentSettingValue(SettingName));
//        }
//
//        if(eventData.moveDir == MoveDirection.Left)
//        {
//            if(SettingType == GameSettingType.Int)
//            {
//
//                var field = typeof(GameSettings).GetField(SettingName);
//                if(field != null)
//                {
//                    var val = (int)field.GetValue(null);
//                    val--;
//
//                    field.SetValue(null, val);
//                    valText.text = val.ToString();
//                }
//            }
//        }
//        else if(eventData.moveDir == MoveDirection.Right)
//        {
//            if(SettingType == GameSettingType.Int)
//            {
//                var field = typeof(GameSettings).GetField(SettingName);
//                if(field != null)
//                {
//                    var val = (int)field.GetValue(null);
//                    val++;
//                    
//                    field.SetValue(null, val);
//                    valText.text = val.ToString();
//                }
//            }
//        }
    }

}

public enum GameSettingType
{
    Int,
    Custom
}