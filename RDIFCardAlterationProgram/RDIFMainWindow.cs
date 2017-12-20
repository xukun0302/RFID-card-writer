﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace RDIFCardAlterationProgram
{
    public partial class RDIFMainWindow : Form
    {
        private SerialPort currentPort = null;
        private bool portFound;

        private void SetComPort()
        {
            try
            {
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    currentPort = new SerialPort(port, 9600);
                    if (DetectArduino(currentPort))
                    {
                        portFound = true;
                        break;
                    }
                    else
                    {
                        currentPort.Close();
                        portFound = false;
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        private bool DetectArduino(SerialPort port)
        {
            try
            {
                //The below setting are for the Hello handshake
                byte[] buffer = new byte[5];
                //16 means message
                buffer[0] = Convert.ToByte(16);
                //128 is the number for checking if the ardunio exists
                buffer[1] = Convert.ToByte(128);
                buffer[2] = Convert.ToByte(0);
                buffer[3] = Convert.ToByte(0);
                //4 is end of message
                buffer[4] = Convert.ToByte(4);

                int intReturnASCII = 0;
                char charReturnValue = (Char)intReturnASCII;



                if (!port.IsOpen)
                {
                    port.Open();
                    Console.WriteLine("Meow port has been opened");
                }
                else
                {
                    Console.WriteLine("port was already open");
                }
                port.Write(buffer, 0, 5);
                Thread.Sleep(1000);

                int count = port.BytesToRead;
                string returnMessage = "";
                while (count > 0)
                {

                    intReturnASCII = port.ReadByte();
                    returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
                    count--;
                }
                //ComPort.name = returnMessage;
                Console.WriteLine("We have data");

                

                if (returnMessage.Contains("HELLO FROM ARDUINO"))
                {
                    Console.WriteLine("We have arduino");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        private void GetComPort_Click(object sender, EventArgs e)
        {
            //we need to call the method 
            SetComPort();
            if (currentPort != null)
            {
                PortName.Text ="Port: " + currentPort.PortName;
            }
            else
            {
                PortName.Text = "Device is not found.";
            }
        }
    }
}