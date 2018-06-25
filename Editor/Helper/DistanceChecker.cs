using UnityEngine;

namespace MeshEditor.Editor.Helper
{
    public class DistanceChecker
    {
        public float InitialNextVertexDistance;
        public float InitialPreviousVertexDistance;
        public float FinalNextVertexDistance;
        public float FinalPreviousVertexDistance;

        public float CheckDistance(Vector3 start, Vector3 end)
        {
            return Vector3.Distance(start, end);
        }




    }
}