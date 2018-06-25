using MeshEditor.Classes;
using MeshEditor.Editor.Helper;
using UnityEditor;
using UnityEngine;

namespace MeshEditor.Editor.Abstract
{
    [CustomEditor(typeof(LineVectorList))]
    public class LineVectorListEditor : AbstractLineVectorListEditor
    {
        protected override Vector3 DrawHandle(int index, Vector3 vector3, out Handler.DragHandleResult dhResult)
        {
            Handles.DrawSolidDisc(vector3, Vector3.forward, 0.2f);
            return Handler.DragHandle(vector3, 0.2f, Color.green, out dhResult);
        }

        public override void ThrowUndoRedo()
        {
            ((LineVectorList)target).UpdateCollider();
            InitializeVector();
        }
    }
}