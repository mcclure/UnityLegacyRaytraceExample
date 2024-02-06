using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

public class RayDriver : MonoBehaviour
{
    RayTracingAccelerationStructure rayStructure;
    public RayTracingShader rayTracingShader;
    public GameObject targetParent;

    // Start is called before the first frame update
    void Start()
    {
        if (!SystemInfo.supportsRayTracing)
        {
            Debug.Log("The RayTracing API is not supported by this GPU or by the current graphics API.");
        }

      /* // From IntersectionShaderTest
            RayTracingAccelerationStructure.RASSettings settings = new RayTracingAccelerationStructure.RASSettings();
            settings.rayTracingModeMask = RayTracingAccelerationStructure.RayTracingModeMask.Everything;
            settings.managementMode = RayTracingAccelerationStructure.ManagementMode.Manual;
            settings.layerMask = 255;
        */

        rayStructure = new RayTracingAccelerationStructure();
        foreach(Transform child in targetParent.transform) {
            Renderer r = child.GetComponent<Renderer>();
            if (r != null) {
        		  rayStructure.AddInstance(r, new RayTracingSubMeshFlags[] {RayTracingSubMeshFlags.Enabled, RayTracingSubMeshFlags.ClosestHitOnly});
            }
    	//	Consider recurse on GetAllChildren(child.gameObject));
      	}
        
        CommandBuffer cmdBuffer = new CommandBuffer();
        cmdBuffer.BuildRayTracingAccelerationStructure(rayStructure);
        cmdBuffer.SetRayTracingAccelerationStructure(rayTracingShader, Shader.PropertyToID("rayStructure"), rayStructure);
        //cmdBuffer.SetRayTracingShaderPass(rayStructure);
        Graphics.ExecuteCommandBuffer(cmdBuffer);
        cmdBuffer.Release();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Call all UpdateInstanceTransform

    }

    private void ReleaseResources() {
      rayStructure?.Release();
    }
}
