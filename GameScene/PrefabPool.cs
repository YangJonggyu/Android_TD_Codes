using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PrefabPool : MonoBehaviour, IPunPrefabPool
{
    private void OnEnable()
    {
        PhotonNetwork.PrefabPool = this;
    }

    public readonly Dictionary<string, GameObject> ResourceCache = new Dictionary<string, GameObject>();
    public readonly Dictionary<string, List<GameObject>> Pool = new Dictionary<string, List<GameObject>>();

    private GameObject res;
    private List<GameObject> poolList;
    private GameObject instance;
    private PrefabPoolData hasPrefabPoolData;
    
    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        res = null;
        poolList = null;
        instance = null;
        
        bool cached = this.ResourceCache.TryGetValue(prefabId, out res);
        if (!cached)
        {
            res = Resources.Load<GameObject>(prefabId);
            if (!res)
            {
                Debug.LogError("DefaultPool failed to load \"" + prefabId + "\". Make sure it's in a \"Resources\" folder. Or use a custom IPunPrefabPool.");
            }
            else
            {
                this.ResourceCache.Add(prefabId, res);
                poolList = new List<GameObject>();
                this.Pool.Add(prefabId, poolList);
            }
        }
        else
        {
            bool pooled = this.Pool.TryGetValue(prefabId, out poolList);
            if (pooled && poolList.Count > 0)
            {
                instance = poolList[0];
                poolList.Remove(instance);
                instance.transform.position = position;
                instance.transform.rotation = rotation;
                if (instance.activeSelf) instance.SetActive(false);
                return instance;
            }
        }
        
        
        bool wasActive = res.activeSelf;
        if (wasActive) res.SetActive(false);

        instance = GameObject.Instantiate(res, position, rotation);

        hasPrefabPoolData = null;
        bool prefabpooled = instance.TryGetComponent(out hasPrefabPoolData);
        if (!prefabpooled) hasPrefabPoolData = instance.AddComponent<PrefabPoolData>();

        hasPrefabPoolData.prefabId = prefabId;

        if (wasActive) res.SetActive(true);
        return instance;
    }

    public void Destroy(GameObject gameObject)
    {
        gameObject.SetActive(false);
        Pool[gameObject.GetComponent<PrefabPoolData>().prefabId].Add(gameObject);
    }
}
