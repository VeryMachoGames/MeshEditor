using System;
using System.Collections.Generic;
using System.Linq;
using MeshEditor.Helpers;
using UnityEngine;

namespace MeshEditor.Abstract
{
    [Serializable]
    [ExecuteInEditMode]
    public abstract partial class VectorList : MonoBehaviour
    {
        [SerializeField]
        public List<Vector3> LocalVector3Coords = new List<Vector3>();


        /// <summary>
        /// Initializes the basic data from the vector list
        /// </summary>
        void Reset()
        {
            InitializeData();
        }


        void Start()
        {
            if (transform.Find(NameHelper.ChildName))
            {
                if (Application.isPlaying)
                {
                    transform.Find(NameHelper.ChildName).gameObject.SetActive(false);
                }
                else
                {
                    transform.Find(NameHelper.ChildName).gameObject.SetActive(true);
                }
            }
        }

        public abstract void InitializeData();

        public abstract void OnAwakened();

        public abstract void MovedVertex(int vectorIndex, Vector3 newPosition);
        public abstract void InsertedVertex(int vectorIndex, Vector3 newPosition);
        public abstract void DeletedVertex(int vectorIndex);
        public abstract bool CanMoveVertex(List<int> vectorIndex, Vector3 newPosition);
        public abstract bool CanInsertVertex(Vector3 newPosition);
        public abstract bool CanDeleteVertex(int vectorIndex);


        /// <summary>
        /// Returns a world vector coords from the local vector coord list
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector3 GetWorldVectorCoord(int index)
        {
            return transform.TransformPoint(LocalVector3Coords[index]);
        }

        /// <summary>
        /// Sets a coordinate that will be translated from world coords to local coords
        /// </summary>
        /// <param name="index"></param>
        /// <param name="worldVector3Coord"></param>
        public void SetFromWorldVectorCoord(int index, Vector3 worldVector3Coord)
        {
            LocalVector3Coords[index] = transform.InverseTransformDirection(worldVector3Coord);
        }

        /// <summary>
        /// Inserts a world coord vector into the local coord list. It will be translated inside the method
        /// </summary>
        /// <param name="index"></param>
        /// <param name="worldVector3Coord"></param>
        public void InsertWorldVectorCoord(int index, Vector3 worldVector3Coord)
        {
            LocalVector3Coords.Insert(index, transform.InverseTransformDirection(worldVector3Coord));
        }
    }
}
