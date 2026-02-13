using UnityEngine;

public class GasCollector : MonoBehaviour
{
    [Header("目标设定")]
    public Transform jupiter;          // 把木星的Transform拖到这里
    
    [Header("合适位置 (Sweet Spot)")]
    public float minRadius = 1.0f;     // 最佳深度的下限 (离木星中心最近)
    public float maxRadius = 1.5f;     // 最佳深度的上限 (离木星中心最远)

    [Header("收集参数")]
    public float requiredTime = 3.0f;  // 收集需要保持的总时间 (秒)
    private float currentProgress = 0f;

    [Header("视觉与反馈")]
    public MeshRenderer bottleRenderer; // 瓶子内部的气体材质/发光部位
    public Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.5f); // 空瓶颜色 (半透明灰)
    public Color collectingColor = Color.yellow;                 // 正在收集的颜色 (黄色警告/充能)
    public Color fullColor = Color.cyan;                         // 收集满的颜色 (青色氧气)
    
    public ParticleSystem collectingParticles; // (可选) 采集中冒泡的粒子特效

    private bool isFull = false;

    void Start()
    {
        // 初始化瓶子颜色
        if (bottleRenderer != null)
            bottleRenderer.material.color = emptyColor;
    }

    void Update()
    {
        // 如果已经收集满了，或者找不到木星，就不执行了
        if (isFull || jupiter == null) return;

        // 1. 计算瓶子和木星中心的距离
        float distance = Vector3.Distance(transform.position, jupiter.position);

        // 2. 判断是否在“合适位置”
        if (distance >= minRadius && distance <= maxRadius)
        {
            // --- 处于正确位置，开始收集 ---
            currentProgress += Time.deltaTime; // 增加进度

            // 视觉反馈：颜色从 empty 渐变到 collecting
            float fillRatio = currentProgress / requiredTime;
            bottleRenderer.material.color = Color.Lerp(emptyColor, collectingColor, fillRatio);

            // 播放粒子特效
            if (collectingParticles != null && !collectingParticles.isPlaying)
                collectingParticles.Play();

            // 3. 判断是否收集完成
            if (currentProgress >= requiredTime)
            {
                CompleteCollection();
            }
        }
        else
        {
            // --- 不在正确位置 ---
            // 停止粒子特效
            if (collectingParticles != null && collectingParticles.isPlaying)
                collectingParticles.Stop();

            // 惩罚机制：如果手抖了离开了区域，进度会慢慢衰减（或者你可以设为直接清零 currentProgress = 0）
            currentProgress = Mathf.Max(0, currentProgress - Time.deltaTime); 
            
            // 恢复空瓶颜色
            bottleRenderer.material.color = emptyColor;
        }
    }

    void CompleteCollection()
    {
        isFull = true;
        
        // 变成满瓶状态的颜色
        bottleRenderer.material.color = fullColor;
        
        if (collectingParticles != null) collectingParticles.Stop();

        Debug.Log("氧气/高能气体 收集完成！");

        // 【下一步逻辑】
        // 这里你可以触发 MRTK 的触觉反馈 (Haptic Feedback) 让手柄震动
        // 或者把这个空瓶子 Destroy 掉，Instantiate 一个不可逆的“满气瓶”Prefab
    }
}