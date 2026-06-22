using HarmonyLib;
using System.Reflection.Emit;

namespace BepInExHelpers.Extensions;

public static class CodeMatcherExtensions
{
    /// <summary>
    /// Replaces a specified number of instructions with NOPs
    /// </summary>
    /// <param name="matcher">The CodeMatcher instance.</param>
    /// <param name="count">The number of instructions to replace with NOPs.</param>
    public static CodeMatcher NopInstructions(this CodeMatcher matcher, int count)
    {
        for (int i = 0; i < count; i++)
        {
            matcher.SetInstruction(new CodeInstruction(OpCodes.Nop)).Advance(1);
        }
        return matcher;
    }
}