using System;
using System.Collections;
using System.Collections.Generic;

namespace MeshEditor.Editor.Helper
{
    public class VertexOrderList : ICollection<int>
    {
        private List<int> _orderList = new List<int>();

        public VertexOrderList(int elements)
        {
            Fill(elements);
        }

        public bool PushLast(int item)
        {
            if (_orderList.Remove(item))
            {
                _orderList.Add(item);
                return true;
            }

            return false;
        }

        public void Fill(int elements)
        {
            _orderList.Clear();

            for (int i = 0; i < elements; i++)
            {
                _orderList.Add(i);
            }
        }

        public int this[int index]
        {
            get { return _orderList[index]; }
        }


        #region Interface Implementation

        public IEnumerator<int> GetEnumerator()
        {
            return _orderList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(int item)
        {
            _orderList.Add(item);
        }

        public void Clear()
        {
            _orderList.Clear();
        }

        public bool Contains(int item)
        {
            return _orderList.Contains(item);
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            _orderList.CopyTo(array, arrayIndex);
        }

        public bool Remove(int item)
        {
            return _orderList.Remove(item);
        }

        public int Count
        {
            get { return _orderList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion

   
    }
}