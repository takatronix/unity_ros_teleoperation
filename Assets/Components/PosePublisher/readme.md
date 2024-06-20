[back](/README.md)

# Pose Publisher

The Pose Publisher component is responsible for publishing a pose from select and drag interaction and publishes as a ActiveMission for anymal locomotion. It also publishes joystick commands from controllers.


## Joystick publishing

![Joystick](/docs/images/joysticks.png)
The joy message contains axis (0-1 floats) and buttons (integers) which are mapped to the controllers as shown above.