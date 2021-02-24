using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Business
{
    public class LetterPressedArgs : EventArgs
    {
        public char Letter { get; set; }
    }

    public class PlayAgainArgs : EventArgs
    {
        public string Response { get; set; }
    }

    public class NetworkServices
    {
        // TCPListener to listen for incomming TCP connection requests.
        static System.Net.Sockets.TcpListener tcpListener;

        // Create handle to connected tcp client.
        TcpClient connectedTcpClient;

        CancellationTokenSource source;
        CancellationToken token;

        public event EventHandler GameStarted;
        public event EventHandler ServerStarted;
        public event EventHandler ServerStoped;
        public event EventHandler ClientConnected;
        public event EventHandler ClientDisconnected;
        public event EventHandler<LetterPressedArgs> ClientPressedLetter;
        public event EventHandler<PlayAgainArgs> ClientPlayAgainResponse;

        public async Task Start(IPAddress iP, int port)
        {
            tcpListener = new TcpListener(iP, port);

            connectedTcpClient = null;
            source = new CancellationTokenSource();
            token = source.Token;

            tcpListener.Start();

            EventHandler serverStartedHandler = ServerStarted;
            if (serverStartedHandler != null)
            {
                serverStartedHandler(this, null);
            }

            connectedTcpClient = await Task.Run(() => tcpListener.AcceptTcpClientAsync(), token);

            if (connectedTcpClient != null)
            {
                EventHandler clientConnectedHandler = ClientConnected;
                if (clientConnectedHandler != null)
                {
                    clientConnectedHandler(this, null);
                }

                var c = new Thread(() => ServerReceive());
                c.Start();
            }
        }

        public void ServerReceive()
        {
            byte[] data = new byte[1000];
            //String text = null;
            while (!source.IsCancellationRequested)
            {
                try
                {
                    NetworkStream stream = connectedTcpClient.GetStream(); //Gets The Stream of The Connection
                    stream.Read(data, 0, data.Length); //Receives Data 
                    List<string> parts = (List<string>)ByteArrayToObject(data);

                    if (!SocketConnected())
                    {
                        Stop();
                        break;
                    }

                    switch (parts[0])
                    {
                        case "start":
                            switch (parts[1])
                            {
                                case ("Yes"):
                                    EventHandler GameStartedHandler = GameStarted;
                                    if (GameStartedHandler != null)
                                    {
                                        GameStartedHandler(this, null);
                                    }
                                    break;
                                case ("No"):
                                    MessageBox.Show("Client Refused to play, start server again", "server stopped");
                                    Stop();
                                    break;
                            }
                            break;
                        case "letter":
                            EventHandler<LetterPressedArgs> clientPressedLetterHandler = ClientPressedLetter;
                            if (clientPressedLetterHandler != null)
                            {
                                clientPressedLetterHandler(this, new LetterPressedArgs() { Letter = parts[1][0] });
                            }
                            break;
                        case "playAgain":
                            EventHandler<PlayAgainArgs>  ClientPlayAgainHandler= ClientPlayAgainResponse;
                            if (ClientPlayAgainHandler != null)
                            {
                                ClientPlayAgainHandler(this, new PlayAgainArgs() { Response = parts[1] });
                            }
                            break;
                    }

                    parts.Clear();
                }
                catch (Exception ex)
                {
                    EventHandler clientDisconnectedHandler = ClientDisconnected;
                    if (clientDisconnectedHandler != null)
                    {
                        clientDisconnectedHandler(this, null);
                    }

                    break;
                }
            }
        }

        public void Stop()
        {
            if (tcpListener != null)
            {
                if (tcpListener.Server.IsBound)
                {
                    if (connectedTcpClient != null)
                    {
                        if (connectedTcpClient.Client.IsBound)
                        {
                            connectedTcpClient.Client.Close();
                            connectedTcpClient = null;
                        }
                    }

                    source.Cancel();
                    tcpListener.Stop();

                    EventHandler serverStopedHandler = ServerStoped;
                    if (serverStopedHandler != null)
                    {
                        serverStopedHandler(this, null);
                    }
                }
            }
        }

        public void Send(params string[] text)
        {
            try
            {
                byte[] byData = ObjectToByteArray(text.ToList());

                NetworkStream stm = connectedTcpClient.GetStream();
                stm.Write(byData, 0, byData.Length);
                stm.Flush();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public Object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        public byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        bool SocketConnected() //check whether client is connected server
        {
            bool flag = false;
            try
            {
                bool part1 = connectedTcpClient.Client.Poll(10, SelectMode.SelectRead);
                bool part2 = (connectedTcpClient.Available == 0);
                if (part1 && part2)
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }
            return flag;
        }
    }
}
