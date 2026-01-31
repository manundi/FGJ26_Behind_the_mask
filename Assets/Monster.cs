using System.Diagnostics;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public float speed = 30.0f;
    public float moveTimeStep = 0.3f;
    public float stepTimer = 0.0f;

    public float monsterInSightStep = 1.0f;
    public float monsterNoInSightStep = 2.0f;

    public bool monsterInSight = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        print("Monster spawned");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        stepTimer += Time.fixedDeltaTime;

        if (stepTimer >= moveTimeStep)
        {
            stepTimer = 0.0f;
            if (monsterInSight)
            {
                transform.position += new Vector3(0, 0, speed * monsterInSightStep);
            }
            else
            {
                transform.position += new Vector3(0, 0, speed * monsterNoInSightStep);
            }
        }
    }
}
