using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LethalCamera : MonoBehaviour
{
    [Tooltip("The duration of the shake in seconds")]
    public float shakeTime;
    [Tooltip("The speed at which the camera will move during the shake (number of shakes per two PI)")]
    public float shakeSpeed;
    [Tooltip("The distance the camera will move during the shake in units")]
    public float shakeDistance;

    private Vector3 basePosition;

    private void Awake()
    {
        basePosition = transform.position;
    }

    public void Shake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float timer = .0f;
        while(timer < shakeTime)
        {
            timer += Time.deltaTime;

            transform.position = basePosition += new Vector3(Mathf.Sin(timer * shakeSpeed) * shakeDistance, .0f, .0f);

            yield return 0;
        }

        transform.position = basePosition;
    }
}
