using App.Main.Scripts.Interprocess;
using App.Main.Scripts.Interprocess.Interfaces;
using App.Main.Scripts.Interprocess.Model;
using UnityEngine;
using UniRx;
using Zenject;

namespace App.Main.Scripts.FaceControl.BlendShapeAssignment
{
    public class BlendShapeAssignReceiver : MonoBehaviour
    {
        [Inject] private ReceivedMessageHandler handler = null;
        [SerializeField] private FaceControlManager faceControlManager = null;

        private EyebrowBlendShapeSet EyebrowBlendShape => faceControlManager.EyebrowBlendShape;
        
        private void Start()
        {
            handler.Commands.Subscribe(message =>
            {
                switch (message.Command)
                {
                    case MessageCommandNames.EyebrowLeftUpKey:
                        EyebrowBlendShape.LeftUpKey = message.Content;
                        break;
                    case MessageCommandNames.EyebrowLeftDownKey:
                        EyebrowBlendShape.LeftDownKey = message.Content;
                        RefreshTarget();
                        break;
                    case MessageCommandNames.UseSeparatedKeyForEyebrow:
                        EyebrowBlendShape.UseSeparatedTarget = message.ToBoolean();
                        RefreshTarget();
                        break;
                    case MessageCommandNames.EyebrowRightUpKey:
                        EyebrowBlendShape.RightUpKey = message.Content;
                        RefreshTarget();
                        break;
                    case MessageCommandNames.EyebrowRightDownKey:
                        EyebrowBlendShape.RightDownKey = message.Content;
                        RefreshTarget();
                        break;
                    case MessageCommandNames.EyebrowUpScale:
                        EyebrowBlendShape.UpScale = message.ParseAsPercentage();
                        break;
                    case MessageCommandNames.EyebrowDownScale:
                        EyebrowBlendShape.DownScale = message.ParseAsPercentage();
                        break;
                }
            });
        }
        
        private void RefreshTarget() => EyebrowBlendShape.RefreshTarget(faceControlManager.BlendShapeStore);

        public string[] TryGetBlendShapeNames() => faceControlManager.BlendShapeStore.GetBlendShapeNames();

    }
}
