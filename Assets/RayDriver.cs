using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


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
        if (!SystemInfo.supportsInlineRayTracing)
        {
            Debug.Log("The \"Inline\" RayTracing API is not supported by this GPU or by the current graphics API.");
        }
      // From IntersectionShaderTest
      {
            RayTracingAccelerationStructure.Settings settings = new RayTracingAccelerationStructure.Settings();
            settings.rayTracingModeMask = RayTracingAccelerationStructure.RayTracingModeMask.Everything;
            settings.managementMode = RayTracingAccelerationStructure.ManagementMode.Automatic; // Was .Manual in sample code
            settings.layerMask = 255;

        rayStructure = new RayTracingAccelerationStructure(settings);
        foreach(Transform child in targetParent.transform) {
            Renderer r = child.GetComponent<Renderer>();
            if (r != null) {
        		  rayStructure.AddInstance(r, new RayTracingSubMeshFlags[] {RayTracingSubMeshFlags.Enabled, RayTracingSubMeshFlags.ClosestHitOnly});
            }
    	//	Consider recurse on GetAllChildren(child.gameObject));
      	}
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
        Debug.Log("Count " + rayStructure.GetInstanceCount());
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
        cmdBuffer.SetRayTracingAccelerationStructure(rayTracingShader, Shader.PropertyToID("rayStructure"), rayStructure);
        //cmdBuffer.SetRayTracingShaderPass(rayStructure); // Not used in inline
        Graphics.ExecuteCommandBuffer(cmdBuffer);
        cmdBuffer.Release();    }

    private void ReleaseResources() {
      rayStructure?.Release();
    }
}
