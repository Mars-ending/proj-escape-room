using UnityEngine;

public class BottleDispenser : MonoBehaviour
{
    [Header("分配器设置")]
    [Tooltip("把你做好的 GasBottle 预制体拖到这里")]
    public GameObject bottlePrefab; 
    
    [Tooltip("瓶子从哪里掉出来？(指定一个空物体的位置)")]
    public Transform spawnPoint;

    [Header("特效与音效 (可选)")]
    public AudioSource dispenseSound;
    public ParticleSystem steamEffect; // 模拟机器排气的蒸汽

    // 这个方法就是用来绑定给按钮点击的
    public void SpawnBottle()
    {
        if (bottlePrefab != null && spawnPoint != null)
        {
            // 在指定位置生成瓶子
            Instantiate(bottlePrefab, spawnPoint.position, spawnPoint.rotation);

            // 播放音效
            if (dispenseSound != null) dispenseSound.Play();

            // 播放排气特效
            if (steamEffect != null) steamEffect.Play();

            Debug.Log("生成了一个新的收集瓶！");
        }
        else
        {
            Debug.LogWarning("分配器缺少 Prefab 或 SpawnPoint！");
        }
    }
}