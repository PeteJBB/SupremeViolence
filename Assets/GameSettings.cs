using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class GameSettings 
{
    public static Pickup[] PickupPrefabs = Resources.LoadAll<Pickup>("Pickups");

    public static int NumberOfPlayers = 2;
    public static int NumberOfRounds = 10;
    public static int ScoreLimit = 10;
    public static Pickup StartWeapon = PickupPrefabs.First(x => x.GetPickupName() == "Pistol");
    public static AmmoLevel AmmoLevel = AmmoLevel.Normal;
    public static YesNo SpawnPickups = YesNo.Yes;
    public static int StartingCash = 200;
    public static WinningsLevel WinningsLevel = WinningsLevel.Normal;

    public static Dictionary<string, GameSetting> Settings = new Dictionary<string, GameSetting>()
    {
        { "NumberOfPlayers", new GameSetting(2, 3, 4) },
        { "NumberOfRounds", new GameSetting(1, 2, 3, 4, 5, 10, 15, 20) },
        { "ScoreLimit", new GameSetting(1, 2, 3, 4, 5, 10, 15, 20, 50, 100) },
        { "StartWeapon", new GameSetting(PickupPrefabs.Where(x => x.IsWeapon()).ToArray()) },
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

    public static object IncrementValue(string settingName, int dir = 1)
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
        var newIndex = currentIndex + dir;
        if(newIndex < 0)
            newIndex = 0;
        if(newIndex >= setting.AllowedValues.Length)
            newIndex = setting.AllowedValues.Length - 1;

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
