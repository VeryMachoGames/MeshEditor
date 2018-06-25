using System.Collections.Generic;
using System.Linq;
using MeshEditor.Abstract;
using MeshEditor.Editor.Helper;
using MeshEditor.Extensions;
using UnityEditor;
using UnityEngine;

namespace MeshEditor.Editor.Abstract
{
    public delegate void VertexChangedHandler(int vectorIndex, Vector3 newPosition);

    public delegate void VertexDeletedHandler(int vectorIndex);

    public delegate void ClassEventHandler();

    public abstract partial class AbstractVectorListEditor : UnityEditor.Editor
    {
        public event VertexChangedHandler OnMoveVertex;
        public event VertexChangedHandler OnInsertVertex;
        public event VertexDeletedHandler OnDeleteVertex;
        public event ClassEventHandler OnAwake;
        public event ClassEventHandler OnDestroyed;
        public event ClassEventHandler OnEnabled;
        public event ClassEventHandler OnDisabled;
        public event ClassEventHandler OnSceneGUITriggered;


        /// <summary>
        /// BOOLS
        /// </summary>
        protected bool CanCreateCircle = true;

        protected bool MovingCircle;
        protected bool CreatingCircle;
        protected bool StartMovingCircleLMB;

        protected VertexOrderList VertexOrderList;

        protected VectorList VectorListTarget;
        protected Transform TargetTransform;
        protected Vector3 StartCirclePosition;
        protected float MinimumCreationDistance = 1;

        protected List<int> SelectedVectors = new List<int>();

        protected DistanceChecker DistanceChecker = new DistanceChecker();

        protected Quaternion RotationChecker;


        #region Messages

        public void Awake()
        {
            InitializeVector();
            OnAwakeTrigger();

            EditorHelper.activeListEditor = this;

            Undo.undoRedoPerformed -= EditorHelper.LaunchRedoUndo;

            Undo.undoRedoPerformed += EditorHelper.LaunchRedoUndo;
        }

        public void OnDestroy()
        {
            OnDestroyTrigger();
        }

        public void OnEnable()
        {
            InitializeVector();

            OnEnableTrigger();
        }

        public void OnDisable()
        {
            OnDisableTrigger();
        }

        #endregion

        #region Methods

        public void OnSceneGUI()
        {
            Vector3 sVector3;
            Handler.DragHandleResult dhResult;

            for (int j = 0; j < VertexOrderList.Count; j++)
            {
                var i = VertexOrderList[j];

                sVector3 = VectorListTarget.GetWorldVectorCoord(i);

                Vector3 newPosition  =  DrawHandle(i, sVector3, out dhResult);

              //  Vector3 newPosition = Handler.DragHandle(sVector3, 0.5f, Color.green, out dhResult);

                HandleKnob(dhResult, i, newPosition);
            }

            OnSceneGUITrigger();
        }

        protected abstract Vector3 DrawHandle(int index, Vector3 vector3, out Handler.DragHandleResult dhResult);

        /// <summary>
        /// Checks if there's an event triggered to change the vectorList
        /// </summary>
        /// <param name="dhResult"></param>
        /// <param name="vectorIndex"></param>
        /// <param name="secondVector3"></param>
        private void HandleKnob(Handler.DragHandleResult dhResult, int vectorIndex, Vector3 secondVector3)
        {
            if (dhResult == Handler.DragHandleResult.none)
            {
                return;
            }

            switch (dhResult)
            {
                case Handler.DragHandleResult.LMBRelease:

                    StartMovingCircleLMB = false;

                    break;
                case Handler.DragHandleResult.LMBDrag:

                    var clearAtEnd = false;

                    if (StartMovingCircleLMB == false)
                    {
                        Undo.RecordObject(target, "Moving vertex");
                        StartMovingCircleLMB = true;
                    }

                    if (!SelectedVectors.Any() || !SelectedVectors.Contains(vectorIndex))
                    {
                        SelectedVectors = new List<int>() { vectorIndex };
                        clearAtEnd = true;
                    }

                    if (VectorListTarget.CanMoveVertex(SelectedVectors, secondVector3))
                    {
                        MoveVertex(vectorIndex, SelectedVectors, secondVector3);
                    }

                    if (clearAtEnd)
                    {
                        ClearSelectedVectors();
                    }

                    break;

                case Handler.DragHandleResult.RMBDrag:
                    if (VectorListTarget.CanInsertVertex(secondVector3))
                    {
                        if (CanCreateCircle)
                        {
                            StartCirclePosition = VectorListTarget.GetWorldVectorCoord(vectorIndex);

                            int nextIndex = VectorListTarget.LocalVector3Coords.NextIndex(vectorIndex);
                            int previousIndex = VectorListTarget.LocalVector3Coords.PreviousIndex(vectorIndex);

                            DistanceChecker.InitialNextVertexDistance =
                                DistanceChecker.CheckDistance(StartCirclePosition,
                                    VectorListTarget.GetWorldVectorCoord(nextIndex));
                            DistanceChecker.InitialPreviousVertexDistance =
                                DistanceChecker.CheckDistance(StartCirclePosition,
                                    VectorListTarget.GetWorldVectorCoord(previousIndex));

                            CreatingCircle = true;
                            CanCreateCircle = false;
                            MovingCircle = false;
                        }
                        else if (CreatingCircle && CreationMovingDistanceReached(secondVector3))
                        {
                            int nextIndex = VectorListTarget.LocalVector3Coords.NextIndex(vectorIndex);
                            int previousIndex = VectorListTarget.LocalVector3Coords.PreviousIndex(vectorIndex);

                            DistanceChecker.FinalNextVertexDistance = DistanceChecker.CheckDistance(secondVector3,
                                VectorListTarget.GetWorldVectorCoord(nextIndex));
                            DistanceChecker.FinalPreviousVertexDistance = DistanceChecker.CheckDistance(secondVector3,
                                VectorListTarget.GetWorldVectorCoord(previousIndex));

                            InsertVertex(vectorIndex, secondVector3);
                            CreatingCircle = false;
                            MovingCircle = true;
                        }
                        else if (MovingCircle)
                        {
                            MoveVertex(vectorIndex, new List<int>() { vectorIndex }, secondVector3);
                        }
                    }

                    break;

                case Handler.DragHandleResult.RMBRelease:
                    ClearSelectedVectors();
                    CanCreateCircle = true;
                    MovingCircle = false;
                    ResetVertexOrderList();
                    break;
                case Handler.DragHandleResult.LMBDoubleClick:

                    if (VectorListTarget.CanDeleteVertex(vectorIndex))
                    {
                        DeleteVertex(vectorIndex);
                    }

                    ClearSelectedVectors();
                    break;

                case Handler.DragHandleResult.LMBClick:

                    if (Event.current.control)
                    {
                        // Unselect element
                        if (SelectedVectors.Contains(vectorIndex))
                        {
                            SelectedVectors.Remove(vectorIndex);
                        }
                        // Select element
                        else
                        {
                            SelectedVectors.Add(vectorIndex);
                        }
                    }
                    else
                    {
                        ClearSelectedVectors();
                    }

                    break;
            }
        }

        private void ResetVertexOrderList()
        {
            VertexOrderList.Fill(VectorListTarget.LocalVector3Coords.Count);
        }

        /// <summary>
        /// Deletes the vertex passed via parameter in the VectorList
        /// </summary>
        /// <param name="vectorIndex"></param>
        private void DeleteVertex(int vectorIndex)
        {
            Undo.RecordObject(target, "Delete vertex");

            VectorListTarget.LocalVector3Coords.RemoveAt(vectorIndex);
            ResetVertexOrderList();
            VertexDeleted(vectorIndex);
            EditorHelper.MakeDirty(VectorListTarget, true);
        }


        /// <summary>
        /// Move selected vertrexes. Will work inside the method with LOCAL coordinates.
        /// </summary>
        /// <param name="selectedVectorIndex">Index of the vector that we are moving
        ///         (even if we have more than one selected vector, we will be moving only one, and the rest will come with it)</param>
        /// <param name="vectorIndexList">List of selected vectors that have to move</param>
        /// <param name="newWorldPosition">Mouse position, world coordinates</param>
        private void MoveVertex(int selectedVectorIndex, List<int> vectorIndexList, Vector3 newWorldPosition)
        {
            // calculate the distance the mouse has moved from the original vertex position
            // we will be working only with local coordinates in this method!!!
            var preMovedDistance = VectorListTarget.transform.InverseTransformPoint(newWorldPosition) - VectorListTarget.LocalVector3Coords[selectedVectorIndex];
            var movedDistance = new Vector3(preMovedDistance.x, preMovedDistance.y, 0);

            foreach (int vectorIndex in vectorIndexList)
            {
                _moveVertex(vectorIndex,
                    VectorListTarget.LocalVector3Coords[vectorIndex] + movedDistance);
            }

            EditorHelper.MakeDirty(VectorListTarget);
        }

        /// <summary>
        /// Will change que local index position into a new one.
        /// </summary>
        /// <param name="vectorIndex"></param>
        /// <param name="newLocalPosition"></param>
        private void _moveVertex(int vectorIndex, Vector3 newLocalPosition)
        {
            VectorListTarget.LocalVector3Coords[vectorIndex] =  newLocalPosition;
            VertexMoved(vectorIndex, newLocalPosition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectorIndex"></param>
        /// <param name="finalPosition"></param>
        private void InsertVertex(int vectorIndex, Vector3 finalPosition)
        {
            var nextDist = DistanceChecker.InitialNextVertexDistance - DistanceChecker.FinalNextVertexDistance;
            var previousDist = DistanceChecker.InitialPreviousVertexDistance -
                               DistanceChecker.FinalPreviousVertexDistance;

            int insertIndex;


            if (nextDist >= previousDist)
            {
                insertIndex = vectorIndex + 1;
            }
            else
            {
                insertIndex = vectorIndex;
            }

            Undo.RecordObject(target, "Insert vertex");

            // Since we work only with local coords inside the method, it's not really necessary to use world coords
            VectorListTarget.LocalVector3Coords.Insert(insertIndex, VectorListTarget.LocalVector3Coords[vectorIndex]);

            VertexOrderList = new VertexOrderList(VectorListTarget.LocalVector3Coords.Count);

            if (insertIndex == vectorIndex)
            {
                VertexOrderList.PushLast(vectorIndex + 1);
            }
            else
            {
                VertexOrderList.PushLast(vectorIndex);
            }

            VertexInserted(vectorIndex, finalPosition);
            EditorHelper.MakeDirty(VectorListTarget);
        }

        /// <summary>
        /// Distance right click mouse must be dragged to insert activeListEditor new vertex
        /// </summary>
        /// <param name="movingVector"></param>
        /// <returns></returns>
        private bool CreationMovingDistanceReached(Vector3 movingVector)
        {
            var checker = StartCirclePosition - movingVector;
            return checker.sqrMagnitude > MinimumCreationDistance;
        }

        /// <summary>
        /// Defines the _vectorListTarget member and adds its events into the editor.
        /// </summary>
        protected void InitializeVector()
        {
            VectorListTarget = target as VectorList;
            TargetTransform = VectorListTarget.transform;


            OnAwake -= VectorListTarget.OnAwakened;
            OnMoveVertex -= VectorListTarget.MovedVertex;
            OnInsertVertex -= VectorListTarget.InsertedVertex;
            OnDeleteVertex -= VectorListTarget.DeletedVertex;

            OnAwake += VectorListTarget.OnAwakened;
            OnMoveVertex += VectorListTarget.MovedVertex;
            OnInsertVertex += VectorListTarget.InsertedVertex;
            OnDeleteVertex += VectorListTarget.DeletedVertex;

            VertexOrderList = new VertexOrderList(VectorListTarget.LocalVector3Coords.Count);

            RotationChecker = VectorListTarget.transform.rotation;
        }

        /// <summary>
        /// Unselects all selected vertex
        /// </summary>
        private void ClearSelectedVectors()
        {
            SelectedVectors = new List<int>();
        }

        #endregion


        #region ExternalMessages 

        //public void AlignSelectedVertexVertical()
        //{
        //    if (SelectedVectors.Any())
        //    {
        //        List<Vector3> selectedVector3Values =
        //            VectorListTarget.LocalVector3Coords.Where((p, i) => SelectedVectors.Contains(i)).ToList();

        //        float maxValue = selectedVector3Values.Max(p => p.y);
        //        float minValue = selectedVector3Values.Min(p => p.y);

        //        float distance = maxValue / minValue;
        //        float position = maxValue - distance;

        //        foreach (int index in SelectedVectors)
        //        {
        //            var vector = VectorListTarget.GetWorldVectorCoord(index);

                        /// <summary>
                        ///  hay que cambiar esta parte a coordenadas locales!!!!!
                        /// </summary>
        //            _moveVertex(index, new Vector3(vector.x,
        //                position, vector.z));
        //        }
        //    }
        //}

        //public void AlignSelectedVertexHorizontal()
        //{
        //    if (SelectedVectors.Any())
        //    {
        //        List<Vector3> selectedVector3Values =
        //            VectorListTarget.LocalVector3Coords.Where((p, i) => SelectedVectors.Contains(i)).ToList();

        //        float maxValue = selectedVector3Values.Max(p => p.x);
        //        float minValue = selectedVector3Values.Min(p => p.x);

        //        float distance = maxValue / minValue;
        //        float position = maxValue - distance;

        //        foreach (int index in SelectedVectors)
        //        {
        //            var vector = VectorListTarget.GetWorldVectorCoord(index);

        //            /// <summary>
        //            ///  hay que cambiar esta parte a coordenadas locales!!!!!
        //            /// </summary>
        //            _moveVertex(index, new Vector3(position,
        //                vector.y, vector.z));
        //        }
        //    }
        //}

        #endregion


        #region Events

        protected void VertexMoved(int vectorIndex, Vector3 newPosition)
        {
            if (OnMoveVertex != null)
            {
                OnMoveVertex(vectorIndex, newPosition);
            }
        }

        protected void VertexDeleted(int vectorIndex)
        {
            if (OnDeleteVertex != null)
            {
                OnDeleteVertex(vectorIndex);
            }
        }

        protected void VertexInserted(int vectorIndex, Vector3 newPosition)
        {
            if (OnInsertVertex != null)
            {
                OnInsertVertex(vectorIndex, newPosition);
            }
        }

        protected void OnAwakeTrigger()
        {
            if (OnAwake != null)
            {
                OnAwake();
            }
        }

        protected void OnDestroyTrigger()
        {
            if (OnDestroyed != null)
            {
                OnDestroyed();
            }
        }

        protected void OnEnableTrigger()
        {
            if (OnEnabled != null)
            {
                OnEnabled();
            }
        }

        protected void OnDisableTrigger()
        {
            if (OnDisabled != null)
            {
                OnDisabled();
            }
        }

        protected void OnSceneGUITrigger()
        {
            if (OnSceneGUITriggered != null)
            {
                OnSceneGUITriggered();
            }
        }

        public abstract void ThrowUndoRedo();

        #endregion
    }
}