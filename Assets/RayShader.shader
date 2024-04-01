// Please notice NO_SELF_REFLECT define below
Shader "Unlit/RayShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" } // Lightmode = Raytracing is seen in some sample code; unclear what it does, but it seems to break things
        LOD 100
        Cull off

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            // Note: d3d11 means "d3d11 or d3d12"
            #pragma only_renderers d3d11 
            #pragma require inlineraytracing
            #include "UnityRayQuery.cginc"

            #include "RayPayload.cginc"

            #define NO_SELF_REFLECT

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldVertex : TEXCOORD1;
                float3 worldCamera: TEXCOORD2;
                float3 worldNormal : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            RaytracingAccelerationStructure rayStructure;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldVertex = mul (unity_ObjectToWorld, v.vertex);
                o.worldCamera = float4(_WorldSpaceCameraPos, 1.0);//mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = normalize(mul(unity_ObjectToWorld, v.normal) - mul(unity_ObjectToWorld, float3(0.0, 0.0, 0.0)));
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float3 reflect(float3 v, float3 normal) {
                return -2*dot(v, normal)*normal + v;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 offset = reflect(i.worldVertex - i.worldCamera, i.worldNormal);
                RayDesc ray;
                ray.Origin = i.worldVertex;
#ifdef NO_SELF_REFLECT
                ray.Origin += offset*(1/256.0);
#endif
                ray.TMin = 0;
                ray.TMax = 1e20f;
                ray.Direction = offset;

                // Trace
                UnityRayQuery<RAY_FLAG_ACCEPT_FIRST_HIT_AND_END_SEARCH | RAY_FLAG_CULL_BACK_FACING_TRIANGLES> query;
                query.TraceRayInline(rayStructure, RAY_FLAG_ACCEPT_FIRST_HIT_AND_END_SEARCH | RAY_FLAG_CULL_BACK_FACING_TRIANGLES, 0xff, ray);
                query.Proceed();

                if (query.CommittedStatus() == COMMITTED_TRIANGLE_HIT) { // == COMMITTED_TRIANGLE_HIT gives same results (nothing)
                    return float4(0.0,1.0,0.0,1.0); // Reflected hit-- Green
                } else {
                    // sample the texture
                    //fixed4 col = tex2D(_MainTex, i.uv);
                    // apply fog
                    //UNITY_APPLY_FOG(i.fogCoord, col);
                    //return col;
                    float3 offset = i.worldVertex - i.worldCamera;
                    RayDesc ray;
                    ray.Origin = i.worldVertex;
#ifdef NO_SELF_REFLECT
                    ray.Origin += offset*(1/256.0);
#endif
                    ray.TMin = 0;
                    ray.TMax = 1e20f;
                    ray.Direction = offset;

                    // Trace
                    UnityRayQuery<RAY_FLAG_ACCEPT_FIRST_HIT_AND_END_SEARCH | RAY_FLAG_CULL_BACK_FACING_TRIANGLES> query;
                    query.TraceRayInline(rayStructure, RAY_FLAG_ACCEPT_FIRST_HIT_AND_END_SEARCH | RAY_FLAG_CULL_BACK_FACING_TRIANGLES, 0xff, ray);
                    query.Proceed();

                    if (query.CommittedStatus() == COMMITTED_TRIANGLE_HIT) { // == COMMITTED_TRIANGLE_HIT gives same results (nothing)
                        return float4(0.0,0.0,1.0,1.0); // Passthrough hit-- Red
                    } else {
                    //TraceRay(rayStructure, RAY_FLAG_NONE, 0xFF, 0, 1, 0, ray, payload);
                        
                        return float4(1.0,0.0,0.0,1.0); // No hit -- Blue
                    }
                    //return float4(1.0,0.0,0.0,1.0);
                }
            }
            ENDCG
        }
    }
}
