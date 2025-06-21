using UnityEngine;

public class SpinPropeller : MonoBehaviour
{
    [SerializeField] private float propellerSpeed = 10f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * propellerSpeed, Time.deltaTime);
    }
}
