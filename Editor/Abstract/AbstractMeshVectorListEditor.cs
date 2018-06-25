using System.Collections.Generic;
using MeshEditor.Extensions;
using MeshEditor.Helpers;
using MeshEditor.PolygonMaker;
using UnityEngine;


namespace MeshEditor.Editor.Abstract
{
    public abstract partial class AbstractMeshVectorListEditor : AbstractVectorListEditor
    {
        protected Vector3[] Vertexes;
        protected int[] Triangles;

        private static readonly string GameObjectName = NameHelper.ChildName;

        private GameObject ChildGameObject
        {
            get
            {
                var transform = ((MonoBehaviour)target).transform.Find(GameObjectName);

                if (transform == null)
                {
                    return null;
                }

                return transform.gameObject;
            }
        }


        public AbstractMeshVectorListEditor()
        {
            OnMoveVertex += UpdateMeshVector;
            OnInsertVertex += UpdateMeshVector;
            OnDeleteVertex += DeleteMeshVector;
            OnAwake += CreateMesh;
            OnEnabled += CreateMesh;
        }

        private void CheckGameObject()
        {
            if (!ChildGameObject)
            {
                GameObject childGameObject = new GameObject(GameObjectName);
                childGameObject.transform.parent = VectorListTarget.transform;
                childGameObject.transform.localPosition = new Vector3(0, 0, 0);
            }
        }

        private void DeleteMeshVector(int vectorIndex)
        {
            RecalculateMesh();
        }

        private void UpdateMeshVector(int vectorIndex, Vector3 newPosition)
        {
            RecalculateMesh();
        }

        protected void RecalculateMesh()
        {
            InitializeMeshData();
            FillMesh();
        }

        /// <summary>
        /// Chreates the basic components needed to work with the editor
        /// in case they doesn't exist.
        /// </summary>
        protected void CreateMesh()
        {
            CheckGameObject();

            // Checks if there's activeListEditor meshfilter,
            // if there's not one, it will be
            // deleted when the editor is disabled
            if (ChildGameObject.GetComponent<MeshFilter>() == null)
            {
                MeshFilter meshFilter = ChildGameObject.AddComponent<MeshFilter>();
                meshFilter.mesh = new Mesh();
            }

            // Checks if there's activeListEditor meshrenderer
            if (ChildGameObject.GetComponent<MeshRenderer>() == null)
            {
                var meshRenderer = ChildGameObject.AddComponent<MeshRenderer>();
                Material material = Resources.Load(NameHelper.MaterialName, typeof(Material)) as Material;
                meshRenderer.materials = new[] { material };
            }

            RecalculateMesh();
        }

        /// <summary>
        /// Creates activeListEditor vector and triangle list compatibles with mesh data.
        /// </summary>
        void InitializeMeshData()
        {
            List<CPoint2D> cPointList = new List<CPoint2D>();

            for (int i = 0; i < VectorListTarget.LocalVector3Coords.Count; i++)
            {
                var newCPoint2D = VectorListTarget.LocalVector3Coords[i].ToCPoint2D();
                cPointList.Add(newCPoint2D);
            }

            List<Vector3> verticesList = new List<Vector3>();
            List<int> trianglesList = new List<int>();

            var polygonShape = new CPolygonShape(cPointList.ToArray());

            polygonShape.CutEar();

            Vector3 newVector3;

            for (int i = 0; i < polygonShape.NumberOfPolygons; i++)
            {
                var polygonVertexes = polygonShape.Polygons(i);

                foreach (var polygonVertex in polygonVertexes)
                {
                    newVector3 = new Vector3((float)polygonVertex.X, (float)polygonVertex.Y, 0);

                    verticesList.Add(newVector3);

                    trianglesList.Add(verticesList.IndexOf(newVector3));
                }
            }

            Vertexes = verticesList.ToArray();
            Triangles = trianglesList.ToArray();
        }

        /// <summary>
        /// Inserts the mesh data into the scene view
        /// </summary>
        void FillMesh()
        {
            var mesh = ChildGameObject.GetComponent<MeshFilter>().sharedMesh;

            mesh.Clear();
            mesh.vertices = Vertexes;
            mesh.triangles = Triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }
}