using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] private GameObject target;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(target.transform.position);
    }
}
