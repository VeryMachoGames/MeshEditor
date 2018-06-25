using System.Collections.Generic;
using System.Linq;
using MeshEditor.Abstract;
using MeshEditor.Helpers;
using UnityEngine;

namespace MeshEditor.Classes
{
    [SelectionBase]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshFilter))]
    public class MeshColliderVectorList : VectorList
    {
        [Range(0.1f,10f)]
        public float MeshHeight = 0.25f;
        private float _meshHeightChecker;

        public override void InitializeData()
        {
            if (!LocalVector3Coords.Any())
            {
                LocalVector3Coords = new List<Vector3>
                {
                    new Vector3(-1, 1, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(1, -1, 0),
                    new Vector3(-1, -1, 0)
                };

                var meshCollider = GetComponent<MeshCollider>();
                var meshFilter = GetComponent<MeshFilter>();
                meshFilter.mesh = new Mesh();
                meshCollider.sharedMesh = meshFilter.sharedMesh;
                meshCollider.convex = true;
                _meshHeightChecker = MeshHeight;
            }
        }

        public override void OnAwakened()
        {
            UpdateMeshCollider();
        }

        public override void MovedVertex(int vectorIndex, Vector3 newPosition)
        {
            UpdateMeshCollider();
        }

        public override void InsertedVertex(int vectorIndex, Vector3 newPosition)
        {
            UpdateMeshCollider();
        }

        public override void DeletedVertex(int vectorIndex)
        {
            UpdateMeshCollider();
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
            if (LocalVector3Coords.Count == 3)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// This will be called when variable MeshHeight has changed
        /// among other things. If the variable has changed, then we
        /// will remake the meshcollider to acommodate it to the
        /// new defined height
        /// </summary>
        void OnValidate()
        {
            CheckMeshHeightChange();
        }

        private void CheckMeshHeightChange()
        {
            if (_meshHeightChecker != MeshHeight)
            {
                UpdateMeshCollider();
            }
        }


        /// <summary>
        /// Inserts the mesh data into the scene view
        /// </summary>
        public void UpdateMeshCollider()
        {
            var childGameObject = transform.Find(NameHelper.ChildName).gameObject;

            var childMesh = childGameObject.GetComponent<MeshFilter>().sharedMesh;

            var meshCollider = GetComponent<MeshCollider>();

            var meshFilter = GetComponent<MeshFilter>();

            var mesh = meshFilter.sharedMesh;

            mesh.Clear();
            mesh.vertices = CreateVerticesList(childMesh);
            mesh.triangles = CreateTrianglesArray(childMesh);

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }

        private Vector3[] CreateVerticesList(Mesh mesh)
        {
            var verticesList = mesh.vertices.ToList();

            var verticesCount = verticesList.Count;

            var halfHeight = MeshHeight / 2;

            for (int i = 0; i < verticesCount; i++)
            {
                var vertex = verticesList[i];

                verticesList[i] = new Vector3(vertex.x, vertex.y, vertex.z - halfHeight);

                verticesList.Add(new Vector3(vertex.x, vertex.y, vertex.z + halfHeight));
            }


            return verticesList.ToArray();
        }

        private int[] CreateTrianglesArray(Mesh childMesh)
        {
            var trianglesList = childMesh.triangles.ToList();

            var triangleCount = trianglesList.Count;

            for (int i = 0; i < triangleCount; i++)
            {
                trianglesList.Add(trianglesList[i] + triangleCount);
            }

            return trianglesList.ToArray();
        }
    }
}