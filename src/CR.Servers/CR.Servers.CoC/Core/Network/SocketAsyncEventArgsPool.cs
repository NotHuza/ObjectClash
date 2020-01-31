namespace CR.Servers.CoC.Core.Network
{
    using System.Collections.Concurrent;
    using System.Net.Sockets;

    internal class SocketAsyncEventArgsPool : ConcurrentStack<SocketAsyncEventArgs>
    {
        internal SocketAsyncEventArgsPool()
        {

            /*
            for (int i = 0; i < (sendPool ? Constants.MaxSends : Constants.MaxPlayers); i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();

                if (!sendPool)
                {
                    args.SetBuffer(new byte[Constants.ReceiveBuffer], 0, Constants.ReceiveBuffer);
                }

                args.DisconnectReuseSocket = true;

                this.Enqueue(args);
            }
            */
        }

        internal SocketAsyncEventArgs Dequeue()
        {
            var asyncEvent = default(SocketAsyncEventArgs);
            if (this.TryPop(out asyncEvent))
                return asyncEvent;

            return null;
        }

        internal void Enqueue(SocketAsyncEventArgs asyncEvent)
        {
            if (Count > 4096)
                return;

            asyncEvent.UserToken = null;
            asyncEvent.AcceptSocket = null;
            asyncEvent.SetBuffer(null, 0, 0);

            Push(asyncEvent);
        }
    }
}