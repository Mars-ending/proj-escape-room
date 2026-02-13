using UnityEngine;

public class SimpleOrbit : MonoBehaviour
{
    [Header("核心设置")]
    public Transform defaultCenter;   // 地球
    
    [Header("手部悬浮参数")]
    public float holdingRadius = 0.5f; // 【你要找的参数】悬浮半径
    public float flySpeed = 10.0f;     // 飞向手的速度
    public float handOrbitSpeed = 150.0f; // 在手上的旋转速度(转快点才帅)

    [Header("地球轨道参数")]
    public float earthRadius = 0.4f;   // 地球轨道半径
    public float earthOrbitSpeed = 50.0f; // 在地球边的旋转速度

    private Vector3 axis;
    private Vector3 targetPos;       // 手的位置
    private bool isBeingPulled = false;

    void Start()
    {
        axis = Random.onUnitSphere;
        // 稍微随机化速度
        handOrbitSpeed += Random.Range(-30, 30);
        
        if (defaultCenter == null && transform.parent != null)
        {
            defaultCenter = transform.parent;
        }
    }

    void Update()
    {
        // ============================================
        // 状态 1：被手吸住 (Magnet 激活中)
        // ============================================
        if (isBeingPulled)
        {
            // 1. 如果还在地球名下，先断绝关系
            if (transform.parent != null && transform.parent == defaultCenter) 
                transform.parent = null;

            // 2. 计算离手心的距离
            float currentDist = Vector3.Distance(transform.position, targetPos);

            // 3. 【核心逻辑】位置控制
            if (currentDist > holdingRadius)
            {
                // A. 如果太远：飞向手心 (直到距离等于 holdingRadius)
                transform.position = Vector3.MoveTowards(
                    transform.position, 
                    targetPos, 
                    flySpeed * Time.deltaTime
                );
            }
            else if (currentDist < holdingRadius - 0.05f) // 加一点缓冲防止抖动
            {
                // B. 如果太近 (比如穿模了)：推出去
                Vector3 dir = (transform.position - targetPos).normalized;
                transform.position = Vector3.MoveTowards(
                    transform.position, 
                    targetPos + dir * holdingRadius, 
                    flySpeed * Time.deltaTime
                );
            }
            // C. 如果正好在半径上，就不动位置，只旋转

            // 4. 旋转 (绕手心)
            transform.RotateAround(targetPos, axis, handOrbitSpeed * Time.deltaTime);
        }
        
        // ============================================
        // 状态 2：没被吸住 (回地球 / 绕地球)
        // ============================================
        else if (defaultCenter != null)
        {
            // 1. 飞回地球轨道的逻辑
            float distToEarth = Vector3.Distance(transform.position, defaultCenter.position);

            if (distToEarth > earthRadius + 0.1f)
            {
                // 还没到家，飞回去
                transform.position = Vector3.MoveTowards(transform.position, defaultCenter.position, flySpeed * Time.deltaTime);
            }
            else
            {
                // 到家了，认祖归宗 (为了跟着地球动)
                if (transform.parent != defaultCenter)
                {
                    transform.parent = defaultCenter;
                }
            }

            // 2. 绕地球旋转
            transform.RotateAround(defaultCenter.position, axis, earthOrbitSpeed * Time.deltaTime);
        }

        // 每帧重置
        isBeingPulled = false;
    }

    public void FlyTo(Vector3 pos)
    {
        isBeingPulled = true;
        targetPos = pos;
    }
}