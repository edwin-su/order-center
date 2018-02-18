using OrderCenterClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrderCenterClient.Utilities
{
    public class CacheStore : ICache
    {
        public bool Contains(string key)
        {
            var result = HttpContext.Current.Session[key];
            return result != null;
        }

        public T Get<T>(string key)
        {
            var value = HttpContext.Current.Session[key];
            if (value == null)
            {
                // TODO:
                // throw exception
                if (IsNumberic<T>())
                {
                    return (T)Convert.ChangeType(-1, typeof(T));
                }

                return default(T);
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        private bool IsNumberic<T>()
        {
            return (typeof(T) == typeof(Int64) ||
                    typeof(T) == typeof(Int32) ||
                    typeof(T) == typeof(Decimal));
        }

        public bool TryGet<T>(string key, out T source)
        {
            var value = HttpContext.Current.Session[key];
            if (value == null)
            {
                if (IsNumberic<T>())
                {
                    source = (T)Convert.ChangeType(-1, typeof(T));
                }
                else
                {
                    source = default(T);
                }

                return false;
            }

            source = (T)value;
            return true;
        }

        public void Add<T>(string key, T source)
        {
            HttpContext.Current.Session[key] = source;
        }

        public void Remove(string key)
        {
            HttpContext.Current.Session.Remove(key);
        }
    }
}