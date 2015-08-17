using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class GameSettings 
{
    public static Pickup[] PickupPrefabs = Resources.LoadAll<Pickup>("Pickups");
    public static Decoration[] DecorationPrefabs = Resources.LoadAll<Decoration>("Decorations");

    public static int NumberOfPlayers = 2;
    public static int NumberOfRounds = 10;
    public static int ScoreLimit = 1;
    public static Pickup StartWeapon = PickupPrefabs.First(x => x.GetPickupName() == "Pistol");
    public static AmmoLevel AmmoLevel = AmmoLevel.Normal;
    public static YesNo SpawnPickups = YesNo.Yes;
    public static int StartingCash = 2000;
    public static int CashForKill = 400;
    public static WinningsLevel WinningsLevel = WinningsLevel.Normal;
    public static float SoundVolume = 0;

    public static Dictionary<string, GameSetting> Settings = new Dictionary<string, GameSetting>()
    {
        { "NumberOfPlayers", new GameSetting(2, 3, 4) },
        { "NumberOfRounds", new GameSetting(1, 2, 3, 4, 5, 10, 15, 20) },
        { "ScoreLimit", new GameSetting(1, 2, 3, 4, 5, 10, 15, 20, 50, 100) },
        { "StartWeapon", new GameSetting(PickupPrefabs.Where(x => x.PickupType == PickupType.Weapon).ToArray()) },
        { "AmmoLevel", new GameSetting(AmmoLevel) },
        { "SpawnPickups", new GameSetting(YesNo.Yes) },
        { "StartingCash", new GameSetting(0, 200, 500, 1000, 2000, 5000, 10000, 20000, 50000) },
        { "WinningsLevel", new GameSetting(WinningsLevel.Normal) },
    };

    public static object GetValue(string settingName)
    {
        var field = typeof(GameSettings).GetField(settingName);
        if(field != null)
        {
            return field.GetValue(null);
        }

        return null;
    }

    /// <summary>
    /// Set the value of the setting to the next or previous value from the allowed values list
    /// </summary>
    /// <returns>The newly set value</returns>
    /// <param name="settingName">Setting name.</param>
    /// <param name="dir">Direction of increment 1 = next, -1 = previous</param>
    /// <param name="wrap">If set to <c>true</c> the indexer will wrap around back to zero or to the end of the array if dir = -1.</param>
    public static object IncrementValue(string settingName, int dir = 1, bool wrap = false)
    {
        var field = typeof(GameSettings).GetField(settingName);
        if(field == null)
        {
            Debug.LogError("IncrementSetting: invalid setting name " + settingName);
            return "";
        }
        var currentVal = field.GetValue(null);
        var setting = Settings[settingName];
        var currentIndex = Array.IndexOf(setting.AllowedValues, currentVal);

        int newIndex;
        if(wrap)
        {
            newIndex = (currentIndex + setting.AllowedValues.Length + dir ) % setting.AllowedValues.Length;
        }
        else
        {
            newIndex = Mathf.Clamp(currentIndex + dir, 0, setting.AllowedValues.Length - 1);
        }

        var val = setting.AllowedValues[newIndex];
        field.SetValue(null, val);

        return val;
    }
}

public class GameSetting
{
    public object[] AllowedValues;

    public GameSetting(params object[] allowedValues)
    {
        AllowedValues = allowedValues;
    }

    public GameSetting(Enum defaultValue)
    {
        AllowedValues = Enum.GetValues(defaultValue.GetType()).Cast<object>().ToArray();
    }
}

public enum AmmoLevel
{
    OneShot,
    Low,
    Normal, 
    Double,
    Buttloads,
    Infinite
}

public enum YesNo
{
    No,
    Yes
}

public enum WinningsLevel
{
    None,
    Low,
    Normal,
    Double,
    Buttloads
}
