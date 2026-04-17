using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ParallaxEffect : MonoBehaviour
{
    private Vector2 size, startpos;
    public GameObject cam;
    public float parallaxFactor;
    public float PixelsPerUnit;
 
    void Start()
    {
        startpos = transform.position;
        size = GetComponent<TilemapRenderer>().bounds.size;
    }
 
    void Update()
    {
        float temp     = cam.transform.position.x * (1 - parallaxFactor);
        Vector2 distance = cam.transform.position * parallaxFactor;

        Vector3 newPosition = startpos + distance;

        transform.position = newPosition; //PixelPerfectClamp(newPosition, PixelsPerUnit);
 
        /*
        if (temp > startpos + (length / 2))      startpos += length;
        else if (temp < startpos - (length / 2)) startpos -= length;
    */
    }
 
    private Vector3 PixelPerfectClamp(Vector3 locationVector, float pixelsPerUnit)
    {
        
        Vector3 vectorInPixels = new Vector3(Mathf.CeilToInt(locationVector.x * pixelsPerUnit), Mathf.CeilToInt(locationVector.y * pixelsPerUnit),Mathf.CeilToInt(locationVector.z * pixelsPerUnit));
        return vectorInPixels / pixelsPerUnit;
    }
      
}