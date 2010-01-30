using System;
using System.Configuration;
using System.Drawing;
using System.Linq;

public class GameSettings : ApplicationSettingsBase
{
    public static GameSettings Instance { get { return sInstance; } }

    [UserScopedSetting()]
    [DefaultSettingValue("")]
    public string LastHero
    {
        get
        {
            return ((string)this["LastHero"]);
        }
        set
        {
            this["LastHero"] = (string)value;
        }
    }

    /// <summary>
    /// Force it to be a singleton.
    /// </summary>
    private GameSettings()
    {
    }

    private static GameSettings sInstance = new GameSettings();
}
