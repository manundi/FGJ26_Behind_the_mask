using UnityEngine;

public class PathMaterialUpdater : MonoBehaviour
{
     public Renderer rend;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        Material mat = rend.materials[1];
        mat.SetVector("_PlayerPos", new Vector4( Game.instance.playerController.transform.position.x, 0.0f, Game.instance.playerController.transform.position.z, 0.0f ) );
    }
}
