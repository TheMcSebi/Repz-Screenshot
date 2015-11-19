using RepzScreenshot.Error;
using RepzScreenshot.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RepzScreenshot.DataAccess
{
    class ServerDataAccess : DataAccessBase
    {
        private Server server;
        public ServerDataAccess(Server server)
        {
            this.server = server;
        }

        public async Task<List<Player>> GetPlayersAsync()
        {
            
            using (UdpClient udpClient = new UdpClient())
            {
                
                try
                {
                    udpClient.Client.ReceiveTimeout = 3000;
                    udpClient.Client.SendTimeout = 3000;
                    udpClient.Connect(server.Address, server.Port);
                        

                    Byte[] tmpBytes = Encoding.ASCII.GetBytes("getstatus");
                    byte[] sendBytes = new byte[tmpBytes.Length + 4];

                    sendBytes[0] = byte.Parse("255");
                    sendBytes[1] = byte.Parse("255");
                    sendBytes[2] = byte.Parse("255");
                    sendBytes[3] = byte.Parse("255");

                    int j = 4;

                    for (int i = 0; i < tmpBytes.Length; i++)
                    {
                        sendBytes[j++] = tmpBytes[i];
                    }


                    await udpClient.SendAsync(sendBytes, sendBytes.Length);

                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(server.Address, server.Port);


                    UdpReceiveResult receive = await udpClient.ReceiveAsync();

                    string returnData = Encoding.ASCII.GetString(receive.Buffer);
                        
                    String[] lines = returnData.Split('\n');

                    Regex rx = new Regex("(?<score>\\d+) (?<ping>\\d+) \"(?<name>.*)\"",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase);

                    List<Player> players = new List<Player>();

                    for (int i = 2; i < lines.Length - 1; ++i)
                    {

                        GroupCollection info = rx.Match(lines[i]).Groups;

                        string name = info["name"].Value;
                        int score = Convert.ToInt32(info["score"].Value);
                        int ping = Convert.ToInt32(info["ping"].Value);

                        Player p = await RepzDataAccess.GetPlayer(name, false);
                        p.Score = score;
                        p.Ping = ping;
                        players.Add(p);
                    }
                    return players;
                }
                catch(SocketException ex)
                {
                    switch(ex.SocketErrorCode)
                    {
                        case SocketError.HostUnreachable:
                        case SocketError.NetworkUnreachable:
                        case SocketError.TimedOut:
                        case SocketError.NetworkDown:
                            throw new ApiException("Server unreachable");
                    }
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

    }
}