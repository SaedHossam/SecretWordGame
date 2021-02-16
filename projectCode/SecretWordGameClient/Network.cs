using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SecretWordGameClient
{
    public class LetterPressedArgs : EventArgs
    {
        public char Letter { get; set; }
    }

    public class NewGamePressedArgs : EventArgs
    {
        public string SecretWord { get; set; }
    }

    public class Network
    {
        // Create handle to connected tcp client.
        TcpClient tcpClient;
        public NetworkStream serverStream;

        Thread ctThread;

        CancellationTokenSource source;
        CancellationToken token;

        public event EventHandler GameStarted;
        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public event EventHandler<LetterPressedArgs> ServerPressedLetter;
        public event EventHandler<NewGamePressedArgs> NewGame;

        public async Task Start(IPAddress ip, int port)
        {
            try
            {
                tcpClient = new TcpClient();

                tcpClient.Connect(ip, port);
                source = new CancellationTokenSource();
                token = source.Token;

                EventHandler connectedHandler = Connected;
                if (connectedHandler != null)
                {
                    connectedHandler(this, null);
                }

                ctThread = new Thread(ClientReceive);
                ctThread.Start();
            }
            catch (Exception er)
            {
                MessageBox.Show("Server Not Started");
            }
        }

        public void ClientReceive()
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    serverStream = tcpClient.GetStream();
                    byte[] inStream = new byte[10025];
                    serverStream.Read(inStream, 0, inStream.Length);
                    List<string> parts = null;

                    if (!SocketConnected())
                    {
                        ctThread.Abort();
                    }

                    parts = (List<string>)ByteArrayToObject(inStream);
                    switch (parts[0])
                    {
                        case "askStart":
                            DialogResult result = MessageBox.Show(parts[1], "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            Send("start", result.ToString());
                            break;
                        case "runGame":
                            EventHandler gameStartedHandler = GameStarted;
                            if (gameStartedHandler != null)
                            {
                                gameStartedHandler(this, null);
                            }
                            break;
                        case "newGame":
                            EventHandler<NewGamePressedArgs> NewGameHandler = NewGame;
                            if (NewGameHandler != null)
                            {
                                NewGameHandler(this, new NewGamePressedArgs() { SecretWord = parts[1] });
                            }
                            break;
                        case "letter":
                            EventHandler<LetterPressedArgs> serverPressedLetterHandler = ServerPressedLetter;
                            if (serverPressedLetterHandler != null)
                            {
                                serverPressedLetterHandler(this, new LetterPressedArgs() { Letter = parts[1][0] });
                            }
                            break;
                    }
                }
            }
            catch (IOException e)
            {
                tcpClient.Close();
                EventHandler disconnectedHandler = Disconnected;
                if (disconnectedHandler != null)
                {
                    disconnectedHandler(this, null);
                }
            }
        }

        public void Stop()
        {
            if (tcpClient != null)
            {
                if (tcpClient.Client.IsBound)
                {
                    source.Cancel();
                    tcpClient.Close();
                    tcpClient.Dispose();

                    EventHandler disconnectedHandler = Disconnected;
                    if (disconnectedHandler != null)
                    {
                        disconnectedHandler(this, null);
                    }
                }
            }
        }

        public void Send(params string[] text)
        {
            try
            {
                byte[] byData = ObjectToByteArray(text.ToList());

                NetworkStream stm = tcpClient.GetStream();
                stm.Write(byData, 0, byData.Length);
                stm.Flush();
            }
            catch (SocketException se)
            {
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
                bool part1 = tcpClient.Client.Poll(10, SelectMode.SelectRead);
                bool part2 = (tcpClient.Available == 0);
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
