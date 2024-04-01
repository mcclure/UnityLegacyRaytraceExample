#define AUTOMATIC_RTAS
// #define RTAS_DEBUG_PRINTS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RayDriver : MonoBehaviour
{
    RayTracingAccelerationStructure rayStructure;
    public GameObject targetParent;

    // Start is called before the first frame update
    void Start()
    {
        if (!SystemInfo.supportsRayTracing)
        {
            Debug.Log("The RayTracing API is not supported by this GPU or by the current graphics API.");
        }
        if (!SystemInfo.supportsInlineRayTracing)
        {
            Debug.Log("The \"Inline\" RayTracing API is not supported by this GPU or by the current graphics API.");
        }
      // From IntersectionShaderTest
      {
            RayTracingAccelerationStructure.Settings settings = new RayTracingAccelerationStructure.Settings();
            settings.rayTracingModeMask = RayTracingAccelerationStructure.RayTracingModeMask.Everything;
#if AUTOMATIC_RTAS
            settings.managementMode = RayTracingAccelerationStructure.ManagementMode.Automatic; // Was .Manual in sample code
#else
            settings.managementMode = RayTracingAccelerationStructure.ManagementMode.Manual; // Was .Manual in sample code
#endif
            settings.layerMask = 255;

        rayStructure = new RayTracingAccelerationStructure(settings);
#if AUTOMATIC_RTAS
#else
#if RTAS_DEBUG_PRINTS
        Debug.Log("RTAS Count (about to build)" + rayStructure.GetInstanceCount());
#endif
        foreach(Transform child in targetParent.transform) {
            Debug.Log(child);
            Renderer r = child.GetComponent<Renderer>();
            if (r != null) {
        		  rayStructure.AddInstance(r, new RayTracingSubMeshFlags[] {RayTracingSubMeshFlags.Enabled, RayTracingSubMeshFlags.ClosestHitOnly});
            }
#if RTAS_DEBUG_PRINTS
            Debug.Log("RTAS Count (building) " + rayStructure.GetInstanceCount());
#endif
    	//	Consider recurse on GetAllChildren(child.gameObject));
      	} 
#endif
      }
      Camera.onPreRender += OnPreRenderCallback; // FIXME: In a multicam setup, will this be called multiple times? That would be bad
      Debug.Log("LIVE");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnPreRenderCallback(Camera cam)
    {
#if RTAS_DEBUG_PRINTS
        Debug.Log("RTAS Count " + rayStructure.GetInstanceCount());
#endif
        // TODO: In a later phase objects will move, so this will need rebuilding, but right now we could have done it just once and not bothered with UpdateInstanceTransform

        foreach(Transform child in targetParent.transform) {
            Renderer r = child.GetComponent<Renderer>();
            if (r != null) {
                rayStructure.UpdateInstanceTransform(r);
            }
    	//	Consider recurse on GetAllChildren(child.gameObject));
      	}

        CommandBuffer cmdBuffer = new CommandBuffer();
        cmdBuffer.BuildRayTracingAccelerationStructure(rayStructure);
        cmdBuffer.SetGlobalRayTracingAccelerationStructure("rayStructure", rayStructure);
        //cmdBuffer.SetRayTracingShaderPass(rayStructure); // Not used in inline
        Graphics.ExecuteCommandBuffer(cmdBuffer);
        cmdBuffer.Release();
        //Shader.SetGlobalRayTracingAccelerationStructure("rayStructure", rayStructure); // Alternative to doing it in buffer
    }

    private void ReleaseResources() {
      rayStructure?.Release();
    }
}
