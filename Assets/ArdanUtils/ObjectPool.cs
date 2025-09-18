using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

[System.Serializable]
public class Pool
{
    public string name;
    public GameObject go;
    public int count;
    public List<GameObject> actives;
    public Queue<GameObject> deactives;

    public Pool(string name, GameObject go, int count)
    {
        this.name = name;
        this.go = go;
        this.count = count;
    }

    public void InitaPool(Transform container, Dictionary<int, int> dicClones)
    {
        actives = new List<GameObject>();
        deactives = new Queue<GameObject>();
        for (int i = 0; i < count; i++)
        {
            spawnAClone(container, dicClones);
        }
    }

    void spawnAClone(Transform container, Dictionary<int, int> dicClones)
    {
        var clone = Object.Instantiate(go, container);
       // clone.transform.localScale = Vector3.one;
        clone.name += (actives.Count + deactives.Count);
        deactives.Enqueue(clone);
        dicClones.Add(clone.GetHashCode(), GetHashCode());
    }

    public GameObject Get(Transform container, Dictionary<int, int> dicClones)
    {
        if (deactives.Count == 0)
            spawnAClone(container, dicClones);
        var clone = deactives.Dequeue();
        actives.Add(clone);
        return clone;
    }

    public void Return(GameObject go)
    {
        
        go.SetActive(false);
        if (actives.Contains(go))
            actives.Remove(go);
        if (!deactives.Contains(go))
            deactives.Enqueue(go);
    }

    public void OnDestroy()
    {
        foreach (var active in actives)
        {
            if (active != null)
            {
                active.transform.DOKill();
            }

        }

        foreach (var deactive in deactives)
        {
            if (deactive != null)
            {
                deactive.transform.DOKill();
            }
        }
    }
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    public Pool rippleBig;
    public Pool ripple;
    public Pool parCoin;
    
    public Dictionary<int, int> dicClones = new Dictionary<int, int>();
    public List<Pool> pools = new List<Pool>();
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        pools.Add(rippleBig);
        pools.Add(ripple);
        pools.Add(parCoin);
        foreach (var pool in pools)
        {
            pool.InitaPool(transform, dicClones);
        }
    }

    public void ReturnAllPool()
    {
        foreach (var p in pools)
        {
            while (p.actives.Count > 0)
            {
                p.Return(p.actives[p.actives.Count - 1]);
            }
        }
    }

    public Pool TryAddPoolByScript(Pool p)
    {
        var existedPool = pools.Find(x => x.go == p.go);
        if (existedPool != null)
        {
            Debug.LogWarning($"existed pool: {p.go.name}", p.go.transform);
            return existedPool;
        }
        pools.Add(p);
        p.InitaPool(transform, dicClones);
        return p;
    }

    public GameObject Get(Pool p, bool active = true)
    {
        var obj = p.Get(transform, dicClones);
        //obj.transform.localScale = Vector3.one;
        obj.SetActive(active);
        return obj;
    }

    public Pool CheckAddPool(GameObject obj)
    {
        foreach (var p in pools)
        {
            if (p.go == obj)
            {
                return p;
            }
        }

        var pool = new Pool(obj.name, obj, 0);
        TryAddPoolByScript(pool);
        return pool;

    }

    public void Return(GameObject clone)
    {
        clone.transform.DOKill();
        var hash = clone.GetHashCode();
        if (dicClones.ContainsKey(hash))
        {
            var p = getPool(dicClones[hash]);
            p.Return(clone);
        }
        else
        {
            Debug.LogError(clone.transform.name, clone.transform);
        }
    }
    Pool getPool(int hash)
    {
        foreach (var pool in pools)
        {
            if (pool.GetHashCode() == hash)
                return pool;
        }

        return null;
    }

    private void OnDestroy()
    {
        foreach (var pool in pools)
        {
            pool.OnDestroy();
        }
    }
}