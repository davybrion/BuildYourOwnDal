using System;
using System.Collections.Generic;

namespace BuildYourOwnDAL
{
    public class SessionLevelCache
    {
        private readonly Dictionary<Type, Dictionary<string, object>> cache = new Dictionary<Type, Dictionary<string, object>>();

        public object TryToFind(Type type, object id)
        {
            if (!cache.ContainsKey(type)) return null;

            string idAsString = id.ToString();
            if (!cache[type].ContainsKey(idAsString)) return null;

            return cache[type][idAsString];
        }

        public void Store(Type type, object id, object entity)
        {
            if (!cache.ContainsKey(type)) cache.Add(type, new Dictionary<string, object>());

            cache[type][id.ToString()] = entity;
        }

        public void ClearAll()
        {
            cache.Clear();
        }

        public void RemoveAllInstancesOf(Type type)
        {
            if (cache.ContainsKey(type))
            {
                cache.Remove(type);
            }
        }

        public void Remove(object entity)
        {
            var type = entity.GetType();

            if (!cache.ContainsKey(type)) return;

            string keyToRemove = null;

            foreach (var pair in cache[type])
            {
                if (pair.Value == entity)
                {
                    keyToRemove = pair.Key;
                }
            }

            if (keyToRemove != null)
            {
                cache[type].Remove(keyToRemove);
            }
        }
    }
}