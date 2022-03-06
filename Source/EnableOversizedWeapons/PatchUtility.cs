using UnityEngine;
using Verse;

namespace EnableOversizedWeapons
{
    static class PatchUtility
    {
        public static Vector3 GetDrawOffset(Vector3 drawLoc, Thing thing, Pawn pawn)
        {
            return drawLoc + thing.Graphic.DrawOffset(pawn.Rotation);
        }

        public static Mesh GetMesh(Mesh mesh, Thing thing, float aimAngle)
        {
            if (thing.Graphic.drawSize == Vector2.one) return mesh;
            if (aimAngle > 200f && aimAngle < 340f)
            {
                return MeshPool.GridPlaneFlip(thing.Graphic.drawSize);
            }
            else
            {
                return MeshPool.GridPlane(thing.Graphic.drawSize);
            }
        }

        public static Vector3 RemoveNorthDrawOffsetFromEquipment(Vector3 drawLoc, Thing thing)
        {
            if (thing.Rotation == Rot4.North && thing.def.equipmentType == EquipmentType.Primary) drawLoc -= thing.Graphic.DrawOffset(Rot4.North);
            return drawLoc;
        }
    }
}
