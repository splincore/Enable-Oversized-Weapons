using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace EnableOversizedWeapons
{
    [StaticConstructorOnStartup]
    static class HarmonyPatch
    {
        static HarmonyPatch()
        {
            var harmony = new Harmony("rimworld.carnysenpai.enableoversizedweapons");
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "DrawEquipmentAiming"), null, null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("Transpiler_DrawEquipmentAiming")));
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_DrawEquipmentAiming(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            int firstQuaternionIndex = -1;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].operand.ToStringSafe().Contains("Quaternion")) // searching ankerpoint "Quaternion"
                {
                    firstQuaternionIndex = i;
                    break;
                }
            }
            for (int i = 0; i < firstQuaternionIndex - 3; i++) // return until (including) jump target IL_00E3 
            {
                yield return codes[i];
            }
            // stack: mesh

            // replaces mesh with a modified mesh which uses the drawSize as a factor
            yield return new CodeInstruction(OpCodes.Ldarg_1); // stack: mesh, thing
            yield return new CodeInstruction(OpCodes.Ldarg_3); // stack: mesh, thing, aimAngle
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchUtility), "GetMesh"));  // stack: modified mesh

            // replaces drawLoc with a modified drawLoc, which uses the offsets as a factor
            yield return new CodeInstruction(OpCodes.Ldarg_2); // stack: modified mesh, drawLoc
            yield return new CodeInstruction(OpCodes.Ldarg_1); // stack: modified mesh, drawLoc, thing
            yield return new CodeInstruction(OpCodes.Ldarg_0); // stack: modified mesh, drawLoc, thing, pawnrenderer
            yield return new CodeInstruction(OpCodes.Ldfld, typeof(PawnRenderer).GetField("pawn", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance)); // stack: modified mesh, drawLoc, thing, pawn
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchUtility), "GetDrawOffset")); // stack: modified mesh, modified drawLoc

            for (int i = firstQuaternionIndex - 2; i < codes.Count; i++) // return everything after call
            {
                yield return codes[i];
            }
        }
    }
}
