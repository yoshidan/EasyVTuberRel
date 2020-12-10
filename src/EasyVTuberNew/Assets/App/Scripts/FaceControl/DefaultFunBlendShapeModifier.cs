﻿using VRM;

namespace App.Main.Scripts.FaceControl
{
    /// <summary>
    /// 何もしてないときも若干Funのブレンドシェイプを入れるやつ。
    /// </summary>
    public class DefaultFunBlendShapeModifier
    {
        private BlendShapeKey FunKey { get; } = new BlendShapeKey(BlendShapePreset.Fun);

        public float FaceDefaultFunValue { get; set; } = 0.0f;

        public void Apply(VRMBlendShapeProxy proxy)
        {
            //proxy.AccumulateValue(FunKey, FaceDefaultFunValue);
        }
        
        public void Reset(VRMBlendShapeProxy proxy)
        {
            //proxy.AccumulateValue(FunKey, 0);
        }
    }
}
