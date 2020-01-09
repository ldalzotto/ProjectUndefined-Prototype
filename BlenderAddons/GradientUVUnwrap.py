import bpy
import bmesh

bl_info = {
    "name": "Gradient Unwrap",
    "blender": (2, 80, 0),
    "category": "UV",
}


class GradientUnwrap(bpy.types.Operator):
    """Gradient Unwrap"""
    bl_idname = "uv.gradient_unwrap"
    bl_label = "Gradient Unwrap"
    bl_options = {'REGISTER', 'UNDO'}
    
    pixelColoumn: bpy.props.IntProperty(name="Column", default=0)
    textureSize = [0,0]
    
    def execute(self, context):
        self.findTextureSize(context)
        #self.unwrapView(context)
        self.setUVPosition(context)
        return {'FINISHED'}

    def findTextureSize(self, context):
        for area in context.screen.areas :
            print(area.type)
            if area.type=='IMAGE_EDITOR' :
                self.textureSize = area.spaces.active.image.size

    def unwrapView(self, context):
        bpy.ops.uv.project_from_view(scale_to_bounds=True)
        
    def setUVPosition(self, context):
        #selected_verts = list(filter(lambda v: v.select, context.object.data.vertices))
        
        uvXPosition = ((1 / self.textureSize[0]) / 2) + ((1 / self.textureSize[0]) * self.pixelColoumn) 
        
        bpy.ops.object.mode_set(mode = 'EDIT')
        me = context.object.data
 
        bm = bmesh.from_edit_mesh(me)
        uv_layer = bm.loops.layers.uv.verify()
        sel_faces = (f for f in bm.faces if f.select)
        for f in sel_faces:
            for l in f.loops:
                if l[uv_layer].select:
                    l[uv_layer].uv[0] = uvXPosition

def menu_func(self, context):
    self.layout.operator(GradientUnwrap.bl_idname)

def register():
    bpy.utils.register_class(GradientUnwrap)
    bpy.types.VIEW3D_MT_uv_map.append(menu_func)


def unregister():
    bpy.utils.unregister_class(GradientUnwrap)
    bpy.types.VIEW3D_MT_uv_map.remove(menu_func)