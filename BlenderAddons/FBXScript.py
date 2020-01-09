import bpy

bl_info = {
    "name": "FBX Export Hierarchy",
    "blender": (2, 80, 0),
    "category": "Export",
}

class FBXExport(bpy.types.Operator):
    """FBX Export Hierarchy"""
    bl_idname = "object.fxb_export_hierarchy"
    bl_label = "Fbx Export Hierarchy"
    bl_options = {'REGISTER', 'UNDO'}
    
    
    def GetChildren(self, object):
        returnChildrens = object.children
        if len(returnChildrens) > 0:
            tmpChildrens = returnChildrens
            for child in returnChildrens:
                print(child)
                tmpChildrens = tmpChildrens + self.GetChildren(child)
            returnChildrens = tmpChildrens
        return returnChildrens

    def execute(self, context):
        print(context.object)
        rootObject = context.object
        objectWithHierarchyTuple = self.GetChildren(rootObject) + (rootObject,)

        for objectToSelect in objectWithHierarchyTuple:
            objectToSelect.select_set(True)

        bpy.ops.export_scene.fbx(filepath="G:\GameProjects\ProjectUndefined\Assets\\Undefined\Levels\TrainingLevel\TrainingLevel_Assets\_Scenes\RTP_TEST\Chunks\Models\\" + rootObject.name + ".fbx", use_selection=True)
        
        return {'FINISHED'}


def menu_func(self, context):
    self.layout.operator(FBXExport.bl_idname)

def register():
    bpy.utils.register_class(FBXExport)
    bpy.types.VIEW3D_MT_object.append(menu_func)


def unregister():
    bpy.utils.unregister_class(FBXExport)
    bpy.types.VIEW3D_MT_object.remove(menu_func)


# This allows you to run the script directly from Blender's Text editor
# to test the add-on without having to install it.
if __name__ == "__main__":
    register()