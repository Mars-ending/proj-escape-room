using UnityEngine;

public class WormholeController : MonoBehaviour
{
    [Header("核心绑定")]
    [Tooltip("把你的 MRTK Playspace 或 CameraRig 拖进来")]
    public Transform playerSpaceship;

    [Header("行星列表")]
    public Transform mercury;
    public Transform venus;
    public Transform earth;
    public Transform mars;
    public Transform jupiter;
    public Transform saturn;
    public Transform uranus;
    public Transform neptune;

    [Header("轨道参数")]
    [Tooltip("绕行星旋转的速度 (弧度/秒)")]
    public float orbitSpeed = 0.5f; 
    
    [Tooltip("距离大气层的额外高度")]
    public float heightOffset = 20f;

    // --- 内部变量 ---
    private Transform targetPlanet; // 当前锁定的星球
    private bool isLockedOn = false; // 是否处于锁定模式
    private float currentAngle = 0f; // 当前绕星球转到了多少度
    private float currentDistance = 0f; // 当前轨道半径

    void Update()
    {
        // 如果处于锁定模式，且目标存在
        if (isLockedOn && targetPlanet != null)
        {
            // 1. 增加角度 (让飞船绕圈圈)
            currentAngle += orbitSpeed * Time.deltaTime;

            // 2. 核心数学公式：计算相对于星球的偏移坐标
            // 使用三角函数 (Sin/Cos) 画圆
            float x = Mathf.Cos(currentAngle) * currentDistance;
            float z = Mathf.Sin(currentAngle) * currentDistance;
            
            // 3. 组合最终位置 (星球当前位置 + 偏移量)
            // 这一步最关键！因为它每一帧都读取 targetPlanet.position，所以星球跑多快我们都跟得上！
            Vector3 offset = new Vector3(x, 0, z);
            playerSpaceship.position = targetPlanet.position + offset;

            // 4. 强制飞船看向星球中心
            playerSpaceship.LookAt(targetPlanet);

            // 5. (可选) 如果玩家按了 WASD 想自己飞，就断开锁定
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                BreakOrbit();
            }
        }
    }

    // === 公共传送接口 ===
    public void WarpToMercury() { StartOrbit(mercury); }
    public void WarpToVenus()   { StartOrbit(venus); }
    public void WarpToEarth()   { StartOrbit(earth); }
    public void WarpToMars()    { StartOrbit(mars); }
    public void WarpToJupiter() { StartOrbit(jupiter); }
    public void WarpToSaturn()  { StartOrbit(saturn); }
    public void WarpToUranus()  { StartOrbit(uranus); }
    public void WarpToNeptune() { StartOrbit(neptune); }

    // 启动轨道逻辑
    private void StartOrbit(Transform planet)
    {
        if (planet == null) return;

        targetPlanet = planet;
        
        // 计算轨道半径：(星球大小 / 2) + 安全高度
        // 这里假设星球是统一缩放的，取 x 轴缩放即可
        currentDistance = (planet.localScale.x / 2f) + heightOffset;

        // 重置角度 (从背面开始，即 PI 弧度)
        currentAngle = Mathf.PI; 

        isLockedOn = true;
        Debug.Log($"已捕获进入 {planet.name} 轨道，跟随公转中...");
    }

    // 断开轨道逻辑
    private void BreakOrbit()
    {
        isLockedOn = false;
        targetPlanet = null;
        Debug.Log("脱离自动驾驶模式，转为手动控制。");
    }
}