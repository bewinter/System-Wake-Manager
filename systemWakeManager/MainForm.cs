﻿/*
 * Created by SharpDevelop.
 * User: user
 * Date: 3/7/2016
 * Time: 1:58 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;


namespace systemWakeManager
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		string itemToAlter;

		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
			

		}
		
		string runCommand(string command)
		{
			try {
				
			
			// create the ProcessStartInfo using "cmd" as the program to be run,
			// and "/c " as the parameters.
			// Incidentally, /c tells cmd that we want it to execute the command that follows,
			// and then exit.
				
			//string command = "powercfg -devicequery wake_from_any";
			         
			var procStartInfo =
				new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command);
			
			// The following commands are needed to redirect the standard output.
			// This means that it will be redirected to the Process.StandardOutput StreamReader.
			procStartInfo.RedirectStandardOutput = true;
			procStartInfo.UseShellExecute = false;
			// Do not create the black window.
			procStartInfo.CreateNoWindow = true;
			// Now we create a process, assign its ProcessStartInfo and start it
			var proc = new System.Diagnostics.Process();
			proc.StartInfo = procStartInfo;			
			proc.StartInfo.Verb = "runas";
			
			proc.Start();
			// Get the output into a string
			var result = proc.StandardOutput.ReadToEnd();
			    
			return(result);
			
			} catch (Exception e) {				
				MessageBox.Show(e.ToString());				
				return("");
			}
		}
		
		void getWakeDevices(string command, bool justClear)
		{
			/*// Start the child process.
			 Process p = new Process();
			 // Redirect the output stream of the child process.
			 p.StartInfo.UseShellExecute = false;
			 p.StartInfo.RedirectStandardOutput = true;
			 p.StartInfo.FileName = "getAnyWakeDevice.bat";
			 p.Start();
			 // Do not wait for the child process to exit before
			 // reading to the end of its redirected stream.
			 // p.WaitForExit();
			 // Read the output stream first and then wait.
			 string output = p.StandardOutput.ReadToEnd();
			 p.WaitForExit();	
			 cmdResults.Text = output;
			 
			 string[] lines = output.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
			 
			 disableWakeDeviceListCoB.Items.Add("");
			 
			 foreach (var element in lines) {
			 	
			 	if (element != "" && element.Contains(@":\") == false)
			 	{
			 		disableWakeDeviceListCoB.Items.Add(element);
			 	}
			 }*/
			 
			var result = runCommand(command);
			 	
			try {
				
			 	cmdResults.Text = result;
			    
				//result = result.Replace("\r","");
				string[] lines = result.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			 
				//lines.Sort;
				
				alterWakeDeviceListCoB.Items.Clear();
				
				if (justClear == false) {
				
				
					alterWakeDeviceListCoB.Items.Add("");
			 
					foreach (var element in lines) {
			 	
						if (element != "" && element.Contains(@":\") == false && element.Contains("NONE") == false) {
							alterWakeDeviceListCoB.Items.Add(element);
						}
			    
					}
				}
			} catch (Exception objException) {
			 	MessageBox.Show(objException.ToString());
			}
			 
		}
		void AlterWakeDeviceListCoBSelectedIndexChanged(object sender, EventArgs e)
		{
			itemToAlter = alterWakeDeviceListCoB.Text;
		}
		void DisableButtonClick(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(itemToAlter)) {
				DialogResult dialogResult = MessageBox.Show("Are you sure you want to disable this item? \n\n" + itemToAlter, itemToAlter, MessageBoxButtons.YesNo);
				if (dialogResult == DialogResult.Yes) {
					
					//runCommand(@"powercfg -devicedisablewake “" + itemToAlter + "”");
					
					getWakeDevices(@"powercfg -devicedisablewake """ + itemToAlter + @"""", false);
					
					MessageBox.Show("Done");
					
					label1.Text = "Armed Wake Devices";
                    getWakeDevices("powercfg -devicequery wake_armed | sort", false);
					
				} else if (dialogResult == DialogResult.No) {
					//do something else
				}
			}
		}
		void BtnEnableWakeDeviceClick(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(itemToAlter)) {
				DialogResult dialogResult = MessageBox.Show("Are you sure you want to enable this item? \n\n" + itemToAlter, itemToAlter, MessageBoxButtons.YesNo);
				if (dialogResult == DialogResult.Yes) {
					
					getWakeDevices(@"powercfg -deviceenablewake  """ + itemToAlter + @"""", false);
					//runCommand(@"powercfg -deviceenablewake  “" + itemToAlter + "”");
					
					MessageBox.Show("Done");
					
					label1.Text = "Armed Wake Devices";
                    getWakeDevices("powercfg -devicequery wake_armed | sort", false);
					
				} else if (dialogResult == DialogResult.No) {
					//do something else
				}
			}
		}

		void BtnAllWakeDevicesClick(object sender, EventArgs e)
		{
			label1.Text = "All Wake Devices";
			getWakeDevices("powercfg -devicequery wake_from_any | sort", false);
		}
		void BtnArmedWakeDevicesClick(object sender, EventArgs e)
		{
			label1.Text = "Armed Wake Devices";
            getWakeDevices("powercfg -devicequery wake_armed | sort", false);
		}
		void BtnLastUsedWakeDeviceClick(object sender, EventArgs e)
		{
			label1.Text = "Last Used Wake Devices";
            getWakeDevices("powercfg -lastwake | sort", true);
		}

	}
}
