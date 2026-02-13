using UnityEngine;

public enum ItemType
{
    None,
    // --- 地球 & 火星组 ---
    MagneticBall,   // 磁力球 (地球产出 / 火星原料)
    EmptyBattery,   // 空电池 (火星产出 / 水星原料)
    
    // --- 水星组 ---
    FullBattery,    // 满电池 (水星产出)

    // --- 金星 & 木星组 (共用空罐子) ---
    EmptyCanister,  // 空罐子 (原料)
    AcidCanister,   // 酸液罐 (金星产出)
    HydrogenFuel,   // 氢燃料 (木星产出)

    // --- 土星组 ---
    HeatSink,       // 散热器 (原料)
    IceCore,        // 冰晶核心 (土星产出)

    // --- 天王星 & 海王星组 (钻石流水线) ---
    Coal,           // 煤炭 (原料)
    RoughDiamond,   // 毛坯钻 (天王星产出 / 海王星原料)
    PerfectDiamond  // 完美钻 (海王星产出)
}

public class SpaceItem : MonoBehaviour
{
    public ItemType itemType;
}