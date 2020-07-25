using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering; // Import this namespace for rendering supporting functions
using UnityEngine.SceneManagement;

namespace OpenRT {
    using ISIdx = System.Int32;

    public class BasicPipeInstance : RenderPipeline // Our own renderer should subclass RenderPipeline
    {
        private readonly static string s_bufferName = "Ray Tracing Render Camera";

        private SceneParseResult sceneParseResult;

        private List<RenderPipelineConfigObject> m_allConfig; // A list of config objects containing all global rendering settings   
        private RenderPipelineConfigObject m_config;

        private Color m_clearColor = Color.black;
        private RenderTexture m_target;

        private ComputeShader m_mainShader;
        private CommandBuffer commands;
        private int kIndex = 0;

        private ComputeBuffer m_bvhBuffer;
        private ComputeBuffer m_primitiveBuffer;
        private SortedList<ISIdx, ComputeBuffer> m_geometryInstanceBuffers;

        private ComputeBuffer m_lightInfoBuffer;

        public BasicPipeInstance(Color clearColor, ComputeShader mainShader, List<RenderPipelineConfigObject> allConfig) {
            m_clearColor = clearColor;
            m_mainShader = mainShader;
            m_allConfig = allConfig;

            if (m_mainShader == null) {
                Debug.LogError("Main Shader is gone");
                return;
            }

            commands = new CommandBuffer { name = s_bufferName };

            m_geometryInstanceBuffers = new SortedList<ISIdx, ComputeBuffer>();

            kIndex = mainShader.FindKernel("CSMain");

            m_config = m_allConfig[0];
        }

        protected override void Render(ScriptableRenderContext renderContext, Camera[] cameras) // This is the function called every frame to draw on the screen
        {
            if (m_mainShader == null) {
                return;
            } 

            RunParseScene();
            RunLoadGeometryToBuffer(sceneParseResult, ref m_bvhBuffer, ref m_mainShader, ref m_primitiveBuffer, ref m_geometryInstanceBuffers);
            RunLoadLightToBuffer(SceneParser.Instance, ref m_lightInfoBuffer);
            RunSetAmbientToMainShader(m_config);
            RunSetMissShader(m_mainShader, m_config);
            RunSetRayGenerationShader(m_config.rayGenId);
            RunSetGeometryInstanceToMainShader(ref m_bvhBuffer, ref m_primitiveBuffer, ref m_geometryInstanceBuffers, sceneParseResult.Primitives.Count);
            RunSetLightsToMainShader(sceneParseResult.Lights.Count, ref m_lightInfoBuffer);

            foreach (var camera in cameras) {
                RunTargetTextureInit(ref m_target);
                RunClearCanvas(commands, camera);
                RunSetCameraToMainShader(camera);
                RunRayTracing(ref commands, m_target);
                RunSendTextureToUnity(commands, m_target, renderContext, camera);
            }
            RunBufferCleanUp();
        }

        private void RunParseScene() {
            var scene = SceneManager.GetActiveScene();

            sceneParseResult = SceneParser.Instance.ParseScene(scene);
        }

        private void RunSetMissShader(ComputeShader shader, RenderPipelineConfigObject m_config) {
            shader.SetTexture(kIndex, "_SkyboxTexture", m_config.skybox);
        }

        private void RunTargetTextureInit(ref RenderTexture targetTexture) {
            if (targetTexture == null || targetTexture.width != Screen.width || targetTexture.height != Screen.height) {
                // Release render texture if we already have one
                if (targetTexture != null) {
                    targetTexture.Release();
                }

                // Get a render target for Ray Tracing
                targetTexture = new RenderTexture(Screen.width, Screen.height, 0,
                    RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
                targetTexture.enableRandomWrite = true;
                targetTexture.Create();
            }
        }

        private void RunClearCanvas(CommandBuffer buffer, Camera camera) {
            CameraClearFlags clearFlags = camera.clearFlags; // Each camera can config its clear flag to determine what should be shown if nothing can be seen by the camera
            buffer.ClearRenderTarget(
                ((clearFlags & CameraClearFlags.Depth) != 0),
                ((clearFlags & CameraClearFlags.Color) != 0),
                camera.backgroundColor);
        }

        private void RunLoadGeometryToBuffer(
            SceneParseResult sceneParseResult,
            ref ComputeBuffer bvhBuffer,
            ref ComputeShader mainShader,
            ref ComputeBuffer primitiveBuffer,
            ref SortedList<ISIdx, ComputeBuffer> gemoetryInstanceBuffers) {

            LoadBufferWithGeometryInstances(
                sceneParseResult,
                bvhBuffer : ref bvhBuffer,
                primitiveBuffer : ref primitiveBuffer,
                gemoetryInstanceBuffers : ref gemoetryInstanceBuffers);

            PipelineMaterialToBuffer.MaterialsToBuffer(sceneParseResult.Materials,
                ref mainShader);
        }

        private void LoadBufferWithGeometryInstances(
            SceneParseResult sceneParseResult,
            ref ComputeBuffer bvhBuffer,
            ref ComputeBuffer primitiveBuffer,
            ref SortedList<ISIdx, ComputeBuffer> gemoetryInstanceBuffers) {

            foreach (var item in gemoetryInstanceBuffers) {
                item.Value?.Release();
            }
            gemoetryInstanceBuffers.Clear();

            var geoInsIter = sceneParseResult.GeometryInstances.GetEnumerator();
            while (geoInsIter.MoveNext()) {
                var buffer = new ComputeBuffer(sceneParseResult.GetGeometryInstancesCount(geoInsIter.Current.Key),
                    sceneParseResult.GetGeometryInstancesStride(geoInsIter.Current.Key));
                buffer.SetData(geoInsIter.Current.Value);
                gemoetryInstanceBuffers.Add(geoInsIter.Current.Key, buffer);
            }

            List<RTBoundingBox> flattenBVH;
            List<Primitive> reorderedPrimitives;
            sceneParseResult.TopLevelBVH.Flatten(
                scenePrimitives: sceneParseResult.Primitives,
                flatten: out flattenBVH,
                reorderedPrimitives: out reorderedPrimitives);
            bvhBuffer = new ComputeBuffer(flattenBVH.Count, RTBoundingBox.stride);
            bvhBuffer.SetData(flattenBVH);
            primitiveBuffer = new ComputeBuffer(reorderedPrimitives.Count, Primitive.GetStride());
            primitiveBuffer.SetData(reorderedPrimitives);
        }

        private void RunLoadLightToBuffer(SceneParser sceneParser, ref ComputeBuffer lightInfoBuffer) {
            int numberOfLights = sceneParseResult.Lights.Count;

            lightInfoBuffer = new ComputeBuffer(numberOfLights, RTLightInfo.Stride);
            lightInfoBuffer.SetData(sceneParseResult.Lights);
        }

        private void RunSetCameraToMainShader(Camera camera) {
            m_mainShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
            m_mainShader.SetVector("_CameraForward", camera.transform.forward);
            m_mainShader.SetMatrix("_CameraInverseProjection", camera.projectionMatrix.inverse);
        }

        private void RunSetAmbientToMainShader(RenderPipelineConfigObject config) {
            m_mainShader.SetVector("_AmbientLightUpper", config.upperAmbitent);
        }

        private void RunSetRayGenerationShader(int rayGenId) {
            m_mainShader.SetInt("_RayGenID", rayGenId);
        }

        private void RunSetGeometryInstanceToMainShader(
            ref ComputeBuffer bvhBuffer,
            ref ComputeBuffer primitiveBuffer,
            ref SortedList<ISIdx, ComputeBuffer> geoInsBuffers,
            int count) {

            m_mainShader.SetInt("_NumOfPrimitive", count);
            m_mainShader.SetBuffer(kIndex, "_Primitives", primitiveBuffer);

            m_mainShader.SetBuffer(kIndex, "_BVHTree", bvhBuffer);

            //TODO: Temp solution for demo RT sphere. Make it dynamic
            if (geoInsBuffers.Count > 1) {
                m_mainShader.SetBuffer(kIndex, "_RTSpheres", geoInsBuffers[0]); // FIXME: Hardcoded 0 = Sphere
                m_mainShader.SetBuffer(kIndex, "_Triangles", geoInsBuffers[1]); // FIXME: Hardcoded 1 = triangle
            } else {
                //TODO: Look for solution to avoid assigning empty Structured Buffer
                ComputeBuffer empty = new ComputeBuffer(1, 1);
                m_mainShader.SetBuffer(kIndex, "_Triangles", empty);
                m_mainShader.SetBuffer(kIndex, "_RTSpheres", empty);
                empty.Release();
            }

        }

        private void RunSetLightsToMainShader(int count, ref ComputeBuffer lightInfoBuffer) {
            m_mainShader.SetInt("_NumOfLights", count);
            m_mainShader.SetBuffer(kIndex, "_Lights", lightInfoBuffer);
        }

        private void RunRayTracing(ref CommandBuffer commands, RenderTexture targetTexture) {
            m_mainShader.SetTexture(kIndex, "Result", targetTexture);
            int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
            int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);

            // Prevent dispatching 0 threads to GPU (when the editor is starting or there is no screen to render) 
            if (threadGroupsX > 0 && threadGroupsY > 0) {
                // m_mainShader.Dispatch(kIndex, threadGroupsX, threadGroupsY, 1);
                commands.DispatchCompute(computeShader: m_mainShader,
                    kernelIndex: kIndex,
                    threadGroupsX: threadGroupsX,
                    threadGroupsY: threadGroupsY,
                    threadGroupsZ: 1);
            }
        }

        private void RunBufferCleanUp() {
            m_bvhBuffer.Release();
            m_primitiveBuffer.Release();
            foreach (var item in m_geometryInstanceBuffers) {
                item.Value?.Release();
            }
            m_lightInfoBuffer?.Release();
        }

        private void RunSendTextureToUnity(CommandBuffer commands, RenderTexture targeTexture,
            ScriptableRenderContext renderContext, Camera camera) {
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