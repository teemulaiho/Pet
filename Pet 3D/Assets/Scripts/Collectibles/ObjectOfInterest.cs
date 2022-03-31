using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectOfInterestType
{
    Treasure,
    Count
}

public class ObjectOfInterest : MonoBehaviour
{
    [SerializeField] List<Item> itemList;
    [SerializeField] ObjectOfInterestType objectType;

    private void Awake()
    {
        itemList = new List<Item>();
        itemList.AddRange(Resources.LoadAll<Item>("ScriptableObjects"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            SpawnItem();
    }

    private void SpawnItem()
    {
        int randomIndex = Random.Range(0, itemList.Count);
        GameObject go = Instantiate(itemList[randomIndex].prefab, this.transform.position + new Vector3(0f, 2f, 0f), Quaternion.identity, null);

        if (go.GetComponent<Rigidbody>())
        {
            //go.GetComponent<Rigidbody>().AddExplosionForce(200f, go.transform.position, 2f, 3f, ForceMode.Impulse);

            float xForce = Random.Range(-3f, 3f) * 100f;
            float yForce = Random.Range(30f, 300f);
            float zForce = Random.Range(-3f, 3f) * 100f;
            go.GetComponent<Rigidbody>().AddForce(new Vector3(xForce, yForce, zForce));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pet"))
        {
            SpawnItem();
        }
    }
}
