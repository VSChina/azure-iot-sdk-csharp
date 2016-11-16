﻿using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common;
using System;
using System.Windows.Forms;

namespace DeviceExplorer
{
    public partial class CreateDeviceForm : Form
    {
        private string iotHubConnectionString;
        private int devicesMaxCount;
        private RegistryManager registryManager;
        private bool generateDeviceID;
        private bool generateDeviceKeys;

        public CreateDeviceForm(string iotHubConnectionString, int devicesMaxCount)
        {
            InitializeComponent();

            this.iotHubConnectionString = iotHubConnectionString;
            this.registryManager = RegistryManager.CreateFromConnectionString(iotHubConnectionString);
            this.devicesMaxCount = devicesMaxCount;

            generateIDCheckBox.Checked = false;
            generateDeviceID = false;

            keysRadioButton.Checked = true;

            generateKeysCheckBox.Checked = true;
            generateDeviceKeys = true;
            autoGenerateDeviceKeys();
        }

        private void autoGenerateDeviceKeys()
        {
            primaryKeyTextBox.Text = CryptoKeyGenerator.GenerateKey(32);
            secondaryKeyTextBox.Text = CryptoKeyGenerator.GenerateKey(32);
        }

        private void autoGenerateDeviceID()
        {
            deviceIDTextBox.Text = "device" + Guid.NewGuid().ToString("N");
        }
        private async void createButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(deviceIDTextBox.Text))
                {
                    throw new ArgumentNullException("DeviceId cannot be empty!");
                }

                var device = new Device(deviceIDTextBox.Text);
                device.Authentication = new AuthenticationMechanism();

                if (keysRadioButton.Checked)
                {
                    device.Authentication.SymmetricKey.PrimaryKey = primaryKeyTextBox.Text;
                    device.Authentication.SymmetricKey.SecondaryKey = secondaryKeyTextBox.Text;
                }
                else if (x509RadioButton.Checked)
                {
                    device.Authentication.SymmetricKey = null;
                    device.Authentication.X509Thumbprint = new X509Thumbprint()
                    {
                        PrimaryThumbprint = primaryKeyTextBox.Text,
                        SecondaryThumbprint = secondaryKeyTextBox.Text
                    };
                }

                await registryManager.AddDeviceAsync(device);

                var deviceCreated = new DeviceCreatedForm(device);
                deviceCreated.ShowDialog();

                this.Close();
            }
            catch (Exception ex)
            {
                using (new CenterDialog(this))
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void generateIDCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (generateDeviceID) // was checked prior to the click
            {
                generateDeviceID = false;
                deviceIDTextBox.ResetText();
            }
            else  // was NOT checked prior to the click
            {
                generateDeviceID = true;
                autoGenerateDeviceID();
            }
        }

        private void generateKeysCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if(generateDeviceKeys) // was checked prior to the click
            {
                generateDeviceKeys = false;
                primaryKeyTextBox.ResetText();
                secondaryKeyTextBox.ResetText();
            }
            else  // was NOT checked prior to the click
            {
                generateDeviceKeys = true;
                autoGenerateDeviceKeys();
            }
        }

        private void keysRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (keysRadioButton.Checked)
            {
                primaryLabel.Text = "Primary Key:";
                secondaryLabel.Text = "Secondary Key:";
                generateKeysCheckBox.Enabled = true;
                autoGenerateDeviceKeys();
            }
            else
            {
                primaryLabel.Text = "Primary Thumbprint:";
                secondaryLabel.Text = "Secondary Thumbprint:";
                generateKeysCheckBox.Enabled = false;
                primaryKeyTextBox.ResetText();
                secondaryKeyTextBox.ResetText();
            }
        }
    }
}
