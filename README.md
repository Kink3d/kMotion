# kMotion
### Motion Blur for Unity’s Universal Render Pipeline.

![alt text](https://github.com/Kink3d/kMotion/wiki/Images/Home00.png?raw=true)
*An example of Motion Blur using kMotion with the [Boat Attack](https://github.com/Verasl/BoatAttack) demo.*

kMotion provides full object motion blur for Unity's Universal Render Pipeline. It does this by generating a full camera and object motion vector texture, which can then be used for various temporal effects. Motion blur can then be enabled with the provided **Motion Blur** **Volume Component** for Scriptable Render Pipeline's **Volume** system, supported by default in Universal Render Pipeline.

Refer to the [Wiki](https://github.com/Kink3d/kMotion/wiki/Home) for more information.

## Instructions
- Open your project manifest file (`MyProject/Packages/manifest.json`).
- Add `"com.kink3d.motion": "https://github.com/Kink3d/kMotion.git"` to the `dependencies` list.
- Open or focus on Unity Editor to resolve packages.

## Requirements
- Unity 2019.3.0f3 or higher.