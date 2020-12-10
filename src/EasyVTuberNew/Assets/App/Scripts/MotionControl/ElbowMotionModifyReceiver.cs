using App.Main.Scripts.Interprocess;
using App.Main.Scripts.Interprocess.Model;
using UnityEngine;
using UniRx;
using Zenject;

namespace App.Main.Scripts.MotionControl
{
    public class ElbowMotionModifyReceiver : MonoBehaviour
    {
        [Inject] private ReceivedMessageHandler handler = null;
        [SerializeField] private ElbowMotionModifier modifier = null;
        
        private void Start()
        {
            handler.Commands.Subscribe(message =>
            {
                switch (message.Command)
                {
                    case MessageCommandNames.SetWaistWidth:
                        modifier.SetWaistWidth(message.ParseAsCentimeter());
                        break;
                    case MessageCommandNames.SetElbowCloseStrength:
                        modifier.SetElbowCloseStrength(message.ParseAsPercentage());
                        break;
                }
            });
        }
    }
}
