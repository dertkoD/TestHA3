using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private int poolSize = 10;

    private List<GameObject> pool = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.SetActive(false);
            pool.Add(arrow);
        }
    }

    public GameObject GetArrow()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                return pool[i];
            }
        }

        // if we need infinity pool of arrows
        GameObject newArrow = Instantiate(arrowPrefab);
        pool.Add(newArrow);
        return newArrow;
    }
}