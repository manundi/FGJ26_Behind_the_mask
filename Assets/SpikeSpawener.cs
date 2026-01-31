using UnityEngine;

public class SpikeSpawener : MonoBehaviour
{
    int currrentRow = 0;
    int spikesOnRow = 6;
    public GameObject spikePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnSpikeRows(20);
    }

    // Update is called once per frame
    void Update()
    {
        if(Game.instance.playerController.transform.position.z > currrentRow - 10)
        {
          SpawnSpikeRows(20);
        }
    }

    void SpawnSpikeRows(int amount)
    {
     for (int i = 0; i < amount; i++){
            for (int j = 0; j < spikesOnRow; j++)
            {
                GameObject spike = Instantiate(spikePrefab, new Vector3(j - spikesOnRow / 2, 0, currrentRow), Quaternion.identity);
                spike.transform.parent = transform;
            }
            
        currrentRow ++;
        }
    }
}
