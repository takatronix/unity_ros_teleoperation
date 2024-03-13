# Unity ROS Teleoperation Project

This repo contains a series of components for Unity XR teleoperation with ROS integration. On the ROS side, the custom [TCP Endpoint](https://github.com/leggedrobotics/ROS-TCP-Endpoint) is needed in the catkin_ws.

## Components

| Component | Description | Location | Status |
| --- | --- | --- | --- |
| NeRFViewer | Handheld viewer for rendering NeRFs and scene interaction | [Assets/Components/Viewer](Assets/Components/Viewer) | Functional |
| Hands | Hand tracking and pose publishing over ROS, compatible with Ability hand models | [Assets/Components/Hands](Assets/Components/Hands) | Functional |
| Lidar | GPU rendering for LiDAR and PointCloud2 point viz from ROS | [Assets/Components/Lidar](Assets/Components/Lidar) | Needs clean up |
| PosePublisher | Publishes a pose from select and drag interaction and publishes as a ActiveMission for Alma locomotion | [Assets/Components/PosePublisher](Assets/Components/PosePublisher) | Functional (stop needs work) |
| Voxblox | Voxel mesh rendering | [Assets/Components/Voxblox](Assets/Components/Voxblox) | Complete |
| Splat | 3D viewer for Gaussian Splats | [Assets/Components/Splat](Assets/Components/Splat) | Incomplete |
| Image Renderer | Duplicates pose from a transfrom.json to render an image | [Assets/Components/ImageRenderer](Assets/Components/ImageRenderer) | Needs testing |
| Alma | Full Alma model compatible with Unity and linked to TF syste | [Assets/Components/Alma](Assets/Components/Alma) | Needs mesh clean up |


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

- [ ] Add haptic feedback from psyonic hand
- [ ] Inverted scroll option for moving scene
- [ ] Center around map (inverse tf)
- [ ] New TF manager (singleton)
- [ ] async tcp
- [ ] splat quest shader
- [ ] splat streaming
- [ ] nerf scene poses fix (rotations are off)
- [ ] Grab scaler?
- [ ] Add IP Presets

- [ ] Teleop demo
    - [ ] PS controllers mapped for twist commands
    - [x] Floating camera windows 
    - [x] Sensor spawner
    - [x] Fix hand interaction
    - [ ] Settings
        - [ ] IP settings
        - [ ] Mode settings
        - [ ] Inverted controls
    - [ ] Smaller model
    - [ ] Table spawner