using System.Collections.Generic;

namespace Softperson.Collections
{
    public class LruCache
    {
        struct Entry
        {
            public int Key;
            public int Value;
        }

        private readonly LinkedList<Entry> _list = new LinkedList<Entry>();
        private readonly Dictionary<int, LinkedListNode<Entry>> _hash = new Dictionary<int, LinkedListNode<Entry>>();
        public readonly int Capacity;

        public LruCache(int capacity)
        {
            this.Capacity = capacity;
        }

        public int Get(int key)
        {
            LinkedListNode<Entry> result;
            if (!_hash.TryGetValue(key, out result))
                return -1;
            _list.AddFirst(result);
            return result.Value.Value;
        }


        public void Set(int key, int value)
        {
            LinkedListNode<Entry> result;
            if (_hash.TryGetValue(key, out result))
                _list.Remove(result);

            _hash[key] = _list.AddFirst(new Entry {Key = key, Value = value});

            while (_hash.Count > Capacity)
            {
                var node = _list.Last;
                _list.Remove(node);
                _hash.Remove(node.Value.Key);
            }
        }


    }
}