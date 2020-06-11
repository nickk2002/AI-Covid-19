using UnityEditor;
using UnityEngine;

namespace Covid19.Learn.Custom_Editors.Editor
{
    [CustomEditor(typeof(Projectile))]
    public class ProjectileEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.Active | GizmoType.NonSelected)]
        static void DrawProjectileGizmos(Projectile projectile, GizmoType gizmoType)
        {
            Gizmos.DrawSphere(projectile.transform.position, 0.245f);
        }
        private void OnSceneGUI()
        {
            Projectile projectile = target as Projectile;
            projectile.damageRadius = Handles.RadiusHandle(projectile.transform.rotation, projectile.transform.position, projectile.damageRadius);

        }
    }
}
