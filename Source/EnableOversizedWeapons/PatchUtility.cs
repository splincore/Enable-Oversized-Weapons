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

        public static Mesh GetMesh(Mesh mesh, Thing thing, float aimAngle)
        {
            if (thing.Graphic.drawSize == Vector2.one) return mesh;
            if (aimAngle > 200f && aimAngle < 340f)
            {
                return GridPlaneFlip(thing.Graphic.drawSize); // Test bugfix
            }
            else
            {
                return GridPlane(thing.Graphic.drawSize); // Test bugfix
            }
        }

        public static Vector3 RemoveNorthDrawOffsetFromEquipment(Vector3 drawLoc, Thing thing)
        {
            if (thing.Rotation == Rot4.North && thing.def.equipmentType == EquipmentType.Primary) drawLoc -= thing.Graphic.DrawOffset(Rot4.North);
            return drawLoc;
        }

        // Test with own dictionary for pawn draw size bug
        private static Mesh GridPlane(Vector2 size)
        {
            if (!planes.TryGetValue(size, out Mesh mesh))
            {
                mesh = MeshMakerPlanes.NewPlaneMesh(size, false, false, false);
                planes.Add(size, mesh);
            }
            return mesh;
        }

        private static Mesh GridPlaneFlip(Vector2 size)
        {
            if (!planesFlip.TryGetValue(size, out Mesh mesh))
            {
                mesh = MeshMakerPlanes.NewPlaneMesh(size, true, false, false);
                planesFlip.Add(size, mesh);
            }
            return mesh;
        }

        private static Dictionary<Vector2, Mesh> planes = new Dictionary<Vector2, Mesh>(FastVector2Comparer.Instance);
        private static Dictionary<Vector2, Mesh> planesFlip = new Dictionary<Vector2, Mesh>(FastVector2Comparer.Instance);
    }
}
