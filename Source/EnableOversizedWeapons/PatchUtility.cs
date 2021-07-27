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
            if (thing.def.graphicData.drawSize == Vector2.one) return mesh;
            if (aimAngle > 200f && aimAngle < 340f)
            {
                return MeshPool.GridPlaneFlip(thing.def.graphicData.drawSize);
            }
            else
            {
                return MeshPool.GridPlane(thing.def.graphicData.drawSize);
            }
        }
    }
}
