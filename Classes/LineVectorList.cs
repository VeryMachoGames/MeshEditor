using System.Collections.Generic;
using System.Linq;
using MeshEditor.Abstract;
using MeshEditor.Extensions;
using UnityEngine;

namespace MeshEditor.Classes
{
    [SelectionBase]
    [RequireComponent(typeof(EdgeCollider2D))]
    public class LineVectorList : VectorList
    {
        public override void InitializeData()
        {
            if (!LocalVector3Coords.Any())
            {
                LocalVector3Coords = new List<Vector3>
                {
                    new Vector3(-1, 1, 0),
                    new Vector3(1, 1, 0),
                };

                var edgeCollider = gameObject.GetComponent<EdgeCollider2D>();
                edgeCollider.edgeRadius = 0.05f;
            }
        }

        public override void OnAwakened()
        {
            UpdateCollider();
        }

        public override void MovedVertex(int vectorIndex, Vector3 newPosition)
        {
            UpdateCollider();
        }

        public override void InsertedVertex(int vectorIndex, Vector3 newPosition)
        {
            UpdateCollider();
        }

        public override void DeletedVertex(int vectorIndex)
        {
            UpdateCollider();
        }

        public override bool CanMoveVertex(List<int> vectorIndex, Vector3 newPosition)
        {
            return true;
        }

        public override bool CanInsertVertex(Vector3 newPosition)
        {
            return true;
        }

        public override bool CanDeleteVertex(int vectorIndex)
        {
            if (LocalVector3Coords.Count == 2)
            {
                return false;
            }

            return true;
        }

        public void UpdateCollider()
        {
            var edgeCollider2D = GetComponent<EdgeCollider2D>();

            List<Vector2> vector2List = LocalVector3Coords.ToVector2List();

            edgeCollider2D.points = vector2List.ToArray();
        }

    }
}