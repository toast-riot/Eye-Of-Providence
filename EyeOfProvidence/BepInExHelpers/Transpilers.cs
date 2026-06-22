using HarmonyLib;
using System;
using System.Reflection.Emit;

namespace BepInExHelpers.Transpilers;

static class TranspilerHelpers
{
    /// <summary>
    /// Generates a predicate to match any form of the given opcode (e.g., short or long forms).
    /// </summary>
    /// <param name="opcode">The base opcode to match (e.g., OpCodes.Br).</param>
    /// <returns>A predicate that matches any form of the opcode.</returns>
    internal static Func<CodeInstruction, bool> AnyForm(OpCode opcode)
    {
        OpCode shortForm = GetShortForm(opcode);
        return ci => ci.opcode == opcode || ci.opcode == shortForm;
    }

    static OpCode GetShortForm(OpCode opcode)
    {
        if (opcode == OpCodes.Beq) return OpCodes.Beq_S;
        if (opcode == OpCodes.Bge) return OpCodes.Bge_S;
        if (opcode == OpCodes.Bge_Un) return OpCodes.Bge_Un_S;
        if (opcode == OpCodes.Bgt) return OpCodes.Bgt_S;
        if (opcode == OpCodes.Bgt_Un) return OpCodes.Bgt_Un_S;
        if (opcode == OpCodes.Ble) return OpCodes.Ble_S;
        if (opcode == OpCodes.Ble_Un) return OpCodes.Ble_Un_S;
        if (opcode == OpCodes.Blt) return OpCodes.Blt_S;
        if (opcode == OpCodes.Blt_Un) return OpCodes.Blt_Un_S;
        if (opcode == OpCodes.Bne_Un) return OpCodes.Bne_Un_S;
        if (opcode == OpCodes.Br) return OpCodes.Br_S;
        if (opcode == OpCodes.Brfalse) return OpCodes.Brfalse_S;
        if (opcode == OpCodes.Brtrue) return OpCodes.Brtrue_S;
        if (opcode == OpCodes.Ldarg) return OpCodes.Ldarg_S;
        if (opcode == OpCodes.Ldarga) return OpCodes.Ldarga_S;
        if (opcode == OpCodes.Ldc_I4) return OpCodes.Ldc_I4_S;
        if (opcode == OpCodes.Ldloc) return OpCodes.Ldloc_S;
        if (opcode == OpCodes.Ldloca) return OpCodes.Ldloca_S;
        if (opcode == OpCodes.Leave) return OpCodes.Leave_S;
        if (opcode == OpCodes.Starg) return OpCodes.Starg_S;
        if (opcode == OpCodes.Stloc) return OpCodes.Stloc_S;
        return opcode;
    }
}