using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    // 单例模式：方便其他脚本直接调用它
    public static InventorySystem Instance;

    [Header("调试设置")]
    [Tooltip("勾选后，无需收集即可召唤所有物品")]
    public bool isDebugMode = true; // <--- 1. 新增这个开关，默认开启



    [Header("物品预制体字典 (配置表)")]
    // 这里我们需要把 ItemType 和 Prefab 对应起来
    public List<ItemConfig> itemConfigs; 
    
    // 内部字典：记录玩家是否拥有某个物品
    private Dictionary<ItemType, bool> collectedItems = new Dictionary<ItemType, bool>();
    private Dictionary<ItemType, GameObject> prefabLookup = new Dictionary<ItemType, GameObject>();

    [Header("生成设置")]
    public Transform spawnPoint; // 生成位置 (通常是摄像机前方)

    void Awake()
    {
        Instance = this;
        // 初始化字典
        foreach (var config in itemConfigs)
        {
            collectedItems[config.type] = false; // 默认都没收集
            prefabLookup[config.type] = config.prefab;
        }
    }

    // === 1. 存入背包 ===
    public void CollectItem(ItemType type)
    {
        if (collectedItems.ContainsKey(type))
        {
            collectedItems[type] = true;
            Debug.Log($"背包更新：已获得 {type}");
            // 这里可以加一个 UI 提示： "获得：高能电池"
        }
    }

    // === 2. 从背包取出 (手部菜单调用) ===
    public void SpawnItemFromMenu(string itemTypeName)
    {
        if (System.Enum.TryParse(itemTypeName, out ItemType type))
        {
            // 原来的逻辑：
            // if (collectedItems.ContainsKey(type) && collectedItems[type] == true)
            
            // 现在的逻辑：如果是调试模式 OR 玩家真的拥有这个物品
            if (isDebugMode || (collectedItems.ContainsKey(type) && collectedItems[type] == true))
            {
                Spawn(type);
            }
            else
            {
                Debug.Log($"[未收集] 你还没有获得 {type}，请先去星球收集！");
                // 这里可以播放一个“错误”音效
            }
        }
    }

    void Spawn(ItemType type)
    {
        if (prefabLookup.ContainsKey(type))
        {
            // 在摄像机前方 0.4 米处生成
            Vector3 pos = Camera.main.transform.position + Camera.main.transform.forward * 0.4f + Camera.main.transform.right * 0.2f;
            GameObject obj = Instantiate(prefabLookup[type], pos, Quaternion.identity);
            
            // 刚生成时悬停，防止掉下去
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true; 
            
            // 稍微给点旋转效果
            obj.transform.Rotate(0, 180, 0); 
        }
    }
}

// 这是一个简单的数据结构，用来在 Inspector 里配对
[System.Serializable]
public struct ItemConfig
{
    public string name;
    public ItemType type;
    public GameObject prefab;
}