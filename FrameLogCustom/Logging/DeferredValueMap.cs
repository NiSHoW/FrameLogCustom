using System;
using System.Collections.Generic;
using FrameLog.Exceptions;

namespace FrameLog.Logging
{
    public class DeferredValueMap
    {
        private Dictionary<string, Tuple<DeferredValue, DeferredValue>> map;
        private object container;

        public DeferredValueMap(object container = null)
        {
            this.map = new Dictionary<string, Tuple<DeferredValue, DeferredValue>>();
            this.container = container;
        }

        public void Store(string key, Func<object> futureValue, Func<object> oldFutureValue)
        {
            map[key] = new Tuple<DeferredValue, DeferredValue>(new DeferredValue(futureValue), new DeferredValue(oldFutureValue));
        }
        public IDictionary<string, Tuple<object, object>> CalculateAndRetrieve()
        {
            var result = new Dictionary<string, Tuple<object, object>>();
            foreach (var kv in map)
            {
                try
                {
                    result[kv.Key] = new Tuple<object, object>(
                        kv.Value.Item1.CalculateAndRetrieve(),
                        kv.Value.Item2.CalculateAndRetrieve()
                    );
                }
                catch (Exception e)
                {
                    throw new ErrorInDeferredCalculation(container, kv.Key, e);
                }
            }
            return result;
        }
    }
}
