using System.Collections;
using System.Collections.Generic;

namespace RePKG.Native
{
    public unsafe class WCList<TStruct, TItem> : IList<TItem> where TStruct : unmanaged
    {
        private readonly int* _count;
        private readonly TStruct** _pointerToArrayPointer;
        private TStruct* _lastAddress;
        private readonly NativeEnvironment _environment;
        private readonly List<TItem> _items;
        private readonly CStructToItem _cStructToItemFunc;

        public WCList(
            int* count,
            TStruct** pointerToArrayPointer,
            NativeEnvironment environment,
            CStructToItem cStructToItemFunc)
        {
            _count = count;
            _pointerToArrayPointer = pointerToArrayPointer;
            _environment = environment;
            _items = new List<TItem>();
            _cStructToItemFunc = cStructToItemFunc;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            Refresh();
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TItem item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(TItem item)
        {
            Refresh();
            return _items.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            Refresh();
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(TItem item)
        {
            throw new System.NotImplementedException();
        }

        public int Count => *_count;
        public bool IsReadOnly => true; // TODO: Change after implementing 

        public int IndexOf(TItem item)
        {
            Refresh();
            return _items.IndexOf(item);
        }

        public void Insert(int index, TItem item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public TItem this[int index]
        {
            get
            {
                Refresh();
                return _items[index];
            }
            set => throw new System.NotImplementedException();
        }

        private void Refresh()
        {
            var address = *_pointerToArrayPointer;
            
            if (_lastAddress == address)
                return;
            
            _lastAddress = address;
            _items.Clear();
            
            if (address == null)
                return;

            for (var i = 0; i < *_count; i++)
            {
                _items.Add(_cStructToItemFunc(&address[i], _environment));
            }
        }

        public delegate TItem CStructToItem(TStruct* item, NativeEnvironment e);
    }
}