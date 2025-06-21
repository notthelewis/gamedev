using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float projectileSpeed = 40f; 

    // Update is called once per frame
    void Update()
    {
        transform.Translate(projectileSpeed * Time.deltaTime * Vector3.forward);
    }
}
