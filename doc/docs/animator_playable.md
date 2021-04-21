The animator playable simulates the unity built-in animator controller state machines. It is a system a system that play layered animations via the playable api. It's role is to calculate and push blending factors to the playable api. Blending factor calculation depends of the type of the animation layer.

The animator playable supports :

* Sequenced animation with blending between every clip depending of the evaluated time.
* 1D Blended animation that play multiple animation in parallel weighted by a single float parameter.
* 2D blended animation that play multiple animation in parallel weighted by two float parameters.

# Animation layers

Animation layers are object that play one or more animation clip. Layers are ordered, meaning that the animation of the layer 2 will override results of the layer 1.

## Sequenced animation layer

Sequenced animation layers are animation clips that are played one after another. Transition between one clip to another can be blended in function of the internal clock of the layer.

Transition blending is defined by a begin and end time and is always linear.

<svg-inline src="animator_playable_sequential.svg"></svg-inline>

The sequenced animations can be repeated.

## 1D Blended animation layer

The blended animation layer is the same as the sequenced animation layer. However, it is not the elapsed time that control the weight of animation clips, it's an input variable associated to the layer called "weight evaluation". The weight evaluation is clamped between 0 and 1 and every clip has a blending function that return the desired weight of the animation based on the weight evaluation.

It is up to the consumer to send the desired weight evaluation value.

## 2D Blended animation layer

Use the same logic as the 1D blended animation layer but the weight evaluation variable are 2 floats. Instead of having blending functions for every clip, they have a 2D position clamped between -1 and 1.

Animation clip weights are calculated by using the gradient band interpolation (see  [Automated Semi‐Procedural Animation
for Character Locomotion](http://runevision.com/thesis/rune_skovbo_johansen_thesis.pdf) @ #6.3 Gradient Band Interpolation, page 58)