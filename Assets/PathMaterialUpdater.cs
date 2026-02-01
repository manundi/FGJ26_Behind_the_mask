using System.IO;
using UnityEngine;

public class PathMaterialUpdater : MonoBehaviour
{
     public Renderer rend;
     Material mat;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       foreach (var mat in rend.materials)
        {
            if (mat.name.Contains("path"))
            {
                this.mat = mat;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {


        mat.SetVector("_PlayerPos", new Vector4( Game.instance.player.position.x, 0.0f, Game.instance.player.position.z, 0.0f ) );
    }
}
