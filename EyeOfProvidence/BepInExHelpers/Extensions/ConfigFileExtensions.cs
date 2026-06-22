using System;
using System.Collections.Generic;
using BepInEx.Configuration;

namespace BepInExHelpers.Extensions;

// Most of this is from ComfyLib, with some additions from me. Thanks!
// I think this is the original source: https://github.com/mu-arch

public static class ConfigFileExtensions
{
    static readonly Dictionary<string, int> _sectionToSettingOrder = new();

    static int GetSettingOrder(string section)
    {
        if (!_sectionToSettingOrder.TryGetValue(section, out int order))
        {
            order = 0;
        }

        _sectionToSettingOrder[section] = order - 1;
        return order;
    }

    public static ConfigEntry<T> BindInOrder<T>(
        this ConfigFile config,
        string section,
        string key,
        T defaultValue,
        string description = "",
        AcceptableValueBase acceptableValues = default,
        bool browsable = true,
        bool hideDefaultButton = default,
        bool hideSettingName = default,
        bool isAdvanced = default,
        bool readOnly = default,
        bool showRangeAsPercent = default
    )
    {
        return config.Bind(
            section,
            key,
            defaultValue,
            new ConfigDescription(
                description,
                acceptableValues,
                new ConfigurationManagerAttributes
                {
                    Browsable = browsable,
                    CustomDrawer = default,
                    HideDefaultButton = hideDefaultButton,
                    HideSettingName = hideSettingName,
                    IsAdvanced = isAdvanced,
                    Order = GetSettingOrder(section),
                    ReadOnly = readOnly,
                    ShowRangeAsPercent = showRangeAsPercent
                }
            )
        );
    }

    public static ConfigEntry<T> BindInOrder<T>(
        this ConfigFile config,
        string section,
        string key,
        T defaultValue,
        string description,
        Action<ConfigEntryBase> customDrawer,
        bool browsable = true,
        bool hideDefaultButton = default,
        bool hideSettingName = default,
        bool isAdvanced = default,
        bool readOnly = default,
        bool showRangeAsPercent = default
    )
    {
        return config.Bind(
            section,
            key,
            defaultValue,
            new ConfigDescription(
                description,
                acceptableValues: default,
                new ConfigurationManagerAttributes
                {
                    Browsable = browsable,
                    CustomDrawer = customDrawer,
                    HideDefaultButton = hideDefaultButton,
                    HideSettingName = hideSettingName,
                    IsAdvanced = isAdvanced,
                    Order = GetSettingOrder(section),
                    ReadOnly = readOnly,
                    ShowRangeAsPercent = showRangeAsPercent
                }
            )
        );
    }

    // public static void OnSettingChanged<T>(this ConfigEntry<T> configEntry, Action settingChangedHandler)
    // {
    //     configEntry.SettingChanged += (_, _) => settingChangedHandler();
    // }

    // public static void OnSettingChanged<T>(this ConfigEntry<T> configEntry, Action<T> settingChangedHandler)
    // {
    //     configEntry.SettingChanged +=
    //         (_, eventArgs) => settingChangedHandler((T)((SettingChangedEventArgs)eventArgs).ChangedSetting.BoxedValue);
    // }

    // public static void OnSettingChanged<T>(
    //     this ConfigEntry<T> configEntry, Action<ConfigEntry<T>> settingChangedHandler
    // ){
    //     configEntry.SettingChanged +=
    //         (_, eventArgs) =>
    //             settingChangedHandler((ConfigEntry<T>)((SettingChangedEventArgs)eventArgs).ChangedSetting.BoxedValue);
    // }

    internal sealed class ConfigurationManagerAttributes
    {
        public Action<ConfigEntryBase> CustomDrawer;
        public bool? Browsable;
        public bool? HideDefaultButton;
        public bool? HideSettingName;
        public bool? IsAdvanced;
        public int? Order;
        public bool? ReadOnly;
        public bool? ShowRangeAsPercent;
    }

    public static bool IsDefaultValue<T>(this ConfigEntry<T> entry)
    {
        return Equals(entry.Value, entry.DefaultValue);
    }

    static readonly Dictionary<object, EventHandler> _settingChangedHandlers = new();

    public static void SetOnSettingChanged<T>(this ConfigEntry<T> configEntry, Action<T> handler, bool callImmediately = false)
    {
        if (_settingChangedHandlers.TryGetValue(configEntry, out EventHandler existingHandler))
        {
            configEntry.SettingChanged -= existingHandler;
        }
        EventHandler newHandler = (s, e) => handler((T)((SettingChangedEventArgs)e).ChangedSetting.BoxedValue);
        configEntry.SettingChanged += newHandler;
        _settingChangedHandlers[configEntry] = newHandler;
        if (callImmediately) handler(configEntry.Value);
    }

    public static void RoundToStep(this ConfigEntry<float> configEntry, float step)
    {
        configEntry.SettingChanged += (_, _) =>
            configEntry.Value = (float)(Math.Round(configEntry.Value / step) * step);
    }
}