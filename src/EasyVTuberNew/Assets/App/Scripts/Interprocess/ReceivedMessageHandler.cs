using System;
using System.Collections.Concurrent;
using App.Main.Scripts.Interprocess.Model;
using UnityEngine;
using UniRx;

namespace App.Main.Scripts.Interprocess
{
    /// <summary> メッセージを受け取ってUIスレッドで再配布する </summary>
    public class ReceivedMessageHandler : MonoBehaviour
    {
        private readonly Subject<ReceivedCommand> _commandsSubject = new Subject<ReceivedCommand>();
        public IObservable<ReceivedCommand> Commands => _commandsSubject;
        
        private readonly ConcurrentQueue<ReceivedCommand> _receivedCommands = new ConcurrentQueue<ReceivedCommand>();

        private void Update()
        {
            while (_receivedCommands.TryDequeue(out var command))
            {
                ProcessCommand(command);
            }
        }
        
        public void ReceiveCommand(ReceivedCommand command)
        {
            _receivedCommands.Enqueue(command);
        }

        private void ProcessCommand(ReceivedCommand command)
        {
            _commandsSubject.OnNext(command);
        }
    }
}

