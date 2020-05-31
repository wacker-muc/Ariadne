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
internal class SWA_Ariadne_Logic_SolverFactoryAccessor : BaseAccessor {
    
    protected static Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType m_privateType = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateType(typeof(global::SWA.Ariadne.Logic.SolverFactory));
    
    internal SWA_Ariadne_Logic_SolverFactoryAccessor() : 
            base(m_privateType) {
    }
    
    internal static global::System.Type[] solverTypes {
        get {
            global::System.Type[] ret = ((global::System.Type[])(m_privateType.GetStaticField("solverTypes")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("solverTypes", value);
        }
    }
    
    internal static global::System.Type[] noEfficientSolverTypes {
        get {
            global::System.Type[] ret = ((global::System.Type[])(m_privateType.GetStaticField("noEfficientSolverTypes")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("noEfficientSolverTypes", value);
        }
    }
    
    internal static string EfficientPrefix {
        get {
            string ret = ((string)(m_privateType.GetStaticField("EfficientPrefix")));
            return ret;
        }
        set {
            m_privateType.SetStaticField("EfficientPrefix", value);
        }
    }
    
    internal static global::System.Type SolverType(string name) {
        object[] args = new object[] {
                name};
        global::System.Type ret = ((global::System.Type)(m_privateType.InvokeStatic("SolverType", new System.Type[] {
                    typeof(string)}, args)));
        return ret;
    }
    
    internal static global::SWA.Ariadne.Logic.IMazeSolver CreateSolver(global::System.Type solverType, global::SWA.Ariadne.Model.Maze maze, global::SWA.Ariadne.Logic.IMazeDrawer mazeDrawer) {
        object[] args = new object[] {
                solverType,
                maze,
                mazeDrawer};
        global::SWA.Ariadne.Logic.IMazeSolver ret = ((global::SWA.Ariadne.Logic.IMazeSolver)(m_privateType.InvokeStatic("CreateSolver", new System.Type[] {
                    typeof(global::System.Type),
                    typeof(global::SWA.Ariadne.Model.Maze),
                    typeof(global::SWA.Ariadne.Logic.IMazeDrawer)}, args)));
        return ret;
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
    
    internal global::SWA.Ariadne.Logic.IMazeDrawer mazeDrawer {
        get {
            global::SWA.Ariadne.Logic.IMazeDrawer ret = ((global::SWA.Ariadne.Logic.IMazeDrawer)(m_privateObject.GetField("mazeDrawer")));
            return ret;
        }
        set {
            m_privateObject.SetField("mazeDrawer", value);
        }
    }
    
    internal global::System.Random random {
        get {
            global::System.Random ret = ((global::System.Random)(m_privateObject.GetField("random")));
            return ret;
        }
        set {
            m_privateObject.SetField("random", value);
        }
    }
    
    internal global::SWA.Ariadne.Model.DeadEndChecker deadEndChecker {
        get {
            global::SWA.Ariadne.Model.DeadEndChecker ret = ((global::SWA.Ariadne.Model.DeadEndChecker)(m_privateObject.GetField("deadEndChecker")));
            return ret;
        }
        set {
            m_privateObject.SetField("deadEndChecker", value);
        }
    }
    
    internal bool IsEfficientSolver {
        get {
            bool ret = ((bool)(m_privateObject.GetProperty("IsEfficientSolver")));
            return ret;
        }
    }
    
    internal void MakeEfficient() {
        object[] args = new object[0];
        m_privateObject.Invoke("MakeEfficient", new System.Type[0], args);
    }
    
    internal void CoordinateWithMaster(global::SWA.Ariadne.Logic.IMazeSolver masterSolver) {
        object[] args = new object[] {
                masterSolver};
        m_privateObject.Invoke("CoordinateWithMaster", new System.Type[] {
                    typeof(global::SWA.Ariadne.Logic.IMazeSolver)}, args);
    }
    
    internal static object CreatePrivate(global::SWA.Ariadne.Model.Maze maze, global::SWA.Ariadne.Logic.IMazeDrawer mazeDrawer) {
        object[] args = new object[] {
                maze,
                mazeDrawer};
        Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject priv_obj = new Microsoft.VisualStudio.TestTools.UnitTesting.PrivateObject("SWA.Ariadne.Logic", "SWA.Ariadne.Logic.SolverBase", new System.Type[] {
                    typeof(global::SWA.Ariadne.Model.Maze),
                    typeof(global::SWA.Ariadne.Logic.IMazeDrawer)}, args);
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
    
    internal void Solve() {
        object[] args = new object[0];
        m_privateObject.Invoke("Solve", new System.Type[0], args);
    }
    
    internal void FillStatusMessage(global::System.Text.StringBuilder message) {
        object[] args = new object[] {
                message};
        m_privateObject.Invoke("FillStatusMessage", new System.Type[] {
                    typeof(global::System.Text.StringBuilder)}, args);
    }
    
    internal System.Collections.Generic.List<SWA.Ariadne.Model.Interfaces.WallPosition> OpenWalls(global::SWA.Ariadne.Model.MazeSquare sq, bool notVisitedOnly) {
        object[] args = new object[] {
                sq,
                notVisitedOnly};
        System.Collections.Generic.List<SWA.Ariadne.Model.Interfaces.WallPosition> ret = ((System.Collections.Generic.List<SWA.Ariadne.Model.Interfaces.WallPosition>)(m_privateObject.Invoke("OpenWalls", new System.Type[] {
                    typeof(global::SWA.Ariadne.Model.MazeSquare),
                    typeof(bool)}, args)));
        return ret;
    }
}
}