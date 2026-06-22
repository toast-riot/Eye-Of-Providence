using BepInEx.Configuration;
using BepInExHelpers.Extensions;
using UnityEngine;

namespace EyeOfProvidence;

public static class Settings {
    public enum PerspectiveMode {
        Equirectangular = 0,
        Fisheye = 1,
        Stereographic = 2,
        Hammer = 3,
        Panini = 4
    }

    public static ConfigEntry<bool> Debug { get; private set; }
	public static ConfigEntry<bool> Enabled { get; private set; }
    public static ConfigEntry<KeyCode> ToggleKey { get; private set; }
    public static ConfigEntry<float> FOV { get; private set; }
    public static ConfigEntry<float> Quality { get; private set; }
    public static ConfigEntry<PerspectiveMode> Perspective { get; private set; }
    public static ConfigEntry<bool> Stretch { get; private set; }

    public static ConfigEntry<bool> Grid { get; private set; }
    public static ConfigEntry<KeyCode> GridBind { get; private set; }
    public static ConfigEntry<float> GridOpacity { get; private set; }

    public static ConfigEntry<float> StereoFactor { get; private set; }
    public static ConfigEntry<float> PaniniFactor { get; private set; }

	internal static void Init(ConfigFile config) {
		var section = "1. General";
        Debug = config.BindInOrder(section, "Debug", false);
		Enabled = config.BindInOrder(section, "Enable", true);
        ToggleKey = config.BindInOrder(section, "Toggle Keybind", KeyCode.None);
        FOV = config.BindInOrder(section, "FOV", 120f, "Does not affect stereographic or panini", new AcceptableValueRange<float>(0f, 360f));
        FOV.RoundToStep(1f);
        Quality = config.BindInOrder(section, "Quality", 9.5f, "", new AcceptableValueRange<float>(0f, 10f));
        Quality.RoundToStep(0.1f);
        Perspective = config.BindInOrder(section, "Perspective", PerspectiveMode.Equirectangular);
        Stretch = config.BindInOrder(section, "Stretch to View", false);

        section = "2. Overlays";
        Grid = config.BindInOrder(section, "Grid View", false);
        GridBind = config.BindInOrder(section, "Grid Keybind", KeyCode.None);
        GridOpacity = config.BindInOrder(section, "Grid Opacity", 0.2f, "", new AcceptableValueRange<float>(0f, 1f), showRangeAsPercent: true);
        GridOpacity.RoundToStep(0.05f);

        section = "3. Perspective Specific";
        StereoFactor = config.BindInOrder(section, "Stereographic Scale", 0f, "", new AcceptableValueRange<float>(0f, 3f));
        StereoFactor.RoundToStep(0.01f);
        PaniniFactor = config.BindInOrder(section, "Panini Intensity", 1f, "", new AcceptableValueRange<float>(0f, 1f), showRangeAsPercent: true);
        PaniniFactor.RoundToStep(0.01f);
	}
}