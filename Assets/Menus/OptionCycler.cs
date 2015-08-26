using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Linq;

public class OptionCycler: CustomSelectable, ISubmitHandler, IMoveHandler
{
    public string SettingName;
    public string DisplayFormat;

    private Text valText;

    void Awake()
    {
        valText = GetComponent<Text>();
    }

    new void Start()
    {
        UpdateText();
    }

    void UpdateText()
    {
        var val = GameSettings.GetValue(SettingName);
        if (val == null)
            val = string.Empty;

        if (!string.IsNullOrEmpty(DisplayFormat))
            valText.text = string.Format(DisplayFormat, val);
        else
            valText.text = val.ToString();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        GameSettings.IncrementValue(SettingName, 1, true).ToString();
    }

    public override Selectable FindSelectableOnLeft()
    {
        GameSettings.IncrementValue(SettingName, -1).ToString();
        UpdateText();
        return this;
    }

    public override Selectable FindSelectableOnRight()
    {
        GameSettings.IncrementValue(SettingName).ToString();
        UpdateText();
        return this;
    }
}