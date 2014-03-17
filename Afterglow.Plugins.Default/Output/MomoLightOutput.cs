using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Afterglow.Core.Extensions;
using Afterglow.Core.Plugins;
using System.IO.Ports;
using Afterglow.Core.Configuration;
using System.Reflection;
using Afterglow.Core;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Afterglow.Plugins.Output
{
    /// <summary>
    /// MomoLight Output
    /// </summary>
    [DataContract]
    public class MomoLightOutput: BasePlugin, IOutputPlugin
    {
        private SerialPort _port;
        private byte[] _serialData;
        
        #region Read Only Properties
        /// <summary>
        /// The name of the current plugin
        /// </summary>
        [DataMember]
        public override string Name
        {
            get { return "MomoLight Output"; }
        }
        /// <summary>
        /// A description of this plugin
        /// </summary>
        [DataMember]
        public override string Description
        {
            get { return "Output to the MomoLight HW controller"; }
        }
        /// <summary>
        /// The author of this plugin
        /// </summary>
        [DataMember]
        public override string Author
        {
            get { return "Silas Mariusz"; }
        }
        /// <summary>
        /// A website for further information
        /// </summary>
        [DataMember]
        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }
        /// <summary>
        /// The version of this plugin
        /// </summary>
        [DataMember]
        public override Version Version
        {
            get { return new Version(1, 0, 1); }
        }
        #endregion

        [DataMember]
        [Required]
        [Display(Name = "Serial Port", Order = 100)]
        [ConfigLookup(RetrieveValuesFrom = "Ports")]
        public string Port
        {
            get { return Get(() => Port, () => Ports[0]); }
            set { Set(() => Port, value); }
        }

        /// <summary>
        /// Gets the available Serial/USB ports that
        /// If none are found it is possible the driver is not installed
        /// </summary>
        [DataMember]
        public string[] Ports
        {
            get
            {
                return SerialPort.GetPortNames();
            }
        }

        [DataMember]
        [Required]
        [Display(Name = "Baud Rate", Order = 200)]
        [Range(0, 999999)]
        public int BaudRate
        {
            get { return Get(() => BaudRate, () => 9600); }
            set { Set(() => BaudRate, value); }
        }

        [DataMember]
        [Required]
        [Display(Name = "Firmware", 
            Description = "Please select MoMoLight firmware type. Both v1 and v2 are standard, where v3 is backward-compatible but additionaly support data chksum to prevent LED black-out's on cheap USB-UARTs adapters.",
            Order = 300)]
        [Range(0, 3)]
        public int Firmware
        {
            get { return Get(() => Firmware, () => 0); }
            set { Set(() => Firmware, value); }
        }

        /// <summary>
        /// Start this Plugin
        /// </summary>
        public override void Start()
        {
            //TODO: error checking and configuration of port
            //TODO: possibly check device manager and see if MomoLight is founds just not installed
            if (Ports.Length == 0)
            {
                //Logger.Warn("No serial ports found");
                _port = null;
                //throw new Exception("No serial ports found");
            }
            else
            {
                _port = new SerialPort(Port, BaudRate);
                _port.ErrorReceived += new SerialErrorReceivedEventHandler(ErrorReceived);
                try
                {
                    _port.Open();
                }
                catch (IOException e)
                {
                    //TODO: try and reset the serial connection so the user does not need to disconnect and reattach the cable
                    string message = "Please un plug and re attach the cable";

                    Stop();

                    throw new Exception(message, e);
                    
                    //Logger.Error(ex, "MomoLight not found");
                }
            }
        }

        public bool TryStart(out string errorMessage)
        {
            bool result = true;
            errorMessage = string.Empty;

            try
            {
                Start();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                result = false;
            }

            return result;
        }

        void ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Stop();
            Start();
        }

        /// <summary>
        /// Send the light information to the MomoLight
        /// </summary>
        /// <param name="lights"></param>
        public void Output(List<Core.Light> lights)
        {
            if (_port != null && _port.IsOpen)
            {
                int serialDataPos = 0;

                if (_serialData == null || _serialData.Length != lights.Count)
                {

                    // only fw v3
                    if (Firmware == 3)
                    {
                        _serialData = new byte[3 + lights.Count*3];
                        _serialData[0] = 36; // say hello to momotype v3 fw

                        serialDataPos = 1;
                    }
                    else
                    {
                        _serialData = new byte[lights.Count*3];
                    }

                }

                int _checkSum = 0;
                foreach (var led in lights.OrderBy(l => l.Index))
                {
                    _serialData[serialDataPos++] = Convert.ToByte(led.LightColour.R);
                    _serialData[serialDataPos++] = Convert.ToByte(led.LightColour.G);
                    _serialData[serialDataPos++] = Convert.ToByte(led.LightColour.B);

                    if (Firmware == 3) _checkSum += led.LightColour.R + led.LightColour.G + led.LightColour.B;
                }

                // only fw v3
                if (Firmware == 3)
                {
                    if (_checkSum > 65535)
                    {
                        _checkSum = 65535;
                    }
                    else if (_checkSum < 0)
                    {
                        _checkSum = 0;
                    }

                    ushort checkSum = (ushort)(65535 - _checkSum);
                    string chksum = HexString2Ascii(checkSum.ToString("X4"));
                    _serialData[serialDataPos++] = Convert.ToByte(chksum[0]);
                    _serialData[serialDataPos++] = Convert.ToByte(chksum[1]);
                }


                // Issue data to MomoLight
                try
                {
                    if (_port != null) _port.Write(_serialData, 0, _serialData.Length);
                }
                catch (Exception)
                {
                    //Logger.Warn("MomoLight not found");
                }
            }
            else
            {
                Stop();
                Start();
            }
        }

        /// <summary>
        /// Stop this Plugin
        /// </summary>
        public override void Stop()
        {
            if (_port != null)
            {
                _port.Close();
                _port.Dispose();
                _port = null;
            }
        }


        /// <summary>
        /// Converts Hex String to Ascii String (54 84 returns coresponding ascii built string: "6T")
        /// </summary>
        /// <param name="hexString">Input Hexdecimals String</param>
        /// <returns>Ascii built string</returns>
        string HexString2Ascii(string hexString)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= hexString.Length - 2; i += 2)
            {
                sb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(hexString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber))));
            }
            return sb.ToString();
        }
    }
}
