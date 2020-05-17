using System;

public abstract class A_VEAttribute : Attribute { }
[AttributeUsage(AttributeTargets.Field)]
public class VE_Nested : A_VEAttribute { }
[AttributeUsage(AttributeTargets.Field)]
public class VE_Ignore : A_VEAttribute { }
[AttributeUsage(AttributeTargets.Field)]
public class VE_Array : A_VEAttribute { }