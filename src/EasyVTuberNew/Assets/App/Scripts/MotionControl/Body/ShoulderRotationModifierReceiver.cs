using System;
using App.Main.Scripts.Interprocess;
using App.Main.Scripts.Interprocess.Model;
using UnityEngine;
using UniRx;
using Zenject;

namespace App.Main.Scripts.MotionControl
{
    [RequireComponent(typeof(ShoulderRotationModifier))]
    public class ShoulderRotationModifierReceiver : MonoBehaviour
    {
        [Inject] private ReceivedMessageHandler _handler = null;

        private void Start()
        {
            var modifier = GetComponent<ShoulderRotationModifier>();
            _handler.Commands.Subscribe(c =>
            {
                if (c.Command == MessageCommandNames.EnableShoulderMotionModify)
                {
                    modifier.EnableRotationModification = c.ToBoolean();
                }
            });
        }
    }
}
