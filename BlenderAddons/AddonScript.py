bl_info = {
    "name": "Move X Axis",
    "blender": (2,80,0),
    "category": "Object",
}

import bpy

class ObjectMoveX(bpy.types.Operator):
    """My Object Moving Script"""
    bl_idname = "object.move_x"
    bl_label = "Move X by One"
    bl_options = {'REGISTER', 'UNDO'}
    
    def execute(self, context):
        scene = context.scene
        for obj in scene.objects:
            obj.location.x += 1.0
        return {'FINISHED'}
    
def register():
    bpy.utils.register_class(ObjectMoveX)
    
def unregister():
    bpy.utils.unregister_class(ObjectMoveX)
    
# This allows you to run the script directly from Blender's Text editor
# to test the add-on without having to install it.
if __name__ == "__main__":
    register()