using UnityEngine;
using System.Collections;

public class PlanetProcessor : MonoBehaviour
{
    [Header("配方设置")]
    [Tooltip("这个星球吃什么？")]
    public ItemType inputItem;      
    [Tooltip("这个星球吐什么？(拖入成品的 Prefab)")]
    public GameObject outputPrefab; 

    [Header("加工表现")]
    public float processTime = 2.0f; // 加工需要几秒
    public ParticleSystem processEffect; // 加工时的特效(可选)

    [Header("追踪设置")]
    [Tooltip("要把东西送给谁？(通常拖入 Main Camera)")]
    public Transform playerTarget; 
    
    [Tooltip("如果玩家传送距离超过这个值，物品就瞬移")]
    public float teleportThreshold = 50f; 

    // 当有东西飞进感应区
    private void OnTriggerEnter(Collider other)
    {
        // 检查飞进来的东西有没有贴标签
        SpaceItem item = other.GetComponent<SpaceItem>();
        
        // 如果有标签，而且类型是对的
        if (item != null && item.itemType == inputItem)
        {
            StartCoroutine(ProcessAndDeliver(item.gameObject));
        }
    }

    IEnumerator ProcessAndDeliver(GameObject rawItem)
    {
        // 1. 吞噬原料
        if (processEffect != null) 
            Instantiate(processEffect, rawItem.transform.position, Quaternion.identity);
        
        Destroy(rawItem); // 销毁扔进去的东西

        // 2. 等待加工
        yield return new WaitForSeconds(processTime);

        // 3. 生成成品 (初始位置在星球中心)
        GameObject product = Instantiate(outputPrefab, transform.position, Quaternion.identity);
        
        // 暂时关掉物理，防止掉下去，由代码控制飞行
        Rigidbody rb = product.GetComponent<Rigidbody>();
        if(rb != null) rb.isKinematic = true;

        // 4. 启动追踪逻辑
        StartCoroutine(SmartFly(product, rb));
    }

    IEnumerator SmartFly(GameObject item, Rigidbody rb)
    {
        float flySpeed = 15f; // 飞行速度

        // 只要物品还没销毁，就一直执行
        while (item != null)
        {
            // A. 计算目标点：永远在摄像机正前方 0.5 米处
            Vector3 hoverPos = playerTarget.position + playerTarget.forward * 0.5f;
            
            // B. 计算当前距离
            float dist = Vector3.Distance(item.transform.position, hoverPos);

            // === 核心：防虫洞丢失逻辑 ===
            if (dist > teleportThreshold)
            {
                // 如果距离太远(说明玩家传送了)，直接瞬移到玩家头顶
                item.transform.position = playerTarget.position + Vector3.up * 3f + playerTarget.forward * 1f;
                // 重新计算距离
                dist = Vector3.Distance(item.transform.position, hoverPos);
            }

            // === 飞行与悬停 ===
            if (dist > 0.1f)
            {
                // 飞向目标
                item.transform.position = Vector3.MoveTowards(item.transform.position, hoverPos, flySpeed * Time.deltaTime);
                // 自身旋转展示
                item.transform.Rotate(Vector3.up, 180 * Time.deltaTime);
            }
            else
            {
                // 到达后：死死咬住悬停点
                item.transform.position = hoverPos;
                item.transform.Rotate(Vector3.up, 90 * Time.deltaTime);
                
                // 如果检测到物理被重新开启(说明玩家伸手抓走了)，就退出逻辑
                if (rb != null && !rb.isKinematic) break;
            }

            yield return null; // 等待下一帧
        }
    }
}