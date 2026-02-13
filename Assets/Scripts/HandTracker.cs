using UnityEngine;
using UnityEngine.XR;

public class HandTracker : MonoBehaviour
{
    [Header("没手时的设置")]
    public Transform fallbackTarget; // 相机
    public Vector3 fallbackOffset = new Vector3(0.1f, -0.1f, 0.4f);

    [Header("有手时的微调 (捏住位置)")]
    // 建议初始值：Z轴向前 0.1 到 0.15 (指尖方向)，Y轴微调
    public Vector3 pinchOffset = new Vector3(0, 0.02f, 0.12f); 

    void Update()
    {
        bool handFound = false;
        Vector3 targetPos = Vector3.zero;
        Quaternion targetRot = Quaternion.identity;

        // 1. 尝试读取 XR 信号
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightHand.isValid)
        {
            if (rightHand.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 pos))
            {
                targetPos = pos;
                if (rightHand.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rot))
                {
                    targetRot = rot;
                }
                handFound = true;
            }
        }

        // 2. 尝试读取 MRTK 模拟物体
        if (!handFound)
        {
            GameObject simHand = GameObject.Find("MRTK RightHand Controller");
            if (simHand == null) simHand = GameObject.Find("Right Controller");
            
            if (simHand != null)
            {
                targetPos = simHand.transform.position;
                targetRot = simHand.transform.rotation;
                handFound = true;
            }
        }

        // 3. 应用位置
        if (handFound)
        {
            // 【核心修改】在这里加上 pinchOffset
            // targetRot * pinchOffset 的意思是：让偏移量随着手的旋转而旋转
            // 这样无论你怎么转手腕，"指尖"永远在手的前方
            transform.position = targetPos + (targetRot * pinchOffset);
            
            transform.rotation = targetRot;
        }
        else
        {
            // 没手时跟随相机
            if (fallbackTarget != null)
            {
                transform.position = fallbackTarget.position + fallbackTarget.TransformDirection(fallbackOffset);
                transform.rotation = fallbackTarget.rotation;
            }
        }
    }
}