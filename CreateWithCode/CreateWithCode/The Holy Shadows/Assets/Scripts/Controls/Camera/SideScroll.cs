using UnityEngine;

public class SideScroll : MonoBehaviour
{

    // Make public so that other scripts may increase/decrease scroll speed 
    public float scrollSpeed = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Scroll the Y axis, from left-right
        transform.Translate(0, 0, 0 - scrollSpeed * Time.deltaTime);
    }
}
