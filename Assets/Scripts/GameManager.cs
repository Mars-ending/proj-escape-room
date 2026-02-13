using UnityEngine;
using TMPro; // 如果需要控制文字

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI设置")]
    public GameObject startMenuCanvas; // 把你的 Canvas 拖进来
    public GameObject playerHandMenu;  // 你的手部菜单 (游戏开始前应该隐藏)
    
    [Header("游戏状态")]
    public bool isGameStarted = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 游戏刚运行时
        ShowStartMenu();
    }

    public void ShowStartMenu()
    {
        isGameStarted = false;
        startMenuCanvas.SetActive(true); // 显示主菜单
        if(playerHandMenu != null) playerHandMenu.SetActive(false); // 隐藏手部菜单
        
        // 可选：在这里暂停时间
        // Time.timeScale = 0; 
    }

    public void StartGame()
    {
        Debug.Log("指挥官，任务开始！");
        
        // 1. 隐藏菜单
        startMenuCanvas.SetActive(false);
        
        // 2. 激活手部菜单 (允许玩家召唤物品)
        if(playerHandMenu != null) playerHandMenu.SetActive(true);

        // 3. 改变状态
        isGameStarted = true;

        // 4. (可选) 播放一个语音："Welcome back, Commander."
        // AudioManager.Play("IntroVoice");
    }
}