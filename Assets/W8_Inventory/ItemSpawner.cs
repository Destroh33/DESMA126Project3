using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public Transform minX, maxX, spawnHeight, minZ, maxZ;
    public GameObject prefab; 

    public void SpawnNewItem(Sprite spr) {
    
        GameObject go = Instantiate(prefab);
        go.transform.position = new Vector3(Random.Range(minX.position.x, maxX.position.x), spawnHeight.position.y, Random.Range(minZ.position.z, maxZ.position.z));
        go.transform.rotation = Quaternion.Euler(new Vector3(90+Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3)));
        go.GetComponent<SpriteRenderer>().sprite = spr;
    }
}
