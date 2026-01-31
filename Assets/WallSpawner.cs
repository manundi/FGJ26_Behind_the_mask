using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public List<GameObject> walls = new List<GameObject>();
    int currrentRow = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Game.instance.playerController.transform.position.z > currrentRow * 5 - 10)
        {
          SpawnWalls(20);
  
        }
    }


    void SpawnWalls(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject wall = Instantiate(walls[Random.Range(0, walls.Count)],transform.position + new Vector3(0, 0, currrentRow * 5 ), Quaternion.identity);
            wall.transform.parent = transform;
            currrentRow ++;
      
        }
    }
}
