using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    public abstract class EntitySystem : BaseSystem
    {
        protected abstract override void OnInitialize();
        protected abstract override void OnDispose();
        public abstract override void Tick();

        protected TData GetRandomData<TData>(IList<TData> datas) where TData : class
        {
            if (datas == null || datas.Count == 0)
                return null;

            int idx = Random.Range(0, datas.Count);
            return datas[idx];
        }

        protected List<TData> GetRandomDataList<TData>(IList<TData> datas, int amount)
        {
            var result = new List<TData>();

            if (datas == null || datas.Count == 0 || amount <= 0)
                return result;

            var clone = new List<TData>(datas);

            for (int i = 0; i < amount; i++)
            {
                if (clone.Count == 0) break;

                int idx = Random.Range(0, clone.Count);
                result.Add(clone[idx]);
                clone.RemoveAt(idx);
            }

            return result;
        }
    }
}
