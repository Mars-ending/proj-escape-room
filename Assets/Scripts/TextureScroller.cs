using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    public float scrollSpeedX = 0.02f; // 水平流动速度
    public float scrollSpeedY = 0.005f; // 垂直流动速度
    
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // 计算当前的时间偏移量
        float offsetX = Time.time * scrollSpeedX;
        float offsetY = Time.time * scrollSpeedY;
        
        // 让材质的贴图 (MainTex) 不断偏移，产生气体流动的视觉错觉
        rend.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
    }
}