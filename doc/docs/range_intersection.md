
The range intersection system is the system that allows the enemies to spot the player when he is in sight. The system takes into account obstacles, meaning that the range will be occluded by it.

![](https://img.itch.zone/aW1nLzI4MjIzNzIuZ2lm/original/OAlluE.gif)

# Range object

The range object is what defines the range shape which will be used for intersection test.

The range object is composed of two shapes.

**Intersection shape:**
The intersection shape is the geometry that will be used by the intersection calculation to precisely tell if one shape is contained into another.

**Trigger shape:**
The trigger shape acts as a filter to avoid calculating precise intersection against all interactive object. The trigger shape is a Trigger component that will register other interactive objects when a trigger event if fired. The goal is to increase efficiency by not calculating precise intersection when the interactive object is far away.

The intersection shape must be a shape that how to calculate if a point is inside or not.

The range object keeps tracks of obstacle objects that are inside the trigger shape.

# Visibility probe

Visibility probes are points that defines the visibility points of an interactive object. It is against these points that the intersection calculation is performed against. 

This method has been preferred over 3D shape intersection because I wasn't aware of sat algorithm. And the sat algorithm is probably cheaper than probe checking against a 3D geometry.

# Obstacle object

Obstacle objects are objects that occlude the range object. When a range object is near an obstacle, occlusion frustums are projected from the point of view of the range object. These frustums will be excluded during the intersection calculation. This means that if another range object is inside the intersection shape but within an occlusion frustum, then the intersection doesn't happen.

# Data model

<svg-inline src="range_intersection_data_model.svg"></svg-inline>

# Intersection calculation

A single intersection calculation consists of checking if any point of the visibility probe is inside the range object considering occlusion frustums.

The calculation is divide in two steps, the occlusion frustum calculations and the visibility probe test.

## Occlusion frustum calculation

For every obstacle objects that are inside the trigger shape of the range object, occlusion frustums are projected from the origin of the range object.

For now, occlusion frustums are generated from quad faces only. Points of the quad are expanded in the direction defined by the origin point by a distance defined in the visibility probe structure.

To improve the efficiency of frustum calculations, there are some rules that avoid frustum recalculations :

* If the range object has not moved the last frame
    * If the obstacle object has moved
        * -> do frustum calculations for this specific obstacle
    
* If the range object has moved the last frame
    * -> do frustum calculations for all obstacles relative to the range object

 
## Visibility probe test 

The visibility probe test calculates if any of the probe points are contained within the range object's intersection shape. 

If at least one visibilivisibility probe is inside the intersection shape and not occluded by frustum, then there is intersection.
All other case result in no intersection.