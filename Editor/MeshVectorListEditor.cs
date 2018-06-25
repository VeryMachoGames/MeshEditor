using MeshEditor.Classes;
using MeshEditor.Editor.Helper;
using UnityEditor;
using UnityEngine;

namespace MeshEditor.Editor.Abstract
{
    [CustomEditor(typeof(EdgeColliderVectorList))]
    public class MeshVectorListEditor : AbstractMeshVectorListEditor
    {
        protected override Vector3 DrawHandle(int index, Vector3 vector3, out Handler.DragHandleResult dhResult)
        {
            Handles.DrawSolidDisc(vector3, Vector3.forward, 0.2f);

            if (SelectedVectors.Contains(index))
            {
                Handles.color = Color.black;
                Handles.DrawSolidDisc(vector3, new Vector3(0, 0, 1), 0.1f);
                Handles.color = Color.white;
            }

            Handles.color = Color.white;
            return Handler.DragHandle(vector3, 0.2f, Color.green, out dhResult);
        }

        public override void ThrowUndoRedo()
        {
            ((EdgeColliderVectorList)target).UpdateCollider();

            InitializeVector();
            RecalculateMesh();
        }
    }
}