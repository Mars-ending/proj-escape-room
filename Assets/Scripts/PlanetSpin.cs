using UnityEngine;

public class PlanetSpin : MonoBehaviour
{
    [Header("旋转设置")]
    public float speed = 10.0f;

    // 这里通常不需要改，保持 (0, 1, 0) 就是绕着星球的南北极转
    public Vector3 axis = Vector3.up; 

    void Update()
    {
        // 关键修改：添加 Space.Self
        // 这句话的意思是：不管我现在歪成什么样，我都绕着我自己的头顶(axis)转
        transform.Rotate(axis, speed * Time.deltaTime, Space.Self);
    }
}