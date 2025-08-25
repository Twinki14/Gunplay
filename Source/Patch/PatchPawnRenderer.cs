using HarmonyLib;
using UnityEngine;
using Verse;

namespace Gunplay.Patch
{
    [HarmonyPatch(typeof(PawnRenderUtility), "DrawEquipmentAiming", typeof(Thing), typeof(Vector3), typeof(float))]
    public class PatchPawnRenderUtilityDrawEquipmentAiming
    {
        private static readonly Vector3 EquipmentDir = new Vector3(0f, 0f, 0.4f);
        private static Vector3 _drawingScale = new Vector3(1f, 1f, 1f);
        private static Matrix4x4 _drawingMatrix;

        public static bool Prefix(ref Thing eq, ref Vector3 drawLoc, ref float aimAngle)
        {
            if (!Gunplay.settings.EnableWeaponAnimations) return true;

            var comp = eq.TryGetComp<CompGun>();
            if (comp == null) return true;

            drawLoc -= EquipmentDir.RotatedBy(aimAngle);
            aimAngle = (aimAngle + comp.RotationOffset) % 360;
            drawLoc += EquipmentDir.RotatedBy(aimAngle);

            var prop = GunplaySetup.GunProp(eq);
            if (prop == null) return true;

            var num = aimAngle - 90f;
            Mesh mesh;
            if (aimAngle > 20f && aimAngle < 160f)
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
            else if (aimAngle > 200f && aimAngle < 340f)
            {
                mesh = MeshPool.plane10Flip;
                num -= 180f;
                num -= eq.def.equippedAngleOffset;
            }
            else
            {
                mesh = MeshPool.plane10;
                num += eq.def.equippedAngleOffset;
            }
            num %= 360f;

            _drawingScale.x = _drawingScale.z = prop.drawScale;
            var primer = eq.TryGetComp<CompPrimer>();
            if (primer != null)
            {
                primer.Draw(mesh, drawLoc, num, _drawingScale);
                return false;
            }

            if (Mathf.Approximately(prop.drawScale, 1f))
            {
                return true;
            }

            var mat = eq.Graphic is Graphic_StackCount graphicStackCount
                ? graphicStackCount.SubGraphicForStackCount(1, eq.def).MatSingle
                : eq.Graphic.MatSingle;

            _drawingMatrix.SetTRS(drawLoc, Quaternion.AngleAxis(num, Vector3.up), _drawingScale);
            Graphics.DrawMesh(mesh, _drawingMatrix, mat, 0);

            return false;
        }
    }
}
