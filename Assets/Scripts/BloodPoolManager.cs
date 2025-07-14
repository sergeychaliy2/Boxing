using System.Collections.Generic;
using UnityEngine;

public class BloodPoolManager : MonoBehaviour
{
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private int poolSize = 20;

    private readonly Queue<GameObject> pool = new();

    public static BloodPoolManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var blood = Instantiate(bloodPrefab);
            blood.SetActive(false);
            pool.Enqueue(blood);
        }
    }

    public void SpawnBlood(Vector3 position, Quaternion rotation)
    {
        if (pool.Count == 0) return;

        var blood = pool.Dequeue();
        blood.transform.position = position;
        blood.transform.rotation = rotation;
        blood.SetActive(true);

        StartCoroutine(DespawnAfterDelay(blood, 1.5f));
    }

    private System.Collections.IEnumerator DespawnAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
