using UnityEngine;

public class ChestCollector : MonoBehaviour
{
    public AudioSource collectSound; // 收集时的音效 (比如“叮”的一声)
    public ParticleSystem collectEffect; // 收集时的闪光特效

    void OnTriggerEnter(Collider other)
    {
        // 检查是不是 SpaceItem
        SpaceItem item = other.GetComponent<SpaceItem>();

        if (item != null)
        {
            // 调用背包系统存入数据
            InventorySystem.Instance.CollectItem(item.itemType);

            // 播放反馈
            if (collectSound) collectSound.Play();
            if (collectEffect) Instantiate(collectEffect, transform.position, Quaternion.identity);

            // 销毁手里的物体 (看起来像是放进包里了)
            Destroy(other.gameObject);
        }
    }
}