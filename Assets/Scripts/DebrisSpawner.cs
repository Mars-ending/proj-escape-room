using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    public GameObject metalPrefab; // 红色金属
    public GameObject rockPrefab;  // 灰色石头
    public int count = 30;         // 生成多少个

    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            // 1. 随机找个位置 (在地球周围 0.25米 的范围内)
            Vector3 randomPos = Random.onUnitSphere * 2f;
            
            // 2. 随机决定是红的还是灰的
            GameObject prefab = (Random.value > 0.5f) ? metalPrefab : rockPrefab;
            
            // 3. 生成出来
            GameObject obj = Instantiate(prefab, transform.position + randomPos, Quaternion.identity);
            
            // 4. 设为地球的子物体
            obj.transform.parent = transform;
        }
    }
}