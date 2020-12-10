using System;
using UnityEngine;
using VRM;

namespace App.Main.Scripts.VRMLoad
{
    public interface IVRMLoadable
    {
        /// <summary>VRMがロードされると呼び出されます。VrmLoadedより先に呼ばれます。</summary>
        event Action<VrmLoadedInfo> PreVrmLoaded;
        /// <summary>VRMがロードされると呼び出されます。</summary>
        event Action<VrmLoadedInfo> VrmLoaded;
        /// <summary>VRMをアンロードするときに呼び出されます。</summary>
        event Action VrmDisposing;    
        
    }
    
    [Serializable]
    public struct VrmLoadedInfo
    {
        public VRMImporterContext context;
        public Transform vrmRoot;
        public Animator animator;
        public VRMBlendShapeProxy blendShape;
    }
}