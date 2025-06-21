using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private Vector3 offset;

    void Start()
    {
        if (target == null) throw new System.Exception("Missing target...");
        offset = target.transform.position - transform.position;
    }

    void Update()
    {
        transform.position = target.transform.position - offset; 
    }
}
