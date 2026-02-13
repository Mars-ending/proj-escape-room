using UnityEngine;

public class MarsInteraction : MonoBehaviour
{
    [Header("奖励设置")]
    public GameObject rewardPrefab; // 电池预制体
    public GameObject effectPrefab; // (可选) 爆炸/光效预制体

    // 只有当有东西“进入”火星感应区时触发
    void OnTriggerEnter(Collider other)
    {
        // 1. 判断撞我的是不是那个叫 "MagnetCenter" 的物体（你的手）
        if (other.name == "MagnetCenter")
        {
            Debug.Log("检测到手部接触！正在检查能量...");

            // 2. 因为 Magnet 脚本挂在相机上，我们需要去全局找它
            Magnet playerMagnet = FindObjectOfType<Magnet>();

            if (playerMagnet != null)
            {
                // 3. 检查有没有收集够方块 (比如5个)
                if (playerMagnet.IsReadyToTransform())
                {
                    Debug.Log("能量达标！执行转化！");

                    // A. 记录当前手的位置 (电池生成在这)
                    Vector3 spawnPos = other.transform.position;

                    // B. 消耗掉所有的红方块
                    playerMagnet.ConsumeAllHeldCubes();

                    // C. 生成高能电池
                    if (rewardPrefab != null)
                    {
                        Instantiate(rewardPrefab, spawnPos, Quaternion.identity);
                    }

                    // D. (可选) 生成一个特效，增加“摩擦生热”的感觉
                    if (effectPrefab != null)
                    {
                        Instantiate(effectPrefab, spawnPos, Quaternion.identity);
                    }
                }
                else
                {
                    Debug.Log("能量不足！还需要更多红方块。");
                }
            }
        }
    }
}