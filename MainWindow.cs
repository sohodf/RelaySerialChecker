using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FTD2XX_NET;

namespace RelaySerialChecker
{
    public partial class MainWindow : Form
    {
        
        private FTDI.FT_STATUS _ftStatus;
        private byte[] _sentBytes = new byte[2];
        FTDI ftdi = new FTDI();
        public MainWindow()
        {
            InitializeComponent();
            groupBox1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = false;
            //create ftdi object for the relay
            //check if there are two relays connected
            if (OpenRelay(ftdi, 1))
            {
                MessageBox msg = new MessageBox("Error", "More than one relay connected. Please disconnect all but one relay and try again", this);
                msg.ShowDialog();
                ftdi.Close();
            }
            else
            {
                if (OpenRelay(ftdi, 0))
                {
                    textBox1.Text = GetSerial(ftdi);
                    groupBox1.Visible = true;
                    ftdi.Close();
                }
                else
                {
                    MessageBox msg2 = new MessageBox("Error", "Couldn't find/open a relay", this);
                    msg2.ShowDialog();
                }
            }
        }

        public string GetSerial(FTDI relay)
        {
            // Get device info
            string serialNumber = "";
            relay.GetSerialNumber(out serialNumber);
            return serialNumber.ToString();
            
        }


        //Opens the com port to the relay
        public Boolean OpenRelay(FTDI ftdi, uint deviceIndex)
        {
            _ftStatus = ftdi.OpenByIndex(deviceIndex);

            if (_ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                return false;
            }

            _ftStatus = ftdi.SetBaudRate(921600);
            if (_ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                return false;
            }

            _ftStatus = ftdi.SetBitMode(255, 4);
            if (_ftStatus != FTDI.FT_STATUS.FT_OK)
            {
                return false;
            }

            return true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText(textBox1.Text);
        }

        //test the recieved serial
        private void button4_Click(object sender, EventArgs e)
        {
            //open the relay by the new serial
            FTDI newRelay = new FTDI();
            string serial = textBox1.Text;
            newRelay.OpenBySerialNumber(serial);

            byte[] sentBytes = new byte[2];
            uint receivedBytes = 0;
            
            //toggle all on/all off

            sentBytes[0] = (byte)(255);
            newRelay.Write(sentBytes, 1, ref receivedBytes);
            System.Threading.Thread.Sleep(1000);
            sentBytes[0] = (byte)(0);
            newRelay.Write(sentBytes, 1, ref receivedBytes);
            newRelay.Close();
        }

    }
}