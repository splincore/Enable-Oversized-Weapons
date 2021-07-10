using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
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
            // replaces Graphics.DrawMesh(mesh, drawLoc, Quaternion.AngleAxis(num, Vector3.up), matSingle, 0); with Graphics.DrawMesh(mesh, Matrix4x4.TRS(PatchUtility.GetDrawOffset(Vector3 drawLoc, Thing thing, Pawn pawn), rotation, PatchUtility.GetDrawSize(Thing thing)), matSingle, 0);

            yield return new CodeInstruction(OpCodes.Ldarg_2); // stack: mesh, drawLoc
            yield return new CodeInstruction(OpCodes.Ldarg_1); // stack: mesh, drawLoc, thing
            yield return new CodeInstruction(OpCodes.Ldarg_0); // stack: mesh, drawLoc, thing, pawnrenderer
            yield return new CodeInstruction(OpCodes.Ldfld, typeof(PawnRenderer).GetField("pawn", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance)); // stack: mesh, drawLoc, thing, pawn
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchUtility), "GetDrawOffset")); // stack: mesh, modified drawLoc

            yield return new CodeInstruction(OpCodes.Ldloc_1); // stack: mesh, modified drawLoc, float(aimAngle)
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Vector3), "get_up")); // stack: mesh, modified drawLoc, float(aimAngle), vector.up
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Quaternion), "AngleAxis", new Type[] { typeof(float), typeof(Vector3) })); // stack: mesh, modified drawLoc, vanilla Quaternion (rotation) 

            yield return new CodeInstruction(OpCodes.Ldarg_1); // stack: mesh, modified drawLoc, vanilla Quaternion (rotation), thing
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PatchUtility), "GetDrawSize")); // stack: mesh, modified drawLoc, vanilla Quaternion (rotation), modified size

            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Matrix4x4), "TRS", new Type[] { typeof(Vector3), typeof(Quaternion), typeof(Vector3) })); // stack: mesh, Matrix4x4 (with modified values)
            yield return new CodeInstruction(OpCodes.Ldloc_3); // stack: mesh, Matrix4x4 (with modified values), material
            yield return new CodeInstruction(OpCodes.Ldc_I4_0); // stack: mesh, Matrix4x4 (with modified values), material, layer
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Graphics), "DrawMesh", new Type[] { typeof(Mesh), typeof(Matrix4x4), typeof(Material), typeof(int) })); // called another DrawMesh instead, stack empty

            for (int i = firstQuaternionIndex + 4; i < codes.Count; i++) // return everything after call
            {
                yield return codes[i];
            }
        }
    }
}
