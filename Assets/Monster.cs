
using UnityEngine;

public class Monster : MonoBehaviour
{

    public float moveTimeStep = 0.3f;
    public float stepTimer = 0.0f;

    public float speedNotInSight = 2.0f;
    public float speedInSight = 1.0f;

    public float monsterInSightStep = 1.0f;


    public bool monsterInSight = true;
    public float smoothTime = 0.1f;

    Vector3 targetPosition;
    Animator animator;
    Vector3 lastPos;
    float currentVelocity = 0.0f;



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

        
        if (stepTimer >= moveTimeStep )
        {
            
            stepTimer = 0.0f;
            currentVelocity =speedInSight;
            
        }

        if(!monsterInSight)
        {
            currentVelocity = speedNotInSight;
        }
        
        if( currentVelocity > 0.0f)
        {
            currentVelocity -= Time.fixedDeltaTime * 5.0f; 
        }
        
        
}

    void Update()
    {
        // when monster becomes to sight it should slowly stop not instantly


       
        if (monsterInSight)
        {
            transform.position = Vector3.Lerp(transform.position,transform.position + Vector3.forward, Time.deltaTime * currentVelocity ); 
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.forward, Time.deltaTime *speedNotInSight); 
            targetPosition = transform.position;
        }
        animator.SetFloat("speed", (transform.position - lastPos).magnitude / Time.deltaTime);
        lastPos = transform.position;

    }

   
}
