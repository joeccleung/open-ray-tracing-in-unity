using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering; // Import this namespace for rendering supporting functions
using UnityEngine.SceneManagement;

namespace OpenRT {
    public class BasicPipeInstance : RenderPipeline // Our own renderer should subclass RenderPipeline
    {
        private readonly static string s_bufferName = "Ray Tracing Render Camera";

        private List<RenderPipelineConfigObject> m_allConfig; // A list of config objects containing all global rendering settings   
        private RenderPipelineConfigObject m_config;

        private Color m_clearColor = Color.black;
        private RenderTexture m_target;

        private ComputeShader m_mainShader;
        private CommandBuffer commands;
        private int kIndex = 0;

        private SceneParser m_sceneParser;
        private ComputeBuffer m_triangleBuffer;

        public BasicPipeInstance(Color clearColor, ComputeShader mainShader, List<RenderPipelineConfigObject> allConfig) {
            m_clearColor = clearColor;
            m_mainShader = mainShader;
            m_allConfig = allConfig;

            commands = new CommandBuffer { name = s_bufferName };

            kIndex = mainShader.FindKernel("CSMain");

            m_config = m_allConfig[0];
            InitSceneParsing();
        }

        protected override void Render(ScriptableRenderContext renderContext, Camera[] cameras) // This is the function called every frame to draw on the screen
        {
            RunParseScene();
            foreach (var camera in cameras) {
                RunTargetTextureInit(ref m_target);
                RunClearCanvas(commands, camera);
                RunLoadGeometryToBuffer(m_sceneParser, ref commands, ref m_triangleBuffer);
                RunSetCameraToMainShader(camera);
                RunSetAmbientToMainShader(m_config);
                RunSetTrianglesToMainShader(ref m_triangleBuffer, m_sceneParser.GetTriangles().Count);
                RunRayTracing(ref commands, m_target);
                RunSendTextureToUnity(commands, m_target, renderContext, camera);
                RunBufferCleanUp();

                // // Create an structure to hold the culling paramaters
                // ScriptableCullingParameters cullingParams;

                // //Populate the culling paramaters from the camera
                // if (camera.TryGetCullingParameters(out cullingParams))
                // {
                //    continue;
                // }

                // // Perform the culling operation
                // CullingResults cullingResults = renderContext.Cull(ref cullingParams);

                // // Get the opaque rendering filter settings
                // var opaqueRange = new FilteringSettings();

                // //Set the range to be the opaque queues
                // // relacing min/max with lowerBound/upperBound
                // opaqueRange.renderQueueRange = new RenderQueueRange()
                // {
                //    lowerBound = 0,
                //    upperBound = (int)UnityEngine.Rendering.RenderQueue.GeometryLast,
                // };

                // //Include all layers
                // opaqueRange.layerMask = ~0;

                // // Create the draw render settings
                // // note that it takes a shader pass name
                // var sortingSettings = new SortingSettings(camera);
                // sortingSettings.criteria = SortingCriteria.CommonOpaque;
                // var drs = new DrawingSettings(new ShaderTagId("Opaque"), sortingSettings);

                // // enable instancing for the draw call
                // drs.enableInstancing = true;

                // // A batch of rendering commands
                // var cmd = new CommandBuffer();
                // cmd.ClearRenderTarget(true, true, m_clearColor);

                // // Execute the commands
                // renderContext.ExecuteCommandBuffer(cmd);

                // // Release the memory hold by the buffer
                // cmd.Release();

                // // draw all of the renderers
                // renderContext.DrawRenderers(cullingResults, ref drs, ref opaqueRange);

                // // Return the render context
                // renderContext.Submit();
            }

        }

        private void InitSceneParsing() {
            m_sceneParser = SceneParser.Instance;
        }

        private void RunParseScene() {
            var scene = SceneManager.GetActiveScene();

            m_sceneParser.ParseScene(scene);
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

        private void RunLoadGeometryToBuffer(SceneParser sceneParser, ref CommandBuffer commands, ref ComputeBuffer triangleDataBuffer) {
            PipelineMaterialToBuffer.MaterialsToBuffer(sceneParser.GetMaterials(),
                ref commands);
            LoadBufferWithTriangles(sceneParser,
                triangleDataBuffer : ref triangleDataBuffer);
        }

        private void LoadBufferWithTriangles(SceneParser sceneParser, ref ComputeBuffer triangleDataBuffer) {
            int triCount = sceneParser.GetTriangles().Count;

            triangleDataBuffer?.Release();

            if (triCount > 0) {
                triangleDataBuffer = new ComputeBuffer(triCount, RTTriangle_t.GetSize());
                triangleDataBuffer.SetData(sceneParser.GetTriangles());
            } else {
                triangleDataBuffer = new ComputeBuffer(1, RTTriangle_t.GetSize());
            }
        }

        private void RunSetCameraToMainShader(Camera camera) {
            m_mainShader.SetMatrix("_CameraToWorld", camera.cameraToWorldMatrix);
            m_mainShader.SetMatrix("_CameraInverseProjection", camera.projectionMatrix.inverse);
        }

        private void RunSetAmbientToMainShader(RenderPipelineConfigObject config) {
            m_mainShader.SetVector("_AmbientLightUpper", config.upperAmbitent);
        }

        private void RunSetTrianglesToMainShader(ref ComputeBuffer buffer, int count) {
            m_mainShader.SetInt("_NumOfTriangles", count);
            m_mainShader.SetBuffer(kIndex, "_Triangles", buffer);
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
            m_triangleBuffer.Release();
        }

        private void RunSendTextureToUnity(CommandBuffer buffer, RenderTexture targeTexture,
            ScriptableRenderContext renderContext, Camera camera) {
            buffer.Blit(targeTexture, camera.activeTexture); // This also mark dest as active render target

            // End Unity profiler sample for frame debugger
            //            buffer.EndSample(s_bufferName);
            renderContext
                .ExecuteCommandBuffer(
                    buffer); // We copied all the commands to an internal memory that is ready to send to GPU
            buffer.Clear(); // Clear the command buffer

            renderContext.Submit(); // Send all the batched commands to GPU
        }
    }
}