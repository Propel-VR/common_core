-----------------------------
ProceduralTube version 1.0
-----------------------------


[There are four prefabs in Prefabs folder]

(1) ProceduralManager
- Manager will collect all the ProceduralTube in the scene, 
and it will calculate curves and draw tubes at the same time. 
One Manager must be placed in the scene.

(2) ProceduralTube
- Each tube has bezier curve data. 
Manager will collect all the tubes curve datasand draw at the same time.

(3) ProceduralTube_constraint
- If you want this tube follow some points, drag&drop the target object¡¯s transform.
- This is not good for the performance. 
So be aware if you want to use this for a massive number of tubes.
If constraint option is useful and should be needed,
I will add GPU constraintin the future. Please give me your feedback.

(4) ProceduralTube_static
- Use this tube prefab if the tube is static and will not be moved.
This is very important as it affects huge for the performance.




[Supported Devices]
- DX11 on PC.
- Vulkan on Android devices.
- Metal on iOS devices.
- OpenGL ES 3.1+ on mobiles.




[Each Class Description]

(1) ProceduralTubeData
- Contains curve and node struct data.


(2) ProceduralTube.cs
- This class have curve's handles data.

(bool) IsStatic : Static mode.
(struct) Handle
  (Vector3) p0,p1,p2,p3 : Handles' position data(local)
  (float) radius start/end : Tube's radius size
(enum) TaperType : Radius intepolation options.

(3) ProceduralTubeInspector.cs
- This class draws handles only for Editor.

(4) ProceduralTubeManager
- This class collect Tubes data and draw all the tubes at the same time.
- This class uses computershader named CalcBezierCurve.compute
to calculate bezier curves' points.
- This class calls Renderer's methods.

(5) ProceduralTubeRenderer
- Renderer will create one cylinder mesh procedurally 
and draw multiple cylindersusing gpu instancing.

(6) ProceduralTube_ConstraintHandles
- This class will get transform's position data and 
update ProceduralTube.cs's handle positions.



