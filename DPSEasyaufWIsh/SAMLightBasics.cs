using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DPSEasyaufWish
{
    public class SAMLightBasics
    {
        private string _ipAddress;
        private int _port;

        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private Encoding _encoding;

        public SAMLightBasics(SAMLightConfig config)
        {

            _ipAddress = config.IpSamlight;
            _port = config.PortSamlight;
            _encoding = Encoding.GetEncoding(config.Codepage);
        }

        public bool Initialize()
        {
            try
            {
                _tcpClient = new TcpClient(_ipAddress, _port);
                _tcpClient.NoDelay = true;
                _tcpClient.ReceiveTimeout = 5000;
                _tcpClient.SendTimeout = 5000;

                _networkStream = _tcpClient.GetStream();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool ExecuteCommand(string command, out string response)
        {
            try
            {
                Console.WriteLine($"Command to Send: {command.TrimEnd()}");
                byte[] byteCommand = _encoding.GetBytes(command);
                Console.WriteLine(BitConverter.ToString(byteCommand));
                _networkStream.Write(byteCommand, 0, byteCommand.Length);
                _networkStream.Flush();

                byte[] responseByte = new byte[512];
                //int lengthTmp = _networkStream.Read(responseByte, 0, responseByte.Length);
                byte endbyte = _encoding.GetBytes("\n").AsSpan()[0];
                Console.WriteLine($"Endbyte is {BitConverter.ToString(new[] { endbyte })}");
                int length = 0;
                while (true)
                {
                    int lengthTmp = _networkStream.Read(responseByte, length, responseByte.Length - length);
                    Console.WriteLine($"Recived {lengthTmp} Bytes");

                    if (responseByte.AsSpan().Contains(endbyte))
                    {
                        if (length == 0)
                        {
                            length = length + lengthTmp;
                        }
                        break;
                    }
                    length = length + lengthTmp;
                }
                //_logger.LogDebug(BitConverter.ToString(responseByte));
                response = _encoding.GetString(responseByte, 0, length);
                Console.WriteLine($"Recived {response.TrimEnd()}");
                _networkStream.Flush();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                response = "";
                return false;
            }
        }

        public void Close()
        {
            _tcpClient?.Close();
        }
    }
}
