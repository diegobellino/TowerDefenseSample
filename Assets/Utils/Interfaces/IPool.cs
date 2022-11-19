using UnityEngine;

namespace Utils.Interfaces
{
    public interface IPool
    {
        void PoolObject(GameObject poolObject, string id);
        GameObject RetrieveObject(string id);
    }

}