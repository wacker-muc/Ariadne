﻿// ------------------------------------------------------------------------------
//<autogenerated>
//        This code was generated by Microsoft Visual Studio Team System 2005.
//
//        Changes to this file may cause incorrect behavior and will be lost if
//        the code is regenerated.
//</autogenerated>
//------------------------------------------------------------------------------
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SWA.Ariadne.Logic.Tests
{
[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class BaseAccessor {
    
    protected Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject m_privateObject;
    
    protected BaseAccessor(object target, Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType type) {
        m_privateObject = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject(target, type);
    }
    
    protected BaseAccessor(Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType type) : 
            this(null, type) {
    }
    
    internal virtual object Target {
        get {
            return m_privateObject.Target;
        }
    }
    
    public override string ToString() {
        return this.Target.ToString();
    }
    
    public override bool Equals(object obj) {
        if (typeof(BaseAccessor).IsInstanceOfType(obj)) {
            obj = ((BaseAccessor)(obj)).Target;
        }
        return this.Target.Equals(obj);
    }
    
    public override int GetHashCode() {
        return this.Target.GetHashCode();
    }
}


[System.Diagnostics.DebuggerStepThrough()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TestTools.UnitTestGeneration", "1.0.0.0")]
internal class SWA_Ariadne_Logic_SolverBaseAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType("SWA.Ariadne.Logic", "SWA.Ariadne.Logic.SolverBase");
    
    internal SWA_Ariadne_Logic_SolverBaseAccessor(object target) : 
            base(target, m_privateType) {
    }
    
    internal global::SWA.Ariadne.Model.Maze maze {
        get {
            global::SWA.Ariadne.Model.Maze ret = ((global::SWA.Ariadne.Model.Maze)(m_privateObject.GetField("maze")));
            return ret;
        }
        set {
            m_privateObject.SetField("maze", value);
        }
    }
    
    internal static object CreatePrivate(global::SWA.Ariadne.Model.Maze maze) {
        object[] args = new object[] {
                maze};
        Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject priv_obj = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject("SWA.Ariadne.Logic", "SWA.Ariadne.Logic.SolverBase", new System.Type[] {
                    typeof(global::SWA.Ariadne.Model.Maze)}, args);
        return priv_obj.Target;
    }
    
    internal void Reset() {
        object[] args = new object[0];
        m_privateObject.Invoke("Reset", new System.Type[0], args);
    }
    
    internal void Step(out global::SWA.Ariadne.Model.MazeSquare sq1, out global::SWA.Ariadne.Model.MazeSquare sq2, out bool forward) {
        object[] args = new object[] {
                null,
                null,
                null};
        m_privateObject.Invoke("Step", new System.Type[] {
                    typeof(global::SWA.Ariadne.Model.MazeSquare).MakeByRefType(),
                    typeof(global::SWA.Ariadne.Model.MazeSquare).MakeByRefType(),
                    typeof(bool).MakeByRefType()}, args);
        sq1 = ((global::SWA.Ariadne.Model.MazeSquare)(args[0]));
        sq2 = ((global::SWA.Ariadne.Model.MazeSquare)(args[1]));
        forward = ((bool)(args[2]));
    }
    
    internal void StepI(out global::SWA.Ariadne.Model.MazeSquare sq1, out global::SWA.Ariadne.Model.MazeSquare sq2, out bool forward) {
        object[] args = new object[] {
                null,
                null,
                null};
        m_privateObject.Invoke("StepI", new System.Type[] {
                    typeof(global::SWA.Ariadne.Model.MazeSquare).MakeByRefType(),
                    typeof(global::SWA.Ariadne.Model.MazeSquare).MakeByRefType(),
                    typeof(bool).MakeByRefType()}, args);
        sq1 = ((global::SWA.Ariadne.Model.MazeSquare)(args[0]));
        sq2 = ((global::SWA.Ariadne.Model.MazeSquare)(args[1]));
        forward = ((bool)(args[2]));
    }
    
    internal static System.Collections.Generic.List<SWA.Ariadne.Model.MazeSquare.WallPosition> OpenWalls(global::SWA.Ariadne.Model.MazeSquare sq, bool notVisitedOnly) {
        object[] args = new object[] {
                sq,
                notVisitedOnly};
        System.Collections.Generic.List<SWA.Ariadne.Model.MazeSquare.WallPosition> ret = ((System.Collections.Generic.List<SWA.Ariadne.Model.MazeSquare.WallPosition>)(m_privateType.InvokeStatic("OpenWalls", new System.Type[] {
                    typeof(global::SWA.Ariadne.Model.MazeSquare),
                    typeof(bool)}, args)));
        return ret;
    }
}
}