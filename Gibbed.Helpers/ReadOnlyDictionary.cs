using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Gibbed.Helpers
{
    public sealed class ReadOnlyDictionary<TKey, TValue> :
        IDictionary<TKey, TValue>,
        ICollection<KeyValuePair<TKey, TValue>>,
        IEnumerable<KeyValuePair<TKey, TValue>>,
        IEnumerable
    {
        private IDictionary<TKey, TValue> Source;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> source)
        {
            this.Source = source;
        }

        #region IDictionary<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        public bool ContainsKey(TKey key)
        {
            return this.Source.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get
            {
                return new ReadOnlyCollection<TKey>(new List<TKey>(this.Source.Keys));
            }
        }

        public bool Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.Source.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get
            {
                return new ReadOnlyCollection<TValue>(new List<TValue>(this.Source.Values));
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                return this.Source[key];
            }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                return this[key];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.Source.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this.Source.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return this.Source.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (KeyValuePair<TKey, TValue> item in this.Source)
            {
                yield return item;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
