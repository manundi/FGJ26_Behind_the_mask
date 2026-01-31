using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    public Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerTransform != null)
        {
        transform.position = Vector3.Lerp(transform.position, new Vector3(0, 0, playerTransform.position.z ), Time.deltaTime * 5f);
        }
    }
}
