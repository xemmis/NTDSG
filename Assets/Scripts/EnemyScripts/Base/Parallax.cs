using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] Transform followTarget;
    [SerializeField, Range(0f, 1f)] float parallaxStrength = 0.1f;
    [SerializeField] bool disableVerticalParallax;
    Vector3 targetPreviousPosition;

    void Start()
    {
        if (!followTarget)
            followTarget = Camera.main.transform;

        targetPreviousPosition = followTarget.position;
    }

    void Update()
    {
        var delta = followTarget.position - targetPreviousPosition;
        if (disableVerticalParallax)
            delta.y = 0;

        targetPreviousPosition = followTarget.position;
        transform.position += delta * parallaxStrength;
    }
}
