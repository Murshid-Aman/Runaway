using UnityEngine;

public class BGScroller : MonoBehaviour
{
    public GameObject cam;
    public float parallaxSpeed;
    private float length, startPos;

    private void Start() 
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update() 
    {
        float temp = (cam.transform.position.x * (1-parallaxSpeed));
        float dist = (cam.transform.position.x * parallaxSpeed);
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        if (temp > startPos + length) startPos += length;
        else if (temp < startPos - length) startPos -= length;
    }
}
