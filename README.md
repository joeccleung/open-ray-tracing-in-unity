# A user programmable ray-tracing framework for Unity

In this project, we will design and implement a ray-trace framework based on programming models presented by modern commodity GPUs. The framework will define and support replaceable processing modules in ray-tracing, such as ray generation, ray-object intersections, and illumination. The framework will be designed and implemented targeting general-purpose commodity GPUs and does not require special-purpose hardware. This feature will enable platform portability. 

![Image of multiple reflection between spheres](https://squawker-ee149.firebaseapp.com/images/MultipleReflectionBetweenSphere.jpg)
Multiple reflections (4 generations) between primitive spheres. Intersect program, closest hit shader, and secondary ray generations, are programmable.

## Objectives
- Design and implement a ray-tracing framework on the Unity 3D engine. This framework will allow users to program various ray-tracing modules.

- Implement a collection of the modules to demonstrate the feasibility and modularity of the framework and assess the performance of the framework.

## Feature Backlog

- Provides real-time ray-tracing rendering capability in both offline editing (edit-mode) and runtime (play-mode).
- Supports at least three different replaceable and programmable ray-tracing modules.
- Defines the interfaces between the three modules.
- Allows custom programs to modify the rendering in runtime.
- Support different ray generation kernels.
- Support multiple illumination models in the same scene.
- Support directional light sources, point light sources, and spotlight light sources.
- Support shadow rendering by shadow rays or by shadow maps.
- Refraction and transparency.
- Volumetric rendering, such as light shaft and scattering.
- Allow users to define programs for generating acceleration structures.
