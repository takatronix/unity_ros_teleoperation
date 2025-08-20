
# Unity ROS Teleoperation Project
[Project Page](https://rffr.leggedrobotics.com/works/teleoperation/)

This repo contains a series of components for Unity XR teleoperation with ROS integration. It is designed to be run on a Quest 3 VR headset, and support OpenXR, hand tracking, Unity input system, and is built with Unity 2022.3.12f1.

For information on setting up Unity and opening this project [Unity Quickstart](docs/unity.md), and for Quest information see [Quest Quickstart](docs/quest.md).


## Quickstart
On the ROS side, the custom [TCP Endpoint](https://github.com/leggedrobotics/ROS-TCP-Endpoint) needs to be run somewhere on the ROS network. This node is pretty lightweight so it can be run directly on a robot. Once the node is running the device running this app needs to connect to the ROS network, and the IP of the device running the TCP Endpoint needs to be set in the menu (see [Menu](Assets/Components/Menu) for more information). If everything works, the menu should turn green, and data should be streaming between the app and ROS.


## Components

| Component | Description | Location | Preview |
| --- | --- | --- | --- |
| Audio Streamer | Implements bidirectional audio stream over ROS | [Assets/Components/AudioStreamer](Assets/Components/AudioStreamer) | ![](/docs/images/AudioStreamer.gif) |
| Camera Viewer | Renders a ROS image stream to a floating image window | [Assets/Components/CameraView](Assets/Components/CameraView) | ![](/docs/images/images.gif) |
| Grid Map | Renders a 2.5D grid map such as elevation maps from Anymal | [Assets/Components/GridMap](Assets/Components/GridMap) | ![](/docs/images/gridmap.gif) |
| Hands | Hand tracking and pose publishing over ROS, compatible with Ability hand models | [Assets/Components/Hands](Assets/Components/Hands) | ![](/docs/images/hands.gif) |
| Haptic | Bhaptic glove support  plus controller haptics | [Assets/Components/Haptics](Assets/Components/Haptics) | ![](/docs/images/haptics.png) | 
| Headset Publisher | Publishes headset and hand poses on TF and Pose | [Assets/Components/HeadsetPublisher](Assets/Components/HeadsetPublisher) | ![](/docs/images/tf.gif) |
| Lidar | GPU rendering for LiDAR and PointCloud2 point viz from ROS | [Assets/Components/Lidar](Assets/Components/Lidar) | ![](/docs/images/lidar.gif) |
| Menu | Palm menu for interaction and toggling | [Assets/Components/Menu](Assets/Components/Menu) | ![](/docs/images/menu.gif) |
| NeRFViewer | Handheld viewer for rendering NeRFs and scene interaction | [Assets/Components/NeRFViewer](Assets/Components/NeRFViewer) | ![](/docs/images/nerf.gif) |
| PathStreamer | Displays nav paths with a line and series of arrows | [Assets/Components/PathStreaming](Assets/Components/PathStreaming) | ![](/docs/images/path.gif) |
| PosePublisher | Publishes poses and Joystick commands for locomotion | [Assets/Components/PosePublisher](Assets/Components/PosePublisher) | ![](/docs/images/posegoals.gif) |
| PoseStreamer | Displays stamped poses | [Assets/Components/PoseStreaming](Assets/Components/PoseStreaming) | ![](/docs/images/poses.gif) |
| Robots | Robot model manager, holds the meshes, materials and the scripts to swap between Anymal, ALMA, Tytan, Standalone Dynaarm and Franka Panda | [Assets/Components/Robots](Assets/Components/Robots) | ![](/docs/images/robots.jpg) |
| Stereo | Stereo camera rendering, renders to each eye for human depth perception | [Assets/Components/StereoImage](Assets/Components/StereoImage) | ![](/docs/images/stereo.gif) |
| TF | WiP new TF system for managing robots and reorientation | [Assets/Components/TFSystem](Assets/Components/TFSystem) | ![](/docs/images/tf.gif) |
| Voxblox | Voxel mesh rendering | [Assets/Components/VoxBlox](Assets/Components/VoxBlox) | ![](/docs/images/vox.gif) |
| VR Debug | Debugging tools for VR, namely a console | [Assets/Components/VRDebug](Assets/Components/VRDebug) | ![](/docs/images/vr_console.jpg) |
| VR Streamer | Streams a the VR view to a ROS topic (w/o AR view) | [Assets/Components/VRStreamer](Assets/Components/VRStreamer) | ![](/docs/images/streamer.jpg) |
| Tactile Images | Displays tactile images on finger tips | [Assets/Components/TactileImages](Assets/Components/TactileImages) | ![](/docs/images/tactile.gif) |
| Markers | Displays various ROS markers in the scene (supports points, cubes, spheres) | [Assets/Components/Markers](Assets/Components/Markers) | ![](/docs/images/markers.gif) |
| TF Viz | Debug viewer to show currently streamed TF links | [Assets/Components/TFViz](Assets/Components/TFViz) | ![](/docs/images/tfdebug.gif) |


## Scenes
In general the scenes should have a few objects by default:
- Light source (usually the default directional light)
- MR Interaction Setup (this enables AR/VR support and acts as a camera)
- Debug canvas (will autolink to the menu and shows debug messages)
- Palm Menu (menu to interact and toggle things with)
- Root (The roof of the TF/object tree and tagged at 'root')

---

## Minor Version History

### 0.0.9
- Simplified dynaarm model (down to 98 draw calls and 330k tris)
- Fixed TF pose publishing so it should work even without a model
- Removed some duplicated publishers/gameobjects

### 0.0.8
- Added versioning with display in debug mode
- Added tytan, standalone dynaarm, and franka panda robots
- Updated dynaarm for newer model (may still need simplification)
- Added robot manager for switching between robots

---
