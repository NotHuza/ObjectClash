using System;
using System.Collections.Concurrent;
using System.Threading;
using CR.Servers.CoC.Logic;
using CR.Servers.CoC.Packets;
using System.Threading.Tasks;
using CR.Servers.CoC.Packets.Messages.Client.Home;

namespace CR.Servers.CoC.Core.Network
{
    internal class Processor
    {
        private readonly int _processorCount;
        /* Number of processor that processes non-high priority messages. */
        private readonly int _normalPriorityProcessorCount;

        /* Processors that processes messages like KeepAlive. */
        private readonly int _highPriorityIncomingProcessor;
        private readonly int _highPriorityOutgoingProcessor;

        private readonly IncomingProcessorThread[] _incomingThreads;
        private readonly OutgoingProcessorThread[] _outgoingThreads;

        internal Processor()
        {
            _processorCount = Environment.ProcessorCount;

            if (Environment.ProcessorCount > 1)
            {
                _normalPriorityProcessorCount = _processorCount - 1;
                _highPriorityIncomingProcessor = _processorCount - 1;
                _highPriorityOutgoingProcessor = _processorCount - 1;
            }
            else
            {
                _normalPriorityProcessorCount = _processorCount;
                _highPriorityIncomingProcessor = 0;
                _highPriorityOutgoingProcessor = 0;
            }

            _incomingThreads = new IncomingProcessorThread[_processorCount];
            _outgoingThreads = new OutgoingProcessorThread[_processorCount];

            /* Initialize the processor threads. */
            for (int i = 0; i < _processorCount; i++)
            {
                _incomingThreads[i] = new IncomingProcessorThread();
                _outgoingThreads[i] = new OutgoingProcessorThread();
            }

            /* Start the processor threads. */
            for (int i = 0; i < _processorCount; i++)
            {
                _incomingThreads[i].Start();
                _outgoingThreads[i].Start();
            }
        }

        public IncomingProcessorThread[] IncomingThreads => _incomingThreads;
        public OutgoingProcessorThread[] OutgoingThreads => _outgoingThreads;

        public int GetNextOutgoingQueueId()
        {
            /* Select a processor with the least load. */
            int index = 0;
            int min = int.MaxValue;
            int count = _normalPriorityProcessorCount;

            for (int i = 0; i < count; i++)
            {
                int messageCount = _outgoingThreads[i].Count;
                if (messageCount < min)
                {
                    min = messageCount;
                    index = i;
                }
            }

            return index;
        }

        public int GetNextIncomingQueueId()
        {
            /* Select a processor with the least load. */
            int index = 0;
            int min = int.MaxValue;
            int count = _normalPriorityProcessorCount;

            for (int i = 0; i < count; i++)
            {
                int messageCount = _incomingThreads[i].Count;
                if (messageCount < min)
                {
                    min = messageCount;
                    index = i;
                }
            }

            return index;
        }

        /* Enqueues a message we're sending on a processing thread. */
        public void EnqueueOutgoing(Message message, int queueId)
        {
            if (queueId == -1)
                queueId = _highPriorityOutgoingProcessor;

            var processor = _outgoingThreads[queueId];
            processor._queue.Enqueue(message);
        }

        /* Enqueues a message we've received on a processing thread. */
        public void EnqueueIncoming(Message message, int queueId)
        {
            if (queueId == -1)
                queueId = _highPriorityIncomingProcessor;

            var processor = _incomingThreads[queueId];
            processor._queue.Enqueue(message);
        }

        /* Represents a thread which processes incoming messages. */
        public class IncomingProcessorThread
        {
            private readonly Thread _thread;
            internal readonly ConcurrentQueue<Message> _queue;

            public IncomingProcessorThread()
            {
                _thread = new Thread(Process);
                _queue = new ConcurrentQueue<Message>();
            }

            public int Count => _queue.Count;

            public void Start()
            {
                _thread.Start();
            }

            private async void Process()
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            Message message;
                            while (_queue.TryDequeue(out message))
                            {
                                try
                                { message.Decode(); }
                                catch (Exception ex)
                                { Logging.Error(message.GetType(), "Exception while decoding message: " + ex); }

                                message.Timer.Restart();
                                Task processTask = message.ProcessAsync();
                                message.Timer.Stop();

                                try
                                { await processTask; }
                                catch (Exception ex)
                                { Logging.Error(message.GetType(), "Exception while processing incoming message async: " + ex); }


                                if (message.Timer.Elapsed.TotalMilliseconds > 2000)
                                {
                                    Console.WriteLine($"{message.GetType().Name}: Blocking processor -> {message.Timer.Elapsed.TotalMilliseconds}ms.");
                                    if (message is EndClientTurnMessage)
                                    {
                                        var endClientTurn = (EndClientTurnMessage)message;
                                        foreach (var command in endClientTurn.Commands)
                                            Console.WriteLine($" ----> {command.GetType().Name}");
                                    }
                                }

                                message.Device.Flush();
                            }

                            Thread.Sleep(1);
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(typeof(IncomingProcessorThread), "Exception inside incoming processing queue: " + ex);
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    // Space
                }
            }
        }

        /* Represents a thread which processes outgoing messages. */
        public class OutgoingProcessorThread
        {
            private readonly Thread _thread;
            internal readonly ConcurrentQueue<Message> _queue;

            public OutgoingProcessorThread()
            {
                _thread = new Thread(Process);
                _queue = new ConcurrentQueue<Message>();
            }

            public int Count => _queue.Count;

            public void Start()
            {
                _thread.Start();
            }

            private void Process()
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            Message message;
                            while (_queue.TryDequeue(out message))
                            {
                                Device device = message.Device;

                                try
                                { message.Encode(); }
                                catch (Exception ex)
                                { Logging.Error(message.GetType(), "Exception while encoding message: " + ex); }

                                byte[] body = message.Data.ToArray();

                                if (device.SendEncrypter != null)
                                {
                                    /* Encrypt the message locking the Encrypter to avoid breaking the xor stream.*/
                                    lock (device.SendEncrypter)
                                        body = device.SendEncrypter.Encrypt(body);
                                }

                                try
                                { message.Process(); }
                                catch (Exception ex)
                                { Logging.Error(message.GetType(), "Exception while processing outgoing message: " + ex); }

                                byte[] packet = new byte[body.Length + 7];

                                /* Write header. */
                                packet[1] = (byte)message.Type;
                                packet[0] = (byte)(message.Type >> 8);

                                packet[4] = (byte)body.Length;
                                packet[3] = (byte)(body.Length >> 8);
                                packet[2] = (byte)(body.Length >> 16);

                                packet[6] = (byte)message.Version;
                                packet[5] = (byte)(message.Version >> 8);

                                /* Write body. */
                                Array.Copy(body, 0, packet, 7, body.Length);

                                /* Send data to the socket. */
                                Resources.Gateway.Send(packet, message.Device.Token);
                                message.Device.Flush();
                            }

                            Thread.Sleep(1);
                        }
                        catch (Exception ex)
                        {
                            Logging.Error(typeof(OutgoingProcessorThread), "Exception inside outgoing processing queue: " + ex);
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    // Space
                }
            }
        }
    }
}