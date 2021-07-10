using UnityEngine;
using Verse;

namespace EnableOversizedWeapons
{
    static class PatchUtility
    {
        public static Vector3 GetDrawSize(Thing thing)
        {
            Vector2 vector2 = thing.def.graphicData.drawSize;
            return new Vector3(vector2.x, 1f, vector2.y);
        }

        public static Vector3 GetDrawOffset(Vector3 drawLoc, Thing thing, Pawn pawn)
        {
            return drawLoc + thing.def.graphicData.DrawOffsetForRot(pawn.Rotation);
        }
    }
}
