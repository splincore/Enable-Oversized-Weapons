using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace EnableOversizedWeapons
{
    [StaticConstructorOnStartup]
    static class HarmonyPatch
    {
        static HarmonyPatch()
        {
            var harmony = new Harmony("rimworld.carnysenpai.enableoversizedweapons");
            harmony.Patch(AccessTools.Method(typeof(PawnRenderUtility), "DrawEquipmentAiming"), new HarmonyMethod(typeof(HarmonyPatch).GetMethod("Prefix_Drawloc")), null, null);
            if (LoadedModManager.GetMod<EnableOversizedWeaponsMod>().GetSettings<EnableOversizedWeaponsModSettings>().removeNorthDrawOffsetFromEquipment)
            {
                harmony.Patch(AccessTools.Method(typeof(Graphic), "Print"), null, null, new HarmonyMethod(typeof(HarmonyPatch).GetMethod("Transpiler_Print")));
            }
        }

        [HarmonyPrefix]
        public static bool Prefix_Drawloc(Thing eq, ref Vector3 drawLoc)
        {
            Pawn pawn = null;
            CompEquippable compEquippable = eq.TryGetComp<CompEquippable>();
            if (compEquippable != null)
            {
                ThingWithComps parent = compEquippable.parent;
                Pawn_EquipmentTracker pawn_EquipmentTracker = ((parent != null) ? parent.ParentHolder : null) as Pawn_EquipmentTracker;
                if (pawn_EquipmentTracker != null)
                {
                    if (pawn_EquipmentTracker.pawn == null)
                    {
                        return true;
                    }
                    else
                    {
                        pawn = pawn_EquipmentTracker.pawn;
                    }
                }
            }
            if (eq.StyleDef != null && eq.StyleDef.graphicData != null)
            {
                drawLoc += eq.StyleDef.graphicData.DrawOffsetForRot(pawn.Rotation);
            }
            else
            {
                drawLoc += eq.def.graphicData.DrawOffsetForRot(pawn.Rotation);
            }
            return true;
        }

        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler_Print(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
            int firstDrawOffset = -1;
            for (int i = 0; i < codes.Count; i++)
            {
                if (codes[i].operand.ToStringSafe().Contains("DrawOffset")) // searching ankerpoint "DrawOffset"
                {
                    firstDrawOffset = i;
                    break;
                }
            }
            for (int i = 0; i < firstDrawOffset + 3; i++) // return until (including) IL_00C5
            {
                yield return codes[i];
            }
            // stack: -

            // replaces mesh with a modified mesh which uses the drawSize as a factor
            yield return new CodeInstruction(OpCodes.Ldloc_3); // stack: Vector3
            yield return new CodeInstruction(OpCodes.Ldarg_2); // stack: Vector3, thing
            yield return new CodeInstruction(OpCodes.Ldarg_0); // stack: Vector3, thing, graphic
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchUtility), "RemoveNorthDrawOffsetFromEquipment"));  // stack: Vector3
            yield return new CodeInstruction(OpCodes.Stloc_3); // stack: -

            for (int i = firstDrawOffset + 3; i < codes.Count; i++) // return everything after call
            {
                yield return codes[i];
            }
        }
    }
}
