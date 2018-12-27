using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace FileMoose
{
    public class Connection
    {
        private TcpClient _tCPClient;

        private NetworkStream _networkStream;
        private StreamReader _reader;
        private StreamWriter _writer;
        private string _transferType;

        private string _username;

        public Connection(TcpClient client) {
            _tCPClient = client;
            _networkStream = client.GetStream();

            _reader = new StreamReader(_networkStream);
            _writer = new StreamWriter(_networkStream);
        }

        public void HandleClient(object obj) {
            _writer.WriteLine("220 Service ready");
            _writer.Flush();

            string line;

            try
            {
                while (!string.IsNullOrEmpty(line = _reader.ReadLine()))
                {
                    string response = null;

                    string[] command = line.Split(' ');

                    string cmd = command[0].ToUpperInvariant();
                    string arguments = command.Length > 1 ? line.Substring(command[0].Length + 1) : null;

                    if (string.IsNullOrWhiteSpace(arguments))
                        arguments = null;
                    if (response == null)
                    {

                        switch (cmd)
                        {
                            case "USER":
                                response = User(arguments);
                                break;
                            case "PASS":
                                response = Password(arguments);
                                break;
                            case "CWD":
                                response = ChangeWorkingDirectory(arguments);
                                break;
                            case "CDUO":
                                response = ChangeWorkingDirectory("..");
                                break;
                            case "PWD":
                                response = "257 \"/\" is current directory.";
                                break;
                            case "QUIT":
                                response = "221 Service closing connection";
                                break; 
                            case "TYPE":
                                string[] splitArgs = arguments.Split(' ');
                                response = Type(splitArgs[0], splitArgs.Length > 1 ? splitArgs[1] : null);
                                break;
                            default:
                                response = "502 Have no idea of what you mean";
                                break;
                        }
                    }

                    if (_tCPClient == null || !_tCPClient.Connected)
                    {
                        break;
                    }
                    else
                    {
                        _writer.WriteLine(response);
                        _writer.Flush();

                        if (response.StartsWith("221"))
                        {
                            break;
                        }
                    }

                }
            }

            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private string Type(string typeCode, string formatControl)
        {
            string response = "500 Error";
            switch (typeCode) {
                case "I":
                    _transferType = typeCode;
                    response = "200 OK";
                    break;
                case "A":
                    break;
                case "E":
                    break;
                case "L":
                    break;
                default:
                    response = "504 Command not implemented for that parameter";
                    break;
            }

            if (formatControl != null)
            {
                switch (formatControl)
                {
                    case "N":
                        response = "200 OK";
                        break;
                    case "T":
                        break;
                    case "C":
                        break;
                    default:
                        response = "504 Command not implemented for that parameter";
                        break;
                }
            }

            return response;
        }

        private string User(string username) {
            _username = username;

            return "331 Username is valid, need a password";
        }

        private string Password(string password) {

            if (true)
            {
                return "230 User logged in";
            }
            else {
                return "530 Not logged in";
            }
        }

        private string ChangeWorkingDirectory(string pathname) {
            return "250 Changed to new directory";
        }
    }
}
