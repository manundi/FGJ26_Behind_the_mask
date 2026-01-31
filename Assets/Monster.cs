
using UnityEngine;

public class Monster : MonoBehaviour
{

    public float moveTimeStep = 0.3f;
    public float stepTimer = 0.0f;

    public float speedNotInSight = 2.0f;

    public float monsterInSightStep = 1.0f;


    public bool monsterInSight = true;
    public float smoothTime = 0.1f;

    Vector3 targetPosition;
    Animator animator;
    Vector3 lastPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPosition = transform.position;
       
        monsterInSight = true;
        animator = GetComponent<Animator>();
    
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        stepTimer += Time.fixedDeltaTime;

        if (stepTimer >= moveTimeStep)
        {
            
            stepTimer = 0.0f;
            targetPosition = transform.position + new Vector3(0, 0,  monsterInSightStep);
            
        }
    }

    void Update()
    {
       
        if (monsterInSight)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime ); 
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, transform.forward, Time.deltaTime *speedNotInSight); 
            targetPosition = transform.position;
        }
        animator.SetFloat("speed", (transform.position - lastPos).magnitude / Time.deltaTime);
        lastPos = transform.position;

    }
}
