using UnityEngine;

public class HandMenuSpawner : MonoBehaviour
{
    [Header("要生成的物品")]
    public GameObject bottlePrefab; // 把你的 GasBottle 预制体拖到这里

    public void SpawnGasBottle()
    {
        if (bottlePrefab != null)
        {
            // 获取玩家头部（主摄像机）的位置和朝向
            Transform headTransform = Camera.main.transform;

            // 计算生成位置：在玩家头部正前方 0.5 米，稍微偏下一点的位置
            Vector3 spawnPos = headTransform.position + headTransform.forward * 0.5f - headTransform.up * 0.2f;

            // 生成瓶子
            Instantiate(bottlePrefab, spawnPos, Quaternion.identity);

            Debug.Log("通过手部菜单生成了收集瓶！");
        }
        else
        {
            Debug.LogWarning("请在 Inspector 中赋予 bottlePrefab！");
        }
    }
}