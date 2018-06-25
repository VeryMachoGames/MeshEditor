using MeshEditor.Classes;
using MeshEditor.Editor.Helper;
using UnityEditor;
using UnityEngine;

namespace MeshEditor.Editor.Abstract
{
    [CustomEditor(typeof(MeshColliderVectorList))]
    public class MultipleMeshVectorListEditor : AbstractMeshVectorListEditor
    {
        protected override Vector3 DrawHandle(int index, Vector3 vector3, out Handler.DragHandleResult dhResult)
        {
            Handles.SphereHandleCap(vector3.GetHashCode(), vector3, Quaternion.identity, 0.2f, EventType.Repaint);

            return Handler.DragHandle(vector3, 0.1f, Color.green, out dhResult);
        }

        public override void ThrowUndoRedo()
        {
            ((MeshColliderVectorList)target).UpdateMeshCollider();

            InitializeVector();
            RecalculateMesh();
        }

    }
}