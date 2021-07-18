using UnityEngine;
using Verse;

namespace EnableOversizedWeapons
{
    static class PatchUtility
    {
        public static Vector3 GetDrawOffset(Vector3 drawLoc, Thing thing, Pawn pawn)
        {
            return drawLoc + thing.def.graphicData.DrawOffsetForRot(pawn.Rotation);
        }

        public static Mesh GetMesh(Mesh mesh, Thing thing, float aimAngle)
        {
            return MeshMakerPlanes.NewPlaneMesh(thing.def.graphicData.drawSize, (aimAngle > 200f && aimAngle < 340f), false, false);
        }
    }
}
