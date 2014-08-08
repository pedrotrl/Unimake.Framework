using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unimake.Data.Generic
{
    public sealed class DataDictionary: IDictionary<string, object>
    {
        #region locais
        IDictionary<string, object> dictionary = new Dictionary<string, object>();
        #endregion

        #region IDictionary<string, object>
        public void Add(string key, object value)
        {
            dictionary.Add(key.ToLower(), value);
        }

        public bool ContainsKey(string key)
        {
            return dictionary.ContainsKey(key.ToLower());
        }

        public ICollection<string> Keys
        {
            get { return dictionary.Keys; }
        }

        public bool Remove(string key)
        {
            return dictionary.Remove(key.ToLower());
        }

        public bool TryGetValue(string key, out object value)
        {
            return dictionary.TryGetValue(key.ToLower(), out value);
        }

        public ICollection<object> Values
        {
            get { return dictionary.Values; }
        }

        public object this[string key]
        {
            get { return dictionary[key.ToLower()]; }
            set { dictionary[key.ToLower()] = value; }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            dictionary.Add(item);
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            dictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return dictionary.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return dictionary.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return dictionary.Values.GetEnumerator();
        }
        #endregion
    }
}
