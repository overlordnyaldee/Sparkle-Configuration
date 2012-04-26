using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Sparkle_Configuration
{

    // TODO: COMPLETE REWRITE FOR JSON COMMUNICATION OVER HTTP
    class BotCommunication
    {
        public const int defaultPort = 5700;
        public const String loginCommand = "login";

        public byte[] computePasswordHash(String passwordString)
        {
            using (SHA256 shaM = new SHA256Managed())
            {
                return shaM.ComputeHash(StringToByteArray(passwordString));
            }
        }

        private bool sendServerDataAndCheckResponseTCP(String hostname, String port, byte[] dataToSend, String expectedResponse)
        {

            int portNum = convertStringToInt(port);

            try
            {
                // Setup a listener at the local IP adres, port 2200
                IPAddress localAddress = IPAddress.Parse("127.0.0.1");

                //TcpListener listener = new TcpListener(localAddress, portNum);
                TcpClient sender = new TcpClient(hostname, portNum);
                //TcpListener listener = sender.
                NetworkStream io = sender.GetStream();

                // open listener
                //listener.Start(1);
                
                //Debug.WriteLine("Server is waiting on socket {0}", listener.LocalEndpoint);
                Debug.WriteLine("Server is waiting on socket {0}", sender.Client);

                // send auth data
                io.Write(dataToSend, 0, dataToSend.Length);
                Debug.WriteLine("Sending auth code..");

                

                // The program is suspended while waiting for an incoming connection.
                // This is a synchronous TCP application
                //TcpClient client = listener.AcceptTcpClient();
                //TcpListener listener = sender.g

                // Obtain a stream object for reading and writing
                //NetworkStream io = client.GetStream();

                // An incoming connection needs to be processed.
                //Debug.WriteLine("Received Connection from {0}", sender.Client.RemoteEndPoint);
                //Debug.WriteLine("Sending message..");
                //io.Write(dataToSend, 0, dataToSend.Length);


                StringBuilder sb = new StringBuilder();
                byte[] buf = new byte[8192];
                string tempString = null;
                int count = 0;

                do
                {
                    // fill the buffer with data
                    count = io.Read(buf, 0, 6);//buf.Length);

                    // make sure we read some data
                    if (count != 0)
                    {
                        // translate from bytes to ASCII text
                        tempString = Encoding.ASCII.GetString(buf, 0, count);

                        // continue building the string
                        sb.Append(tempString);
                    }
                }
                while (count > 0); // any more data to read?

                Debug.WriteLine("Ending the connection");
                sender.Close();
                Debug.WriteLine(sb.ToString());

                // check response
                if (sb.ToString().Equals(expectedResponse))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {

                Console.WriteLine("Caught Exception: {0}", e.ToString());
                return false;
            }

            
        }

        private bool sendServerDataAndCheckResponse(String url, byte[] dataToSend, String expectedResponse)
        {
            // used to build entire input
            StringBuilder sb = new StringBuilder();

            // buffer for each read operation
            byte[] buf = new byte[8192];

            // prepare to send login data
            HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            ((HttpWebRequest)request).UserAgent = "Sparkle-Configuration .NET Client";
            request.Method = "POST";
            request.ContentLength = dataToSend.Length;
            request.ContentType = "application/x-www-form-urlencoded";

            // try to send login data
            Stream dataStream;
            try
            {
                dataStream = request.GetRequestStream();
                dataStream.Write(dataToSend, 0, dataToSend.Length);
                dataStream.Close();
            }
            catch (WebException)
            {
                Debug.WriteLine("ERROR: Did not recieve response from server");
                //Debug.WriteLine(e.ToString());
                return false;
            }

            // get response from server
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException)
            {
                Debug.WriteLine("ERROR: Unexpected response from server");
                //Debug.WriteLine(e.ToString());
                return false;
            }
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            Stream data = response.GetResponseStream();
            response.Close();

            string tempString = null;
            int count = 0;

            do
            {
                // fill the buffer with data
                count = data.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0)
                {
                    // translate from bytes to ASCII text
                    tempString = Encoding.ASCII.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?

            Console.WriteLine(sb.ToString());

            // check response
            if (sb.ToString().Equals(expectedResponse))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool connect(String addressString, String portString, String passwordString)
        {
            // sanity check on parameters
            if (addressString == null || addressString.Length == 0)
            {
                return false;
            }

            // compute hash of password
            byte[] result = computePasswordHash(passwordString);
            Debug.WriteLine("Password hash: " + BitConverter.ToString(result));

            // create login command
            byte[] dataToSend = ConcatArrays(StringToByteArray(loginCommand + " "), result);


            //bool ret = sendServerDataAndCheckResponse("http://" + addressString + ":" + portString, dataToSend, "");

            bool ret = sendServerDataAndCheckResponseTCP(addressString, portString, dataToSend, "");

            return ret;
        }

        public static byte[] StringToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }

        public static T[] ConcatArrays<T>(params T[][] list)
        {
            var result = new T[list.Sum(a => a.Length)];
            int offset = 0;
            for (int x = 0; x < list.Length; x++)
            {
                list[x].CopyTo(result, offset);
                offset += list[x].Length;
            }
            return result;
        }

        public static int convertStringToInt(String stringToConvert)
        {
            try
            {
                return Convert.ToInt32(stringToConvert);
            }
            catch (FormatException)
            {
                Console.WriteLine("WARNING: Input string is invalid.");
            }
            catch (OverflowException)
            {
                Console.WriteLine("WARNING: The number cannot fit in an Int32.");
            }

            return 0;
        }

    }

}
