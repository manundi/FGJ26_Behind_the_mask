using System.Diagnostics;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float speed = 0.3f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("Monster spawned");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += new Vector3(0, 0, speed) * Time.fixedDeltaTime;
    }
}
