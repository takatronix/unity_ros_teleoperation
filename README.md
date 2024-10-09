
# Unity ROS Teleoperation Project


This repo contains a series of components for Unity XR teleoperation with ROS integration. It is designed to be run on a Quest 3 VR headset, and support OpenXR, hand tracking, Unity input system, and is built with Unity 2022.3.12f1.

For information on setting up Unity and opening this project [Unity Quickstart](docs/unity.md), and for Quest information see [Quest Quickstart](docs/quest.md).


## Quickstart
On the ROS side, the custom [TCP Endpoint](https://github.com/leggedrobotics/ROS-TCP-Endpoint) needs to be run somewhere on the ROS network. This node is pretty lightweight so it can be run directly on a robot. Once the node is running the device running this app needs to connect to the ROS network, and the IP of the device running the TCP Endpoint needs to be set in the menu (see [Menu](Assets/Components/Menu) for more information). If everything works, the menu should turn green, and data should be streaming between the app and ROS.


## Components

| Component | Description | Location | Preview | Status |
| --- | --- | --- | --- | --- |
| Camera Viewer | Renders a ROS image stream to a floating image window | [Assets/Components/CameraView](Assets/Components/CameraView) | ![](/docs/images/images.gif) | Functional |
| Hands | Hand tracking and pose publishing over ROS, compatible with Ability hand models | [Assets/Components/Hands](Assets/Components/Hands) | ![](/docs/images/hands.gif) | Functional |
| Haptic | Bhaptic glove support  plus controller haptics | [Assets/Components/Haptics](Assets/Components/Haptics) | ![](/docs/images/haptics.gif) | Functional |
| Headset Publisher | Publishes headset and hand poses on TF and Pose | [Assets/Components/HeadsetPublisher](Assets/Components/HeadsetPublisher) | ![](/docs/images/tf.gif) | Functional |
| Lidar | GPU rendering for LiDAR and PointCloud2 point viz from ROS | [Assets/Components/Lidar](Assets/Components/Lidar) | ![](/docs/images/lidar.gif) | Functional |
| Menu | Palm menu for interaction and toggling | [Assets/Components/Menu](Assets/Components/Menu) | ![](/docs/images/menu.gif) | Functional |
| NeRFViewer | Handheld viewer for rendering NeRFs and scene interaction | [Assets/Components/NeRFViewer](Assets/Components/NeRFViewer) | ![](/docs/images/nerf.gif) | Functional (needs updates) |
| PosePublisher | Publishes poses and Joystick commands for locomotion | [Assets/Components/PosePublisher](Assets/Components/PosePublisher) | ![](/docs/images/posegoals.gif) | Functional |
| Robots | Robot model manager, holds the meshes, materials and the scripts to swap between Anymal, ALMA, Tytan, Standalone Dynaarm and Franka Panda | [Assets/Components/Robots](Assets/Components/Robots) | ![](/docs/images/robots.jpg) | Functional |
| Splat | 3D viewer for Gaussian Splats | [Assets/Components/Splat](Assets/Components/Splat) | ![]() | Incomplete |
| Stereo | Stereo camera rendering, renders to each eye for human depth perception | [Assets/Components/StereoImage](Assets/Components/StereoImage) | ![](/docs/images/stereo.gif) | Functional |
| TF | WiP new TF system for managing robots and reorientation | [Assets/Components/TFSystem](Assets/Components/TFSystem) | ![](/docs/images/tf.gif) | Incomplete |
| Voxblox | Voxel mesh rendering | [Assets/Components/VoxBlox](Assets/Components/VoxBlox) | ![](/docs/images/vox.gif) | Functional |
| VR Debug | Debugging tools for VR, namely a console | [Assets/Components/VRDebug](Assets/Components/VRDebug) | ![](/docs/images/vr_console.jpg) | Functional |
| VR Streamer | Streams a the VR view to a ROS topic (w/o AR view) | [Assets/Components/VRStreamer](Assets/Components/VRStreamer) | ![](/docs/images/streamer.jpg) | Functional |
| Tactile Images | Displays tactile images on finger tips | [Assets/Components/TactileImages](Assets/Components/TactileImages) | ![](/docs/images/tactile.gif) | Functional |


## Scenes
In general the scenes should have a few objects by default:
- Light source (usually the default directional light)
- MR Interaction Setup (this enables AR/VR support and acts as a camera)
- Debug canvas (will autolink to the menu and shows debug messages)
- Palm Menu (menu to interact and toggle things with)
- Root (The roof of the TF/object tree and tagged at 'root')

---

## Version History

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

# THINGS TO DO

- [x] ROS to Unity
    - works with basic topic and image streaming
- [x] Unity to ROS
    - sending str topics on button press
- [x] Robot creation
    - using URDF Importer, converts to mesh, with helper script converts to tf linked thing
- [x] TF streaming
    - moves gameobjects based on ros TF
- [x] TF static 
    - fixes static tf offset issue
- [x] URP setup
    - setup URP for better rendering
- [x] Quest deployment
    - runs with URP on quest 3
- [x] OpenXR interaction mapping
    - maps a button, and finger pinch to action button (triggers pub)
- [x] OpenXR physics interaction
    - grab and toss mechanics working to move things around
- [x] Depth occlusion shader
    - custom shader to set alpha based on main camera depth
- [x] Overlay ROS stream
    - on single camera renders ROS image stream
- [x] Display NeRF stats
    - shows on gui loss, step, resolution
- [x] Action server integration in Unity
    - sends requests
- [x] Action server NeRF render
    - renders from feedback topic
- [x] Move overlay to XR compat
    - sphere render
- [x] Render request with tolerances
    - sphere track follows and rerenders
- [x] Basic menu UI (IP settings (tbd), mode toggle, render mode)
    - attached to the viewer
- [x] Fix render after training
    - renders on state training or complete
- [x] Realtime requests and render to mat
    - renders on move for sphere
- [x] Action feedback and result res -> result
    - renamed params to be consistent
- [x] Screen UI (floating screen)
    - sphere/sphere tracker
- [x] Swap to compressed nerf images
    - improved performance, 90mbs to 12mbs
- [x] Fix shimmer from parallax
    - amplitude too high
- [x] Add transision animations
    - viewer and sphere animated
- [x] Viewer summoner
    - summons on button or palm up
- [x] Movable base (odom)
    - moves on controller joysticks and tapping floor
- [x] Better floor
    - transparent grid floor
- [x] Summon offset better
    - angled properly at eye level + controller check
- [x] Local pose
    - local pose for render requests
- [x] Save IP settings to user pref
    - saves ip settings and added port/ip input text (keypad + keyboard)
- [x] Add image view selector
    - can select which image topic to view
- [x] better labels
    - relabeled UI
- [x] Fix action server preempt on tf restart
    - caught exception, only happens on --clock, modified package in unity for looping data
- [x] Viewer zoom
    - zoom via changeable fov
- [x] Local rotations and viewer offset
    - works with physics derived motion such as grab interactable
- [x] Test splat
    - added test splat scene using VFX graph for splatting (poor performance)
- [x] Msg viz
    - got working viz for different messages including point cloud 2
- [x] hand mapping
    - maps XR hands to google Mano hand model
- [x] lidar viz toggle
    - lidar support on with editor for toggle
- [x] realsense support
    - realsense support with depth and color pointcloud 2
- [x] hand joint state publishing
    - publishes landmark points relative to the headset origin
- [x] integration on arm
    - integrated on arm, works with realsense or headset on router
- [x] add alma hand model
    - viz tf data from alma hand in scene for teleop
- [x] Debayer image
    - debayer image manually on image receive
- [x] Updated URDF converter
    - _seems_ to work out of the box for tf tracking (2 joints mislabeled/rotated, although might be issue with source)
- [x] Pointcloud2 viz to particle system
    - works well enough, need to determine if lag is from network or render delays
- [x] Change pose to activemission
    - verifed working pose for location
- [x] Add cancel big red button (Trigger service call) 
    - Sends cancel mission signal
- [x] Fix realsense tf tracking
    - temp ensure within odom hierachy
- [x] Add haptic support
    - Bhaptic gloves supported
- [x] Add IP Presets
    - Added presets for IP settings in player prefs
- [x] Stereo camera support
    - Added stereo camera support for left/right images
- [x] Compute shader debayering
    - Added compute shader for debayering for raw images
- [x] Add haptic feedback from psyonic hand
    - Works
- [x] Palm menu v2
    - Added new menu with more options and better linking
- [x] Docs
    - Mostly complete
- [x] splat streaming
    - Streams as point cloud 2, currently vized the same as RGBD
- [x] nerf scene poses fix (rotations are off)
    - Fixed in TF rewrites

- [ ] Convert to Unity package on github with proper docs/releases
- [ ] Center around map frame (inverse tf)
- [ ] New TF manager (singleton)
- [ ] Switch to UDP
- [ ] splat quest shader
- [ ] Grab scaler?
- [ ] Nimbro integration

- [ ] Teleop demo
    - [x] PS controllers mapped for twist commands
    - [x] Floating camera windows 
    - [x] Sensor spawner
    - [x] Fix hand interaction
    - [x] Settings
        - [x] IP settings
        - [x] Mode settings
        - [ ] Inverted controls
    - [x] Smaller model
    - [ ] Table spawner
    - [ ] Spherical orbit
    - [x] fix jumps on lock
    - [ ] Recenter on reconnect
    - [x] Enable move unlock at start + point and click
    - [ ] speed settings
    - [x] Scene state saver
