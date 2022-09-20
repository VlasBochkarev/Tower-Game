using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform camTransform;
    private float shakeDur = 1f, shakeAmount = 0.2f, decreaseFactor = 1.5f;

    private Vector3 orignPos;

    private void Start()
    {
        camTransform = GetComponent<Transform>();
        orignPos = camTransform.localPosition;
    }

    private void Update()
    {
        if(shakeDur > 0)
        {
            camTransform.localPosition = orignPos + Random.insideUnitSphere * shakeAmount;
            shakeDur -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDur = 0;
            camTransform.localPosition = orignPos;
        }
        
    }
}

