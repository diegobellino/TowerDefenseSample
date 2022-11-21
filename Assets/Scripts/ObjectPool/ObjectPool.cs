using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Interfaces;

namespace TowerDefense.ObjectPool
{
    public class ObjectPool : MonoBehaviour, IPool
    {
        #region VARIABLES


        [System.Serializable]
        private struct PoolConfig
        {
            public GameObject Poolable;
            public int InitialSize;
        }

        [SerializeField] private List<PoolConfig> poolConfigs = new List<PoolConfig>();

        private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

        private bool initializingPool;
        private Vector3 UnpooledPosition = new Vector3(-10f, 0f, -10f);

        #endregion

        private void Awake()
        {
            if (initializingPool)
            {
                return;
            }

            initializingPool = true;
            InstantiatePool();
        }

        private void InstantiatePool()
        {
            foreach (var config in poolConfigs)
            {
                for (int i = 0; i < config.InitialSize; i++)
                {
                    var pooledObject = Instantiate(config.Poolable);
                    var poolable = pooledObject.GetComponent<IPoolable>();
                    poolable.Pool = this;

                    PoolObject(pooledObject, poolable.PoolId);
                }
            }

            initializingPool = false;
        }

        public void PoolObject(GameObject pooledObject, string poolId)
        {
            pooledObject.SetActive(false);
            pooledObject.transform.position = UnpooledPosition;
            pooledObject.transform.SetParent(transform);

            if (pools.ContainsKey(poolId))
            {
                pools[poolId].Enqueue(pooledObject);
            }
            else
            {
                var newPool = new Queue<GameObject>();
                newPool.Enqueue(pooledObject);
                pools.Add(poolId, newPool);
            }
        }

        public GameObject RetrieveObject(string poolId)
        {
            if (pools.ContainsKey(poolId))
            {
                var objects = pools[poolId];
                
                if (objects.Count > 0)
                {
                    return objects.Dequeue();
                }
            }

            Debug.Log($"ObjectPool: Run out of pooled elements for {poolId}");
            return null;
        }
    }
}
