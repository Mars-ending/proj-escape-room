using UnityEngine;

public class PlanetOrbit : MonoBehaviour
{
    [Header("绑定太阳")]
    [Tooltip("把场景里的 Sun (太阳) 拖到这里")]
    public Transform centerPoint; 

    [Header("运动速度")]
    [Tooltip("公转速度 (地球建议 10-20，水星 50，木星 5)")]
    public float orbitSpeed = 10f; 
    
    [Tooltip("自转轴 (通常保持默认 Y 轴)")]
    public Vector3 axis = Vector3.up;

    void Update()
    {
        // 如果没有拖入太阳，就不执行，防止报错
        if (centerPoint == null) return;

        // 核心公式：绕着 centerPoint，沿着 axis 轴，旋转 orbitSpeed 角度
        transform.RotateAround(centerPoint.position, axis, orbitSpeed * Time.deltaTime);
    }
}