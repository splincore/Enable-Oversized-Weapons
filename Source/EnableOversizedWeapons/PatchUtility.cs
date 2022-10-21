using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace EnableOversizedWeapons
{
    static class PatchUtility
    {
        public static Vector3 GetDrawOffset(Vector3 drawLoc, Thing thing, Pawn pawn)
        {
            if (thing.StyleDef != null && thing.StyleDef.graphicData != null)
            {
                return drawLoc + thing.StyleDef.graphicData.DrawOffsetForRot(pawn.Rotation);
            }
            else
            {
                return drawLoc + thing.def.graphicData.DrawOffsetForRot(pawn.Rotation);
            }
        }

        public static Vector3 RemoveNorthDrawOffsetFromEquipment(Vector3 drawLoc, Thing thing, Graphic graphic)
        {
            if (thing.Rotation == Rot4.North && thing.def.equipmentType == EquipmentType.Primary) drawLoc -= graphic.DrawOffset(thing.Rotation);
            return drawLoc;
        }
    }
}
