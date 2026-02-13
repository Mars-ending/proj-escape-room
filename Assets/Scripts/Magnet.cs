using UnityEngine;
using System.Collections.Generic;

public class Magnet : MonoBehaviour
{
    [Header("追踪中心")]
    public Transform magnetCenter; 

    [Header("磁吸设置")]
    public float range = 20.0f;
    public int requiredCount = 5; 

    [HideInInspector]
    public List<GameObject> heldCubes = new List<GameObject>();

    void Update()
    {
        // 只有按下时才工作
        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
        {
            MaintainHeldCubes(); // 抓住列表里的
            AttractNewCubes();   // 吸新的
        }
        else
        {
            // 【核心修改】一旦松手：
            // 1. 清空列表，不再控制它们
            if (heldCubes.Count > 0)
            {
                ReleaseCubes();
            }
        }
        
        heldCubes.RemoveAll(item => item == null);
    }

    // 松手时的处理
    void ReleaseCubes()
    {
        // 只需要清空列表，SimpleOrbit 脚本会自动检测到没人管它了，然后飞回地球
        heldCubes.Clear();
        Debug.Log("磁力中断，方块回归地球");
    }

    void MaintainHeldCubes()
    {
        foreach (var cube in heldCubes)
        {
            if (cube != null)
            {
                // 吸附时，让方块临时变成 magnetCenter 的子物体，保证跟随手
                if (cube.transform.parent != magnetCenter)
                {
                    cube.transform.parent = magnetCenter;
                }
                
                SimpleOrbit orbit = cube.GetComponent<SimpleOrbit>();
                if (orbit != null) orbit.FlyTo(magnetCenter.position);
            }
        }
    }

    void AttractNewCubes()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, range);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Metal"))
            {
                if (heldCubes.Contains(hit.gameObject)) continue;

                SimpleOrbit orbit = hit.GetComponent<SimpleOrbit>();
                if (orbit != null)
                {
                    orbit.FlyTo(magnetCenter.position);

                    if (Vector3.Distance(magnetCenter.position, hit.transform.position) < 0.5f)
                    {
                        hit.transform.parent = magnetCenter;
                        heldCubes.Add(hit.gameObject);
                    }
                }
            }
        }
    }

    public bool IsReadyToTransform()
    {
        return heldCubes.Count >= requiredCount;
    }

    public void ConsumeAllHeldCubes()
    {
        foreach (var cube in heldCubes)
        {
            if (cube != null) Destroy(cube);
        }
        heldCubes.Clear();
    }
}