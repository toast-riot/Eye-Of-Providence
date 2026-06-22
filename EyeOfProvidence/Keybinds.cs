using BepInExHelpers;
using UnityEngine;

namespace EyeOfProvidence;

static class Keybinds {
    [PluginUpdate]
    static void Update() {
        if (Input.GetKeyDown(Settings.ToggleKey.Value)) Settings.Enabled.Value = !Settings.Enabled.Value;
        if (Input.GetKeyDown(Settings.GridBind.Value)) Settings.Grid.Value = !Settings.Grid.Value;
    }
}