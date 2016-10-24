using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;

namespace RobotFollowerWPF2
{
    class SerialCommunication
    {
        public List<string> PortNameToDetect = new List<string>();
        bool tryAgain = true;
        public bool currentlySending = false;
        SerialPort serialArduino;
        string lastCommand = "";
        public bool currentlyConnected = false;

        public bool responseReceived = false;
        public string stringReceived = "";

        public SerialCommunication()
        {
        }

        /// <summary>
        /// Used to open a serial connection with the device. Keeps trying to connect until it is successful. Writes to the output if it fails.
        /// </summary>
        /// <param name="DeviceName">Name of the device as shown in the device manager.</param>
        public void Connect()
        {
            currentlyConnected = false;
            string portName;

            //keep trying to detect the port until it is found
            do
            {
                portName = AutoDetectPort();
                Thread.Sleep(10);
            } while (portName == "" || portName == "NOTFOUND");


            serialArduino = new SerialPort();
            serialArduino.PortName = portName;
            serialArduino.BaudRate = 115200;

            while (tryAgain)
            {
                tryAgain = false;
                try
                {
                    serialArduino.Close();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Port Closing Failed: " + e.ToString());
                }

                try
                {
                    serialArduino.Open();
                    Thread.Sleep(5000);
                    serialArduino.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceivedHandlerArduino);
                    CheckConnect();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Port opening Failed: " + e.ToString());
                    Thread.Sleep(10);
                    tryAgain = true;
                }
            }
        }

        public bool CheckConnect()
        {
            currentlySending = true;
            responseReceived = false;

            try
            {
                serialArduino.WriteLine("RUALIVE");
                Thread.Sleep(100);
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
                currentlySending = false;
                return false;
            }

            int counter = 0;

            while (!responseReceived)
            {
                Thread.Sleep(1);
                counter++;
                if (counter > 1000)
                {
                    currentlySending = false;
                    return false;
                }
            }

            if (stringReceived.Contains("ALIVE"))
            {
                currentlyConnected = true;
                currentlySending = false;
                return true;
            }

            currentlySending = false;
            return false;
        }

        /// <summary>
        /// Sends the string directly to Arduino and waits for response before returning
        /// </summary>
        /// <param name="cmd"></param>
        public void SendString(string cmd)
        {
            while (currentlySending)
            {
                Thread.Sleep(10);
            }

            currentlySending = true;

            responseReceived = false;
            try
            {
                serialArduino.WriteLine(cmd);
                //Thread.Sleep(100);
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }

            //while (!responseReceived)
            //{
            //    Thread.Sleep(1);
            //}
            currentlySending = false;
            return;

        }

        public string AutoDetectPort()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity WHERE ClassGuid=\"{4d36e978-e325-11ce-bfc1-08002be10318}\"");

            try
            {
                foreach (ManagementObject item in searcher.Get())
                {
                    string description = item["Description"].ToString();
                    //string deviceId = item["DeviceID"].ToString();
                    string deviceNameDetected = item["Name"].ToString();
                    //string deviceId = item["Caption"].ToString();

                    deviceNameDetected = deviceNameDetected.Split('(').Last<string>().Replace(")", "");

                    foreach (string port in PortNameToDetect)
                    {
                        if (description.Contains(port))
                        {
                            Thread.Sleep(100);
                            return deviceNameDetected;
                        }
                    }
                }
            }
            catch (ManagementException e)
            {

            }

            return "NOTFOUND";
        }

        private void SerialDataReceivedHandlerArduino(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            stringReceived = sp.ReadLine();
            Debug.WriteLine(stringReceived);
            responseReceived = true;
            Debug.Print(stringReceived);
        }
    }
}
