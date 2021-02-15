using System.Collections;
using System.Collections.Generic;
// - 
using UnityEngine;
using UnityEngine.Rendering; // Import this namespace for rendering supporting functions
using UnityEngine.SceneManagement;
using GUID = System.String;

namespace OpenRT
{
    using GeometryInstanceBuffer = ComputeBuffer; // (float)
    using ObjectLevelAccelerationGeometryBuffer = ComputeBuffer; // (float)
    using ObjectLevelAccelerationGeometryMappingCollectionBuffer = ComputeBuffer; // (int)
    using PrimitiveBuffer = ComputeBuffer; // (Primitive)
    using TopLevelAccelerationBuffer = ComputeBuffer; // (RTBoundingBoxToGPU)
    using TopLevelAccelerationGeometryMappingCollectionBuffer = ComputeBuffer; // (int)
    using WorldToLocalBuffer = ComputeBuffer;   // (Matrix4x4)
    using ISIdx = System.Int32;

    public class BasicPipeInstance : RenderPipeline // Our own renderer should subclass RenderPipeline
    {
        private readonly static string s_bufferName = "Ray Tracing Render Camera";

        // -
        private SortedList<ISIdx, GeometryInstanceBuffer> m_gemoetryInstanceBuffer = new SortedList<ISIdx, GeometryInstanceBuffer>();
        private SortedList<ISIdx, ObjectLevelAccelerationGeometryBuffer> m_objectLevelAccGeoBuffers = new SortedList<ISIdx, ObjectLevelAccelerationGeometryBuffer>();
        private SortedList<ISIdx, ObjectLevelAccelerationGeometryMappingCollectionBuffer> m_objectLevelAccGeoMapBuffers = new SortedList<ISIdx, ObjectLevelAccelerationGeometryMappingCollectionBuffer>();
        private PrimitiveBuffer m_primitiveBuffer;
        // -
        private List<ComputeBuffer> m_computeBufferForMaterials;
        private SceneParseResult sceneParseResult;
        private TopLevelAccelerationBuffer m_topLevelAcc;
        private TopLevelAccelerationGeometryMappingCollectionBuffer m_topLevelAccGeoMap;
        // -
        private WorldToLocalBuffer m_worldToPrimitiveBuffer;

        private List<RenderPipelineConfigObject> m_allConfig; // A list of config objects containing all global rendering settings   
        private RenderPipelineConfigObject m_config;

        private Color m_clearColor = Color.black;
        private RenderTexture m_target;

        private ComputeShader m_mainShader;
        private CommandBuffer commands;
        private ComputeBuffer empty;
        private int kIndex = 0;

        private ComputeBuffer m_lightInfoBuffer;

        public BasicPipeInstance(Color clearColor, ComputeShader mainShader, List<RenderPipelineConfigObject> allConfig)
        {
            m_clearColor = clearColor;
            m_mainShader = mainShader;
            m_allConfig = allConfig;

            if (m_mainShader == null)
            {
                Debug.LogError("Main Shader is gone");
                return;
            }

            commands = new CommandBuffer { name = s_bufferName };

            kIndex = mainShader.FindKernel("CSMain");

            m_config = m_allConfig[0];

            m_computeBufferForMaterials = new List<GeometryInstanceBuffer>();
        }

        protected override void Render(ScriptableRenderContext renderContext, Camera[] cameras) // This is the function called every frame to draw on the screen
        {
            if (m_mainShader == null)
            {
                return;
            }

            RunParseScene();
            // -
            RunLoadGeometryToBuffer(sceneParseResult,
                                    ref m_topLevelAcc,
                                    ref m_objectLevelAccGeoBuffers,
                                    ref m_objectLevelAccGeoMapBuffers,
                                    ref m_primitiveBuffer,
                                    ref m_worldToPrimitiveBuffer,
                                    ref m_gemoetryInstanceBuffer,
                                    ref m_topLevelAccGeoMap);
            RunLoadMaterialToBuffer(m_computeBufferForMaterials,
                                    sceneParseResult,
                                    ref m_mainShader);
            RunLoadLightToBuffer(sceneParseResult, ref m_lightInfoBuffer);
            RunSetAmbientToMainShader(m_config);
            RunSetMissShader(m_mainShader, m_config);
            RunSetRayGenerationShader(m_config.rayGenId);
            RunSetGeometryInstanceToMainShader(sceneParseResult.Primitives.Count,
                                               ref m_topLevelAcc,
                                               ref m_objectLevelAccGeoBuffers,
                                               ref m_objectLevelAccGeoMapBuffers,
                                               ref m_primitiveBuffer,
                                               ref m_worldToPrimitiveBuffer,
                                               CustomShaderDatabase.Instance.ShaderNameList(EShaderType.Intersect),
                                               ref m_gemoetryInstanceBuffer,
                                               ref m_topLevelAccGeoMap);
            RunSetLightsToMainShader(sceneParseResult.Lights.Count, ref m_lightInfoBuffer);

            foreach (var camera in cameras)
            {
                RunTargetTextureInit(ref m_target);
                RunClearCanvas(commands, camera);
                RunSetCameraToMainShader(camera);
                RunRayTracing(ref commands, m_target);
                RunSendTextureToUnity(commands, m_target, renderContext, camera);
            }
            RunBufferCleanUp();
        }

        private void RunLoadMaterialToBuffer(List<ComputeBuffer> computeShadersForMaterials,
                                             SceneParseResult sceneParseResult,
                                             ref ComputeShader mainShader)
        {
            sceneParseResult.ClearAllMaterials();

            SceneTextureCollection sceneTexture = new SceneTextureCollection();

            PipelineMaterialToBuffer.MaterialsToBuffer(computeShadersForMaterials,
                                                       sceneParseResult,
                                                       ref mainShader,
                                                       ref sceneTexture);

            PipelineMaterialToBuffer.LoadTextureToBuffer(sceneTexture, ref mainShader);
        }

        private void RunParseScene()
        {
            var scene = SceneManager.GetActiveScene();

            sceneParseResult = SceneParser.Instance.ParseScene(scene);
        }

        private void RunSetMissShader(ComputeShader shader, RenderPipelineConfigObject m_config)
        {
            shader.SetTexture(kIndex, "_SkyboxTexture", m_config.skybox);
        }

        private void RunSecondaryRayStack(ref ComputeShader shader, ComputeBuffer secondaryRayBuffer)
        {
            shader.SetBuffer(kIndex, "_secondaryRayStack", secondaryRayBuffer);
        }

        private void RunTargetTextureInit(ref RenderTexture targetTexture)
        {
            if (targetTexture == null || targetTexture.width != Screen.width || targetTexture.height != Screen.height)
            {
                // Release render texture if we already have one
                if (targetTexture != null)
                {
                    targetTexture.Release();
                }

                // Get a render target for Ray Tracing
                targetTexture = new RenderTexture(Screen.width, Screen.height, 0,
                    RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
                targetTexture.enableRandomWrite = true;
                targetTexture.Create();
            }
        }

        private void RunClearCanvas(CommandBuffer buffer, Camera camera)
        {
            CameraClearFlags clearFlags = camera.clearFlags; // Each camera can config its clear flag to determine what should be shown if nothing can be seen by the camera
            buffer.ClearRenderTarget(
                ((clearFlags & CameraClearFlags.Depth) != 0),
                ((clearFlags & CameraClearFlags.Color) != 0),
                camera.backgroundColor);
        }

        private void LoadBufferWithGeometryInstances(SceneParseResult sceneParseResult,
                                                     ref TopLevelAccelerationBuffer bvhBuffer,
                                                     ref SortedList<ISIdx, ObjectLevelAccelerationGeometryBuffer> objectLvAccGeoBuffers,
                                                     ref SortedList<ISIdx, ObjectLevelAccelerationGeometryMappingCollectionBuffer> objectLvAccGeoMapBuffers,
                                                     ref PrimitiveBuffer primitiveBuffer,
                                                     ref WorldToLocalBuffer worldToPrimitiveBuffer,
                                                     ref SortedList<ISIdx, GeometryInstanceBuffer> gemoetryInstanceBuffers,
                                                     ref TopLevelAccelerationGeometryMappingCollectionBuffer topLevelAccGeoMapColBuffer)
        {
            gemoetryInstanceBuffers.Clear();
            objectLvAccGeoBuffers.Clear();
            objectLvAccGeoMapBuffers.Clear();

            var geoInsIter = sceneParseResult.GeometryInstances.GetEnumerator();
            while (geoInsIter.MoveNext())
            {
                var buffer = new GeometryInstanceBuffer(sceneParseResult.GetGeometryInstancesCount(geoInsIter.Current.Key), sceneParseResult.GetGeometryInstancesStride(geoInsIter.Current.Key));
                buffer.SetData(geoInsIter.Current.Value);
                gemoetryInstanceBuffers.Add(geoInsIter.Current.Key, buffer);
            }

            var objectLevelAccGeoIter = sceneParseResult.ObjectLevelAccelerationGeometries.GetEnumerator();
            while (objectLevelAccGeoIter.MoveNext())
            {
                var buffer = new ObjectLevelAccelerationGeometryBuffer(objectLevelAccGeoIter.Current.Value.Count, sizeof(float));
                buffer.SetData(objectLevelAccGeoIter.Current.Value);
                objectLvAccGeoBuffers.Add(objectLevelAccGeoIter.Current.Key, buffer);
            }

            var objectLvAccGeoMapIter = sceneParseResult.ObjectLevelAccelerationGeometryMapping.GetEnumerator();
            while (objectLvAccGeoMapIter.MoveNext())
            {
                var buffer = new ObjectLevelAccelerationGeometryMappingCollectionBuffer(objectLvAccGeoMapIter.Current.Value.Count, sizeof(int));
                buffer.SetData(objectLvAccGeoMapIter.Current.Value);
                objectLvAccGeoMapBuffers.Add(objectLvAccGeoMapIter.Current.Key, buffer);
            }

            var objectLevelAccStrInsIter = sceneParseResult.ObjectLevelAccelerationStructures.GetEnumerator();
            while (objectLevelAccStrInsIter.MoveNext())
            {
                var buffer = new GeometryInstanceBuffer(objectLevelAccStrInsIter.Current.Value.Count, sizeof(float));
                buffer.SetData(objectLevelAccStrInsIter.Current.Value);
                gemoetryInstanceBuffers.Add(objectLevelAccStrInsIter.Current.Key, buffer);
            }

            sceneParseResult.TopLevelBVH.Flatten(
                flatten: out List<RTBoundingBoxToGPU> _flattenBVH,
                accelerationGeometryMapping: out List<int> _topLevelAccelerationGeometryMapping);
            bvhBuffer = new TopLevelAccelerationBuffer(_flattenBVH.Count, RTBoundingBox.stride);
            bvhBuffer.SetData(_flattenBVH);
            primitiveBuffer = new PrimitiveBuffer(sceneParseResult.Primitives.Count, Primitive.GetStride());
            primitiveBuffer.SetData(sceneParseResult.Primitives);
            topLevelAccGeoMapColBuffer = new TopLevelAccelerationGeometryMappingCollectionBuffer(_topLevelAccelerationGeometryMapping.Count, sizeof(int));
            topLevelAccGeoMapColBuffer.SetData(_topLevelAccelerationGeometryMapping);
            worldToPrimitiveBuffer = new WorldToLocalBuffer(sceneParseResult.WorldToPrimitive.Count, sizeof(float) * 16);
            worldToPrimitiveBuffer.SetData(sceneParseResult.WorldToPrimitive);
        }

        private void RunLoadGeometryToBuffer(SceneParseResult sceneParseResult,
                                             ref TopLevelAccelerationBuffer topLevelAcc,
                                             ref SortedList<ISIdx, ObjectLevelAccelerationGeometryBuffer> objectLvAccGeoBuffers,
                                             ref SortedList<ISIdx, ObjectLevelAccelerationGeometryMappingCollectionBuffer> objectLvAccGeoMapBuffers,
                                             ref PrimitiveBuffer primitiveBuffer,
                                             ref WorldToLocalBuffer worldToPrimitiveBuffer,
                                             ref SortedList<ISIdx, GeometryInstanceBuffer> gemoetryInstanceBuffers,
                                             ref TopLevelAccelerationGeometryMappingCollectionBuffer topLevelAccGeoMap)
        {
            LoadBufferWithGeometryInstances(sceneParseResult,
                                            bvhBuffer: ref topLevelAcc,
                                            objectLvAccGeoBuffers: ref objectLvAccGeoBuffers,
                                            objectLvAccGeoMapBuffers: ref objectLvAccGeoMapBuffers,
                                            primitiveBuffer: ref primitiveBuffer,
                                            worldToPrimitiveBuffer: ref worldToPrimitiveBuffer,
                                            gemoetryInstanceBuffers: ref gemoetryInstanceBuffers,
                                            topLevelAccGeoMapColBuffer: ref topLevelAccGeoMap);
        }

        private void RunLoadLightToBuffer(SceneParseResult sceneParseResult, ref ComputeBuffer lightInfoBuffer)
        {
            int numberOfLights = sceneParseResult.Lights.Count;

            lightInfoBuffer = new ComputeBuffer(numberOfLights, RTLightInfo.Stride);
            lightInfoBuffer.SetData(sceneParseResult.Lights);
        }

        private void RunSetCameraToMainShader(Camera camera)
        {
            m_mainShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
            m_mainShader.SetVector("_CameraForward", camera.transform.forward);
            m_mainShader.SetMatrix("_CameraInverseProjection", camera.projectionMatrix.inverse);
        }

        private void RunSetAmbientToMainShader(RenderPipelineConfigObject config)
        {
            m_mainShader.SetVector("_AmbientLightUpper", config.upperAmbitent);
        }

        private void RunSetRayGenerationShader(int rayGenId)
        {
            m_mainShader.SetInt("_RayGenID", rayGenId);
        }

        private void RunSetGeometryInstanceToMainShader(int count,
                                                        ref TopLevelAccelerationBuffer bvhBuffer,
                                                        ref SortedList<ISIdx, ObjectLevelAccelerationGeometryBuffer> objectLvAccGeoBuffers,
                                                        ref SortedList<ISIdx, ObjectLevelAccelerationGeometryMappingCollectionBuffer> objectLvAccGeoMapBuffers,
                                                        ref PrimitiveBuffer primitiveBuffer,
                                                        ref WorldToLocalBuffer worldToPrimitiveBuffer,
                                                        string[] intersectShaderNames,
                                                        ref SortedList<ISIdx, GeometryInstanceBuffer> geoInsBuffers,
                                                        ref TopLevelAccelerationGeometryMappingCollectionBuffer topLevelAccGeoMapColBuffer)
        {
            m_mainShader.SetInt("_NumOfPrimitive", count);
            m_mainShader.SetBuffer(kIndex, "_Primitives", primitiveBuffer);
            m_mainShader.SetBuffer(kIndex, "_WorldToPrimitives", worldToPrimitiveBuffer);
            m_mainShader.SetBuffer(kIndex, "_BVHTree", bvhBuffer);
            m_mainShader.SetBuffer(kIndex, "_TopLevelAccelerationGeometryMapping", topLevelAccGeoMapColBuffer);

            empty = new ComputeBuffer(1, sizeof(float));

            for (int intersectIdx = 0; intersectIdx < intersectShaderNames.Length; intersectIdx++)
            {
                if (geoInsBuffers.ContainsKey(intersectIdx))
                {
                    m_mainShader.SetBuffer(kIndex, $"_{intersectShaderNames[intersectIdx]}", geoInsBuffers[intersectIdx]);
                }
                else
                {
                    m_mainShader.SetBuffer(kIndex, $"_{intersectShaderNames[intersectIdx]}", empty);
                }
            }
            // -
            // -
            // -
            // -
            // -

            var objectLvAccGeoBuffersIter = objectLvAccGeoBuffers.GetEnumerator();
            while (objectLvAccGeoBuffersIter.MoveNext())
            {
                m_mainShader.SetBuffer(kIndex, $"_{intersectShaderNames[objectLvAccGeoBuffersIter.Current.Key]}GeometryData", objectLvAccGeoBuffersIter.Current.Value);
            }

            var objectLvAccGeoMapBuffersIter = objectLvAccGeoMapBuffers.GetEnumerator();
            while (objectLvAccGeoMapBuffersIter.MoveNext())
            {
                m_mainShader.SetBuffer(kIndex, $"_{intersectShaderNames[objectLvAccGeoMapBuffersIter.Current.Key]}GeometryMapping", objectLvAccGeoMapBuffersIter.Current.Value);
            }
        }

        private void RunSetLightsToMainShader(int count, ref ComputeBuffer lightInfoBuffer)
        {
            m_mainShader.SetInt("_NumOfLights", count);
            m_mainShader.SetBuffer(kIndex, "_Lights", lightInfoBuffer);
        }

        private void RunRayTracing(ref CommandBuffer commands, RenderTexture targetTexture)
        {
            m_mainShader.SetTexture(kIndex, "Result", targetTexture);
            int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
            int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);

            // Prevent dispatching 0 threads to GPU (when the editor is starting or there is no screen to render) 
            if (threadGroupsX > 0 && threadGroupsY > 0)
            {
                // m_mainShader.Dispatch(kIndex, threadGroupsX, threadGroupsY, 1);
                commands.DispatchCompute(computeShader: m_mainShader,
                    kernelIndex: kIndex,
                    threadGroupsX: threadGroupsX,
                    threadGroupsY: threadGroupsY,
                    threadGroupsZ: 1);
            }
        }

        private void RunBufferCleanUp()
        {
            empty.Release();
            m_topLevelAcc.Release();
            m_topLevelAccGeoMap.Release();
            m_primitiveBuffer.Release();
            m_worldToPrimitiveBuffer.Release();
            foreach (var item in m_gemoetryInstanceBuffer)
            {
                item.Value?.Release();
            }
            m_gemoetryInstanceBuffer.Clear();
            foreach (var item in m_objectLevelAccGeoBuffers)
            {
                item.Value?.Release();
            }
            m_objectLevelAccGeoBuffers.Clear();
            foreach (var item in m_objectLevelAccGeoMapBuffers)
            {
                item.Value?.Release();
            }
            m_objectLevelAccGeoMapBuffers.Clear();
            m_lightInfoBuffer?.Release();

            foreach (var materialBuffers in m_computeBufferForMaterials)
            {
                materialBuffers.Release();
            }
            m_computeBufferForMaterials.Clear();
        }

        private void RunSendTextureToUnity(CommandBuffer commands, RenderTexture targeTexture,
            ScriptableRenderContext renderContext, Camera camera)
        {
            commands.Blit(targeTexture, camera.activeTexture); // This also mark dest as active render target

            // End Unity profiler sample for frame debugger
            //            buffer.EndSample(s_bufferName);
            renderContext
                .ExecuteCommandBuffer(
                    commands); // We copied all the commands to an internal memory that is ready to send to GPU
            commands.Clear(); // Clear the command buffer

            renderContext.Submit(); // Send all the batched commands to GPU
        }
    }
}