﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
using System.CodeDom.Compiler;
using System.Windows.Input;
using MouseButtons = System.Windows.Forms.MouseButtons;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using System;
using System.CodeDom;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UITest.Common;
using TechTalk.SpecFlow;
using Warewolf.UITests.Common;

namespace Warewolf.UITests
{
    [Binding]
    public partial class UIMap
    {
        const int _lenientSearchTimeout = 30000;
        const int _lenientMaximumRetryCount = 3;
        const int _strictSearchTimeout = 3000;
        const int _strictMaximumRetryCount = 1;

        public void SetPlaybackSettings()
        {
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.Disabled;
            Playback.PlaybackSettings.ShouldSearchFailFast = false;
            Playback.PlaybackSettings.ContinueOnError = false;
#if DEBUG
            Playback.PlaybackSettings.ThinkTimeMultiplier = 2;
#else  
            Playback.PlaybackSettings.ThinkTimeMultiplier = 2;
#endif
            Playback.PlaybackSettings.MaximumRetryCount = _lenientMaximumRetryCount;
            Playback.PlaybackSettings.SearchTimeout = _lenientSearchTimeout;
            Playback.PlaybackSettings.MatchExactHierarchy = true;
            Playback.PlaybackSettings.SkipSetPropertyVerification = true;
            Playback.PlaybackSettings.SmartMatchOptions = SmartMatchOptions.None;
        }

        [Given("The Warewolf Studio is running")]
        public void AssertStudioIsRunning()
        {
            Assert.IsTrue(MainStudioWindow.Exists, "Warewolf studio is not running. You are expected to run \"Dev\\TestScripts\\Studio\\Startup.bat\" as an administrator and wait for it to complete before running any coded UI tests");
            Keyboard.SendKeys(MainStudioWindow, "{Tab}", ModifierKeys.None);
            Keyboard.SendKeys(MainStudioWindow, "^%{F4}");
#if !DEBUG
            var TimeBefore = System.DateTime.Now;
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.Checkbox.Spinner);
            Console.WriteLine("Waited " + (System.DateTime.Now - TimeBefore).Milliseconds.ToString() + "ms for the explorer spinner to disappear.");
#endif
        }

        [When(@"I Try Click Message Box OK")]
        [Then(@"I Try Click Message Box OK")]
        [Given(@"I Try Click Message Box OK")]
        public void TryClickMessageBoxOK()
        {
            var TimeBefore = System.DateTime.Now;
            try
            {
                if (ControlExistsNow(MessageBoxWindow.OKButton))
                {
                    Mouse.Click(MessageBoxWindow.OKButton, new Point(35, 11));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught a " + e.Message + " trying to close a hanging message box before the test starts.");
            }
            finally
            {
                Console.WriteLine("No hanging message box to clean up after trying for " + (System.DateTime.Now - TimeBefore).Milliseconds.ToString() + "ms.");
            }
        }


        [Given(@"I Try Click MessageBox No")]
        [When(@"I Try Click MessageBox No")]
        [Then(@"I Try Click MessageBox No")]
        public void TryClickMessageBoxNo()
        {
            var TimeBefore = System.DateTime.Now;
            try
            {
                if (ControlExistsNow(MessageBoxWindow.NoButton))
                {
                    Click_MessageBox_No();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught a " + e.Message + " trying to close a hanging message box before the test starts.");
            }
            finally
            {
                Console.WriteLine("No hanging message box to clean up after trying for " + (System.DateTime.Now - TimeBefore).Milliseconds.ToString() + "ms.");
            }

        }

        public void TryPin_Unpinned_Pane_To_Default_Position()
        {
            var TimeBefore = System.DateTime.Now;
            try
            {
                if (ControlExistsNow(MainStudioWindow.UnpinnedTab))
                {
                    Restore_Unpinned_Tab_Using_Context_Menu();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught a " + e.Message + " trying to Pin Unpinned Pane To Default Position before the test starts.");
            }
            finally
            {
                Console.WriteLine("No hanging Unpinned Pane to clean up after trying for " + (System.DateTime.Now - TimeBefore).Milliseconds.ToString() + "ms.");
            }
        }

        public bool ControlExistsNow(UITestControl thisControl)
        {
            Playback.PlaybackSettings.MaximumRetryCount = _strictMaximumRetryCount;
            Playback.PlaybackSettings.SearchTimeout = _strictSearchTimeout;
            bool controlExists = false;
            controlExists = thisControl.TryFind();
            Playback.PlaybackSettings.MaximumRetryCount = _lenientMaximumRetryCount;
            Playback.PlaybackSettings.SearchTimeout = _lenientSearchTimeout;
            return controlExists;
        }

        public void InitializeABlankWorkflow()
        {
            Click_NewWorkflow_RibbonButton();
        }

        public void TryClearExplorerFilter()
        {
            if (MainStudioWindow.DockManager.SplitPaneLeft.Explorer.SearchTextBox.Text != string.Empty)
            {
                Click_Explorer_Filter_Clear_Button();
                Click_Explorer_Refresh_Button();
            }
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.SearchTextBox.Text == string.Empty, "Explorer filter textbox text value of " + MainStudioWindow.DockManager.SplitPaneLeft.Explorer.SearchTextBox.Text + " is not empty after clicking clear filter button.");
        }

        [When(@"I Try Clear Toolbox Filter")]
        public void TryClearToolboxFilter()
        {
            if (MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text != string.Empty)
            {
                Click_Clear_Toolbox_Filter_Clear_Button();
            }
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text == string.Empty, "Toolbox filter textbox text value of " + MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text + " is not empty after clicking clear filter button.");
        }

        public void Click_Settings_Resource_Permissions_Row1_Add_Resource_Button()
        {
            Mouse.Click(FindAddResourceButton(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1));
            Assert.IsTrue(ServicePickerDialog.Exists, "Service picker dialog does not exist.");
        }

        public void Click_Settings_Resource_Permissions_Row1_Delete_Button()
        {
            Mouse.Click(FindAddRemoveRowButton(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1));
        }

        public void Click_Settings_Resource_Permissions_Row1_Windows_Group_Button()
        {
            Mouse.Click(FindAddWindowsGroupButton(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1));
            Assert.IsTrue(SelectWindowsGroupDialog.Exists, "Select windows group dialog does not exist.");
            Assert.IsTrue(SelectWindowsGroupDialog.ItemPanel.ObjectNameTextbox.Exists, "Select windows group object name textbox does not exist.");
        }

        public UITestControl FindAddResourceButton(UITestControl row)
        {
            var firstOrDefaultCell = row.GetChildren().Where(child => child.ControlType == ControlType.Cell).ElementAtOrDefault(0);
            return firstOrDefaultCell?.GetChildren().FirstOrDefault(child => child.ControlType == ControlType.Button);
        }

        public WpfText FindSelectedResourceText(UITestControl row)
        {
            var firstOrDefaultCell = row.GetChildren().Where(child => child.ControlType == ControlType.Cell).ElementAtOrDefault(0);
            return firstOrDefaultCell?.GetChildren().FirstOrDefault(child => child.ControlType == ControlType.Text) as WpfText;
        }

        public UITestControl FindAddWindowsGroupButton(UITestControl row)
        {
            var firstOrDefaultCell = row.GetChildren().Where(child => child.ControlType == ControlType.Cell).ElementAtOrDefault(1);
            return firstOrDefaultCell?.GetChildren().FirstOrDefault(child => child.ControlType == ControlType.Button);
        }

        public WpfEdit FindWindowsGroupTextbox(UITestControl row)
        {
            var firstOrDefaultCell = row.GetChildren().Where(child => child.ControlType == ControlType.Cell).ElementAtOrDefault(1);
            return firstOrDefaultCell?.GetChildren().FirstOrDefault(child => child.ControlType == ControlType.Edit) as WpfEdit;
        }

        public WpfCheckBox FindViewPermissionsCheckbox(UITestControl row)
        {
            var firstOrDefaultCell = row.GetChildren().Where(child => child.ControlType == ControlType.Cell).ElementAtOrDefault(2);
            return firstOrDefaultCell?.GetChildren().FirstOrDefault(child => child.ControlType == ControlType.CheckBox) as WpfCheckBox;
        }

        public WpfCheckBox FindExecutePermissionsCheckbox(UITestControl row)
        {
            var firstOrDefaultCell = row.GetChildren().Where(child => child.ControlType == ControlType.Cell).ElementAtOrDefault(3);
            return firstOrDefaultCell?.GetChildren().FirstOrDefault(child => child.ControlType == ControlType.CheckBox) as WpfCheckBox;
        }

        public WpfCheckBox FindContributePermissionsCheckbox(UITestControl row)
        {
            var firstOrDefaultCell = row.GetChildren().Where(child => child.ControlType == ControlType.Cell).ElementAtOrDefault(4);
            return firstOrDefaultCell?.GetChildren().FirstOrDefault(child => child.ControlType == ControlType.CheckBox) as WpfCheckBox;
        }

        public UITestControl FindAddRemoveRowButton(UITestControl row)
        {
            var firstOrDefaultCell = row.GetChildren().Where(child => child.ControlType == ControlType.Cell).ElementAtOrDefault(5);
            return firstOrDefaultCell?.GetChildren().FirstOrDefault(child => child.ControlType == ControlType.Button);
        }

        public void TryDisconnectFromRemoteServerAndRemoveSourceFromExplorer(string SourceName)
        {
            try
            {
                if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.SelectedItemAsRemoteConnectionIntegrationConnected))
                {
                    Click_Explorer_RemoteServer_Connect_Button();
                }
                else
                {
                    Click_Connect_Control_InExplorer();
                    if (ControlExistsNow(MainStudioWindow.ComboboxListItemAsTSTCIREMOTEConnected))
                    {
                        Select_TSTCIREMOTEConnected_From_Explorer_Remote_Server_Dropdown_List();
                        Click_Explorer_RemoteServer_Connect_Button();
                    }
                }
                Select_LocalhostConnected_From_Explorer_Remote_Server_Dropdown_List();
                Filter_Explorer(SourceName);
                WaitForControlNotVisible(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.Checkbox.Spinner);
                if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem))
                {
                    RightClick_Explorer_Localhost_FirstItem();
                    Select_Delete_From_ExplorerContextMenu();
                    Click_MessageBox_Yes();
                }
                TryClearExplorerFilter();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cleanup failed to remove remote server " + SourceName + ". Test may have crashed before remote server " + SourceName + " was connected.\n" + e.Message);
                TryClearExplorerFilter();
            }
        }

        [Given(@"I Try Remove ""(.*)"" From Explorer")]
        [When(@"I Try Remove ""(.*)"" From Explorer")]
        [Then(@"I Try Remove ""(.*)"" From Explorer")]
        public void WhenITryRemoveFromExplorer(string ResourceName)
        {
            Filter_Explorer(ResourceName);
            try
            {
                var resourcesFolder = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources";
                if (File.Exists(resourcesFolder + @"\" + ResourceName + ".xml"))
                {
                    WaitForSpinner(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.Checkbox.Spinner);
                    if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem))
                    {
                        RightClick_Explorer_Localhost_FirstItem();
                        Select_Delete_From_ExplorerContextMenu();
                        Click_MessageBox_Yes();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cleanup failed to remove resource " + ResourceName + " from the explorer.\n" + e.Message);
            }
            finally
            {
                TryClearExplorerFilter();
            }
        }

        [When(@"I Connect To Remote Server")]
        public void ConnectToRemoteServer()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.ToggleButton, new Point(136, 7));
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegration.Exists, "Remote Connection Integration option does not exist in Source server combobox.");
            Mouse.Click(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegration.Text, new Point(226, 13));
            Click_Explorer_RemoteServer_Connect_Button();
        }

        [When(@"I Try Connect To Remote Server")]
        public void TryConnectToRemoteServer()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.ToggleButton, new Point(136, 7));
            if (ControlExistsNow(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegrationConnected))
            {
                Assert.IsTrue(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegrationConnected.Exists, "Remote Connection Integration option does not exist in Source server combobox.");
                Mouse.Click(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegrationConnected.Text, new Point(226, 13));
            }
            else
            {
                Assert.IsTrue(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegration.Exists, "RemoteConnectionIntegration item does not exist in remote server combobox list.");
                Mouse.Click(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegration.Text, new Point(138, 6));
                Click_Explorer_RemoteServer_Connect_Button();
            }
        }

        [Given(@"I Try Remove ""(.*)"" From Remote Server Explorer")]
        [When(@"I Try Remove ""(.*)"" From Remote Server Explorer")]
        [Then(@"I Try Remove ""(.*)"" From Remote Server Explorer")]
        public void I_Try_Remove_From_Remote_Server_Explorer(string ResourceName)
        {
            TryConnectToRemoteServer();
            Filter_Explorer(ResourceName);
            try
            {
                if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem))
                {
                    MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem.DrawHighlight();
                    WaitForSpinner(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.Checkbox.Spinner);
                    RightClick_Explorer_FirstRemoteServer_FirstItem();
                    Select_Delete_From_ExplorerContextMenu();
                    Click_MessageBox_Yes();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Cleanup failed to remove resource " + ResourceName + " from the explorer.\n" + e.Message);
            }
            finally
            {
                if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.SearchTextBox.ClearFilterButton))
                    TryClearExplorerFilter();
            }
        }


        public void Close_And_Lock_Side_Menu_Bar()
        {
            Mouse.Click(MainStudioWindow.SideMenuBar.LockMenuButton);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer);
            Mouse.Click(MainStudioWindow.SideMenuBar.LockMenuButton);
        }

        public void Click_Settings_Security_Tab_ResourcePermissions_Row1_Execute_Checkbox()
        {
            FindExecutePermissionsCheckbox(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Checked = true;
            Assert.IsTrue(FindExecutePermissionsCheckbox(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Checked, "Settings security tab resource permissions row 1 execute checkbox is not checked.");
            Assert.IsTrue(MainStudioWindow.SideMenuBar.SaveButton.Enabled, "Save ribbon button is not enabled");
        }

        public void Click_Settings_Security_Tab_Resource_Permissions_Row1_View_Checkbox()
        {
            FindViewPermissionsCheckbox(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Checked = true;
            Assert.IsTrue(FindViewPermissionsCheckbox(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Checked, "Settings resource permissions row1 view checkbox is not checked.");
            Assert.IsTrue(MainStudioWindow.SideMenuBar.SaveButton.Enabled, "Save ribbon button is not enabled");
        }

        public void Click_Settings_Security_Tab_Resource_Permissions_Row1_Contribute_Checkbox()
        {
            FindContributePermissionsCheckbox(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Checked = true;
            Assert.IsTrue(FindContributePermissionsCheckbox(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Checked, "Settings resource permissions row1 view checkbox is not checked.");
            Assert.IsTrue(MainStudioWindow.SideMenuBar.SaveButton.Enabled, "Save ribbon button is not enabled");
        }

        public void TryCloseAllTabs()
        {
            var WorkflowWizardTabCloseButtonExists = true;
            var SettingsWizardTabCloseButtonExists = true;
            var serverSourceWizardTabCloseButtonExists = true;
            while (WorkflowWizardTabCloseButtonExists || SettingsWizardTabCloseButtonExists || serverSourceWizardTabCloseButtonExists)
            {
                if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.CloseButton))
                {
                    Click_Close_Workflow_Tab_Button();
                }
                else
                {
                    WorkflowWizardTabCloseButtonExists = false;
                }
                if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.CloseButton))
                {
                    TryCloseSettingsWizardTab();
                }
                else
                {
                    SettingsWizardTabCloseButtonExists = false;
                }
                if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.CloseButton))
                {
                    TryCloseServerSourceWizardTab();
                }
                else
                {
                    serverSourceWizardTabCloseButtonExists = false;
                }
            }
        }

        public void TryCloseAllWorkflowWizardTabs()
        {
            var WorkflowWizardTabCloseButtonExists = true;
            while (WorkflowWizardTabCloseButtonExists)
            {
                try
                {
                    if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.CloseButton))
                    {
                        Click_Close_Workflow_Tab_Button();
                    }
                    else
                    {
                        WorkflowWizardTabCloseButtonExists = false;
                    }
                }
                catch (Exception e)
                {
                    WorkflowWizardTabCloseButtonExists = false;
                    Console.WriteLine("TryClose method failed to close all Workflow tabs.\n" + e.Message);
                }
            }
            Assert.IsFalse(MainStudioWindow.SideMenuBar.RunAndDebugButton.Enabled, "RunDebug button is enabled");
        }

        public void TryCloseWorkflowTestingTab()
        {
            var TimeBefore = System.DateTime.Now;
            try
            {
                if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.CloseButton))
                {
                    Click_Close_Tests_Tab();
                }
                if (ControlExistsNow(MessageBoxWindow.NoButton))
                {
                    Click_MessageBox_No();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception trying to close Workflow Testing tab.\n" + e.Message);
            }
            finally
            {
                Console.WriteLine("No hanging workflow testing tab to clean up after trying for " + (System.DateTime.Now - TimeBefore).Milliseconds.ToString() + "ms.");
            }
        }

        [When(@"I Try Close Settings Tab")]
        public void TryCloseSettingsWizardTab()
        {
            var TimeBefore = System.DateTime.Now;
            try
            {
                if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab))
                {
                    Click_Close_Settings_Tab_Button();
                }
                if (ControlExistsNow(MessageBoxWindow.NoButton))
                {
                    Click_MessageBox_No();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception trying to close settings tab.\n" + e.Message);
            }
            finally
            {
                Console.WriteLine("No hanging settings tab to clean up after trying for " + (System.DateTime.Now - TimeBefore).Milliseconds.ToString() + "ms.");
            }
        }

        private void TryCloseServerSourceWizardTab()
        {
            try
            {
                if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.CloseButton))
                {
                    Click_Close_Server_Source_Wizard_Tab_Button();
                }
                if (ControlExistsNow(MessageBoxWindow.NoButton))
                {
                    Click_MessageBox_No();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("TryClose method failed to close Server Source tab.\n" + e.Message);
            }
        }

        public void WaitForControlVisible(UITestControl control, int searchTimeout = 60000)
        {
            control.WaitForControlCondition((uicontrol) =>
            {
                var point = new Point();
                return control.TryGetClickablePoint(out point);
            }, searchTimeout * int.Parse(Playback.PlaybackSettings.ThinkTimeMultiplier.ToString()));
        }

        public void WaitForControlEnabled(UITestControl control, int searchTimeout = 60000)
        {
            control.WaitForControlCondition((uicontrol) =>
            {
                return control.Enabled;
            }, searchTimeout * int.Parse(Playback.PlaybackSettings.ThinkTimeMultiplier.ToString()));
        }

        public void WaitForControlNotVisible(UITestControl control, int searchTimeout = 60000)
        {
            control.WaitForControlCondition((uicontrol) =>
            {
                var point = new Point();
                return !uicontrol.TryGetClickablePoint(out point);
            }, searchTimeout * int.Parse(Playback.PlaybackSettings.ThinkTimeMultiplier.ToString()));
        }

        [When(@"I Wait For Explorer Localhost Spinner")]
        public void WaitForExplorerLocalhostSpinner()
        {
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.Checkbox.Spinner);
        }

        [Given(@"I Wait For Explorer First Remote Server Spinner")]
        [When(@"I Wait For Explorer First Remote Server Spinner")]
        [Then(@"I Wait For Explorer First Remote Server Spinner")]
        public void WaitForExplorerFirstRemoteServerSpinner()
        {
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.Checkbox.Spinner);
        }

        [When(@"I Wait For Save Dialog Explorer Spinner")]
        public void WaitForSaveDialogExplorerSpinner()
        {
            WaitForSpinner(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.Checkbox.Spinner);
        }

        public void WaitForSpinner(UITestControl control, int searchTimeout = 30000)
        {
            WaitForControlNotVisible(control, searchTimeout);
        }

        [Given(@"I Try DisConnect To Remote Server")]
        [When(@"I Try DisConnect To Remote Server")]
        [Then(@"I Try DisConnect To Remote Server")]
        public void TryDisConnectToRemoteServer()
        {
            if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.SelectedItemAsRemoteConnectionIntegrationConnected))
            {
                Click_Explorer_RemoteServer_Connect_Button();
                Click_Connect_Control_InExplorer();
                Mouse.Click(MainStudioWindow.ComboboxListItemAsLocalhostConnected.Text);
            }
            else
            {
                Click_Connect_Control_InExplorer();
                if (ControlExistsNow(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegrationConnected))
                {
                    Mouse.Click(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegrationConnected.Text);
                    Click_Explorer_RemoteServer_Connect_Button();
                    Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.SelectedItemAsRemoteConnectionIntegration.Exists);
                    Click_Connect_Control_InExplorer();
                    Mouse.Click(MainStudioWindow.ComboboxListItemAsLocalhostConnected.Text);
                }
                Mouse.Click(MainStudioWindow.ComboboxListItemAsLocalhostConnected.Text);
            }
        }

        [Given(@"I Enter Invalid Service Name With Whitespace Into Save Dialog As ""(.*)""")]
        [When(@"I Enter Invalid Service Name With Whitespace Into Save Dialog As ""(.*)""")]
        [Then(@"I Enter Invalid Service Name With Whitespace Into Save Dialog As ""(.*)""")]
        public void I_Enter_Invalid_Service_Name_With_Whitespace_Into_SaveDialog(string ServiceName)
        {
            WpfText errorLabel = SaveDialogWindow.ErrorLabel;
            SaveDialogWindow.ServiceNameTextBox.Text = ServiceName;
            Assert.AreEqual("'Name' contains leading or trailing whitespace characters.", errorLabel.DisplayText, "Error is not the same as expected");
            Assert.IsFalse(SaveDialogWindow.SaveButton.Enabled, "Save dialog save button is not enabled. Check workflow name is valid and that another workflow by that name does not already exist.");
        }

        [Given(@"I Enter Invalid Service Name With Whitespace Into Duplicate Dialog As ""(.*)""")]
        [When(@"I Enter Invalid Service Name With Whitespace Into Duplicate Dialog As ""(.*)""")]
        [Then(@"I Enter Invalid Service Name With Whitespace Into Duplicate Dialog As ""(.*)""")]
        public void Enter_Invalid_Service_Name_With_Whitespace_Into_Duplicate_Dialog(string ServiceName)
        {
            WpfText errorLabel = SaveDialogWindow.ErrorLabel;
            SaveDialogWindow.ServiceNameTextBox.Text = ServiceName;
            Assert.AreEqual("'Name' contains leading or trailing whitespace characters.", errorLabel.DisplayText, "Error is not the same as expected");
            Assert.IsFalse(SaveDialogWindow.DuplicateButton.Enabled, "Save dialog save button is not enabled. Check workflow name is valid and that another workflow by that name does not already exist.");
        }

        [Given(@"I Enter Invalid Service Name Into Save Dialog As ""(.*)""")]
        [When(@"I Enter Invalid Service Name Into Save Dialog As ""(.*)""")]
        [Then(@"I Enter Invalid Service Name Into Save Dialog As ""(.*)""")]
        public void I_Enter_Invalid_Service_Name_Into_SaveDialog(string ServiceName)
        {
            WpfText errorLabel = SaveDialogWindow.ErrorLabel;
            SaveDialogWindow.ServiceNameTextBox.Text = ServiceName;
            Assert.AreEqual("'Name' contains invalid characters", errorLabel.DisplayText, "Error is not the same as expected");
        }

        [Given(@"I Enter Invalid Service Name Into Duplicate Dialog As ""(.*)""")]
        [When(@"I Enter Invalid Service Name Into Duplicate Dialog As ""(.*)""")]
        [Then(@"I Enter Invalid Service Name Into Duplicate Dialog As ""(.*)""")]
        public void Enter_Invalid_Service_Name_Into_Duplicate_Dialog(string ServiceName)
        {
            WpfText errorLabel = SaveDialogWindow.ErrorLabel;
            SaveDialogWindow.ServiceNameTextBox.Text = ServiceName;
            Assert.AreEqual("'Name' contains leading or trailing whitespace characters.", errorLabel.DisplayText, "Error is not the same as expected");
            Assert.IsFalse(SaveDialogWindow.DuplicateButton.Enabled, "Save dialog save button is not enabled. Check workflow name is valid and that another workflow by that name does not already exist.");
        }

        [Given(@"I Enter Service Name Into Duplicate Dialog As ""(.*)""")]
        [When(@"I Enter Service Name Into Duplicate Dialog As ""(.*)""")]
        [Then(@"I Enter Service Name Into Duplicate Dialog As ""(.*)""")]
        public void Enter_Service_Name_Into_Duplicate_Dialog(string ServiceName)
        {
            WpfText errorLabel = SaveDialogWindow.ErrorLabel;
            SaveDialogWindow.ServiceNameTextBox.Text = ServiceName;
            Assert.IsTrue(SaveDialogWindow.DuplicateButton.Enabled, "Save dialog save button is not enabled. Check workflow name is valid and that another workflow by that name does not already exist.");
        }

        [Given(@"I Enter Service Name Into Save Dialog As ""(.*)""")]
        [When(@"I Enter Service Name Into Save Dialog As ""(.*)""")]
        [Then(@"I Enter Service Name Into Save Dialog As ""(.*)""")]
        public void Enter_Valid_Service_Name_Into_Save_Dialog(string ServiceName)
        {
            Assert.IsTrue(SaveDialogWindow.Exists, "Save dialog does not exist on the Surface.");
            SaveDialogWindow.ServiceNameTextBox.Text = ServiceName;
        }

        [Given(@"same name error message is shown")]
        public void GivenSameNameErrorMessageIsShown()
        {
            Assert.AreEqual("An item with this name already exists in this folder.", SaveDialogWindow.ErrorLabel.DisplayText);
        }

        public void Enter_Service_Name_Into_Save_Dialog(string ServiceName, bool duplicate = false, bool invalid = false, bool nameHasWhiteSpace = false, SaveOrDuplicate saveOrDuplicate = SaveOrDuplicate.Save)
        {
            WpfText errorLabel = SaveDialogWindow.ErrorLabel;
            SaveDialogWindow.ServiceNameTextBox.Text = ServiceName;
            Assert.IsTrue(SaveDialogWindow.SaveButton.Enabled, "Save dialog save button is not enabled. Check workflow name is valid and that another workflow by that name does not already exist.");
        }

        public void Select_FirstItem_From_ServicePicker_Tree()
        {
            Mouse.Click(ServicePickerDialog.Explorer.ExplorerTree.Localhost.TreeItem1);
        }

        [Given(@"I Double Click Resource On The Service Picker")]
        [When(@"I Double Click Resource On The Service Picker")]
        [Then(@"I Double Click Resource On The Service Picker")]
        public void DoubleClick_FirstItem_From_ServicePicker_Tree()
        {
            var firstItem = ServicePickerDialog.Explorer.ExplorerTree.Localhost.TreeItem1;
            Mouse.DoubleClick(firstItem);
        }

        [Given(@"I Double Click Resource On The Save Dialog")]
        [When(@"I Double Click Resource On The Save Dialog")]
        [Then(@"I Double Click Resource On The Save Dialog")]
        public void DoubleClickResourceOnTheSaveDialog()
        {
            Mouse.DoubleClick(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem);
        }

        public void Filter_ServicePicker_Explorer(string FilterText)
        {
            ServicePickerDialog.Explorer.FilterTextbox.Text = FilterText;
            WaitForControlVisible(ServicePickerDialog.Explorer.ExplorerTree.Localhost.TreeItem1);
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.Spinner);
        }

        [When(@"I Click Localhost")]
        [Then(@"I Click Localhost")]
        [Given(@"I Click Localhost")]
        public void Click_LocalHost_Once()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost);
        }

        [Given(@"I Select LocalHost on the Save Dialog")]
        [When(@"I Select LocalHost on the Save Dialog")]
        [Then(@"I Select LocalHost on the Save Dialog")]
        public void WhenISelectLocalHostOnTheSaveDialog()
        {
            Mouse.Click(SaveDialogWindow.ExplorerView.ExplorerTree.localhost);
        }

        [Given(@"I Filter the Explorer with ""(.*)""")]
        [When(@"I Filter the Explorer with ""(.*)""")]
        [Then(@"I Filter the Explorer with ""(.*)""")]
        public void Filter_Explorer(string FilterText)
        {
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.SearchTextBox.Text = FilterText;
        }

        [Given(@"I Filter Save Dialog Explorer with ""(.*)""")]
        [When(@"I Filter Save Dialog Explorer with ""(.*)""")]
        [Then(@"I Filter Save Dialog Explorer with ""(.*)""")]
        public void Filter_Save_Dialog_Explorer(string FilterText)
        {
            var searchTextBox = SaveDialogWindow.ExplorerView.SearchTextBox;
            searchTextBox.Text = FilterText;
        }

        [When(@"I Move FirstSubItem Into FirstItem Folder")]
        public void Move_FirstSubItem_Into_FirstItem_Folder()
        {
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem);
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem);
        }

        [When(@"I Filter the ToolBox with ""(.*)""")]
        public void Filter_ToolBox(string FilterText)
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = FilterText;
        }

        public void Enter_GroupName_Into_Windows_Group_Dialog(string GroupName)
        {
            SelectWindowsGroupDialog.ItemPanel.ObjectNameTextbox.Text = GroupName;
            Assert.IsTrue(SelectWindowsGroupDialog.OKPanel.OK.Enabled, "Windows group dialog OK button is not enabled.");
        }
        public void Select_First_Service_From_Service_Picker_Dialog(string ServiceName)
        {
            ServicePickerDialog.Explorer.FilterTextbox.Text = ServiceName;
            Mouse.Click(ServicePickerDialog.Explorer.ExplorerTree.Localhost.TreeItem1);
            Playback.Wait(500);
            Assert.IsTrue(ServicePickerDialog.OK.Enabled, "Service picker dialog OK button is not enabled.");
            Click_Service_Picker_Dialog_OK();
        }

        [Given(@"I Select ""(.*)"" From Service Picker")]
        [When(@"I Select ""(.*)"" From Service Picker")]
        [Then(@"I Select ""(.*)"" From Service Picker")]
        public void Select_SubItem_Service_From_Service_Picker_Dialog(string ServiceName)
        {
            ServicePickerDialog.Explorer.FilterTextbox.Text = ServiceName;
            Mouse.Click(ServicePickerDialog.Explorer.ExplorerTree.Localhost.TreeItem1.TreeItem11);
            Assert.IsTrue(ServicePickerDialog.OK.Enabled, "Service picker dialog OK button is not enabled.");
            Click_Service_Picker_Dialog_OK();
        }

        public void TryRefreshExplorerUntilOneItemOnly(int retries = 3)
        {
            while ((ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem) || ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.SecondItem)) && retries-- > 0)
            {
                Click_Explorer_Refresh_Button();
            }
        }

        public void Select_RemoteConnectionIntegration_From_Explorer_Remote_Server_Dropdown_List(WpfText comboboxListItem)
        {
            Click_Explorer_Remote_Server_Dropdown_List();
            Assert.IsTrue(comboboxListItem.Exists, "Server does not exist in explorer remote server drop down list.");
            Mouse.Click(comboboxListItem, new Point(79, 8));
        }

        public void Select_TSTCIREMOTEConnected_From_Explorer_Remote_Server_Dropdown_List()
        {
            Mouse.Click(MainStudioWindow.ComboboxListItemAsTSTCIREMOTEConnected, new Point(80, 13));
        }

        [When(@"I Select NewRemoteServer From Explorer Server Dropdownlist")]
        public void Select_NewRemoteServer_From_Explorer_Server_Dropdownlist()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.ToggleButton, new Point(217, 8));
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsNewRemoteServer.Exists, "New Remote Server... does not exist in explorer remote server drop down list");
            Mouse.Click(MainStudioWindow.ComboboxListItemAsNewRemoteServer.NewRemoteServerItemText, new Point(114, 10));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.NewServerSource.Exists, "Server source wizard does not exist.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.NewServerSource.ProtocolCombobox.ToggleDropdown.Exists, "Server source wizard protocol dropdown does not exist.");
        }

        public void Select_LocalhostConnected_From_Explorer_Remote_Server_Dropdown_List()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.ToggleButton, new Point(217, 8));
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsLocalhostConnected.Exists, "localhost (connected) does not exist in explorer remote server drop down list");
            Mouse.Click(MainStudioWindow.ComboboxListItemAsLocalhostConnected, new Point(94, 10));
            Assert.AreEqual("localhost", MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.SelectedItemAsLocalhost.DisplayText, "Selected remote server is not localhost");
        }

        public void Select_localhost_From_Explorer_Remote_Server_Dropdown_List()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.ToggleButton, new Point(174, 8));
            Mouse.Click(MainStudioWindow.ComboboxListItemAsLocalhostConnected.Text);
        }

        [When(@"I Enter Dice Roll Values")]
        public void WhenIEnterDiceRollValues()
        {
            Enter_Dice_Roll_Values();
        }

        [When(@"I Save With Ribbon Button and Dialog As ""(.*)"" and Append Unique Guid")]
        public void WhenISaveWithRibbonButtonAndDialogAsAndAppendUniqueGuid(string p0)
        {
            Save_With_Ribbon_Button_And_Dialog(p0 + Guid.NewGuid().ToString().Substring(0, 8));
        }

        [Given(@"I Enter Service Name Into Save Dialog As ""(.*)"" and Append Unique Guid")]
        [When(@"I Enter Service Name Into Save Dialog As ""(.*)"" and Append Unique Guid")]
        [Then(@"I Enter Service Name Into Save Dialog As ""(.*)"" and Append Unique Guid")]
        public void Enter_Service_Name_Into_Save_Dialog_and_Append_Unique_Guid(string ServiceName)
        {
            WpfText errorLabel = SaveDialogWindow.ErrorLabel;
            SaveDialogWindow.ServiceNameTextBox.Text = ServiceName + Guid.NewGuid().ToString().Substring(0, 8);
            Assert.IsTrue(SaveDialogWindow.SaveButton.Enabled, "Save dialog save button is not enabled. Check workflow name is valid and that another workflow by that name does not already exist.");
        }

        [Given(@"I Save With Ribbon Button And Dialog As ""(.*)""")]
        [When(@"I Save With Ribbon Button And Dialog As ""(.*)""")]
        [Then(@"I Save With Ribbon Button And Dialog As ""(.*)""")]
        public void Save_With_Ribbon_Button_And_Dialog(string Name)
        {
            Click_Save_RibbonButton();
            Enter_Service_Name_Into_Save_Dialog(Name);
            Click_SaveDialog_Save_Button();
        }

        [Given(@"I Save Valid Service With Ribbon Button And Dialog As ""(.*)""")]
        [When(@"I Save Valid Service With Ribbon Button And Dialog As ""(.*)""")]
        [Then(@"I Save Valid Service With Ribbon Button And Dialog As ""(.*)""")]
        public void Save_Valid_Service_With_Ribbon_Button_And_Dialog(string Name)
        {
            Click_Save_RibbonButton();
            Enter_Valid_Service_Name_Into_Save_Dialog(Name);
            Click_SaveDialog_Save_Button();
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.Spinner);
        }

        [Given(@"I Click SaveDialog Save Button")]
        [When(@"I Click SaveDialog Save Button")]
        [Then(@"I Click SaveDialog Save Button")]
        public void Click_SaveDialog_Save_Button()
        {
            Assert.IsTrue(SaveDialogWindow.SaveButton.Enabled, "Save button on the Save Dialog is not Enabled");
            Mouse.Click(SaveDialogWindow.SaveButton, new Point(25, 4));
        }

        public void TryCloseNewWebSourceWizardTab()
        {
            if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.CloseButton))
            {
                Click_Close_Web_Source_Wizard_Tab_Button();
                if (ControlExistsNow(MessageBoxWindow.NoButton))
                {
                    Click_MessageBox_No();
                }
            }
        }

        public void Enter_Text_Into_Unpinned_Assign_Large_View_Row1_Variable_Textbox_As_SomeVariabeName()
        {
            MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text = "[[SomeVariable]]";
            Assert.AreEqual("[[SomeVariable]]", MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text, "Multiassign small view row 1 variable textbox text does not equal \"[[SomeVariable]]\".");
        }

        public void Enter_Text_Into_Debug_Input_Row1_Value_Textbox(string text)
        {
            if (MainStudioWindow.DebugInputDialog.TabItemsTabList.InputDataTab.InputsTable.Row1.InputValueCell.InputValueComboboxl.InputValueText.Text != text)
            {
                MainStudioWindow.DebugInputDialog.TabItemsTabList.InputDataTab.InputsTable.Row1.InputValueCell.InputValueComboboxl.InputValueText.Text = text;
            }
            Assert.AreEqual(text, MainStudioWindow.DebugInputDialog.TabItemsTabList.InputDataTab.InputsTable.Row1.InputValueCell.InputValueComboboxl.InputValueText.Text, "Debug input data row1 textbox text is not equal to \'" + text + "\' after typing that in.");
        }

        public void Enter_Text_Into_Debug_Input_Row2_Value_Textbox(string text)
        {
            MainStudioWindow.DebugInputDialog.TabItemsTabList.InputDataTab.InputsTable.Row2.InputValueCell.InputValueComboboxl.InputValueText.Text = text;
            Assert.AreEqual(text, MainStudioWindow.DebugInputDialog.TabItemsTabList.InputDataTab.InputsTable.Row2.InputValueCell.InputValueComboboxl.InputValueText.Text, "Debug input data row2 textbox text is not equal to \'" + text + "\' after typing that in.");
        }

        [When(@"I Type ""(.*)"" into Plugin Source Wizard Assembly Textbox")]
        public void Type_dll_into_Plugin_Source_Wizard_Assembly_Textbox(string text)
        {
            if (!File.Exists(text))
            {
                text = text.Replace("Framework64", "Framework");
                if (!File.Exists(text))
                {
                    throw new Exception("No suitable DLL could be found for this test to use.");
                }
            }
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DotNetPluginSourceTab.WorkSurfaceContext.AssemblyComboBox.TextEdit.Text = text;
            Assert.IsTrue(MainStudioWindow.SideMenuBar.SaveButton.Enabled, "Save button is not enabled after DLL has been selected in plugin source wizard.");
        }

        public void Enter_GroupName_Into_Settings_Dialog_Resource_Permissions_Row1_Windows_Group_Textbox(string GroupName)
        {
            FindWindowsGroupTextbox(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Text = GroupName;
            Assert.AreEqual(FindWindowsGroupTextbox(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Text, GroupName, "Settings security tab resource permissions row 1 windows group textbox text does not equal Public.");
        }

        [Given(@"I Set Resource Permissions For ""(.*)"" to Group ""(.*)"" and Permissions for View to ""(.*)"" and Contribute to ""(.*)"" and Execute to ""(.*)""")]
        [When(@"I Set Resource Permissions For ""(.*)"" to Group ""(.*)"" and Permissions for View to ""(.*)"" and Contribute to ""(.*)"" and Execute to ""(.*)""")]
        [Then(@"I Set Resource Permissions For ""(.*)"" to Group ""(.*)"" and Permissions for View to ""(.*)"" and Contribute to ""(.*)"" and Execute to ""(.*)""")]
        public void SetResourcePermissions(string ResourceName, string WindowsGroupName, bool setView = false, bool setExecute = false, bool setContribute = false)
        {
            Click_Settings_RibbonButton();
            Click_Settings_Resource_Permissions_Row1_Add_Resource_Button();
            Select_SubItem_Service_From_Service_Picker_Dialog(ResourceName);
            Enter_GroupName_Into_Settings_Dialog_Resource_Permissions_Row1_Windows_Group_Textbox(WindowsGroupName);
            if (setView)
            {
                Click_Settings_Security_Tab_Resource_Permissions_Row1_View_Checkbox();
            }
            if (setExecute)
            {
                Click_Settings_Security_Tab_ResourcePermissions_Row1_Execute_Checkbox();
            }
            if (setContribute)
            {
                Click_Settings_Security_Tab_Resource_Permissions_Row1_Contribute_Checkbox();
            }
            Click_Save_Ribbon_Button_With_No_Save_Dialog();
        }
        public void Set_FirstResource_ResourcePermissions(string ResourceName, string WindowsGroupName, bool setView = false, bool setExecute = false, bool setContribute = false)
        {            
            Click_Settings_Resource_Permissions_Row1_Add_Resource_Button();
            Select_First_Service_From_Service_Picker_Dialog(ResourceName);
            Enter_GroupName_Into_Settings_Dialog_Resource_Permissions_Row1_Windows_Group_Textbox(WindowsGroupName);
            if (setView)
            {
                Click_Settings_Security_Tab_Resource_Permissions_Row1_View_Checkbox();
            }
            if (setExecute)
            {
                Click_Settings_Security_Tab_ResourcePermissions_Row1_Execute_Checkbox();
            }
            if (setContribute)
            {
                Click_Settings_Security_Tab_Resource_Permissions_Row1_Contribute_Checkbox();
            }
            Click_Save_Ribbon_Button_With_No_Save_Dialog();
        }

        [Given(@"I Create Remote Server Source As ""(.*)"" with address ""(.*)""")]
        [When(@"I Create Remote Server Source As ""(.*)"" with address ""(.*)""")]
        [Then(@"I Create Remote Server Source As ""(.*)"" with address ""(.*)""")]
        public void CreateRemoteServerSource(string ServerSourceName, string ServerAddress)
        {
            CreateRemoteServerSource(ServerSourceName, ServerAddress, false);
        }

        public void CreateRemoteServerSource(string ServerSourceName, string ServerAddress, bool PublicAuth = false)
        {
            Select_http_From_Server_Source_Wizard_Address_Protocol_Dropdown();
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext
                .NewServerSource.AddressComboBox.AddressEditBox.Text = ServerAddress;
            if (ServerAddress == "tst-ci-")
            {
                Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.NewServerSource.AddressComboBox.TSTCIREMOTE.Exists, "TSTCIREMOTE does not exist in server source wizard drop down list after starting by typing tst-ci-.");
                Select_TSTCIREMOTE_From_Server_Source_Wizard_Dropdownlist();
            }
            if (PublicAuth)
            {
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.PublicRadioButton.Selected = true;
            }
            Click_Server_Source_Wizard_Test_Connection_Button();
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.ErrorText.Spinner);
            Save_With_Ribbon_Button_And_Dialog(ServerSourceName);
            Click_Close_Server_Source_Wizard_Tab_Button();
        }

        [When(@"I Select Deploy First Source Item")]
        [Then(@"I Select Deploy First Source Item")]
        [Given(@"I Select Deploy First Source Item")]
        public void Select_Deploy_First_Source_Item()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerExplorer.ExplorerTree.SourceServerName.FirstExplorerTreeItem.CheckBox.Checked = true;
        }

        [Given(@"I Click Deploy Tab Deploy Button")]
        [When(@"I Click Deploy Tab Deploy Button")]
        [Then(@"I Click Deploy Tab Deploy Button")]
        public void Click_Deploy_Tab_Deploy_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DeployButton);
        }

        [Given(@"I setup Public Permissions for ""(.*)"" for localhost")]
        public void SetupPublicPermissionsForForLocalhost(string resource)
        {
            Click_Settings_RibbonButton();
            var deleteFirstResourceButton = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1.RemovePermissionButton;
            if (deleteFirstResourceButton.Enabled)
            {
                var isViewChecked = FindViewPermissionsCheckbox(
                    MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext
                        .SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Checked;

                var isExecuteChecked = FindExecutePermissionsCheckbox(
                    MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext
                        .SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Checked;

                if (isViewChecked && isExecuteChecked)
                {
                    Click_Close_Settings_Tab_Button();
                    return;
                }
            }            
            Set_FirstResource_ResourcePermissions(resource, "Public", true, true);
            Click_Close_Settings_Tab_Button();
        }

        [Given(@"I setup Public Permissions for ""(.*)"" for Remote Server")]
        public void SetupPublicPermissionsForForRemoteServer(string resource)
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost);
            Select_RemoteConnectionIntegration_From_Explorer();
            Click_Explorer_RemoteServer_Connect_Button();
            Playback.Wait(1000);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer);
            Click_Settings_RibbonButton();
            var deleteFirstResourceButton = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1.RemovePermissionButton;
            if (deleteFirstResourceButton.Enabled)
            {
                var isViewChecked = FindViewPermissionsCheckbox(
                    MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext
                        .SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Checked;

                var isExecuteChecked = FindExecutePermissionsCheckbox(
                    MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext
                        .SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Checked;

                if (isViewChecked && isExecuteChecked)
                {
                    Click_Close_Settings_Tab_Button();
                    return;
                }
            }
            Set_FirstResource_ResourcePermissions(resource, "Public", true, true);
            Click_Close_Settings_Tab_Button();
        }

        [Then(@"I validate I can Deploy ""(.*)""")]
        public void ValidateICanDeploy(string resource)
        {
            Filter_Deploy_Source_Explorer(resource);
            Playback.Wait(1000);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerExplorer.ExplorerTree.RemoteServer.FirstRemoteResource.FirstRemoteResourceCheckBox.Enabled, "The Deploy selection checkbox is not Enabled");
        }

        [Then(@"I validate I can not Deploy ""(.*)""")]
        public void ValidateICanNotDeploy(string resource)
        {
            Filter_Deploy_Source_Explorer(resource);
            Playback.Wait(2000);
            Assert.IsFalse(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerExplorer.ExplorerTree.RemoteServer.FirstRemoteResource.FirstRemoteResourceCheckBox.Enabled, "The Deploy selection checkbox is Enabled");
        }

        public void TryCloseDeployWizardTab()
        {
            try
            {
                if (ControlExistsNow(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab))
                {
                    Click_Close_Deploy_Tab_Button();
                }
                if (ControlExistsNow(MessageBoxWindow.NoButton))
                {
                    Click_MessageBox_No();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("TryClose method failed to close Deploy tab.\n" + e.Message);
            }
        }

        [Given(@"Dirty is ""(.*)"" When I Click EnableDisable test ""(.*)""")]
        [When(@"Dirty is ""(.*)"" When I Click EnableDisable test ""(.*)""")]
        [Then(@"Dirty is ""(.*)"" When I Click EnableDisable test ""(.*)""")]
        public void ThenDirtyIsWhenIClickEnableDisableTest(string dirty, int test)
        {
            if (dirty.ToUpper() == "NO")
                Click_EnableDisable_This_Test_CheckBox(testInstance: test);
            else
                Click_EnableDisable_This_Test_CheckBox(true, testInstance: test);
        }

        [Then(@"I Click EnableDisable Test (.*), dirty ""(.*)""")]
        public void ThenIClickEnableDisableTestDirty(int instance, string dirty)
        {
            Click_EnableDisable_This_Test_CheckBox(dirty == "true", instance);
        }

        public void Click_EnableDisable_This_Test_CheckBox(bool nameContainsStar = false, int testInstance = 1)
        {
            var currentTest = GetCurrentTest(testInstance);
            var testRunState = GetTestRunState(testInstance, currentTest);
            var selectedTestDeleteButton = GetSelectedTestDeleteButton(currentTest, testInstance);
            var beforeClick = testRunState.Checked;

            Mouse.Click(testRunState);
            Assert_Display_Text_ContainStar(Tab, nameContainsStar, testInstance);
            Assert_Display_Text_ContainStar(Test, nameContainsStar, testInstance);
        }

        public void Drag_From_Explorer_Onto_DesignSurface(string ServicePath)
        {
            Filter_Explorer(ServicePath);
            Drag_Explorer_Localhost_First_Item_Onto_Workflow_Design_Surface();
        }

        [Given("I Drag Dice Roll Example Onto DesignSurface")]
        [When("I Drag Dice Roll Example Onto DesignSurface")]
        [Then("I Drag Dice Roll Example Onto DesignSurface")]
        public void Drag_Dice_Roll_Example_Onto_DesignSurface()
        {
            Filter_Explorer("Dice Roll");
            Drag_Explorer_Localhost_Second_Item_Onto_Workflow_Design_Surface();
        }

        [Given(@"I Click DB Source Wizard Test Connection Button")]
        [When(@"I Click DB Source Wizard Test Connection Button")]
        [Then(@"I Click DB Source Wizard Test Connection Button")]
        public void Click_DB_Source_Wizard_Test_Connection_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.TestConnectionButton, new Point(21, 16));
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.Spinner);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.Exists, "Database Combobox is not visible.");
        }


        [Given(@"The DB Source Wizard Test Succeeded Image Is Visible")]
        [When(@"The DB Source Wizard Test Succeeded Image Is Visible")]
        [Then(@"The DB Source Wizard Test Succeeded Image Is Visible")]
        public void Assert_The_DB_Source_Wizard_Test_Succeeded_Image_Is_Visible()
        {
            var point = new Point();
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ConnectionPassedImage.TryGetClickablePoint(out point), "New DB source wizard test succeeded image is not visible after testing with RSAKLFSVRGENDEV and waiting for the spinner.");
        }

        [When(@"I Select ""(.*)"" from the source tab")]
        [Then(@"I Select ""(.*)"" from the source tab")]
        [Given(@"I Select ""(.*)"" from the source tab")]
        public void WhenISelectFromTheSourceTab(string ServiceName)
        {
            Enter_DeployViewOnly_Into_Deploy_Source_Filter(ServiceName);
            Select_Deploy_First_Source_Item();
        }

        [Given(@"I Select localhost from the source tab")]
        [When(@"I Select localhost from the source tab")]
        [Then(@"I Select localhost from the source tab")]
        public void WhenISelectLocalhostFromTheSourceTab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerExplorer.ExplorerTree.LocalHost.EnvironmentNameCheckCheckBox);
        }


        [When(@"I filter for ""(.*)"" on the source filter")]
        [Then(@"I filter for ""(.*)"" on the source filter")]
        [Given(@"I filter for ""(.*)"" on the source filter")]
        public void WhenIFilterForOnTheSourceFilter(string ServiceName)
        {
            Enter_DeployViewOnly_Into_Deploy_Source_Filter(ServiceName);
        }

        [When(@"Resources is visible on the tree")]
        [Then(@"Resources is visible on the tree")]
        public void WhenResourcesIsVisibleOnTheTree()
        {
            var controlExistsNow = ControlExistsNow(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerExplorer.ExplorerTree.SourceServerName.FirstExplorerTreeItem);
            Assert.IsTrue(controlExistsNow);
        }


        [Then(@"Deploy Button is enabled  ""(.*)""")]
        [When(@"Deploy Button is enabled  ""(.*)""")]
        [Given(@"Deploy Button is enabled  ""(.*)""")]
        public void ThenDeployButtonIsEnabled(string enabled)
        {
            var isEnabled = bool.Parse(enabled);
            if (isEnabled)
            {
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DeployButton.WaitForControlEnabled();
            }
            Assert.AreEqual(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DeployButton.Enabled, isEnabled);
        }

        [Then(@"I Click Deploy button")]
        [Given(@"I Click Deploy button")]
        [When(@"I Click Deploy button")]
        public void ThenIClickDeployButton()
        {

            Click_Deploy_Tab_Deploy_Button();

        }


        [Given(@"I Deploy ""(.*)"" From Deploy View")]
        [When(@"I Deploy ""(.*)"" From Deploy View")]
        [Then(@"I Deploy ""(.*)"" From Deploy View")]
        public void Deploy_Service_From_Deploy_View(string ServiceName)
        {
            Enter_DeployViewOnly_Into_Deploy_Source_Filter(ServiceName);
            Select_Deploy_First_Source_Item();
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DeployButton.Enabled,
                "Deploy button is not enabled after valid server and resource are selected.");
            Click_Deploy_Tab_Deploy_Button();
        }

        public void Enter_Text_Into_Workflow_Tests_Output_Row1_Value_Textbox_As_CodedUITest()
        {
            WpfEdit textbox = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestOutputsTable.Row1.Cell.IntellisenseComboBox.Textbox;

            var helloUser = "Hello User.";
            Keyboard.SendKeys(textbox, helloUser, ModifierKeys.None);

            // Verify that the 'Text' property of 'Text' text box equals 'User'
            Assert.AreEqual(helloUser, textbox.Text, "Workflow tests output row 1 value textbox text does not equal 'Hello User' after typing that in.");
        }

        public void Select_Test(int instance = 1)
        {
            var currentTest = GetCurrentTest(instance);
            Mouse.Click(currentTest);
        }

        public void Click_RunAll_Button(string BrokenRule = null)
        {
            string DuplicateNameError = "DuplicateNameError";
            string UnsavedResourceError = "UnsavedResourceError";
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.RunAllButton, new Point(35, 10));
            Assert.AreEqual("Window", MessageBoxWindow.ControlType.ToString(), "Messagebox does not exist after clicking RunAll button");

            if (!string.IsNullOrEmpty(BrokenRule))
            {
                if (BrokenRule.ToUpper().Equals(UnsavedResourceError))
                    Assert.AreEqual("Please save currently edited Test(s) before running the tests.", MessageBoxWindow.UIPleasesavecurrentlyeText.DisplayText, "Message is not Equal to Please save currently edited Test(s) before running the t" +
                            "ests.");
                if (BrokenRule.ToUpper().Equals(DuplicateNameError))
                    Assert.AreEqual("Please save currently edited Test(s) before running the tests.", MessageBoxWindow.UIPleasesavecurrentlyeText.DisplayText, "Messagebox does not show duplicated name error");
            }
        }

        public void CreateAndSave_Dice_Workflow(string WorkflowName)
        {
            Select_NewWorkFlowService_From_ContextMenu();
            Drag_Toolbox_Random_Onto_DesignSurface();
            Enter_Dice_Roll_Values();
            Save_With_Ribbon_Button_And_Dialog(WorkflowName);
            Click_Close_Workflow_Tab_Button();
        }

        [When(@"I UnCheck Public Administrator")]
        public void UnCheck_Public_Administrator()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_AdministratorCell.Public_AdministratorCheckBox.Checked = false;
            Assert.IsFalse(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_AdministratorCell.Public_AdministratorCheckBox.Checked, "Public Administrator checkbox is checked after UnChecking Administrator.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_ViewCell.Public_ViewCheckBox.Checked, "Public View checkbox is unchecked after unChecking Administrator.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_ExecuteCell.Public_ExecuteCheckBox.Checked, "Public Execute checkbox unchecked after unChecking Administrator.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_ContributeCell.Public_ContributeCheckBox.Checked, "Public Contribute checkbox is unchecked after unChecking Administrator.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_DeployFromCell.Public_DeployFromCheckBox.Checked, "Public DeplotFrom checkbox is unchecked after unChecking Administrator.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_DeployToCell.Public_DeployToCheckBox.Checked, "Public DeployTo checkbox is unchecked after unChecking Administrator.");
        }

        [Given(@"I Check Resource Contribute")]
        [When(@"I Check Resource Contribute")]
        [Then(@"I Check Resource Contribute")]
        public void Check_Resource_Contribute()
        {
            FindContributePermissionsCheckbox(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Checked = true;
            Assert.IsTrue(FindContributePermissionsCheckbox(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Checked, "Resource View checkbox is NOT checked after Checking Contribute.");
            Assert.IsTrue(FindContributePermissionsCheckbox(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Checked, "Resource Execute checkbox is NOT checked after Checking Contribute.");
            Assert.IsTrue(FindAddRemoveRowButton(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Enabled, "Resource Delete button is disabled");
        }

        public void Scroll_DownThenUp_On_DataMergeTool_SmallView()
        {
            Mouse.MoveScrollWheel(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataMerge.SmallView.DataGrid, -1);
        }

        [Given(@"I UnCheck Public View")]
        [When(@"I UnCheck Public View")]
        [Then(@"I UnCheck Public View")]
        public void UnCheck_Public_View()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_ViewCell.Public_ViewCheckBox.Checked = false;
            Assert.IsFalse(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_ViewCell.Public_ViewCheckBox.Checked, "Public View checkbox is checked after Checking Contribute.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_ExecuteCell.Public_ExecuteCheckBox.Checked, "Public Execute checkbox is NOT checked after Checking Contribute.");
            Assert.IsFalse(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_ContributeCell.Public_ContributeCheckBox.Checked, "Public Contribute checkbox is checked after UnChecking Execute/View.");
            Assert.IsFalse(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_AdministratorCell.Public_AdministratorCheckBox.Checked, "Public Administrator checkbox is checked after UnChecking Contribute.");
        }

        [Given(@"I Update Test Name To ""(.*)""")]
        [When(@"I Update Test Name To ""(.*)""")]
        [Then(@"I Update Test Name To ""(.*)""")]
        public void Update_Test_Name(string overrideName = null)
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestNameTextbox, new Point(59, 16));
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestNameTextbox.Text = "";
            if (!string.IsNullOrEmpty(overrideName))
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestNameTextbox.Text = overrideName;
            else
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestNameTextbox.Text = "Dice_Test";
        }

        public void Save_Tets_With_Shortcut()
        {
            var testsTabPage = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestNameTextbox;
            Keyboard.SendKeys(testsTabPage, "S", (ModifierKeys.Control));
        }

        [Given(@"I Delete Test ""(.*)""")]
        [When(@"I Delete Test ""(.*)""")]
        [Then(@"I Delete Test ""(.*)""")]
        public void Click_Delete_Test_Button(int testInstance = 1)
        {
            var currentTest = GetCurrentTest(testInstance);
            var selectedTestDeleteButton = GetSelectedTestDeleteButton(currentTest, testInstance);
            Mouse.Click(selectedTestDeleteButton);
            Assert.IsTrue(MessageBoxWindow.Exists, "Delete Confirmation MessageBox did not Open");
        }

        private static WpfText GetSelectedTestRunTimeDisplay(WpfListItem test, int instance)
        {
            WpfText testRunTimeDisplay;
            switch (instance)
            {
                case 2:
                    var test2 = test as Test2;
                    testRunTimeDisplay = test2.RunTimeDisplay;
                    break;
                case 3:
                    var test3 = test as Test3;
                    testRunTimeDisplay = test3.RunTimeDisplay;
                    break;
                case 4:
                    var test4 = test as Test4;
                    testRunTimeDisplay = test4.RunTimeDisplay;
                    break;
                default:
                    var test1 = test as Test1;
                    testRunTimeDisplay = test1.RunTimeDisplay;
                    break;
            }
            return testRunTimeDisplay;
        }

        private static WpfText GetSelectedTestNeverRunDisplay(WpfListItem test, int instance)
        {
            WpfText neverRunDisplay;
            switch (instance)
            {
                case 2:
                    var test2 = test as Test2;
                    neverRunDisplay = test2.NeverRunDisplay;
                    break;
                case 3:
                    var test3 = test as Test3;
                    neverRunDisplay = test3.NeverRunDisplay;
                    break;
                case 4:
                    var test4 = test as Test4;
                    neverRunDisplay = test4.NeverRunDisplay;
                    break;
                default:
                    var test1 = test as Test1;
                    neverRunDisplay = test1.NeverRunDisplay;
                    break;
            }
            return neverRunDisplay;
        }

        public void Click_Run_Test_Button(TestResultEnum? expectedTestResultEnum = null, int instance = 1)
        {
            var currentTest = GetCurrentTest(instance);
            Keyboard.SendKeys(MainStudioWindow, "{F5}", ModifierKeys.None);
            if (expectedTestResultEnum != null)
                AssertTestResults(expectedTestResultEnum.Value, instance, currentTest);
        }

        public void ClickConstructorMockRadio(bool isChecked)
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.DotnetDllTreeItem.ConstructorExpander.StepOutputs_ctor_Table.ColumnHeadersPrHeader.ColumnHeader.UIMockRadioButton.Selected = isChecked;
        }
        public void ClickFavouriteMockRadio(bool isChecked)
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.DotnetDllTreeItem.FavouriteFoodsExpander.StepOutputs_FavouTable.ColumnHeadersPrHeader.ItemColumnHeader.MockRadioButton.Selected = isChecked;
        }

        private void AssertTestResults(TestResultEnum expectedTestResultEnum, int instance, WpfListItem currentTest)
        {
            switch (expectedTestResultEnum)
            {
                case TestResultEnum.Invalid:
                    TestResults.GetSelectedTestInvalidResult(currentTest, instance);
                    break;
                case TestResultEnum.Pending:
                    TestResults.GetSelectedTestPendingResult(currentTest, instance);
                    break;
                case TestResultEnum.Pass:
                    TestResults.GetSelectedTestPassingResult(currentTest, instance);
                    break;
                case TestResultEnum.Fail:
                    TestResults.GetSelectedTestFailingResult(currentTest, instance);
                    break;
            }
        }

        public void Click_Duplicate_Test_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.DuplicateButton, new Point(14, 10));
        }

        public void Assert_Test_Result(string result)
        {
            WpfText passing = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.Passing;
            WpfText invalid = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.Invalid;
            WpfText failing = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.Failing;
            WpfText pending = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.Pending;
            if (result == "Passing")
                Assert.IsTrue(passing.Exists, "Test is not passing");
            if (result == "Failing")
                Assert.IsTrue(failing.Exists, "Test is not failing");
            if (result == "Invalid")
                Assert.IsTrue(invalid.Exists, "Test is not invalid");
            if (result == "Pending")
                Assert.IsTrue(pending.Exists, "Test is not pending");
        }

        const string Tab = "Tab";
        const string Test = "Test";
        public void Click_Create_New_Tests(bool nameContainsStar = false, int testInstance = 1)
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.CreateTest.CreateTestButton, new Point(158, 10));

            var currentTest = GetCurrentTest(testInstance);
            var testEnabledSelector = GetTestRunState(testInstance, currentTest).Checked;
            var testNeverRun = GetSelectedTestNeverRunDisplay(currentTest, testInstance);

            Assert.AreEqual("Never run", testNeverRun.DisplayText);
            AssertTestResults(TestResultEnum.Pending, testInstance, currentTest);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestNameText.Exists, string.Format("Test{0} Name textbox does not exist after clicking Create New Test", testInstance));
            Assert.IsTrue(testEnabledSelector, string.Format("Test {0} is diabled after clicking Create new test from context menu", testInstance));

            Assert_Display_Text_ContainStar(Tab, nameContainsStar, testInstance);
            Assert_Display_Text_ContainStar(Test, nameContainsStar, testInstance);
        }


        public void Assert_Display_Text_ContainStar(string control, bool containsStar, int instance = 1)
        {
            WpfList testsListBox = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList;
            var test = GetCurrentTest(instance);
            string description = string.Empty;
            if (control == "Tab")
            {
                description = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.TabDescription.DisplayText;
                if (containsStar)
                    Assert.IsTrue(description.Contains("*"), description + " DOES NOT contain a Star");
                else
                    Assert.IsFalse(description.Contains("*"), description + " contains a Star");
            }
            else if (control == "Test")
            {
                description = GetTestNameDisplayText(instance, test).DisplayText;
                if (containsStar)
                    Assert.IsTrue(description.Contains("*"), description + " DOES NOT contain a Star");
                else
                    Assert.IsFalse(description.Contains("*"), description + " contains a Star");
            }

            if (containsStar)
                Assert.IsTrue(description.Contains("*"), "Description does not contain *");
            else
                Assert.IsFalse(description.Contains("*"), "Description contains *");
            if (instance == 0)
            {
                var descriptions = testsListBox.GetContent();
                Assert.IsFalse(descriptions.Contains("*"), "Description contains *");
            }
        }

        private WpfCheckBox GetTestRunState(int testInstance, WpfListItem test)
        {
            WpfCheckBox value;
            switch (testInstance)
            {
                case 2:
                    var test2 = test as Test2;
                    value = test2.TestEnabledSelector;
                    break;
                case 3:
                    var test3 = test as Test3;
                    value = test3.TestEnabledSelector;
                    break;
                case 4:
                    var test4 = test as Test4;
                    value = test4.TestEnabledSelector;
                    break;
                default:
                    var test1 = test as Test1;
                    value = test1.TestEnabledSelector;
                    break;
            }
            return value;
        }
        private WpfText GetTestNameDisplayText(int instance, WpfListItem test)
        {
            WpfText property;
            switch (instance)
            {
                case 2:
                    var test2 = test as Test2;
                    property = test2.TestNameDisplay;
                    break;
                case 3:
                    var test3 = test as Test3;
                    property = test3.TestNameDisplay;
                    break;
                case 4:
                    var test4 = test as Test4;
                    property = test4.TestNameDisplay;
                    break;
                default:
                    var test1 = test as Test1;
                    property = test1.TestNameDisplay;
                    break;
            }

            return property;
        }

        public WpfListItem GetCurrentTest(int testInstance)
        {
            WpfListItem test;
            switch (testInstance)
            {
                case 2:
                    test = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test2;
                    break;
                case 3:
                    test = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test3;
                    break;
                case 4:
                    test = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test4;
                    break;
                default:
                    test = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1;
                    break;
            }
            return test;
        }

        public WpfButton GetSelectedTestRunButton(WpfListItem test, int testInstance = 1)
        {
            WpfButton value;
            switch (testInstance)
            {
                case 2:
                    var test2 = test as Test2;
                    value = test2.RunButton;
                    break;
                case 3:
                    var test3 = test as Test3;
                    value = test3.RunButton;
                    break;
                case 4:
                    var test4 = test as Test4;
                    value = test4.RunButton;
                    break;
                default:
                    var test1 = test as Test1;
                    value = test1.RunButton;
                    break;
            }
            return value;
        }

        public WpfButton GetSelectedTestDeleteButton(WpfListItem test, int testInstance = 1)
        {
            WpfButton value;
            switch (testInstance)
            {
                case 2:
                    var test2 = test as Test2;
                    value = test2.DeleteButton;
                    break;
                case 3:
                    var test3 = test as Test3;
                    value = test3.DeleteButton;
                    break;
                case 4:
                    var test4 = test as Test4;
                    value = test4.DeleteButton;
                    break;
                default:
                    var test1 = test as Test1;
                    value = test1.DeleteButton;
                    break;
            }
            return value;
        }

        [Given("I Click Save Ribbon Button Without Expecting a Dialog")]
        [When("I Click Save Ribbon Button Without Expecting a Dialog")]
        [Given("I Click Save Ribbon Button Without Expecting a Dialog")]
        [Then("I Click Save Ribbon Button Without Expecting a Dialog")]
        public void Click_Save_Ribbon_Button_Without_Expecting_A_Dialog()
        {
            Click_Save_Ribbon_Button_With_No_Save_Dialog(2000);
        }

        [Given(@"I Click Save Ribbon Button With No Save Dialog")]
        [When(@"I Click Save Ribbon Button With No Save Dialog")]
        [Then(@"I Click Save Ribbon Button With No Save Dialog")]
        public void Click_Save_Ribbon_Button_With_No_Save_Dialog(int WaitForSave = 2000)
        {
            Assert.IsTrue(MainStudioWindow.SideMenuBar.SaveButton.Exists, "Save ribbon button does not exist");
            Mouse.Click(MainStudioWindow.SideMenuBar.SaveButton, new Point(10, 5));
        }

        public void Enter_Recordset_values()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text = "[[rec().a]]";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.ValueCell.IntellisenseCombobox.Textbox.Text = "5";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row2.VariableCell.IntellisenseCombobox.Textbox.Text = "[[rec().b]]";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row2.ValueCell.IntellisenseCombobox.Textbox.Text = "10";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row3.VariableCell.IntellisenseCombobox.Textbox.Text = "[[var]]";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row3.ValueCell.IntellisenseCombobox.Textbox.Text = "1";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row4.VariableCell.IntellisenseCombobox.Textbox.Text = "[[mr()]]";
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.Exists, "var does not exist in the variable explorer");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.RecordsetTreeItem.TreeItem1.Field1.Exists, "rec().a does not exist in the variable explorer");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.RecordsetTreeItem.TreeItem1.Field2.Exists, "rec().b does not exist in the variable explorer");
        }

        public void Enter_Text_Into_Debug_Input_Row1_Value_Textbox_With_Special_Test_For_Textbox_Height(string text)
        {
            var varValue = MainStudioWindow.DebugInputDialog.TabItemsTabList.InputDataTab.InputsTable.Row1.InputValueCell.InputValueComboboxl.InputValueText;

            var heightBeforeEnterClick = varValue.Height;
            varValue.Text = text;
            Keyboard.SendKeys(varValue, "{Enter}", ModifierKeys.None);
            Assert.IsTrue(varValue.Height > heightBeforeEnterClick, "Debug input dialog does not resize after adding second line.");

            Keyboard.SendKeys(varValue, "{Back}", ModifierKeys.None);
            Assert.AreEqual(heightBeforeEnterClick, varValue.Height, "Debug input dialog value textbox does not resize after deleting second line.");
        }

        public void Press_F5_To_Debug()
        {
            Keyboard.SendKeys(MainStudioWindow, "{F5}", ModifierKeys.None);
            Assert.IsTrue(MainStudioWindow.DebugInputDialog.Exists, "Debug Input window does not exist after pressing F5.");
            Assert.IsTrue(MainStudioWindow.DebugInputDialog.DebugF6Button.Exists, "Debug button in Debug Input window does not exist.");
            Assert.IsTrue(MainStudioWindow.DebugInputDialog.CancelButton.Exists, "Cancel Debug Input Window button does not exist.");
            Assert.IsTrue(MainStudioWindow.DebugInputDialog.RememberDebugInputCheckBox.Exists, "Remember Checkbox does not exist in the Debug Input window.");
            Assert.IsTrue(MainStudioWindow.DebugInputDialog.ViewInBrowserF7Button.Enabled, "View in Browser button does not exist in Debug Input window.");
            Assert.IsTrue(MainStudioWindow.DebugInputDialog.TabItemsTabList.InputDataTab.InputsTable.Exists, "Input Data Window does not exist in Debug Input window.");
            Assert.IsTrue(MainStudioWindow.DebugInputDialog.TabItemsTabList.XMLTab.Exists, "Xml tab does not Exist in the Debug Input window.");
            Assert.IsTrue(MainStudioWindow.DebugInputDialog.TabItemsTabList.JSONTab.Exists, "Assert Json tab does not exist in the debug input window.");
        }

        [Given(@"I Click New Web Source Test Connection Button")]
        [When(@"I Click New Web Source Test Connection Button")]
        [Then(@"I Click New Web Source Test Connection Button")]
        public void Click_NewWebSource_TestConnectionButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.WorkSurfaceContext.TestConnectionButton, new Point(52, 14));
        }

        public void Click_Service_Picker_Dialog_OK()
        {
            Mouse.Click(ServicePickerDialog.OK, new Point(52, 10));
        }

        public void Click_ServicePickerDialog_CancelButton()
        {
            Mouse.Click(ServicePickerDialog.Cancel, new Point(57, 6));
        }

        public void Click_Service_Picker_Dialog_Refresh_Button()
        {
            Mouse.Click(ServicePickerDialog.Explorer.Refresh, new Point(10, 11));
            WaitForSpinner(ServicePickerDialog.Explorer.ExplorerTree.Localhost.Checkbox.Spinner);
        }

        [Given("I Click Subworkflow Done Button")]
        [When("I Click Subworkflow Done Button")]
        [Then("I Click Subworkflow Done Button")]
        public void Click_Subworkflow_Done_Button()
        {
            Assert.IsTrue(
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView
                    .DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ExternalWorkFlow
                    .DoneButton.Exists, "Done button does not exist afer dragging dice service onto design surface");
            Mouse.Click(
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView
                    .DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ExternalWorkFlow
                    .DoneButton, new Point(53, 16));
        }

        public void Click_Assign_Tool_url()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.TopScrollViewerPane.UnsavedWorkflowLinkText.Hyperlink.Exists, "Url hyperlink does not exist");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.TopScrollViewerPane.UnsavedWorkflowLinkText.Hyperlink, new Point(201, 10));
            Assert.IsTrue(MessageBoxWindow.OKButton.Exists, "Did you know popup does not exist after clicking workflow hyperlink.");
            Mouse.Click(MessageBoxWindow.OKButton, new Point(38, 12));
        }

        public void Click_Assign_Tool_url_On_Unpinned_Tab()
        {
            Assert.IsTrue(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.TopScrollViewerPane.UnsavedWorkflowLinkText.Hyperlink.Exists, "Url hyperlink does not exist on unpinned tab.");
            Mouse.Click(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.TopScrollViewerPane.UnsavedWorkflowLinkText.Hyperlink, new Point(201, 10));
            Assert.IsTrue(MessageBoxWindow.OKButton.Exists, "Did you know popup does not exist after clicking workflow hyperlink on unpinned tab.");
            Mouse.Click(MessageBoxWindow.OKButton, new Point(38, 12));
        }

        public void Click_Assign_Tool_Remove_Variable_From_Tool()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.Exists, "Assign tool large view on the design surface does not exist");
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text = "[[SomeOtherVariable]]";
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox, "{Right}{Tab}", ModifierKeys.None);
            Assert.AreEqual("[[Some$Invalid%Variable]]", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text, "Multiassign small view row 1 variable textbox text does not equal \"[[Some$Invalid%Variable]]\".");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Exists, "Assign large view row 1 variable textbox does not exist");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.SearchTextbox.Exists, "Variable filter textbox does not exist");
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.SearchTextbox.Text = "Other";
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.SearchTextbox.ClearSearchButton.Exists, "Variable clear filter button does not exist");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.SearchTextbox.ClearSearchButton, new Point(8, 13));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.ScrollViewerPane.NameTextbox.DeleteButton.Exists, "Variable delete does not exist");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.ScrollViewerPane.NameTextbox.DeleteButton, new Point(9, 8));
        }

        [Given(@"I Refresh Explorer")]
        [When(@"I Refresh Explorer")]
        [Then(@"I Refresh Explorer")]
        public void Click_Explorer_Refresh_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerRefreshButton, new Point(10, 10));
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.Spinner);
        }

        [When(@"I Refresh Explorer Withpout Waiting For Spinner")]
        public void RefreshExplorerWithpoutWaitingForSpinner()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerRefreshButton, new Point(10, 10));
        }

        [Given(@"I Click Close Clean Workflow Tab")]
        [When(@"I Click Close Clean Workflow Tab")]
        [Then(@"I Click Close Clean Workflow Tab")]
        public void ThenIClickCloseCleanWorkflowTab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.CloseButton);
        }

        public void TryRemoveTests()
        {
            WpfList testsListBox = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList;
            if (testsListBox.GetContent().Length >= 6)
            {
                Select_Test(3);
                Point point;
                if (MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test5.TestEnabledSelector.Checked && MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test5.TestEnabledSelector.TryGetClickablePoint(out point))
                    Click_EnableDisable_This_Test_CheckBox(true, 5);
                Click_Delete_Test_Button(5);
                Click_MessageBox_Yes();
            }
            if (testsListBox.GetContent().Length >= 5)
            {
                Select_Test(3);
                Point point;
                if (MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test4.TestEnabledSelector.Checked && MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test4.TestEnabledSelector.TryGetClickablePoint(out point))
                    Click_EnableDisable_This_Test_CheckBox(true, 4);
                Click_Delete_Test_Button(4);
                Click_MessageBox_Yes();
            }
            if (testsListBox.GetContent().Length >= 4)
            {
                Select_Test(3);
                Point point;
                if (MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test3.TestEnabledSelector.Checked && MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test3.TestEnabledSelector.TryGetClickablePoint(out point))
                    Click_EnableDisable_This_Test_CheckBox(true, 3);
                Click_Delete_Test_Button(3);
                Click_MessageBox_Yes();
            }
            if (testsListBox.GetContent().Length >= 3)
            {
                Select_Test(2);
                Point point;
                if (MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test2.TestEnabledSelector.Checked && MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test2.TestEnabledSelector.TryGetClickablePoint(out point))
                    Click_EnableDisable_This_Test_CheckBox(true, 2);
                Click_Delete_Test_Button(2);
                Click_MessageBox_Yes();
            }
            if (testsListBox.GetContent().Length >= 2)
            {
                Select_Test();
                Point point;
                if (MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.TestEnabledSelector.Checked && MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.TestEnabledSelector.TryGetClickablePoint(out point))
                    Click_EnableDisable_This_Test_CheckBox(true);
                Click_Delete_Test_Button();
                Click_MessageBox_Yes();
            }
            Click_Close_Tests_Tab();
        }

        [Given(@"I Select Acceptance Test in delete")]
        [When(@"I Select Acceptance Test in delete")]
        [Then(@"I Select Acceptance Test in delete")]
        public void Select_AcceptanceTestin_From_DeleteListItemsTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDeleteListItem.SmallView.MethodList, new Point(119, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDeleteListItem.SmallView.MethodList.UIAcceptanceTesting_CrListItem, new Point(114, 13));
        }

        [Given(@"I Select AppData From MethodList")]
        [When(@"I Select AppData From MethodList")]
        [Then(@"I Select AppData From MethodList")]
        public void Select_AppData_From_MethodList_From_DeleteTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDeleteListItem.SmallView.MethodList, new Point(174, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDeleteListItem.SmallView.MethodList.UIAppdataListItem, new Point(43, 15));
        }

        [Given(@"I Select AppData From MethodList")]
        [When(@"I Select AppData From MethodList")]
        [Then(@"I Select AppData From MethodList")]
        public void Select_AppData_From_MethodList_From_UpdateTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUpdateListItem.SmallView.MethodList, new Point(174, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUpdateListItem.SmallView.MethodList.UIAppdataListItem, new Point(43, 15));
        }

        [Given(@"I Click View Tests In Explorer Context Menu for ""(.*)""")]
        [When(@"I Click View Tests In Explorer Context Menu for ""(.*)""")]
        [Then(@"I Click View Tests In Explorer Context Menu for ""(.*)""")]
        public void Click_View_Tests_In_Explorer_Context_Menu(string ServiceName)
        {
            Filter_Explorer(ServiceName);
            Show_ExplorerFirstItemTests_With_ExplorerContextMenu();
        }

        [Given(@"I Click View Tests In Explorer Context Menu for Sub Item ""(.*)""")]
        [When(@"I Click View Tests In Explorer Context Menu for Sub Item ""(.*)""")]
        [Then(@"I Click View Tests In Explorer Context Menu for Sub Item ""(.*)""")]
        public void Click_View_Tests_In_Explorer_Context_Menu_For_Sub_Item(string ServiceName)
        {
            Filter_Explorer(ServiceName);
            Show_ExplorerFirstSubItemTests_With_ExplorerContextMenu();
        }

        [Given(@"That The First Test ""(.*)"" Unsaved Star")]
        [When(@"The First Test ""(.*)"" Unsaved Star")]
        [Then(@"The First Test ""(.*)"" Unsaved Star")]
        public void Assert_Workflow_Testing_Tab_First_Test_Has_Unsaved_Star(string HasHasNot)
        {
            Assert_Workflow_Testing_Tab_First_Test_Has_Unsaved_Star(HasHasNot == "Has");
        }

        [Given(@"The Added Test ""(.*)"" Unsaved Star")]
        [When(@"The Added Test ""(.*)"" Unsaved Star")]
        [Then(@"The Added Test ""(.*)"" Unsaved Star")]
        public void ThenTheAddedTestUnsavedStar(string p0)
        {
            Assert_Workflow_Testing_Tab_Added_Test_Has_Unsaved_Star(p0 == "Has");
        }

        [Given(@"I delete Second Added Test")]
        [When(@"I delete Second Added Test")]
        [Then(@"I delete Second Added Test")]
        public void ThenIDeleteSecondAddedTest()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test5.TestEnabledSelector, new Point(10, 10));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test5.DeleteButton, new Point(10, 10));
            Click_MessageBox_Yes();
        }

        [Given(@"I delete Added Test")]
        [When(@"I delete Added Test")]
        [Then(@"I delete Added Test")]
        public void ThenIDeleteAddedTest()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test4.DeleteButton, new Point(10, 10));
            Click_MessageBox_Yes();
        }

        [Given(@"That The Added Test ""(.*)"" Unsaved Star")]
        [When(@"That The Added Test ""(.*)"" Unsaved Star")]
        [Then(@"The Added ""(.*)"" Unsaved Star")]
        public void Assert_Workflow_Testing_Tab_Added_Test_Has_Unsaved_Star(string HasHasNot)
        {
            Assert_Workflow_Testing_Tab_Added_Test_Has_Unsaved_Star(HasHasNot == "Has");
        }

        public void Assert_Workflow_Testing_Tab_First_Test_Has_Unsaved_Star(bool HasStar)
        {
            Assert.AreEqual(HasStar, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.TabDescription.DisplayText.Contains("*"), "Test tab title does not contain unsaved star.");
            Assert.AreEqual(HasStar, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test4.TestNameDisplay.DisplayText.Contains("*"), "First test title does not contain unsaved star.");
        }
        public void Assert_Workflow_Testing_Tab_Added_Test_Has_Unsaved_Star(bool HasStar)
        {
            Assert.AreEqual(HasStar, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.TabDescription.DisplayText.Contains("*"), "Test tab title does not contain unsaved star.");
        }

        public void Assert_Test_Has_Unsaved_Star(string test, bool HasStar)
        {
            if (test == "1st")
                Assert.AreEqual(HasStar, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.TestNameDisplay.DisplayText.Contains("*"), "1st test title does not contain unsaved star.");
            if (test == "2nd")
                Assert.AreEqual(HasStar, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test2.TestNameDisplay.DisplayText.Contains("*"), "2nd test title does not contain unsaved star.");
            if (test == "3rd")
                Assert.AreEqual(HasStar, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test3.TestNameDisplay.DisplayText.Contains("*"), "3rd test title does not contain unsaved star.");
            if (test == "4th")
                Assert.AreEqual(HasStar, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test4.TestNameDisplay.DisplayText.Contains("*"), "4th test title does not contain unsaved star.");
        }

        [Given(@"That The Second Test ""(.*)"" Unsaved Star")]
        [When(@"The Second Test ""(.*)"" Unsaved Star")]
        [Then(@"The Second Test ""(.*)"" Unsaved Star")]
        public void Assert_Workflow_Testing_Tab_Second_Test_Has_Unsaved_Star(string HasHasNot)
        {
            Assert.AreEqual((HasHasNot == "Has"), MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.TabDescription.DisplayText.Contains("*"), "Test tab title does not contain unsaved star.");
            Assert.AreEqual((HasHasNot == "Has"), MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test5.TestNameDisplay.DisplayText.Contains("*"), "Second test title does not contain unsaved star.");
        }
        [Given(@"That The Second Added Test ""(.*)"" Unsaved Star")]
        [When(@"The Second Added Test ""(.*)"" Unsaved Star")]
        [Then(@"The Second Added Test ""(.*)"" Unsaved Star")]
        public void Assert_Workflow_Testing_Tab_Second_Added_Test_Has_Unsaved_Star(string HasHasNot)
        {
            Assert.AreEqual((HasHasNot == "Has"), MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.TabDescription.DisplayText.Contains("*"), "Test tab title does not contain unsaved star.");
            Assert.AreEqual((HasHasNot == "Has"), MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test5.TestNameDisplay.DisplayText.Contains("*"), "Second Added test title does not contain unsaved star.");
        }

        [Given(@"I Click The Create a New Test Button")]
        [When(@"I Click The Create a New Test Button")]
        [Then(@"I Click The Create a New Test Button")]
        public void Click_Workflow_Testing_Tab_Create_New_Test_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.CreateTest.CreateTestButton, new Point(158, 10));
        }

        [Given("The First Test Exists")]
        [When("The First Test Exists")]
        [Then("The First Test Exists")]
        public void Assert_Workflow_Testing_Tab_First_Test_Exists()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.Exists, "No first test on workflow testing tab.");
        }

        [Given("The Added Test Exists")]
        [When("The Added Test Exists")]
        [Then("The Added Test Exists")]
        public void Assert_Workflow_Testing_Tab_Added_Test_Exists()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test4.Exists, "No 4th test on workflow testing tab.");
        }

        [Given(@"The ""(.*)"" Added Test Exists")]
        [When(@"The ""(.*)"" Added Test Exists")]
        [Then(@"The ""(.*)"" Added Test Exists")]
        public void ThenTheAddedTestExists(string test)
        {
            if (test == "1st")
                Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.Exists, "No 1st test on workflow testing tab.");
            if (test == "2nd")
                Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test2.Exists, "No 2nd test on workflow testing tab.");
            if (test == "3rd")
                Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test3.Exists, "No 3rd test on workflow testing tab.");
            if (test == "4th")
                Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test4.Exists, "No 4th test on workflow testing tab.");
        }

        [Given(@"The ""(.*)"" Added Test ""(.*)"" Unsaved Star")]
        [When(@"The ""(.*)"" Added Test ""(.*)"" Unsaved Star")]
        [Then(@"The ""(.*)"" Added Test ""(.*)"" Unsaved Star")]
        public void TheAddedTestUnsavedStar(string test, string star)
        {
            Assert_Workflow_Testing_Tab_Added_Test_Has_Unsaved_Star(star == "Has");
            Assert_Test_Has_Unsaved_Star(test, star == "Has");
        }


        [Given("The Second Test Exists")]
        [When("The Second Test Exists")]
        [Then("The Second Test Exists")]
        public void Assert_Workflow_Testing_Tab_Second_Test_Exists()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test2.Exists, "No 2nd test on workflow testing tab.");
        }

        [Given("The Second Added Test Exists")]
        [When("The Second Added Test Exists")]
        [Then("The Second Added Test Exists")]
        public void Assert_Workflow_Testing_Tab_Second_Added_Test_Exists()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test5.Exists, "No 5th Added test on workflow testing tab.");
        }

        [Given("I Toggle First Test Enabled")]
        [When("I Toggle First Test Enabled")]
        [Then("I Toggle First Test Enabled")]
        public void Toggle_Workflow_Testing_Tab_First_Test_Enabled()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.TestEnabledSelector, new Point(10, 10));
        }

        [Given("I Toggle First Added Test Enabled")]
        [Then("I Toggle First Added Test Enabled")]
        [When("I Toggle First Added Test Enabled")]
        public void Toggle_Workflow_Testing_Tab_First_Added_Test_Enabled()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test4.TestEnabledSelector, new Point(10, 10));
        }

        [Given(@"I Toggle ""(.*)"" Added Test Enabled")]
        [When(@"I Toggle ""(.*)"" Added Test Enabled")]
        [Then(@"I Toggle ""(.*)"" Added Test Enabled")]
        public void WhenIToggleAddedTestEnabled(string test)
        {
            if (test == "1st")
                Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.TestEnabledSelector, new Point(10, 10));
            if (test == "2nd")
                Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test2.TestEnabledSelector, new Point(10, 10));
            if (test == "3rd")
                Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test3.TestEnabledSelector, new Point(10, 10));
            if (test == "4th")
                Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test4.TestEnabledSelector, new Point(10, 10));
        }

        [Given("I Click Test (.*) Run Button")]
        [When("I Click Test (.*) Run Button")]
        [Then("I Click Test (.*) Run Button")]
        public void Click_Test_Run_Button(int index)
        {
            switch (index)
            {
                default:
                    Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.RunButton, new Point(10, 10));
                    break;
                case 2:
                    Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test2.RunButton, new Point(10, 10));
                    break;
                case 3:
                    Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test3.RunButton, new Point(10, 10));
                    break;
                case 4:
                    Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test4.RunButton, new Point(10, 10));
                    break;
            }
        }
        [Then(@"I delete Test(.*) as a Cleanup step")]
        public void ThenIDeleteTestAsACleanupStep(int p0)
        {
            Click_EnableDisable_This_Test_CheckBox(true, 4);
            Click_Delete_Test_Button(4);
            Click_MessageBox_Yes();
            Click_Close_Tests_Tab();
        }

        [Given("I Click First Test Delete Button")]
        [When("I Click First Test Delete Button")]
        [Then("I Click First Test Delete Button")]
        public void Click_First_Test_Delete_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.DeleteButton, new Point(10, 10));
        }

        [Given(@"I Click First Test Run Button")]
        [When(@"I Click First Test Run Button")]
        [Then(@"I Click First Test Run Button")]
        public void Click_First_Test_Run_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.RunButton, new Point(10, 10));
        }

        public void Select_First_Test()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1, new Point(80, 10));
        }

        public void Click_Sharepoint_RefreshButton_From_SharepointUpdate()
        {
            var refreshButton = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUpdateListItem.SmallView.RefreshButton;
            Mouse.Click(refreshButton);
        }

        public void Click_Sharepoint_RefreshButton()
        {
            var refreshButton = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCreateListItem.SmallView.RefreshButton;
            Mouse.Click(refreshButton);
        }
        public void Click_Sharepoint_RefreshButton_From_SharepointRead()
        {
            var refreshButton = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadListItem.SmallView.RefreshButton;
            Mouse.Click(refreshButton);
        }

        [Given(@"I wait for output spinner")]
        [When(@"I wait for output spinner")]
        [Then(@"I wait for output spinner")]
        public void WhenIWaitForOutputSpinner()
        {
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.StatusBar.Spinner);
        }

        [Given("I Click Run All Button")]
        [When("I Click Run All Button")]
        [Then("I Click Run All Button")]
        public void Click_Workflow_Testing_Tab_Run_All_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.RunAllButton, new Point(35, 10));
        }

        [Given(@"That The First Test ""(.*)"" Passing")]
        [When(@"The First Test ""(.*)"" Passing")]
        [Then(@"The First Test ""(.*)"" Passing")]
        public void Assert_Workflow_Testing_Tab_First_Test_Is_Passing(string IsIsNot)
        {
            Assert_Workflow_Testing_Tab_First_Test_Is_Passing(IsIsNot == "Is");
        }

        [Given(@"That The Second Test ""(.*)"" Passing")]
        [When(@"The Second Test ""(.*)"" Passing")]
        [Then(@"The Second Test ""(.*)"" Passing")]
        public void Assert_Workflow_Testing_Tab_Second_Test_Is_Passing(string IsIsNot)
        {
            Assert_Workflow_Testing_Tab_Second_Test_Is_Passing(IsIsNot == "Is");
        }

        [Given(@"That The Third Test ""(.*)"" Passing")]
        [When(@"The Third Test ""(.*)"" Passing")]
        [Then(@"The Third Test ""(.*)"" Passing")]
        public void Assert_Workflow_Testing_Tab_Third_Test_Is_Passing(string IsIsNot)
        {
            Assert_Workflow_Testing_Tab_Third_Test_Is_Passing(IsIsNot == "Is");
        }

        public void Assert_Workflow_Testing_Tab_First_Test_Is_Passing(bool passing = true)
        {
            Assert.AreEqual(passing, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.Passing.Exists, (passing ? "First test is not passing." : "First test is passing."));
        }

        public void Assert_Workflow_Testing_Tab_Second_Test_Is_Passing(bool passing = true)
        {
            Assert.AreEqual(passing, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test2.Passing.Exists, (passing ? "Second test is not passing." : "Second test is passing."));
        }

        public void Assert_Workflow_Testing_Tab_Third_Test_Is_Passing(bool passing = true)
        {
            Assert.AreEqual(passing, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test3.Passing.Exists, (passing ? "Third test is not passing." : "Third test is passing."));
        }

        [Given(@"That The First Test ""(.*)"" Invalid")]
        [When(@"The First Test ""(.*)"" Invalid")]
        [Then(@"The First Test ""(.*)"" Invalid")]
        public void Assert_Workflow_Testing_Tab_First_Test_Is_Invalid(string IsIsNot)
        {
            Assert_Workflow_Testing_Tab_First_Test_Is_Invalid(IsIsNot == "Is");
        }

        public void Assert_Workflow_Testing_Tab_First_Test_Is_Invalid(bool invalid = true)
        {
            Assert.AreEqual(invalid, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.Test1.Invalid.Exists, (invalid ? "First test is not invalid." : "First test is invalid."));
        }

        public void Delete_Assign_With_Context_Menu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign, MouseButtons.Right, ModifierKeys.None, new Point(115, 10));
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.Delete, new Point(27, 18));
            Assert.IsFalse(ControlExistsNow(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView), "Assign tool still exists on design surface after deleting with context menu.");
        }
        public void Delete_HelloWorld_With_Context_Menu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.HelloWorldWorkFlow, MouseButtons.Right, ModifierKeys.None, new Point(115, 10));
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.Delete, new Point(27, 18));
        }

        public void Delete_Assign_With_Context_Menu_On_Unpinned_Tab()
        {
            Mouse.Click(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign, MouseButtons.Right, ModifierKeys.None, new Point(115, 10));
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.Delete, new Point(27, 18));
            Assert.IsFalse(ControlExistsNow(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView), "Assign tool still exists on unpinned design surface after deleting with context menu.");
        }

        public void Debug_Workflow_With_Ribbon_Button()
        {
            Click_Debug_RibbonButton();
            Click_DebugInput_Debug_Button();
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.StatusBar.Spinner);
        }

        public void Debug_Unpinned_Workflow_With_F6()
        {
            Press_F6();
            WaitForSpinner(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.SplitPaneRight.DebugOutput.StatusBar.Spinner);
        }

        public void Remove_Assign_Row_1_With_Context_Menu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.RowNumberCell.Text, MouseButtons.Right, ModifierKeys.None, new Point(5, 5));
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.DeleteRowMenuItem, MouseButtons.Left, ModifierKeys.None, new Point(6, 6));
            Assert.IsFalse(ControlExistsNow(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row3), "Assign tool row 3 still exists after deleting row 1.");
        }

        public void Remove_Assign_Row_1_With_Context_Menu_On_Unpinned_Tab()
        {
            MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.RowNumberCell.DrawHighlight();
            Mouse.Click(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.RowNumberCell.Text, MouseButtons.Right, ModifierKeys.None, new Point(5, 5));
            StartNodePopupWindow.DesignSurfaceMenu.DeleteRowMenuItem.DrawHighlight();
            Mouse.Click(StartNodePopupWindow.DesignSurfaceMenu.DeleteRowMenuItem, MouseButtons.Left, ModifierKeys.None, new Point(6, 6));
            Assert.IsFalse(ControlExistsNow(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row3), "Assign tool row 3 still exists after deleting row 1 on unpinned tab.");
        }

        [Given(@"I Enter variable text as ""(.*)"" and value text as ""(.*)"" into assign row 1")]
        [When(@"I Enter variable text as ""(.*)"" and value text as ""(.*)"" into assign row 1")]
        [Then(@"I Enter variable text as ""(.*)"" and value text as ""(.*)"" into assign row 1")]
        public void Enter_Variable_And_Value_Into_Assign(string VariableText, string ValueText)
        {
            Enter_Variable_And_Value_Into_Assign(VariableText, ValueText, 1);
        }

        public void Enter_Variable_And_Value_Into_Assign(string VariableText, string ValueText, int RowNumber)
        {
            switch (RowNumber)
            {
                case 2:
                    Assign_Value_To_Variable_With_Assign_Tool_Small_View_Row_2();
                    Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row3.Exists, "Assign row 3 does not exist after enter data into row 2.");
                    break;
                default:
                    Assign_Value_To_Variable_With_Assign_Tool_Small_View_Row_1();
                    Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row2.Exists, "Assign row 2 does not exist after enter data into row 1.");
                    break;
            }
        }

        [Given(@"I Enter variable text as ""(.*)"" and value text as ""(.*)"" into assign row 1 on unpinned tab")]
        [When(@"I Enter variable text as ""(.*)"" and value text as ""(.*)"" into assign row 1 on unpinned tab")]
        [Then(@"I Enter variable text as ""(.*)"" and value text as ""(.*)"" into assign row 1 on unpinned tab")]
        public void Enter_Variable_And_Value_Into_Assign_On_Unpinned_Tab(string VariableText, string ValueText, int RowNumber)
        {
            switch (RowNumber)
            {
                case 2:
                    Assign_Value_To_Variable_With_Assign_Tool_Small_View_Row_2_On_Unpinned_tab();
                    Assert.IsTrue(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row3.Exists, "Assign row 3 does not exist after enter data into row 2 on unpinned tab.");
                    break;
                default:
                    Assign_Value_To_Variable_With_Assign_Tool_Small_View_Row_1_On_Unpinned_tab();
                    Assert.IsTrue(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row2.Exists, "Assign row 2 does not exist after enter data into row 1 on unpinned tab.");
                    break;
            }
        }

        [When(@"I Enter variable text as ""(.*)"" into assign row 1 on unpinned tab")]
        public void Enter_Variable_Into_Assign_Row1_On_Unpinned_Tab(string VariableText)
        {
            Assert.IsTrue(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.Exists);
            MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text = VariableText;
            Assert.IsTrue(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row2.Exists, "Assign row 2 does not exist after enter data into row 1 on unpinned tab.");
        }

        [When(@"I Enter variable text as ""(.*)"" into assign row 2 on unpinned tab")]
        public void Enter_Variable_Into_Assign_Row2_On_Unpinned_Tab(string VariableText)
        {
            MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row2.VariableCell.IntellisenseCombobox.Textbox.Text = VariableText;
            Assert.IsTrue(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row3.Exists, "Assign row 3 does not exist after enter data into row 2 on unpinned tab.");
        }

        public void Click_Explorer_RemoteServer_Edit_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.EditServerButton, new Point(11, 10));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.Exists, "Server Source Tab was not open.");
        }

        public void Enter_Text_Into_Assign_QviLarge_View()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.QuickVariableInputContent.QviVariableListBoxEdit.Text = "varOne,varTwo,varThree";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.QuickVariableInputContent.QviSplitOnCharacterEdit.Text = ",";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.QuickVariableInputContent.PrefixEdit.Text = "some(<).";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.QuickVariableInputContent.SuffixEdit.Text = "_suf";
        }

        public void Enter_Text_Into_EmailSource_Tab()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.EmailSourceTab.SendTestModelsCustom.HostTextBoxEdit.Text = "localhost";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.EmailSourceTab.SendTestModelsCustom.UserNameTextBoxEdit.Text = "test";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.EmailSourceTab.SendTestModelsCustom.PasswordTextBoxEdit.Text = "test";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.EmailSourceTab.SendTestModelsCustom.PortTextBoxEdit.Text = "2";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.EmailSourceTab.SendTestModelsCustom.FromTextBoxEdit.Text = "AThorLocal@norsegods.com";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.EmailSourceTab.SendTestModelsCustom.ToTextBoxEdit.Text = "dev2warewolf@gmail.com";
        }

        public void Edit_Timeout_On_EmailSource()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.EmailSourceTab.SendTestModelsCustom.TimeoutTextBoxEdit.Text = "2000";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.EmailSourceTab.SendTestModelsCustom.FromTextBoxEdit.Text = "AThorLocal@norsegods.com";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.EmailSourceTab.SendTestModelsCustom.ToTextBoxEdit.Text = "dev2warewolf@gmail.com";
        }

        public void Edit_Timeout_On_ExchangeSource()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ExchangeSourceTab.SendTestModelsCustom.TimeoutTextBoxEdit.Text = "2000";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ExchangeSourceTab.SendTestModelsCustom.ToTextBox.Text = "dev2warewolf@gmail.com";
        }

        public void Pin_Unpinned_Pane_To_Default_Position()
        {
            Mouse.StartDragging(MainStudioWindow.UnpinnedTab, new Point(5, 5));
            Mouse.StopDragging(MainStudioWindow.UnpinnedTab);
        }
        public void Unpin_Tab_With_Drag(UITestControl Tab)
        {
            Mouse.StartDragging(Tab);
            Mouse.StopDragging(0, 21);
            Playback.Wait(2000);
        }

        public void Enter_Recordset_On_Length_tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Length.SmallViewContentCustom.RecordsetComboBox.TextEdit.Text = "[[rec()]]";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Length.SmallViewContentCustom.ResultComboBox.TextEdit.Text = "[[result]]";
        }

        public void Enter_Recordset_On_SortRecorsds_tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SortRecords.SmallViewContentCustom.SortFieldComboBox.TextEdit.Text = "[[rec().a]]";
        }
        public void Enter_Recordset_On_UniqueRecorsds_tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Unique.SmallViewContentCustom.InFieldsComboBox.TextEdit.Text = "[[rec().a]],[[rec().b]],[[rec().c]]";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Unique.SmallViewContentCustom.ReturnFieldsComboBox.TextEdit.Text = "[[rec().b]]";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Unique.SmallViewContentCustom.ResultsComboBox.TextEdit.Text = "[[rec().c]]";

        }

        public void Enter_Recordset_On_CountRecordset_tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CountRecordset.SmallViewContentCustom.RecorsetComboBox.TextEdit.Text = "[[Recordset()]]";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CountRecordset.SmallViewContentCustom.ResultComboBox.TextEdit.Text = "[[Result]]";
        }

        public void Enter_Person_Age_On_Assign_Object_tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.AssignObject.LargeView.DataGrid.Row2.FieldCell.FieldNameComboBox.TextEdit.Text = "[[@Person.Age]]";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.AssignObject.LargeView.DataGrid.Row2.FieldValueCell.FieldValueComboBox.TextEdit.Text = "10";
        }

        public void Check_Debug_Input_Dialog_Remember_Inputs_Checkbox()
        {
            MainStudioWindow.DebugInputDialog.RememberDebugInputCheckBox.Checked = true;
        }

        public void Enter_Person_Name_On_Assign_Object_tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.AssignObject.LargeView.DataGrid.Row1.FieldCell.FieldNameComboBox.TextEdit.Text = "[[@Person.Name]]";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.AssignObject.LargeView.DataGrid.Row1.FieldValueCell.FieldValueComboBox.TextEdit.Text = "Bob";
        }

        public void Enter_Values_Into_Case_Conversion_Tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CaseConvert.SmallViewContentCustom.SmallDataGridTable.Row1.ValueCell.ValueComboBox.TextEdit.Text = "res";
        }

        public void Connect_Assign_to_Next_tool()
        {
            Mouse.Hover(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(200, 220));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(300, 220));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, 0, 44);
        }

        public void Enter_Values_Into_Replace_Tool_Large_View()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Replace.SmallViewContentCustom.InFiledsComboBox.TextEdit.Text = "[[rec().a]]";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Replace.SmallViewContentCustom.FindComboBox.TextEdit.Text = "u";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Replace.SmallViewContentCustom.ReplaceComboBox.TextEdit.Text = "o";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Replace.SmallViewContentCustom.ResultComboBox.TextEdit.Text = "res";
        }

        public void Enter_Values_Into_FindIndex_Tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindIndex.SmallViewContentCustom.InFieldComboBox.TextEdit.Text = "SomeLongString";
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindIndex.SmallViewContentCustom.IndexComboBox, new Point(85, 13));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindIndex.SmallViewContentCustom.IndexComboBox, new Point(62, 19));
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindIndex.SmallViewContentCustom.CharactersComboBox.TextEdit.Text = "r";
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindIndex.SmallViewContentCustom.CharactersComboBox.TextEdit, new Point(45, 2));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindIndex.SmallViewContentCustom.CharactersComboBox.TextEdit, new Point(39, 12));
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindIndex.SmallViewContentCustom.CharactersComboBox.TextEdit, "{Escape}", ModifierKeys.None);
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindIndex.SmallViewContentCustom.ResultComboBox.TextEdit.Text = "res";
        }

        public void Enter_Text_Into_Copy_Tool()
        {
            var resourcesFolder = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests";
            var resourcesFolderCopy = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests_Copy";

            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathCopy.LargeViewContentCustom.FileOrFolderComboBox.TextEdit.Text = resourcesFolder;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathCopy.LargeViewContentCustom.DestinationComboBox.TextEdit.Text = resourcesFolderCopy;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathCopy.LargeViewContentCustom.ResultComboBox.TextEdit.Text = "[[results]]";
        }
        public void Enter_Text_Into_CommentTool(string comment)
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Comment.LargeViewContentCustom.CommentComboBox.TextEdit.Text = comment;
        }

        public void Enter_Text_Into_Delete_Tool()
        {
            var resourcesFolder = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests_Copy";

            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathDelete.LargeViewContentCustom.FileOrFolderComboBox.TextEdit.Text = resourcesFolder;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathDelete.LargeViewContentCustom.ResultComboBox.TextEdit.Text = "[[results]]";
        }

        public void Enter_Text_Into_Write_Tool()
        {
            var file = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests\Test File.txt";

            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FileWrite.LargeViewContentCustom.FileNameComboBox.TextEdit.Text = file;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FileWrite.LargeViewContentCustom.ContentsComboBox.TextEdit.Text = "Some Content";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FileWrite.LargeViewContentCustom.OverwriteRadioButton.Selected = true;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FileWrite.LargeViewContentCustom.ResultComboBox.TextEdit.Text = "[[results]]";
        }

        public void Enter_Text_Into_Move_Tool()
        {
            var resourcesFolder = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests_Copy";
            var destinationFolder = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests";

            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathMove.LargeViewContentCustom.FileOrFolderComboBox.TextEdit.Text = resourcesFolder;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathMove.LargeViewContentCustom.DestinationComboBox.TextEdit.Text = destinationFolder;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathMove.LargeViewContentCustom.OverwriteCheckBox.Checked = true;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathMove.LargeViewContentCustom.ResultComboBox.TextEdit.Text = "[[results]]";
        }

        public void Enter_Text_Into_Zip_Tool()
        {
            var resourcesFolder = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests";
            var destinationFolder = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests_Zip";

            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Zip.LargeViewContentCustom.FileOrFolderComboBox.TextEdit.Text = resourcesFolder;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Zip.LargeViewContentCustom.DestinationComboBox.TextEdit.Text = destinationFolder;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Zip.LargeViewContentCustom.OverwriteCheckBox.Checked = true;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Zip.LargeViewContentCustom.ResultComboBox.TextEdit.Text = "[[results]]";
        }

        public void Enter_Text_Into_UnZip_Tool()
        {
            var resourcesFolder = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests";
            var unZipFolder = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests_UnZip";

            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.UnZip.LargeViewContentCustom.UnZipNameComboBox.TextEdit.Text = resourcesFolder;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.UnZip.LargeViewContentCustom.DestinationComboBox.TextEdit.Text = unZipFolder;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.UnZip.LargeViewContentCustom.OverwriteCheckBox.Checked = true;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.UnZip.LargeViewContentCustom.ResultComboBox.TextEdit.Text = "[[results]]";
        }

        public void Enter_Text_Into_Rename_Tool()
        {
            var resourcesFolder = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests";

            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathRename.LargeViewContentCustom.FileOrFolderComboBox.TextEdit.Text = resourcesFolder;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathRename.LargeViewContentCustom.NewNameComboBox.TextEdit.Text = "Acceptance Tests_New";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathRename.LargeViewContentCustom.OverwriteCheckBox.Checked = true;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathRename.LargeViewContentCustom.ResultComboBox.TextEdit.Text = "[[results]]";
        }

        public void Enter_Text_Into_ReadFolder_Tool()
        {
            var resourcesFolder = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests";

            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FolderRead.LargeViewContentCustom.DirectoryComboBox.TextEdit.Text = resourcesFolder;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FolderRead.LargeViewContentCustom.FilesFoldersRadioButton.Selected = true;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FolderRead.LargeViewContentCustom.ResultComboBox.TextEdit.Text = "[[results]]";
        }

        public void Enter_Text_Into_Read_Tool()
        {
            var resourcesFolder = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests";
            var file = resourcesFolder + @"\" + "Hello World" + ".xml";

            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FileRead.LargeViewContentCustom.FileNameComboBox.TextEdit.Text = file;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FileRead.LargeViewContentCustom.ResultComboBox.TextEdit.Text = "[[results]]";
        }

        public void Enter_Text_Into_Create_Tool()
        {
            var resourcesFolder = Environment.ExpandEnvironmentVariables("%programdata%") + @"\Warewolf\Resources\Acceptance Tests_Create";

            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathCreate.LargeViewContentCustom.FileNameoComboBox.TextEdit.Text = resourcesFolder;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathCreate.LargeViewContentCustom.OverwriteCheckBox.Checked = true;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathCreate.LargeViewContentCustom.ResultComboBox.TextEdit.Text = "[[results]]";
        }

        public void Enter_Values_Into_Data_Split_Tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataSplit.SmallViewContentCustom.SourceStringComboBox.TextEdit.Text = "some long string here";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataSplit.SmallViewContentCustom.SmallDataGridTable.Row1.ValueCell.ValueComboBox.TextEdit.Text = "[[res]]";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataSplit.SmallViewContentCustom.SmallDataGridTable.Row1.AtIndexCell.AtIndexComboBox.TextEdit.Text = "5";
        }

        public void Drag_Toolbox_Switch_Onto_Foreach()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Switch";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ForEach.SmallView.DropActivityHereCustom.EnsureClickable(new Point(155, 22));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FlowTools.Switch, new Point(13, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ForEach.SmallView.DropActivityHereCustom);
        }

        public void Drag_Toolbox_Decision_Onto_Foreach()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Decision";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ForEach.SmallView.DropActivityHereCustom.EnsureClickable(new Point(155, 22));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FlowTools.Decision, new Point(13, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ForEach.SmallView.DropActivityHereCustom);
        }

        public void Drag_Toolbox_AssignObject_Onto_Foreach()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Assign Object";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ForEach.SmallView.DropActivityHereCustom.EnsureClickable(new Point(155, 22));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.AssignObject, new Point(13, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ForEach.SmallView.DropActivityHereCustom);
        }

        [Given(@"I Click User Button On Sharepoint Source")]
        [When(@"I Click User Button On Sharepoint Source")]
        [Then(@"I Click User Button On Sharepoint Source")]
        public void Click_UserButton_On_SharepointSource()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SharepointServerSourceTab.SharepointServerSourceView.SharepointView.UserRadioButton.Selected = true;
        }

        [Given(@"I Click Windows Button On Sharepoint Source")]
        [When(@"I Click Windows Button On Sharepoint Source")]
        [Then(@"I Click Windows Button On Sharepoint Source")]
        public void Click_WindowsButton_On_SharepointSource()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SharepointServerSourceTab.SharepointServerSourceView.SharepointView.WindowsRadioButton.Selected = true;
        }

        [Given(@"I Click UserButton On Database Source")]
        [When(@"I Click UserButton On Database Source")]
        [Then(@"I Click UserButton On Database Source")]
        public void Click_UserButton_On_DatabaseSource()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.UserRadioButton);
        }

        [Given(@"I Click WindowsButton On Database Source")]
        [When(@"I Click WindowsButton On Database Source")]
        [Then(@"I Click WindowsButton On Database Source")]
        public void Click_WindowsButton_On_DatabaseSource()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.WindowsRadioButton);
        }

        [Given(@"I Enter TestUser Username And Password on Database source")]
        [When(@"I Enter TestUser Username And Password on Database source")]
        [Then(@"I Enter TestUser Username And Password on Database source")]
        public void IEnterRunAsUserTestUserOnDatabaseSource()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.UserNameTextBox.Text = "testuser";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.PasswordTextBox.Text = "test123";
        }

        public void Drag_Toolbox_AssignObject_Onto_Sequence_LargeTool()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Assign Object";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence.SequenceLargeView.AddModeNewActivity.EnsureClickable(new Point(155, 22));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.AssignObject, new Point(13, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence.SequenceLargeView.AddModeNewActivity.UISacdVerticalConnectoCustom);
        }
        public void Drag_Toolbox_AssignObject_Onto_Sequence_SmallTool()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Assign Object";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence.EnsureClickable(new Point(155, 22));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.AssignObject, new Point(13, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence);
        }

        public void Drag_Toolbox_Decision_Onto_Sequence_SmallTool()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Decision";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence.EnsureClickable(new Point(155, 22));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FlowTools.Decision, new Point(13, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence);
        }

        public void Drag_Toolbox_Switch_Onto_Sequence_SmallTool()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Switch";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence.EnsureClickable(new Point(155, 22));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FlowTools.Switch, new Point(13, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence);
        }

        public void Drag_Toolbox_Switch_Onto_Sequence_LargeTool()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Switch";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence.SequenceLargeView.AddModeNewActivity.EnsureClickable(new Point(155, 22));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FlowTools.Switch, new Point(13, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence.SequenceLargeView.AddModeNewActivity.UISacdVerticalConnectoCustom);
        }

        public void Drag_Toolbox_Decision_Onto_Sequence_LargeTool()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Decision";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence.SequenceLargeView.AddModeNewActivity.EnsureClickable(new Point(155, 22));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FlowTools.Decision, new Point(13, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence.SequenceLargeView.AddModeNewActivity.UISacdVerticalConnectoCustom);
        }

        [Given("Dice Is Selected InSettings Tab Permissions Row 1")]
        [When(@"I Assert Dice Is Selected InSettings Tab Permissions Row1")]
        [Then("Dice Is Selected InSettings Tab Permissions Row 1")]
        public void Assert_Dice_Is_Selected_InSettings_Tab_Permissions_Row_1()
        {
            Assert.AreEqual("Dice1", FindSelectedResourceText(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).DisplayText, "Resource Name is not set to Dice after selecting Dice from Service picker");
        }

        public void Enter_Dice_Roll_Values()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Random.SmallView.FromComboBox.FromTextEdit.Exists, "From textbox does not exist");
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Random.SmallView.FromComboBox.FromTextEdit.Text = "1";
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Random.SmallView.ToComboBox.ToTextEdit.Exists, "To textbox does not exist");
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Random.SmallView.ToComboBox.ToTextEdit.Text = "6";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Random.SmallView.ResultComboBox.TextEdit.Text = "[[out]]";
        }

        public void Drag_Toolbox_MultiAssign_Onto_Unpinned_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Assign";
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.MultiAssign, new Point(2, 10));
            Mouse.StopDragging(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 126));
            Assert.IsTrue(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.Exists, "MultiAssign does not exist on unpinned tab design surface after dragging from toolbox.");
        }

        public void Toggle_Between_Studio_and_Unpinned_Tab()
        {
            Keyboard.SendKeys(MainStudioWindow, "{ALT}{TAB}");
            Point point;
            Assert.IsFalse(MainStudioWindow.UnpinnedTab.TryGetClickablePoint(out point), "Unpinned pane still visible after Alt+TAB");
        }

        public void Debug_Using_Play_Icon()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.ExecuteIcon.DrawHighlight();
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.ExecuteIcon);
        }

        [Given(@"I Assign Value To Variable With Assign Tool Small View Row 1")]
        [When(@"I Assign Value To Variable With Assign Tool Small View Row 1")]
        [Then(@"I Assign Value To Variable With Assign Tool Small View Row 1")]
        public void Assign_Value_To_Variable_With_Assign_Tool_Small_View_Row_1()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text = "[[SomeVariable]]";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.ValueCell.IntellisenseCombobox.Textbox.Text = "50";
        }

        [Given(@"I Assign Value To Variable With Assign Tool Small View Row 1 On Unpinned tab")]
        [When(@"I Assign Value To Variable With Assign Tool Small View Row 1 On Unpinned tab")]
        [Then(@"I Assign Value To Variable With Assign Tool Small View Row 1 On Unpinned tab")]
        public void Assign_Value_To_Variable_With_Assign_Tool_Small_View_Row_1_On_Unpinned_tab()
        {
            MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text = "[[SomeVariable]]";
            MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.ValueCell.IntellisenseCombobox.Textbox.Text = "50";
        }

        [Given(@"I Assign Value To Variable With Assign Tool Small View Row 2")]
        [When(@"I Assign Value To Variable With Assign Tool Small View Row 2")]
        [Then(@"I Assign Value To Variable With Assign Tool Small View Row 2")]
        public void Assign_Value_To_Variable_With_Assign_Tool_Small_View_Row_2()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row2.VariableCell.IntellisenseCombobox.Textbox.Text = "[[SomeOtherVariable]]";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row2.ValueCell.IntellisenseCombobox.Textbox.Text = "100";
        }

        [Given(@"I Assign Value To Variable With Assign Tool Small View Row 2 On Unpinned tab")]
        [When(@"I Assign Value To Variable With Assign Tool Small View Row 2 On Unpinned tab")]
        [Then(@"I Assign Value To Variable With Assign Tool Small View Row 2 On Unpinned tab")]
        public void Assign_Value_To_Variable_With_Assign_Tool_Small_View_Row_2_On_Unpinned_tab()
        {
            MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row2.VariableCell.IntellisenseCombobox.Textbox.Text = "[[SomeOtherVariable]]";
            MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row2.ValueCell.IntellisenseCombobox.Textbox.Text = "100";
        }

        [Given(@"I Check Public Administrator")]
        [When(@"I Check Public Administrator")]
        [Then(@"I Check Public Administrator")]
        public void Check_Public_Administrator()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_AdministratorCell.Public_AdministratorCheckBox.Checked = true;
        }

        [Given(@"I Check Public Deploy To")]
        [When(@"I Check Public Deploy To")]
        [Then(@"I Check Public Deploy To")]
        public void Check_Public_Deploy_To()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_DeployToCell.Public_DeployToCheckBox.Checked = true;
        }

        [Given(@"I Check Public Deploy From")]
        [When(@"I Check Public Deploy From")]
        [Then(@"I Check Public Deploy From")]
        public void Check_Public_Deploy_From()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_DeployFromCell.Public_DeployFromCheckBox.Checked = true;
        }

        [Given(@"I Check Public View")]
        [When(@"I Check Public View")]
        [Then(@"I Check Public View")]
        public void Check_Public_View()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_ViewCell.Public_ViewCheckBox.Checked = true;
        }

        [Given(@"I Check Public Execute")]
        [When(@"I Check Public Execute")]
        [Then(@"I Check Public Execute")]
        public void Check_Public_Execute()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_ExecuteCell.Public_ExecuteCheckBox.Checked = true;
        }

        [Given(@"I Check Public Contribute")]
        [When(@"I Check Public Contribute")]
        [Then(@"I Check Public Contribute")]
        public void Check_Public_Contribute()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_ContributeCell.Public_ContributeCheckBox.Checked = true;
        }

        [Given(@"I Uncheck Public Administrator")]
        [When(@"I Uncheck Public Administrator")]
        [Then(@"I Uncheck Public Administrator")]
        public void Uncheck_Public_Administrator()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_AdministratorCell.Public_AdministratorCheckBox.Checked = false;
        }

        [Given(@"I Uncheck Public Deploy To")]
        [When(@"I Uncheck Public Deploy To")]
        [Then(@"I Uncheck Public Deploy To")]
        public void Uncheck_Public_Deploy_To()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_DeployToCell.Public_DeployToCheckBox.Checked = false;
        }

        [Given(@"I Uncheck Public Deploy From")]
        [When(@"I Uncheck Public Deploy From")]
        [Then(@"I Uncheck Public Deploy From")]
        public void Uncheck_Public_Deploy_From()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_DeployFromCell.Public_DeployFromCheckBox.Checked = false;
        }

        [Given(@"I Uncheck Public View")]
        [When(@"I Uncheck Public View")]
        [Then(@"I Uncheck Public View")]
        public void Uncheck_Public_View()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_ViewCell.Public_ViewCheckBox.Checked = false;
        }

        [Given(@"I Uncheck Public Execute")]
        [When(@"I Uncheck Public Execute")]
        [Then(@"I Uncheck Public Execute")]
        public void Uncheck_Public_Execute()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_ExecuteCell.Public_ExecuteCheckBox.Checked = false;
        }

        [Given(@"I Uncheck Public Contribute")]
        [When(@"I Uncheck Public Contribute")]
        [Then(@"I Uncheck Public Contribute")]
        public void Uncheck_Public_Contribute()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ServerPermissions.PublicROW.Public_ContributeCell.Public_ContributeCheckBox.Checked = false;
        }

        [Given(@"I Click Assign Tool CollapseAll")]
        [When(@"I Click Assign Tool CollapseAll")]
        [Then(@"I Click Assign Tool CollapseAll")]
        public void Click_Assign_Tool_CollapseAll()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.CollapseAllToggleButton.Exists, "Expand all button does not exist");
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.CollapseAllToggleButton.Pressed = true;
        }

        [Given(@"I Click Assign Tool ExpandAll")]
        [When(@"I Click Assign Tool ExpandAll")]
        [Then(@"I Click Assign Tool ExpandAll")]
        public void Click_Assign_Tool_ExpandAll()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ExpandAllToggleButton.Exists, "Expand all button does not exist");
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ExpandAllToggleButton.Pressed = true;
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.Exists, "Assign tool large view on the design surface does not exist");
        }

        [Given(@"I Click Assign Tool Large View Done Button")]
        [When(@"I Click Assign Tool Large View Done Button")]
        [Then(@"I Click Assign Tool Large View Done Button")]
        public void Click_Assign_Tool_Large_View_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.DoneButton, new Point(35, 6));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.OpenQuickVariableInpToggleButton.Exists, "QVI toggle button does not exist in assign tool small view after clicking done button on large view.");
        }

        [Given(@"I Click Assign Tool Large View Done Button On Unpinned Tab")]
        [When(@"I Click Assign Tool Large View Done Button On Unpinned Tab")]
        [Then(@"I Click Assign Tool Large View Done Button On Unpinned Tab")]
        public void Click_Assign_Tool_Large_View_Done_Button_On_Unpinned_Tab()
        {
            Mouse.Click(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.DoneButton, new Point(35, 6));
            Assert.AreEqual("SomeVariable", MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.ScrollViewerPane.NameTextbox.Text, "Variable list scalar row 1 textbox text does not equal somevariable after using that variable on a unpinned tab.");
        }

        [Given(@"I Click Assign Tool Large View Done Button With Row1 Variable Textbox As SomeInvalidVariableName")]
        [When(@"I Click Assign Tool Large View Done Button With Row1 Variable Textbox As SomeInvalidVariableName")]
        [Then(@"I Click Assign Tool Large View Done Button With Row1 Variable Textbox As SomeInvalidVariableName")]
        public void Click_Assign_Tool_Large_View_Done_Button_With_Row1_Variable_Textbox_As_SomeInvalidVariableName()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.DoneButton, new Point(35, 6));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Error1.Hyperlink.Exists, "Error popup does not exist on flowchart designer.");
            Assert.AreEqual("", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.ScrollViewerPane.NameTextbox.Text, "Variable list scalar row 1 textbox text is not blank with invalid variable.");
        }

        [Given(@"I Click Assign Tool Large View Done Button With Row1 Variable Textbox As SomeInvalidVariableName On Unpinned Tab")]
        [When(@"I Click Assign Tool Large View Done Button With Row1 Variable Textbox As SomeInvalidVariableName On Unpinned Tab")]
        [Then(@"I Click Assign Tool Large View Done Button With Row1 Variable Textbox As SomeInvalidVariableName On Unpinned Tab")]
        public void Click_Assign_Tool_Large_View_Done_Button_With_Row1_Variable_Textbox_As_SomeInvalidVariableName_On_Unpinned_Tab()
        {
            Mouse.Click(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.DoneButton, new Point(35, 6));
            Assert.IsTrue(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Error1.Hyperlink.Exists, "Error popup does not exist on flowchart designer.");
            Assert.AreEqual("", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.ScrollViewerPane.NameTextbox.Text, "Variable list scalar row 1 textbox text is not blank with invalid variable.");
        }

        [Given(@"I Click Assign Tool QviLarge Preview")]
        [When(@"I Click Assign Tool QviLarge Preview")]
        [Then(@"I Click Assign Tool QviLarge Preview")]
        public void Click_Assign_Tool_QviLarge_Preview()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.QuickVariableInputContent.PreviewCustom.PreviewGroup.PreviewButton, new Point(30, 4));
        }

        [Given(@"I click AssignObject Done")]
        [When(@"I click AssignObject Done")]
        [Then(@"I click AssignObject Done")]
        public void click_AssignObject_Done()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.AssignObject.DoneButton, new Point(18, 10));
        }

        [Given(@"I Click Calculate Large View Done Button")]
        [When(@"I Click Calculate Large View Done Button")]
        [Then(@"I Click Calculate Large View Done Button")]
        public void Click_CalculateTool_DoneButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Calculate.DoneButton, new Point(45, 8));
        }

        [Given(@"I Click Cancel DebugInput Window")]
        [When(@"I Click Cancel DebugInput Window")]
        [Then(@"I Click Cancel DebugInput Window")]
        public void Click_Cancel_DebugInput_Window()
        {
            Mouse.Click(MainStudioWindow.DebugInputDialog.CancelButton, new Point(26, 13));
        }

        [Given(@"I Click Clear Toolbox Filter Clear Button")]
        [When(@"I Click Clear Toolbox Filter Clear Button")]
        [Then(@"I Click Clear Toolbox Filter Clear Button")]
        public void Click_Clear_Toolbox_Filter_Clear_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.ClearFilterButton, new Point(8, 7));
        }

        public void DoubleClick_Toolbox()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Assign";
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.MultiAssign, new Point(2, 10));
        }

        public void SingleClick_Toolbox()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Assign";
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.MultiAssign, new Point(2, 10));
        }

        [Given(@"I Click Close Sharepoint Server Source Tab")]
        [When(@"I Click Close Sharepoint Server Source Tab")]
        [Then(@"I Click Close Sharepoint Server Source Tab")]
        public void WhenIClickCloseSharepointServerSourceWizardTab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SharepointServerSourceTab.CloseButton);
        }

        [Given(@"I Click Close WCFService Source Tab Button")]
        [When(@"I Click Close WCFService Source Tab Button")]
        [Then(@"I Click Close WCFService Source Tab Button")]

        public void Click_Close_WCFServiceSource_TabButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WCFServiceSourceTab.CloseTabButton);
        }

        [Given(@"I Click Close OAuthSource Source Tab Button")]
        [When(@"I Click Close OAuthSource Source Tab Button")]
        [Then(@"I Click Close OAuthSource Source Tab Button")]
        public void Click_OAuthSource_CloseTabButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.OAuthSourceWizardTab.CloseTabButton);
        }

        [Given(@"I Click Close COMPlugin Source Tab Button")]
        [When(@"I Click Close COMPlugin Source Tab Button")]
        [Then(@"I Click Close COMPlugin Source Tab Button")]
        public void Click_COMPluginSource_CloseTabButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.COMPlugInSourceTab.CloseTabButton);
        }

        [Given(@"I Click Close Critical Error Dialog")]
        [When(@"I Click Close Critical Error Dialog")]
        [Then(@"I Click Close Critical Error Dialog")]
        public void Click_Close_Critical_Error_Dialog()
        {
            Mouse.Click(CriticalErrorWindow.CloseButton, new Point(9, 11));
        }

        [Given(@"I Click Close RabbitMQ Source Tab Button")]
        [When(@"I Click Close RabbitMQ Source Tab Button")]
        [Then(@"I Click Close RabbitMQ Source Tab Button")]
        public void Click_Close_RabbitMQSource_Tab_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.RabbitMqSourceTab.CloseTabButton, new Point(13, 4));
        }

        [Given(@"I Click Close Exchange Source Tab Button")]
        [When(@"I Click Close Exchange Source Tab Button")]
        [Then(@"I Click Close RabExchangebitMQ Source Tab Button")]
        public void Click_ExchangeSource_CloseTabButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ExchangeSourceTab.CloseButton);
        }

        [Given(@"I Click Close DB Source Wizard Tab Button")]
        [When(@"I Click Close DB Source Wizard Tab Button")]
        [Then(@"I Click Close DB Source Wizard Tab Button")]
        public void Click_Close_DB_Source_Wizard_Tab_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.CloseButton, new Point(13, 4));
        }

        [Given(@"I Click Close Dependecy Tab")]
        [When(@"I Click Close Dependecy Tab")]
        [Then(@"I Click Close Dependecy Tab")]
        public void Click_Close_Dependecy_Tab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DependencyGraphTab.CloseButton, new Point(13, 10));
        }

        [Given(@"I Click Close Deploy Tab Button")]
        [When(@"I Click Close Deploy Tab Button")]
        [Then(@"I Click Close Deploy Tab Button")]
        public void Click_Close_Deploy_Tab_Button()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.CloseButton.Exists, "DeployTab close tab button does not exist.");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.CloseButton, new Point(16, 6));
        }

        [Given(@"I Click Close DotNetPlugin Source Tab")]
        [When(@"I Click Close DotNetPlugin Source Tab")]
        [Then(@"I Click Close DotNetPlugin Source Tab")]
        public void Click_Close_DotNetPlugin_Source_Tab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DotNetPluginSourceTab.CloseButton, new Point(13, 4));
        }

        [Given(@"I Click Close EmailSource Tab")]
        [When(@"I Click Close EmailSource Tab")]
        [Then(@"I Click Close EmailSource Tab")]
        public void Click_Close_EmailSource_Tab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.EmailSourceTab.CloseButton, new Point(13, 10));
        }

        [Given(@"I Click Close Error Dialog")]
        [When(@"I Click Close Error Dialog")]
        [Then(@"I Click Close Error Dialog")]
        public void Click_Close_Error_Dialog()
        {
            Mouse.Click(ErrorWindow.CloseButton, new Point(8, 9));
        }

        [Given(@"I Click Close FullScreen")]
        [When(@"I Click Close FullScreen")]
        [Then(@"I Click Close FullScreen")]
        public void Click_Close_FullScreen()
        {
            Mouse.Click(MainStudioWindow.ExitFullScreenF11Text.ExitFullScreenF11Hyperlink, new Point(64, 5));
        }

        [Given(@"I Click Close Server Source Wizard Tab Button")]
        [When(@"I Click Close Server Source Wizard Tab Button")]
        [Then(@"I Click Close Server Source Wizard Tab Button")]
        public void Click_Close_Server_Source_Wizard_Tab_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.CloseButton, new Point(5, 5));
        }

        [Given(@"I Click Close Settings Tab Button")]
        [When(@"I Click Close Settings Tab Button")]
        [Then(@"I Click Close Settings Tab Button")]
        public void Click_Close_Settings_Tab_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.CloseButton, new Point(16, 6));
        }

        [Given(@"I Click Close SharepointSource Tab Button")]
        [When(@"I Click Close SharepointSource Tab Button")]
        [Then(@"I Click Close SharepointSource Tab Button")]
        public void Click_Close_SharepointSource_Tab_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SharepointServerSourceTab.CloseButton, new Point(13, 7));
        }

        [Given(@"I Click Close Studio TopRibbon Button")]
        [When(@"I Click Close Studio TopRibbon Button")]
        [Then(@"I Click Close Studio TopRibbon Button")]
        public void Click_Close_Studio_TopRibbon_Button()
        {
            Mouse.Click(MainStudioWindow.CloseStudioButton, new Point(23, 1));
        }

        [Given(@"I Click Close Tab Context Menu Button")]
        [When(@"I Click Close Tab Context Menu Button")]
        [Then(@"I Click Close Tab Context Menu Button")]
        public void Click_Close_Tab_Context_Menu_Button()
        {
            Mouse.Click(MainStudioWindow.TabContextMenu.Close, new Point(27, 13));
        }

        [Given(@"I Click Close Tests Tab")]
        [When(@"I Click Close Tests Tab")]
        [Then(@"I Click Close Tests Tab")]
        [Given(@"I Click Close Tests Tab")]
        public void Click_Close_Tests_Tab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.CloseButton, new Point(11, 5));
        }

        [Given(@"I Click Close Web Source Wizard Tab Button")]
        [When(@"I Click Close Web Source Wizard Tab Button")]
        [Then(@"I Click Close Web Source Wizard Tab Button")]
        public void Click_Close_Web_Source_Wizard_Tab_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.CloseButton, new Point(9, 6));
        }

        [Given(@"I Click Close Workflow Tab Button")]
        [When(@"I Click Close Workflow Tab Button")]
        [Then(@"I Click Close Workflow Tab Button")]
        public void Click_Close_Workflow_Tab_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.CloseButton, new Point(5, 5));
        }

        [Given(@"I Click ConfigureSetting From Menu")]
        [When(@"I Click ConfigureSetting From Menu")]
        [Then(@"I Click ConfigureSetting From Menu")]
        public void Click_ConfigureSetting_From_Menu()
        {
            Mouse.Click(MainStudioWindow.SideMenuBar.ConfigureSettingsButton, new Point(7, 13));
            MainStudioWindow.DockManager.SplitPaneMiddle.DrawHighlight();
        }

        [Given(@"I Click Connect Control InExplorer")]
        [When(@"I Click Connect Control InExplorer")]
        [Then(@"I Click Connect Control InExplorer")]
        public void Click_Connect_Control_InExplorer()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.ToggleButton, new Point(217, 8));
        }

        [Given(@"I Click Debug Output Assign Cell")]
        [When(@"I Click Debug Output Assign Cell")]
        [Then(@"I Click Debug Output Assign Cell")]
        public void Click_Debug_Output_Assign_Cell()
        {
            Assert.AreEqual("[[SomeVariable]]", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.DebugOutputTree.Step1.VariableTextbox2.DisplayText, "Wrong variable name in debug output");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.DebugOutputTree.Step1.Assign1Button, new Point(21, 9));
            Assert.AreEqual("IsPrimarySelection=True IsSelection=True IsCurrentLocation=null IsCurrentContext=" +
                            "null IsBreakpointEnabled=null IsBreakpointBounded=null ValidationState=Valid ", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.ItemStatus, "Multiassign small view is not selected.");
        }

        [Given(@"I Click Debug Output Assign Cell For Unpinned Workflow Tab")]
        [When(@"I Click Debug Output Assign Cell For Unpinned Workflow Tab")]
        [Then(@"I Click Debug Output Assign Cell For Unpinned Workflow Tab")]
        public void Click_Debug_Output_Assign_Cell_For_Unpinned_Workflow_Tab()
        {
            Assert.AreEqual("[[SomeVariable]]", MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.SplitPaneRight.DebugOutput.DebugOutputTree.Step1.VariableTextbox2.DisplayText, "Wrong variable name in debug output");
            Mouse.Click(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.SplitPaneRight.DebugOutput.DebugOutputTree.Step1.Assign1Button, new Point(21, 9));
        }

        [Given(@"I Click Debug Output BaseConvert Cell")]
        [When(@"I Click Debug Output BaseConvert Cell")]
        [Then(@"I Click Debug Output BaseConvert Cell")]
        public void Click_Debug_Output_BaseConvert_Cell()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.DebugOutputTree.Step1.BaseConversion1Button, new Point(33, 7));
            Assert.AreEqual("IsPrimarySelection=True IsSelection=True IsCurrentLocation=null IsCurrentContext=" +
                            "null IsBreakpointEnabled=null IsBreakpointBounded=null ValidationState=Valid ", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.BaseConvert.ItemStatus, "Base conversion small view is not selected.");
        }

        [Given(@"I Click Debug Output Calculate Cell")]
        [When(@"I Click Debug Output Calculate Cell")]
        [Then(@"I Click Debug Output Calculate Cell")]
        public void Click_Debug_Output_Calculate_Cell()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.DebugOutputTree.Step1.CalculateButton, new Point(24, 10));
            Assert.AreEqual("IsPrimarySelection=True IsSelection=True IsCurrentLocation=null IsCurrentContext=" +
                            "null IsBreakpointEnabled=null IsBreakpointBounded=null ValidationState=Valid ", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Calculate.ItemStatus, "Calculate tool small view is not selected.");
        }

        [Given(@"I Click Debug Output GenericResource Name")]
        [When(@"I Click Debug Output GenericResource Name")]
        [Then(@"I Click Debug Output GenericResource Name")]
        public void Click_Debug_Output_GenericResource_Name()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DrawHighlight();
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.DebugOutputTree.ServiceTreeItem.GenericResourceButton, new Point(24, 8));
            Assert.AreEqual("GenericResource", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.Workflow1ciremoteText.DisplayText, "GenericResource remote workflow tab is not open.");
        }

        [Given(@"I Click DebugInput Cancel Button")]
        [When(@"I Click DebugInput Cancel Button")]
        [Then(@"I Click DebugInput Cancel Button")]
        public void Click_DebugInput_Cancel_Button()
        {
            Mouse.Click(MainStudioWindow.DebugInputDialog.CancelButton, new Point(34, 10));
        }

        [Given(@"I Click DebugInput Debug Button")]
        [When(@"I Click DebugInput Debug Button")]
        [Then(@"I Click DebugInput Debug Button")]
        public void Click_DebugInput_Debug_Button()
        {
            Mouse.Click(MainStudioWindow.DebugInputDialog.DebugF6Button, new Point(34, 10));
        }

        [Given(@"I Click DebugInput ViewInBrowser Button")]
        [When(@"I Click DebugInput ViewInBrowser Button")]
        [Then(@"I Click DebugInput ViewInBrowser Button")]
        public void Click_DebugInput_ViewInBrowser_Button()
        {
            Assert.IsTrue(MainStudioWindow.DebugInputDialog.ViewInBrowserF7Button.Enabled, "ViewInBrowserF7Button is not enabled after clicking RunDebug from Menu.");
            Mouse.Click(MainStudioWindow.DebugInputDialog.ViewInBrowserF7Button, new Point(82, 14));
        }

        [Given(@"I Click Decision Dialog Cancel Button")]
        [When(@"I Click Decision Dialog Cancel Button")]
        [Then(@"I Click Decision Dialog Cancel Button")]
        public void Click_Decision_Dialog_Cancel_Button()
        {
            Mouse.Click(DecisionOrSwitchDialog.CancelButton, new Point(10, 14));
        }

        [Given(@"I Click Decision Dialog Done Button")]
        [When(@"I Click Decision Dialog Done Button")]
        [Then(@"I Click Decision Dialog Done Button")]
        public void Click_Decision_Dialog_Done_Button()
        {
            Mouse.Click(DecisionOrSwitchDialog.DoneButton, new Point(10, 14));
            Assert.IsFalse(ControlExistsNow(DecisionOrSwitchDialog), "Decision large view dialog still exists after the done button is clicked.");
        }

        [Given(@"I Click Delete Done Button")]
        [When(@"I Click Delete Done Button")]
        [Then(@"I Click Delete Done Button")]
        public void Click_Delete_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathDelete.DoneButton, new Point(35, 6));
        }

        [Given(@"I Click Deploy Tab Destination Server Combobox")]
        [When(@"I Click Deploy Tab Destination Server Combobox")]
        [Then(@"I Click Deploy Tab Destination Server Combobox")]
        public void Click_Deploy_Tab_Destination_Server_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DestinationServerConectControl.Combobox.ToggleButton, new Point(230, 9));
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsNewRemoteServer.Exists, "New Remote Server... option does not exist in Destination server combobox.");
        }

        [When(@"I Click Deploy Tab Destination Server Connect Button")]
        [Given(@"I Click Deploy Tab Destination Server Connect Button")]
        [Then(@"I Click Deploy Tab Destination Server Connect Button")]
        public void Click_Deploy_Tab_Destination_Server_Connect_Button()
        {
            WaitForControlVisible(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DestinationServerConectControl.ConnectDestinationButton);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DestinationServerConectControl.ConnectDestinationButton, new Point(13, 12));
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DestinationServerConectControl.Spinner);
        }

        [Given(@"I Click Deploy Tab Destination Server New Remote Server Item")]
        [When(@"I Click Deploy Tab Destination Server New Remote Server Item")]
        [Then(@"I Click Deploy Tab Destination Server New Remote Server Item")]
        public void Click_Deploy_Tab_Destination_Server_New_Remote_Server_Item()
        {
            Mouse.Click(MainStudioWindow.ComboboxListItemAsNewRemoteServer, new Point(223, 10));
        }

        [When(@"I Click Deploy Tab Destination Server Remote Connection Intergration Item")]
        [Then(@"I Click Deploy Tab Destination Server Remote Connection Intergration Item")]
        [Given(@"I Click Deploy Tab Destination Server Remote Connection Intergration Item")]
        public void Click_Deploy_Tab_Destination_Server_Remote_Connection_Intergration_Item()
        {
            Mouse.Click(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegration, new Point(223, 10));
        }

        [Given(@"I Click Deploy Tab Source Server Combobox")]
        [When(@"I Click Deploy Tab Source Server Combobox")]
        [Then(@"I Click Deploy Tab Source Server Combobox")]
        public void Click_Deploy_Tab_Source_Server_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerConectControl.Combobox.ToggleButton, new Point(230, 9));
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsNewRemoteServer.Exists, "New Remote Server... option does not exist in Source server combobox.");
        }

        [Given(@"I Click Deploy Tab Source Server Connect Button")]
        [When(@"I Click Deploy Tab Source Server Connect Button")]
        [Then(@"I Click Deploy Tab Source Server Connect Button")]
        public void Click_Deploy_Tab_Source_Server_Connect_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerConectControl.ConnectSourceButton, new Point(13, 8));
        }

        [Given(@"I Click Deploy Tab Source Server Edit Button")]
        [When(@"I Click Deploy Tab Source Server Edit Button")]
        [Then(@"I Click Deploy Tab Source Server Edit Button")]
        public void Click_Deploy_Tab_Source_Server_Edit_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.EditSourceButton, new Point(13, 8));
        }

        [Given(@"I Click Deploy Tab Source Refresh Button")]
        [When(@"I Click Deploy Tab Source Refresh Button")]
        [Then(@"I Click Deploy Tab Source Refresh Button")]
        public void Click_Deploy_Tab_Source_Refresh_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerExplorer.RefreshButton);
        }

        [Given(@"I Click Deploy Tab WarewolfStore Item")]
        [When(@"I Click Deploy Tab WarewolfStore Item")]
        [Then(@"I Click Deploy Tab WarewolfStore Item")]
        public void Click_Deploy_Tab_WarewolfStore_Item()
        {
            Mouse.Click(MainStudioWindow.ComboboxListItemAsWarewolfStore, new Point(214, 9));
        }

        [Given(@"I Click Duplicate From Duplicate Dialog")]
        [When(@"I Click Duplicate From Duplicate Dialog")]
        [Then(@"I Click Duplicate From Duplicate Dialog")]
        public void Click_Duplicate_From_Duplicate_Dialog()
        {
            Assert.IsTrue(SaveDialogWindow.DuplicateButton.Exists, "Duplicate button does not exist");
            Mouse.Click(SaveDialogWindow.DuplicateButton, new Point(26, 10));
        }

        [Given(@"I Click EditSharepointSource Button")]
        [When(@"I Click EditSharepointSource Button")]
        [Then(@"I Click EditSharepointSource Button")]
        public void Click_EditSharepointSource_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCreateListItem.SmallView.EditSourceButton, new Point(98, 12));
        }

        [Given(@"I Click EditSharepointSource Button From SharePointUpdate")]
        [When(@"I Click EditSharepointSource Button From SharePointUpdate")]
        [Then(@"I Click EditSharepointSource Button From SharePointUpdate")]
        public void Click_EditSharepointSource_Button_From_SharePointUpdate()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUpdateListItem.SmallView.EditSourceButton, new Point(98, 12));
        }

        [Given(@"I Click EditSharepointSource Button FromSharePointDelete")]
        [When(@"I Click EditSharepointSource Button FromSharePointDelete")]
        [Then(@"I Click EditSharepointSource Button FromSharePointDelete")]
        public void Click_EditSharepointSource_Button_FromSharePointDelete()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDeleteFile.SmallView.EditSourceButton, new Point(98, 12));
        }

        [Given(@"I Click EditSharepointSource Button FromSharePointRead")]
        [When(@"I Click EditSharepointSource Button FromSharePointRead")]
        [Then(@"I Click EditSharepointSource Button FromSharePointRead")]
        public void Click_EditSharepointSource_Button_FromSharePointRead()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadListItem.SmallView.EditSourceButton, new Point(98, 12));
        }

        [Given(@"I Click EmailSource TestConnection Button")]
        [When(@"I Click EmailSource TestConnection Button")]
        [Then(@"I Click EmailSource TestConnection Button")]
        public void Click_EmailSource_TestConnection_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.EmailSourceTab.SendTestModelsCustom.TestConnectionButton, new Point(58, 16));
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.EmailSourceTab.SendTestModelsCustom.Spinner);
        }

        [Given(@"I Click EndThisWF On XPath LargeView")]
        [When(@"I Click EndThisWF On XPath LargeView")]
        [Then(@"I Click EndThisWF On XPath LargeView")]
        public void Click_EndThisWF_On_XPath_LargeView()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.XPath.LargeViewContentCustom.OnErrorCustom.OnErrorGroup.EndthisworkflowCheckBox.Checked = true;
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.XPath.LargeViewContentCustom.OnErrorCustom.OnErrorGroup.EndthisworkflowCheckBox, "{Tab}", ModifierKeys.None);
        }

        [Given(@"I Click ExpandAndStepIn NestedWorkflow")]
        [When(@"I Click ExpandAndStepIn NestedWorkflow")]
        [Then(@"I Click ExpandAndStepIn NestedWorkflow")]
        public void Click_ExpandAndStepIn_NestedWorkflow()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.DebugOutputTree.SubWorkflow.Expanded = true;
        }

        [Given(@"I Click Explorer Filter Clear Button")]
        [When(@"I Click Explorer Filter Clear Button")]
        [Then(@"I Click Explorer Filter Clear Button")]
        public void Click_Explorer_Filter_Clear_Button()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.SearchTextBox.ClearFilterButton, new Point(6, 8));
        }

        [When(@"I Click Explorer Localhost First Item")]
        [Given(@"I Click Explorer Localhost First Item")]
        [Then(@"I Click Explorer Localhost First Item")]
        public void Click_Explorer_Localhost_First_Item()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem);
        }

        [Given(@"I Click Explorer Remote Server Dropdown List")]
        [When(@"I Click Explorer Remote Server Dropdown List")]
        [Then(@"I Click Explorer Remote Server Dropdown List")]
        public void Click_Explorer_Remote_Server_Dropdown_List()
        {
            WaitForControlVisible(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.ToggleButton);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.ToggleButton, new Point(167, 10));
        }

        [Given(@"I Click Explorer Connect Remote Server Button")]
        [When(@"I Click Explorer Connect Remote Server Button")]
        [Then(@"I Click Explorer Connect Remote Server Button")]
        public void Click_Explorer_RemoteServer_Connect_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ConnectServerButton, new Point(11, 10));
        }

        [Given(@"I Click First Recordset Input Checkbox")]
        [When(@"I Click First Recordset Input Checkbox")]
        [Then(@"I Click First Recordset Input Checkbox")]
        public void Click_First_Recordset_Input_Checkbox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.RecordsetTreeItem.TreeItem1.InputCheckbox.Checked = true;
        }

        [Given(@"I Click FormatNumber Done Button")]
        [When(@"I Click FormatNumber Done Button")]
        [Then(@"I Click FormatNumber Done Button")]
        public void Click_FormatNumber_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FormatNumber.DoneButton, new Point(36, 11));
        }

        [Given(@"I Click FullScreen TopRibbon Button")]
        [When(@"I Click FullScreen TopRibbon Button")]
        [Then(@"I Click FullScreen TopRibbon Button")]
        public void Click_FullScreen_TopRibbon_Button()
        {
            Mouse.Click(MainStudioWindow.MaximizeRestoreStudioButton, new Point(12, 9));
        }

        [Given(@"I Click GET Web Large View Done Button With Invalid Large View")]
        [When(@"I Click GET Web Large View Done Button With Invalid Large View")]
        [Then(@"I Click GET Web Large View Done Button With Invalid Large View")]
        public void Click_GET_Web_Large_View_Done_Button_With_Invalid_Large_View()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebGet.DoneButton, new Point(33, 11));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Error1.Exists, "Error not exist after clicking large view done button on invalid large view.");
        }

        [Given("I Click New Workflow Ribbon Button")]
        [When("I Click New Workflow Ribbon Button")]
        [Then("I Click New Workflow Ribbon Button")]
        public void Click_NewWorkflow_RibbonButton()
        {
            Mouse.Click(MainStudioWindow.SideMenuBar.NewWorkflowButton, new Point(6, 6));
            WaitForControlVisible(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.StartNode.Exists, "Start Node Does Not Exist after clicking new workflow ribbon button.");
        }

        [Given(@"I Click Save Ribbon Button")]
        [When(@"I Click Save Ribbon Button")]
        [Then(@"I Click Save Ribbon Button")]
        public void Click_Save_RibbonButton()
        {
            Mouse.Click(MainStudioWindow.SideMenuBar.SaveButton);
        }

        [Given(@"I Click Deploy Ribbon Button")]
        [When(@"I Click Deploy Ribbon Button")]
        [Then(@"I Click Deploy Ribbon Button")]
        public void Click_Deploy_Ribbon_Button()
        {
            Mouse.Click(MainStudioWindow.SideMenuBar.DeployButton, new Point(16, 11));
            Playback.Wait(2000);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.Exists, "Deploy tab does not exist after clicking deploy ribbon button.");
        }

        [Given(@"I Click Scheduler Ribbon Button")]
        [When(@"I Click Scheduler Ribbon Button")]
        [Then(@"I Click Scheduler Ribbon Button")]
        public void Click_Scheduler_RibbonButton()
        {
            Mouse.Click(MainStudioWindow.SideMenuBar.SchedulerButton, new Point(4, 12));            
        }

        [Given(@"I Click Debug Ribbon Button")]
        [When(@"I Click Debug Ribbon Button")]
        [Then(@"I Click Debug Ribbon Button")]
        public void Click_Debug_RibbonButton()
        {
            Mouse.Click(MainStudioWindow.SideMenuBar.RunAndDebugButton, new Point(13, 14));
            Assert.IsTrue(MainStudioWindow.DebugInputDialog.Exists, "Debug Input Dialog does not exist after clicking debug ribbon button.");
        }

        [Given(@"I Click Settings Ribbon Button")]
        [When(@"I Click Settings Ribbon Button")]
        [Then(@"I Click Settings Ribbon Button")]
        public void Click_Settings_RibbonButton()
        {
            Mouse.Click(MainStudioWindow.SideMenuBar.ConfigureSettingsButton, new Point(7, 2));
            MainStudioWindow.DockManager.SplitPaneMiddle.DrawHighlight();
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.Exists, "Settings tab does not exist after clicking settings ribbon button.");
        }

        [Given(@"I Click Knowledge Ribbon Button")]
        [When(@"I Click Knowledge Ribbon Button")]
        [Then(@"I Click Knowledge Ribbon Button")]
        public void Click_Knowledge_RibbonButton()
        {
            Mouse.Click(MainStudioWindow.SideMenuBar.KnowledgeBaseButton, new Point(4, 8));
        }

        [Given(@"I Click Lock Ribbon Button")]
        [When(@"I Click Lock Ribbon Button")]
        [Then(@"I Click Lock Ribbon Button")]
        public void Click_Lock_RibbonButton()
        {
            Mouse.Click(MainStudioWindow.LockStudioButton, new Point(14, 5));
        }

        [When(@"I Click Unlock Ribbon Button")]
        public void Click_Unlock_RibbonButton()
        {
            Mouse.Click(MainStudioWindow.SideMenuBar.LockMenuButton, new Point(8, 6));
        }

        [When(@"I Click Unlock TopRibbon Button")]
        public void Click_Unlock_TopRibbonButton()
        {
            Mouse.Click(MainStudioWindow.LockStudioButton, new Point(10, 12));
        }

        [Given(@"I Click Maximize Restore TopRibbon Button")]
        [When(@"I Click Maximize Restore TopRibbon Button")]
        [Then(@"I Click Maximize Restore TopRibbon Button")]
        public void Click_MaximizeRestore_TopRibbonButton()
        {
            Mouse.Click(MainStudioWindow.MaximizeStudioButton, new Point(9, 11));
        }

        [Given(@"I Click Maximize TopRibbon Button")]
        [When(@"I Click Maximize TopRibbon Button")]
        [Then(@"I Click Maximize TopRibbon Button")]
        public void Click_Maximize_TopRibbonButton()
        {
            Mouse.Click(MainStudioWindow.MaximizeStudioButton, new Point(14, 14));
        }

        [Given(@"I Click Minimize TopRibbon Button")]
        [When(@"I Click Minimize TopRibbon Button")]
        [Then(@"I Click Minimize TopRibbon Button")]
        public void Click_Minimize_TopRibbonButton()
        {
            Mouse.Click(MainStudioWindow.MinimizeStudioButton, new Point(6, 14));
        }

        [Given(@"I Click MessageBox No")]
        [When(@"I Click MessageBox No")]
        [Then(@"I Click MessageBox No")]
        public void Click_MessageBox_No()
        {
            MessageBoxWindow.NoButton.DrawHighlight();
            Mouse.Click(MessageBoxWindow.NoButton);
        }

        [Given(@"I Click MessageBox OK")]
        [When(@"I Click MessageBox OK")]
        [Then(@"I Click MessageBox OK")]
        [Given(@"I Click MessageBox OK")]
        public void Click_MessageBox_OK()
        {
            Mouse.Click(MessageBoxWindow.OKButton, new Point(35, 11));
        }

        public void Duplicate_Test_Name_MessageBox_Ok()
        {
            Assert.IsTrue(MessageBoxWindow.DuplicateTestNameText.Exists, "Duplicate test name message box does not appear on the surface.");
            Mouse.Click(MessageBoxWindow.OKButton, new Point(35, 11));
        }

        [Then(@"I Click Save Before Continuing MessageBox OK")]
        public void Click_Save_Before_Continuing_MessageBox_OK()
        {
            Assert.IsTrue(MessageBoxWindow.SaveBeforeAddingNewTestText.Exists, "Messagebox does not warn about unsaved tests after clicking create new test.");
            Mouse.Click(MessageBoxWindow.OKButton, new Point(35, 11));
        }

        public void Click_DropNotAllowed_MessageBox_OK()
        {
            Assert.IsTrue(MessageBoxWindow.DropnotallowedText.Exists, "The Shown dialog is not Drop Not 'Allowed MessageBox'");
            Mouse.Click(MessageBoxWindow.OKButton, new Point(35, 11));
        }
        public void Click_DeleteAnyway_MessageBox_OK()
        {
            Mouse.Click(MessageBoxWindow.OKButton, new Point(35, 11));
        }

        [Then(@"I Click Deploy version conflicts MessageBox OK")]
        [When(@"I Click Deploy version conflicts MessageBox OK")]
        public void ClickDeployVersionConflictsMessageBoxOK()
        {
            Assert.IsTrue(MessageBoxWindow.DeployVersionConflicText.Exists, "Deploy Version Conflicts MessageBox does not Exist");
            Mouse.Click(MessageBoxWindow.OKButton, new Point(35, 11));
        }
        [Then(@"I Click Deploy conflicts MessageBox OK")]
        [When(@"I Click Deploy conflicts MessageBox OK")]
        public void ClickDeployConflictsMessageBoxOK()
        {
            Assert.IsTrue(MessageBoxWindow.DeployConflictsText.Exists, "Deploy Conflicts MessageBox does not Exist");
            Mouse.Click(MessageBoxWindow.OKButton, new Point(35, 11));
        }

        [Then(@"I Click Deploy Successful MessageBox OK")]
        [When(@"I Click Deploy Successful MessageBox OK")]
        public void ClickDeploySuccessfulMessageBoxOK()
        {
            Assert.IsTrue(MessageBoxWindow.ResourcesDeployedSucText.Exists, "Deploy Successful MessageBox does not Exist");
            Mouse.Click(MessageBoxWindow.OKButton, new Point(35, 11));
        }


        [Given(@"I Click MessageBox DeleteAnyway")]
        [When(@"I Click MessageBox DeleteAnyway")]
        [Then(@"I Click MessageBox DeleteAnyway")]
        [Given(@"I Click MessageBox DeleteAnyway")]
        public void Click_MessageBox_DeleteAnyway()
        {
            Mouse.Click(MessageBoxWindow.DeleteAnyway, new Point(35, 11));
        }

        [Given(@"I Click MessageBox Yes")]
        [When(@"I Click MessageBox Yes")]
        [Then(@"I Click MessageBox Yes")]
        public void Click_MessageBox_Yes()
        {
            Mouse.Click(MessageBoxWindow.YesButton, new Point(32, 5));
        }

        [Given(@"I Click Nested Workflow Name")]
        [When(@"I Click Nested Workflow Name")]
        [Then(@"I Click Nested Workflow Name")]
        public void Click_Nested_Workflow_Name()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.DebugOutputTree.SubWorkflow.UIHelloWorldTreeItem1.UIHelloWorldButton, new Point(37, 10));
        }

        [Given(@"I Click New Workflow Tab")]
        [When(@"I Click New Workflow Tab")]
        [Then(@"I Click New Workflow Tab")]
        public void Click_New_Workflow_Tab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab, new Point(63, 18));
        }

        [Given(@"I Click New Web Source Explorer Context Menu Button")]
        [When(@"I Click New Web Source Explorer Context Menu Button")]
        [Then(@"I Click New Web Source Explorer Context Menu Button")]
        public void Click_NewWebSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(72, 8));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewWebServiceSource);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.LocalhostConnectedText.Exists);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.Exists, "Web server address textbox does not exist on new web source wizard tab.");
        }

        [Given(@"I Click New SQLServerSource Explorer Context Menu")]
        [When(@"I Click New SQLServerSource Explorer Context Menu")]
        [Then(@"I Click New SQLServerSource Explorer Context Menu")]
        public void Click_NewSQLServerSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(75, 9));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewSQLServerSource);
        }

        [When(@"I Select NewMySQLSource From Explorer Context Menu")]
        public void Select_NewMySQLSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(77, 13));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewMySQLSource);
        }

        [When(@"I Select NewPostgreSQLSource From Explorer Context Menu")]
        public void Select_NewPostgreSQLSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(77, 13));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewPostgreSQLSource);
        }

        [When(@"I Select NewOracleSource From Explorer Context Menu")]
        public void Select_NewOracleSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(77, 13));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewOracleSource);
        }

        [When(@"I Select NewODBCSource From Explorer Context Menu")]
        public void Select_NewODBCSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(77, 13));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewODBCSource);
        }

        [When(@"I Select NewCOMPluginSource From Explorer Context Menu")]
        public void Select_NewCOMPluginSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(77, 13));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewCOMPluginSource);
        }

        [When(@"I Select NewRabbitMQSource From Explorer Context Menu")]
        public void Select_NewRabbitMQSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(77, 13));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewRabbitMQSource);
        }

        [When(@"I Select NewWcfSource From Explorer Context Menu")]
        public void Select_NewWcfSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(77, 13));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewWcfSource);
        }

        [Given(@"I Click New DotNetPluginSource Explorer Context Menu")]
        [When(@"I Click New DotNetPluginSource Explorer Context Menu")]
        [Then(@"I Click New DotNetPluginSource Explorer Context Menu")]
        public void Click_NewDotNetPluginSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(67, 9));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewDotNetPluginSource);
        }

        [When(@"I Select NewDropboxSource From Explorer Context Menu")]
        public void Select_NewDropboxSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(72, 8));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewDropboxSource);
        }

        [When(@"I Select NewEmailSource From Explorer Context Menu")]
        public void Select_NewEmailSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(77, 13));
            Assert.IsTrue(MainStudioWindow.ExplorerContextMenu.Exists, "Explorer Context Menu did not appear after Right click on localhost");
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewEmailSource);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.EmailSourceTab.Exists, "New email source tab does not exist after opening Email source tab");
        }

        [When(@"I Select NewExchangeSource From Explorer Contex tMenu")]
        public void Select_NewExchangeSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(77, 13));
            Assert.IsTrue(MainStudioWindow.ExplorerContextMenu.Exists, "Explorer Context Menu did not appear after Right click on localhost");
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewExchangeSource);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ExchangeSourceTab.Exists, "New exchange source tab does not exist after opening Email source tab");
        }

        [When(@"I Select NewPluginSource From Explorer Context Menu")]
        public void Select_NewPluginSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(72, 8));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewDotNetPluginSource);
        }

        [When(@"I Select NewServerSource From Explorer Context Menu")]
        public void Select_NewServerSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(72, 8));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewServerSource);
        }

        [When(@"I Select NewSharepointSource From Explorer Context Menu")]
        public void Select_NewSharepointSource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(72, 8));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.SourcesMenuItem.NewSharepointSource);
        }

        [When(@"I Click Show Dependencies From Explorer Context Menu")]
        public void Click_ShowDependencies_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.ShowDependencies, new Point(50, 15));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DependencyGraphTab.WorksurfaceContext.DependencyView.ScrollViewer.ShowwhatdependsonthisRadioButton.Selected, "Dependency graph show dependencies radio button is not selected.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DependencyGraphTab.WorksurfaceContext.DependencyView.ScrollViewer.NestingLevelsText.Textbox.Exists, "Dependency graph nesting levels textbox does not exist.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DependencyGraphTab.WorksurfaceContext.DependencyView.ScrollViewer.RefreshButton.Exists, "Refresh button does not exist on dependency graph");
            Assert.AreEqual("RemoteServerUITestWorkflow", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DependencyGraphTab.WorksurfaceContext.DependencyView.ScrollViewer.Node1.Text.DisplayText, "Dependant workflow not shown in dependency diagram");
        }

        [When(@"I Click Show Server Version From Explorer Context Menu")]
        public void Click_ShowServerVersion_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.ShowServerVersion, new Point(45, 13));
        }

        [When(@"I Create New Folder ""(.*)"" In Explorer Second Item With Context Menu")]
        public void Create_NewFolder_In_ExplorerSecondItem_With_ExplorerContextMenu(string FolderName)
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem, MouseButtons.Right, ModifierKeys.None, new Point(126, 12));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.NewFolderMenuItem, new Point(78, 15));
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem.FirstSubItem.ItemEdit.Text = FolderName;
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem.FirstSubItem, "{Enter}", ModifierKeys.None);
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.Spinner);
        }

        [When(@"I Duplicate Explorer Localhost First Item With Context Menu")]
        public void Duplicate_ExplorerLocalhostFirstItem_With_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(107, 9));
            Assert.IsTrue(MainStudioWindow.ExplorerContextMenu.Duplicate.Exists, "Duplicate does not exist in explorer context menu.");
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Duplicate, new Point(62, 10));
            Assert.IsTrue(SaveDialogWindow.Exists, "Duplicate dialog does not exist after clicking duplicate in the explorer context menu.");
        }

        [Given(@"I Open Explorer First Item Context Menu")]
        [Then(@"I Open Explorer First Item Context Menu")]
        [When(@"I Open Explorer First Item Context Menu")]
        public void Open_ExplorerFirstItem_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(69, 10));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Open);
        }

        public void Click_AssignStep_InDebugOutput()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.DebugOutputTree.AssignOnDebugOutput);
        }

        public void Click_DesicionStep_InDebugOutput()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.DebugOutputTree.DecisionOnDebugOutput);
        }

        [When(@"I Open Explorer First Item Tests With Context Menu")]
        [Then(@"I Open Explorer First Item Tests With Context Menu")]
        [Given(@"I Open Explorer First Item Tests With Context Menu")]
        public void Open_ExplorerFirstItemTests_With_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(107, 9));
            Assert.IsTrue(MainStudioWindow.ExplorerContextMenu.Tests.Exists, "View tests does not exist in explorer context menu.");
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Tests);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.RunAllButton.Exists, "Run all button does not exist on tests tab");
        }

        [When(@"I Open Explorer First Item Version History From Explorer Context Menu")]
        public void Open_ExplorerFirstItemVersionHistory_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(69, 10));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.ShowVersionHistory, new Point(66, 15));
        }

        [When(@"I Open Explorer First SubItem With Context Menu")]
        public void Open_ExplorerFirstSubItem_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem, MouseButtons.Right, ModifierKeys.None, new Point(40, 9));
            Assert.IsTrue(MainStudioWindow.ExplorerContextMenu.Open.Exists, "Open does not exist in explorer context menu.");
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Open);
        }

        [Given(@"I Delete FirstResource FromContextMenu")]
        [When(@"I Delete FirstResource FromContextMenu")]
        [Then(@"I Delete FirstResource FromContextMenu")]
        public void Delete_FirstResource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(77, 12));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Delete);
        }

        [Given(@"I Rename First Remote Resource FromContextMenu to ""(.*)""")]
        [When(@"I Rename First Remote Resource FromContextMenu to ""(.*)""")]
        [Then(@"I Rename First Remote Resource FromContextMenu to ""(.*)""")]
        public void Rename_FirstRemoteResource_From_ExplorerContextMenu(string newName)
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(77, 12));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Rename);
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem.ItemEdit.Text = newName;
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem.ItemEdit, "{Enter}", ModifierKeys.None);
        }

        [Given(@"I Select Show Version History From Explorer Context Menu")]
        [When(@"I Select Show Version History From Explorer Context Menu")]
        [Then(@"I Select Show Version History From Explorer Context Menu")]
        public void Select_ShowVersionHistory_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(77, 12));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.ShowVersionHistory);
        }

        [Given(@"I Duplicate FirstResource From Explorer Context Menu")]
        [When(@"I Duplicate FirstResource From Explorer Context Menu")]
        [Then(@"I Duplicate FirstResource From Explorer Context Menu")]
        public void Duplicate_FirstResource_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(77, 12));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Duplicate);
        }

        [Given(@"I Select Delete From Explorer Context Menu")]
        [When(@"I Select Delete From Explorer Context Menu")]
        [Then(@"I Select Delete From Explorer Context Menu")]
        public void Select_Delete_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Delete, new Point(87, 12));
            Assert.IsTrue(MessageBoxWindow.Exists, "Message box does not exist");
            Assert.IsTrue(MessageBoxWindow.YesButton.Exists, "Message box Yes button does not exist");
        }

        [Given(@"I Select Deploy From Explorer Context Menu")]
        [When(@"I Select Deploy From Explorer Context Menu")]
        [Then(@"I Select Deploy From Explorer Context Menu")]
        public void Select_Deploy_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.DeployItem, new Point(57, 11));
            Playback.Wait(2000);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.Exists, "DeployTab does not exist after clicking Deploy");
        }

        [Given(@"I Select NewWorkflow From Explorer Context Menu")]
        [When(@"I Select NewWorkflow From Explorer Context Menu")]
        [Then(@"I Select NewWorkflow From Explorer Context Menu")]
        public void Select_NewWorkflow_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.NewWorkflowItem);
        }

        [Given(@"I Select Open From Explorer Context Menu")]
        [When(@"I Select Open From Explorer Context Menu")]
        [Then(@"I Select Open From Explorer Context Menu")]
        public void Select_Open_From_ExplorerContextMenu()
        {
            Assert.IsTrue(MainStudioWindow.ExplorerContextMenu.Open.Exists);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Open);
        }

        [When(@"I Select Tests From Context Menu")]
        public void Select_Tests_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Tests);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.Exists, "TestsTab does not exist after clicking view tests in the explorer context menu.");
        }
        public void Click_RunAllTests_On_FirstLocalhostItem_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(107, 9));
            Assert.IsTrue(MainStudioWindow.ExplorerContextMenu.RunAllTestsMenuItem.Exists);
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.RunAllTestsMenuItem, new Point(82, 16));
        }

        [Given(@"I Select Show Dependencies In Explorer Context Menu for service ""(.*)""")]
        [When(@"I Select Show Dependencies In Explorer Context Menu for service ""(.*)""")]
        [Then(@"I Select Show Dependencies In Explorer Context Menu for service ""(.*)""")]
        public void Select_ShowDependencies_In_ExplorerContextMenu(string ServiceName)
        {
            Filter_Explorer(ServiceName);
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.Spinner);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(77, 9));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.ShowDependencies);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DependencyGraphTab.WorksurfaceContext.DependencyView.Exists, "Dependency graph tab is not showen after clicking show dependancies explorer content menu item.");
        }

        public void Create_NewWorkflow_Of_ExplorerFirstItem_With_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(75, 10));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.NewWorkflowItem, new Point(79, 13));
        }

        [Given(@"I Click Duplicate From Explorer Context Menu for Service ""(.*)""")]
        [When(@"I Click Duplicate From Explorer Context Menu for Service ""(.*)""")]
        [Then(@"I Click Duplicate From Explorer Context Menu for Service ""(.*)""")]
        public void Click_Duplicate_From_ExplorerContextMenu(string ServiceName)
        {
            Filter_Explorer(ServiceName);
            Assert.AreEqual(ServiceName, MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.ItemEdit.Text, "First Item is not the same as Filtered input.");
            Duplicate_ExplorerLocalhostFirstItem_With_ExplorerContextMenu();
        }

        public void Show_ExplorerFirstItemTests_With_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(107, 9));
            Assert.IsTrue(MainStudioWindow.ExplorerContextMenu.Tests.Exists, "View tests option does not exist in context menu after right clicking an item in the explorer.");
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Tests);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.Exists, "Workflow test tab does not exist after openning it by clicking the explorer context menu item.");
        }

        public void Show_ExplorerFirstSubItemTests_With_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem, MouseButtons.Right, ModifierKeys.None, new Point(107, 9));
            Assert.IsTrue(MainStudioWindow.ExplorerContextMenu.Tests.Exists, "View tests option does not exist in context menu after right clicking an item in the explorer.");
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Tests);
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem.DrawHighlight();
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.Exists, "Workflow test tab does not exist after opening it by clicking the explorer context menu item.");
        }

        public void Show_ExplorerSecondItemTests_With_ExplorerContextMenu(string filter)
        {
            Filter_Explorer(filter);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem, MouseButtons.Right, ModifierKeys.None, new Point(107, 9));
            Assert.IsTrue(MainStudioWindow.ExplorerContextMenu.Tests.Exists, "View tests option does not exist in context menu after right clicking an item in the explorer.");
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Tests);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.Exists, "Workflow test tab does not exist after openning it by clicking the explorer context menu item.");
        }

        [Given(@"I Click New Source Button From ODBC Tool")]
        [When(@"I Click New Source Button From ODBC Tool")]
        [Then(@"I Click New Source Button From ODBC Tool")]
        public void Click_NewSourceButton_From_ODBCTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ODBCDatabaseActivCustom.LargeView.NewSourceButton, new Point(30, 4));
        }

        [Given(@"I Click New Source Button From Oracle Tool")]
        [When(@"I Click New Source Button From Oracle Tool")]
        [Then(@"I Click New Source Button From Oracle Tool")]
        public void Click_NewSourceButton_From_OracleTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.OracleDatabaseActCustom.LargeView.NewSourceButton, new Point(30, 4));
        }

        [Given(@"I Click New Source Button From PostgreSQL Tool")]
        [When(@"I Click New Source Button From PostgreSQL Tool")]
        [Then(@"I Click New Source Button From PostgreSQL Tool")]
        public void Click_NewSourceButton_From_PostgreSQLTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PostgreSqlActivitCustom.LargeView.NewSourceButton);
        }

        [Given(@"I Click New Source Button From MySQL Tool")]
        [When(@"I Click New Source Button From MySQL Tool")]
        [Then(@"I Click New Source Button From MySQL Tool")]
        public void Click_NewSourceButton_From_MySQLTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MySqlDatabase.LargeView.NewSourceButton);
        }

        [Given(@"I Click New Source Button From SQLServer Tool")]
        [When(@"I Click New Source Button From SQLServer Tool")]
        [Then(@"I Click New Source Button From SQLServer Tool")]
        public void Click_NewSourceButton_From_SqlServerTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.NewDbSourceButton, new Point(16, 9));
        }

        [Given(@"I Click New Source Button From WCF Tool")]
        [When(@"I Click New Source Button From WCF Tool")]
        [Then(@"I Click New Source Button From WCF Tool")]
        public void Click_NewSourceButton_From_WCFTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WcfService.LargeView.NewButton);
        }

        [Given(@"I Click New Source Button From RabbitMQConsume Tool")]
        [When(@"I Click New Source Button From RabbitMQConsume Tool")]
        [Then(@"I Click New Source Button From RabbitMQConsume Tool")]
        public void Click_NewSourceButton_From_RabbitMQConsumeTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.RabbitMQConsume.LargeViewContentCustom.NewSourceButton);
        }

        public void Click_NewSourceButton_From_RabbitMQPublishTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.RabbitMQPublish.LargeViewContentCustom.NewSourceButton);
        }

        [Given(@"I Click New Source Button From COMDLLPlugin Tool")]
        [When(@"I Click New Source Button From COMDLLPlugin Tool")]
        [Then(@"I Click New Source Button From COMDLLPlugin Tool")]
        public void Click_NewSourceButton_From_COMDLLPluginTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ComDll.LargeView.NewSourceButton);
        }

        [Given(@"I Click New Source Button From DotNetDLLPlugin Tool")]
        [When(@"I Click New Source Button From DotNetDLLPlugin Tool")]
        [Then(@"I Click New Source Button From DotNetDLLPlugin Tool")]
        public void Click_NewSourceButton_From_DotNetDLLPluginTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.NewSourceButton);
        }

        [Given(@"I Click New Source Button From SQLBulkInsert Tool")]
        [When(@"I Click New Source Button From SQLBulkInsert Tool")]
        [Then(@"I Click New Source Button From SQLBulkInsert Tool")]
        public void Click_NewSource_From_SqlBulkInsertTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlBulkInsert.LargeViewContentCustom.DatabaseComboBox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlBulkInsert.LargeViewContentCustom.DatabaseComboBox.NewDatabaseSource);
        }

        [Given(@"I Click New Source Button From ExchangeSend Tool")]
        [When(@"I Click New Source Button From ExchangeSend Tool")]
        [Then(@"I Click New Source Button From ExchangeSend Tool")]
        public void Click_NewSourceButton_From_ExchangeSendTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ExchangeEmail.LargeViewContent.NewSourceButton);
        }

        [Given(@"I Click New Source Button From HttpWebDelete Tool")]
        [When(@"I Click New Source Button From HttpWebDelete Tool")]
        [Then(@"I Click New Source Button From HttpWebDelete Tool")]
        public void Click_NewSourceButton_From_HttpWebDeleteTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.NewSourceButton);
        }

        [Given(@"I Click New Source Button From HttpWebGet Tool")]
        [When(@"I Click New Source Button From HttpWebGet Tool")]
        [Then(@"I Click New Source Button From HttpWebGet Tool")]
        public void Click_NewSourceButton_From_HttpWebGetTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebGet.LargeView.NewSourceButton);
        }

        [Given(@"I Click New Source Button From HttpWebPost Tool")]
        [When(@"I Click New Source Button From HttpWebPost Tool")]
        [Then(@"I Click New Source Button From HttpWebPost Tool")]
        public void Click_NewSourceButton_From_HttpWebPostTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPost.LargeView.NewSourceButton);
        }

        [Given(@"I Click New Source Button From HttpWebPut Tool")]
        [When(@"I Click New Source Button From HttpWebPut Tool")]
        [Then(@"I Click New Source Button From HttpWebPut Tool")]
        public void Click_NewSourceButton_From_HttpWebPutTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPut.LargeView.NewSourceButton);
        }

        [Given(@"I Click New Source Button From DropboxDelete Tool")]
        [When(@"I Click New Source Button From DropboxDelete Tool")]
        [Then(@"I Click New Source Button From DropboxDelete Tool")]
        public void Click_NewSourceButton_From_DropboxDeleteTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DropboxDelete.LargeViewContent.NewSourceButton);
        }

        [Given(@"I Click New Source Button From DropboxDownload Tool")]
        [When(@"I Click New Source Button From DropboxDownload Tool")]
        [Then(@"I Click New Source Button From DropboxDownload Tool")]
        public void Click_NewSourceButton_From_DropboxDownloadTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DropboxDownload.LargeViewContent.NewSourceButton);
        }

        [Given(@"I Click New Source Button From DropboxListContents Tool")]
        [When(@"I Click New Source Button From DropboxListContents Tool")]
        [Then(@"I Click New Source Button From DropboxListContents Tool")]
        public void Click_NewSourceButton_From_DropboxListContentsTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DropboxFileList.LargeViewContent.NewSourceButton);
        }

        [Given(@"I Click New Source Button From DropboxUpload Tool")]
        [When(@"I Click New Source Button From DropboxUpload Tool")]
        [Then(@"I Click New Source Button From DropboxUpload Tool")]
        public void Click_NewSourceButton_From_DropboxUploadTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DropboxUpload.LargeViewContent.NewSourceButton);
        }

        [Given(@"I Click New Source Button From SMTPSend Tool")]
        [When(@"I Click New Source Button From SMTPSend Tool")]
        [Then(@"I Click New Source Button From SMTPSend Tool")]
        public void Click_NewSource_From_SMTPSendTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SMTPEmail.LargeViewContent.SourceComboBox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SMTPEmail.LargeViewContent.SourceComboBox.NewEmailSource);
        }

        [Given(@"I Click New Source Button From SharepointCopyFile Tool")]
        [When(@"I Click New Source Button From SharepointCopyFile Tool")]
        [Then(@"I Click New Source Button From SharepointCopyFile Tool")]
        public void Click_NewSource_From_SharepointCopyFileTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCopyFile.LargeView.Server);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCopyFile.LargeView.Server.NewSharePointSource);
        }

        [Given(@"I Click New Source Button From SharepointCreateListItem Tool")]
        [When(@"I Click New Source Button From SharepointCreateListItem Tool")]
        [Then(@"I Click New Source Button From SharepointCreateListItem Tool")]
        public void Click_NewSource_From_SharepointCreateListItemTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCreateListItem.LargeView.Server);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCreateListItem.LargeView.Server.NewSharePointSource);
        }

        [Given(@"I Click New Source Button From SharepointDeleteFile Tool")]
        [When(@"I Click New Source Button From SharepointDeleteFile Tool")]
        [Then(@"I Click New Source Button From SharepointDeleteFile Tool")]
        public void Click_NewSource_From_SharepointDeleteFileTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDeleteFile.LargeView.Server);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDeleteFile.LargeView.Server.NewSharePointSource);
        }

        [Given(@"I Click New Source Button From SharepointDeleteListItem Tool")]
        [When(@"I Click New Source Button From SharepointDeleteListItem Tool")]
        [Then(@"I Click New Source Button From SharepointDeleteListItem Tool")]
        public void Click_NewSource_From_SharepointDeleteListItemTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDeleteListItem.LargeView.Server);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDeleteListItem.LargeView.Server.NewSharePointSource);
        }

        [Given(@"I Click New Source Button From SharepointDownloadFile Tool")]
        [When(@"I Click New Source Button From SharepointDownloadFile Tool")]
        [Then(@"I Click New Source Button From SharepointDownloadFile Tool")]
        public void Click_NewSource_From_SharepointDownloadFileTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDownloadFile.LargeView.Server);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDownloadFile.LargeView.Server.NewSharePointSource);
        }

        [Given(@"I Click New Source Button From SharepointUploadFile Tool")]
        [When(@"I Click New Source Button From SharepointUploadFile Tool")]
        [Then(@"I Click New Source Button From SharepointUploadFile Tool")]
        public void Click_NewSource_From_SharepointUploadFileTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUploadFile.LargeView.Server);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUploadFile.LargeView.Server.NewSharePointSource);
        }

        [Given(@"I Click New Source Button From SharepointMoveFile Tool")]
        [When(@"I Click New Source Button From SharepointMoveFile Tool")]
        [Then(@"I Click New Source Button From SharepointMoveFile Tool")]
        public void Click_NewSource_From_SharepointMoveFileTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointMoveFile.LargeView.Server);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointMoveFile.LargeView.Server.NewSharePointSource);
        }

        [Given(@"I Click New Source Button From SharepointReadFolder Tool")]
        [When(@"I Click New Source Button From SharepointReadFolder Tool")]
        [Then(@"I Click New Source Button From SharepointReadFolder Tool")]
        public void Click_NewSource_From_SharepointReadFolderTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadFolder.LargeView.Server);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadFolder.LargeView.Server.NewSharePointSource);
        }

        [Given(@"I Click New Source Button From SharepointReadListItem Tool")]
        [When(@"I Click New Source Button From SharepointReadListItem Tool")]
        [Then(@"I Click New Source Button From SharepointReadListItem Tool")]
        public void Click_NewSource_From_SharepointReadListItemTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadListItem.LargeView.Server);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadListItem.LargeView.Server.NewSharePointSource);
        }

        [Given(@"I Click New Source Button From SharepointUpdateListItem Tool")]
        [When(@"I Click New Source Button From SharepointUpdateListItem Tool")]
        [Then(@"I Click New Source Button From SharepointUpdateListItem Tool")]
        public void Click_NewSource_From_SharepointUpdateListItemTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUpdateListItem.LargeView.Server);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUpdateListItem.LargeView.Server.NewSharePointSource);
        }

        [Given(@"I Click NewVersion button")]
        [When(@"I Click NewVersion button")]
        [Then(@"I Click NewVersion button")]
        public void Click_NewVersion_button()
        {
            Assert.IsTrue(MainStudioWindow.SideMenuBar.NewVersionButton.Enabled, "New version available button is disabled");
            Mouse.Click(MainStudioWindow.SideMenuBar.NewVersionButton, new Point(17, 9));
        }

        [Given(@"I Click Output OnRecordset InVariableList")]
        [When(@"I Click Output OnRecordset InVariableList")]
        [Then(@"I Click Output OnRecordset InVariableList")]
        public void Click_Output_OnRecordset_InVariableList()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.RecordsetTreeItem.TreeItem1.OutputCheckbox.Checked = true;
        }

        [Given(@"I Click Output OnVariable InVariableList")]
        [When(@"I Click Output OnVariable InVariableList")]
        [Then(@"I Click Output OnVariable InVariableList")]
        public void Click_Output_OnVariable_InVariableList()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.OutputCheckbox.Checked = true;
        }

        [Given(@"I Click Pin Toggle DebugOutput")]
        [When(@"I Click Pin Toggle DebugOutput")]
        [Then(@"I Click Pin Toggle DebugOutput")]
        public void Click_Pin_Toggle_DebugOutput()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.DebugOutputUnpinBtn, new Point(11, 10));
        }

        [Given(@"I Click Pin Toggle Documentor")]
        [When(@"I Click Pin Toggle Documentor")]
        [Then(@"I Click Pin Toggle Documentor")]
        public void Click_Pin_Toggle_Documentor()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Help.DocumentorUnpinBtn, new Point(2, 11));
        }

        [Given(@"I Click Pin Toggle Explorer")]
        [When(@"I Click Pin Toggle Explorer")]
        [Then(@"I Click Pin Toggle Explorer")]
        public void Click_Pin_Toggle_Explorer()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerUnpinBtn, new Point(12, 9));
        }

        [Given(@"I Click Pin Toggle Toolbox")]
        [When(@"I Click Pin Toggle Toolbox")]
        [Then(@"I Click Pin Toggle Toolbox")]
        public void Click_Pin_Toggle_Toolbox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolUnpinBtn, new Point(10, 8));
        }

        [Given(@"I Click Pin Toggle VariableList")]
        [When(@"I Click Pin Toggle VariableList")]
        [Then(@"I Click Pin Toggle VariableList")]
        public void Click_Pin_Toggle_VariableList()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.VariableUnpinBtn, new Point(10, 14));
        }

        [Given(@"I Click Position Button")]
        [When(@"I Click Position Button")]
        [Then(@"I Click Position Button")]
        public void Click_Position_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.FilesMenu.PositionButton, new Point(8, 7));
        }

        [Given(@"I Click Postgre Done Button")]
        [When(@"I Click Postgre Done Button")]
        [Then(@"I Click Postgre Done Button")]
        public void Click_Postgre_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PostgreSqlActivitCustom.DoneButton, new Point(36, 11));
        }

        [Given(@"I Click PrefixContainsInvalidText Hyperlink")]
        [When(@"I Click PrefixContainsInvalidText Hyperlink")]
        [Then(@"I Click PrefixContainsInvalidText Hyperlink")]
        public void Click_PrefixContainsInvalidText_Hyperlink()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PrefixcontainsinvaliText.PrefixcontainsinvaliHyperlink, new Point(30, 4));
        }

        public void Expand_DotnetDll_ByClickingCheckbox(bool isChecked)
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.DotnetDllTreeItem.ExpansionIndicatorCheckBox);
        }

        public void SetConstructorAssertValue(string value)
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.DotnetDllTreeItem.ConstructorExpander.StepOutputs_ctor_Table.ItemRow.Cell2.AssertValue_id1tyComboBox.TextEdit.Text = value;
        }

        public void SetConstructorVariable(string value)
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.DotnetDllTreeItem.ConstructorExpander.StepOutputs_ctor_Table.ItemRow.Cell.AssertValue_humanEdit.Text = value;
        }

        public void Click_TestViewDotNet_DLL_Constructor_DeleteButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.DotnetDllTreeItem.ConstructorExpander.UIWarewolfStudioViewMoButton.DeleteButton);
        }

        public void Click_TestViewDotNet_DLL_FavouriteFood_DeleteButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.DotnetDllTreeItem.FavouriteFoodsExpander.WarewolfStudioViewMoButton.DeleteButton);
        }

        [Given(@"I Click Read Done Button")]
        [When(@"I Click Read Done Button")]
        [Then(@"I Click Read Done Button")]
        public void Click_Read_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FileRead.DoneButton, new Point(35, 6));
        }

        [Given(@"I Click ReadFolder Done Button")]
        [When(@"I Click ReadFolder Done Button")]
        [Then(@"I Click ReadFolder Done Button")]
        public void Click_ReadFolder_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FolderRead.DoneButton, new Point(35, 6));
        }

        [Given(@"I Click Remove Unused Variables")]
        [When(@"I Click Remove Unused Variables")]
        [Then(@"I Click Remove Unused Variables")]
        public void Click_Remove_Unused_Variables()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.RemoveUnused, new Point(30, 4));
        }

        [Given(@"I Click Rename Done Button")]
        [When(@"I Click Rename Done Button")]
        [Then(@"I Click Rename Done Button")]
        public void Click_Rename_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathRename.DoneButton, new Point(35, 6));
        }

        [When(@"I Click RequireAllFieldsToMatch CheckBox")]
        public void Click_RequireAllFieldsToMatch_CheckBox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindRecordsIndex.LargeViewContentCustom.RequireAllFieldsToMatchCheckBox.Checked = true;
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindRecordsIndex.LargeViewContentCustom.RequireAllFieldsToMatchCheckBox, "{Tab}", ModifierKeys.None);
        }

        [When(@"I Click Reset Perfomance Counter")]
        public void Click_Reset_Perfomance_Counter()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.PerfomanceCounterTab.PerfmonViewContent.ResetCounter.ItemHyperlink, new Point(49, 9));
            Assert.IsTrue(MessageBoxWindow.Exists, "MessageBoxWindow did not show after clicking reset counters");
            Mouse.Click(MessageBoxWindow.OKButton, new Point(50, 12));
        }

        [Given(@"I Click SaveDialog CancelButton")]
        [When(@"I Click SaveDialog CancelButton")]
        [Then(@"I Click SaveDialog CancelButton")]
        public void Click_SaveDialog_CancelButton()
        {
            Mouse.Click(SaveDialogWindow.CancelButton, new Point(6, 7));
        }

        [When(@"I Click Scheduler Create New Task Button")]
        public void Click_Scheduler_NewTaskButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SchedulerTab.WorkSurfaceContext.SchedulerView.SchedulesList.ScheduleNewTaskListItem.SchedulerNewTaskButton, new Point(151, 13));
        }

        public void Click_SchedulerTab_CloseButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SchedulerTab.CloseButton);
        }

        [When(@"I Click Hello World Erase Schedule Button")]
        public void Click_HelloWorldSchedule_EraseSchedulerButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SchedulerTab.WorkSurfaceContext.SchedulerView.SchedulesList.GenericResourceListItem.EraseScheduleButton, new Point(6, 16));
        }

        [When(@"I Click Scheduler Enable Disable Checkbox Button")]
        public void Click_HelloWorldSchedule_EnableOrDisableCheckbox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SchedulerTab.WorkSurfaceContext.SchedulerView.SchedulesList.GenericResourceListItem.EnableOrDisableCheckBox);
        }

        [When(@"I Click Scheduler ResourcePicker Button")]
        public void Click_Scheduler_ResourcePickerButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SchedulerTab.WorkSurfaceContext.SchedulerView.ResourcePickerButton, new Point(14, 13));
        }

        [When(@"I Click Select Resource Button")]
        public void Click_Select_ResourceButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.PerfomanceCounterTab.PerfmonViewContent.ResourceTable.Row1.ResourceCell.ResourceButton, new Point(9, 8));
        }

        [When(@"I Click Select Resource Button From Resource Permissions")]
        public void Click_Select_Resource_Button_From_Resource_Permissions()
        {
            Mouse.Click(FindAddResourceButton(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1), new Point(13, 16));
            Assert.IsTrue(ServicePickerDialog.Exists, "Service window does not exist after clicking SelectResource button");
        }

        [When(@"I Click Select Windows Group Cancel Button")]
        public void Click_Select_Windows_Group_Cancel_Button()
        {
            Assert.IsTrue(SelectWindowsGroupDialog.CancelPanel.Cancel.Exists, "Select Windows group dialog cancel buttton does not exist.");
            Mouse.Click(SelectWindowsGroupDialog.CancelPanel.Cancel, new Point(28, 9));
        }

        [When(@"I Click Select Windows Group OK Button")]
        public void Click_Select_Windows_Group_OK_Button()
        {
            Mouse.Click(SelectWindowsGroupDialog.OKPanel.OK, new Point(37, 9));
        }

        [When(@"I Click Server Log File Button")]
        public void Click_Server_Log_File_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.LoggingTab.LogSettingsViewConte.ServerLogs.ServerLogFile.ItemHyperlink, new Point(83, 6));
        }

        [When(@"I Click Server Source Wizard Address Protocol Dropdown")]
        public void Click_Server_Source_Wizard_Address_Protocol_Dropdown()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.NewServerSource.ProtocolCombobox.ToggleDropdown, new Point(54, 8));
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsHttp.Exists, "Http does not exist in server source wizard address protocol dropdown list.");
        }

        [When(@"I Click Server Source Wizard Test Connection Button")]
        public void Click_Server_Source_Wizard_Test_Connection_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.NewServerSource.TestConnectionButton, new Point(51, 8));
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.NewServerSource.Spinner);
        }

        [When(@"I Click Service Picker Dialog First Service In Explorer")]
        public void Click_Service_Picker_Dialog_First_Service_In_Explorer()
        {
            Mouse.Click(ServicePickerDialog.Explorer.ExplorerTree.Localhost.TreeItem1, new Point(91, 9));
        }

        [When(@"I Click Settings Security Resource Permissions Add Resource Button")]
        public void Click_Settings_Security_Resource_Permissions_Add_Resource_Button()
        {
            Mouse.Click(FindAddResourceButton(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1), new Point(6, 15));
        }

        [When(@"I Click Sharepoint Server Source TestConnection")]
        public void Click_Sharepoint_Server_Source_TestConnection()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SharepointServerSourceTab.SharepointServerSourceView.SharepointView.TestConnectionButton, new Point(58, 16));
        }

        [When(@"I Click Studio Log File")]
        public void Click_Studio_Log_File()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.LoggingTab.LogSettingsViewConte.StudioLogs.StudioLogFile.ItemHyperlink, new Point(79, 10));
        }

        [Given(@"I Click Switch Dialog Done Button")]
        [When(@"I Click Switch Dialog Done Button")]
        [Then(@"I Click Switch Dialog Done Button")]
        public void Click_Switch_Dialog_Done_Button()
        {
            Mouse.Click(DecisionOrSwitchDialog.DoneButton, new Point(24, 7));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Switch.Exists, "Switch on the design surface does not exist");
        }

        [When(@"I Click System Information Tool Done Button")]
        public void Click_System_Information_Tool_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.GatherSystemInfo.DoneButton, new Point(35, 6));
        }

        [When(@"I Click UnDock Explorer")]
        public void Click_UnDock_Explorer()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerUnpinBtn, new Point(177, -13));
        }

        [When(@"I Click Unpinned Workflow CollapseAll")]
        public void Click_Unpinned_Workflow_CollapseAll()
        {
            Assert.IsTrue(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.CollapseAllToggleButton.Exists, "Expand all button does not exist");
            MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.CollapseAllToggleButton.Pressed = true;
        }

        [When(@"I Click Unpinned Workflow ExpandAll")]
        public void Click_Unpinned_Workflow_ExpandAll()
        {
            Assert.IsTrue(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ExpandAllToggleButton.Exists, "Expand all button does not exist");
            MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ExpandAllToggleButton.Pressed = true;
            Assert.IsTrue(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.Exists, "Assign tool large view on the design surface does not exist");
        }

        [When(@"I Click UnZip Done Button")]
        public void Click_UnZip_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.UnZip.DoneButton, new Point(35, 6));
        }

        [When(@"I Click UpdateDuplicateRelationships")]
        public void Click_UpdateDuplicateRelationships()
        {
            SaveDialogWindow.UpdateDuplicatedRelat.Checked = true;
        }

        [When(@"I Click View Api From Context Menu")]
        public void Click_View_Api_From_Context_Menu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(85, 11));
            Mouse.Click(MainStudioWindow.ExplorerEnvironmentContextMenu.ViewApisJsonMenuItem, new Point(71, 13));
        }

        [When(@"I Click ViewSwagger From ExplorerContextMenu")]
        public void Click_ViewSwagger_From_ExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(107, 9));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.ViewSwagger, new Point(82, 16));
        }

        [When(@"I Click Yes On The Confirm Delete")]
        public void Click_Yes_On_The_Confirm_Delete()
        {
            Mouse.Click(MessageBoxWindow.YesButton, new Point(39, 17));
        }

        [When(@"I Delete Nested Hello World")]
        public void Delete_Nested_Hello_World()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem, MouseButtons.Right, ModifierKeys.None, new Point(93, 14));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Delete, new Point(61, 15));
            Mouse.Click(MessageBoxWindow.YesButton, new Point(7, 12));
        }

//TODO: Start of proposed new UI map "Tools\UIMap.WorkflowDesigner.uitest"

        [When(@"I Click SQL Server Large View Done Button")]
        public void Click_SQL_Server_Large_View_Done_Button()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.DoneButton.Exists, "SQL Server large view done button does not exist.");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.DoneButton, new Point(35, 6));
        }

        [When(@"I Click SQL Server Large View Generate Outputs")]
        public void Click_SQL_Server_Large_View_Generate_Outputs()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.GenerateOutputsButton.Exists, "SQL Server large view does not contain a generate outputs button.");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.GenerateOutputsButton, new Point(7, 7));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.TestInputsTable.Row1.TestDataCell.TestDataComboBox.TestDataTextbox.Exists, "SQL Server large view test inputs row 1 test data textbox does not exist.");
        }

        [When(@"I Click SQL Server Large View Test Inputs Button")]
        public void Click_SQL_Server_Large_View_Test_Inputs_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.TestInputsButton, new Point(21, 11));
        }

        [When(@"I Click SQL Server Large View Test Inputs Done Button")]
        public void Click_SQL_Server_Large_View_Test_Inputs_Done_Button()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.TestInputsDoneButton.Exists, "SQL Server large view test inputs done button does not exist.");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.TestInputsDoneButton, new Point(35, 6));
        }

        [When(@"I Click SqlBulkInsert Done Button")]
        public void Click_SqlBulkInsert_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlBulkInsert.DoneButton, new Point(35, 6));
        }

        [When(@"I Click Start Node")]
        public void Click_Start_Node()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.StartNode, new Point(29, 76));
        }

        [When(@"I Click WebRequest Tool Large View Done Button")]
        public void Click_WebRequest_Tool_Large_View_Done_Button()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebRequest.DoneButton, new Point(35, 6));
        }

        [When(@"I Click Write Done Button")]
        public void Click_Write_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FileWrite.DoneButton, new Point(35, 6));
        }

        [When(@"I Click Variable IsInput")]
        public void Click_Variable_IsInput()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.InputCheckbox.Checked = true;
        }

        [When(@"I Click VariableList Scalar Row1 IsInputCheckbox")]
        public void Click_VariableList_Scalar_Row1_IsInputCheckbox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.InputCheckbox.Checked = true;
        }

        [When(@"I Click VariableList Scalar Row2 IsInputCheckbox")]
        public void Click_VariableList_Scalar_Row2_IsInputCheckbox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem2.InputCheckbox.Checked = true;
        }

        [When(@"I Click VariableList Recordset Row1 IsInputCheckbox")]
        public void Click_VariableList_Recordset_Row1_IsInputCheckbox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.RecordsetTreeItem.TreeItem1.Field1.InputCheckbox.Checked = true;
        }

        [When(@"I Click VariableList Scalar Row1 Delete Button")]
        public void Click_VariableList_Scalar_Row1_Delete_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.ScrollViewerPane.NameTextbox.DeleteButton.Image, new Point(5, 8));
        }

        public void DeleteAssign_FromContextMenu()
        {
            #region Variable Declarations
            WpfMenuItem delete = MainStudioWindow.DesignSurfaceContextMenu.Delete;
            WpfWindow messageBoxWindow = MessageBoxWindow;
            WpfCustom multiAssign = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign;
            #endregion

            var point = new Point();
            Assert.IsTrue(multiAssign.TryGetClickablePoint(out point));
            // Right-Click 'DsfMultiAssignActivity' custom control
            Mouse.Click(multiAssign, MouseButtons.Right, ModifierKeys.None, new Point(115, 10));

            // Click 'Delete' menu item
            Mouse.Click(delete, new Point(27, 18));
            Assert.IsFalse(multiAssign.TryGetClickablePoint(out point));
        }

        [Then(@"The Case Dialog Must Be Open")]
        public void ThenTheCaseDialogMustBeOpen()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Switch);
            Assert.IsTrue(DecisionOrSwitchDialog.Exists, "Switch case dialog does not exist after dragging onto switch case arm.");
            Mouse.Click(DecisionOrSwitchDialog.DoneButton);
        }

        [When(@"I Click Assign tool VariableTextbox")]
        public void Click_Assign_tool_VariableTextbox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox);
        }

        [When(@"I Click Assign tool ValueTextbox")]
        public void Click_Assign_tool_ValueTextbox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.ValueCell.IntellisenseCombobox.Textbox);
        }

        [When(@"I Click Zip Done Button")]
        public void Click_Zip_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Zip.DoneButton, new Point(35, 6));
        }

        [When(@"I Close Data Merge LargeView")]
        public void Open_DataMergeTool_SmallView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataMerge, new Point(257, 7));
        }

        [When(@"I CopyAndPaste Decision Tool On The Designer")]
        public void CopyAndPaste_Decision_Tool_On_The_Designer()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Decision, MouseButtons.Right, ModifierKeys.None, new Point(64, 15));
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.Copy, new Point(64, 15));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, MouseButtons.Right, ModifierKeys.None, new Point(64, 15));
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.Paste, new Point(64, 15));
        }

        public void Enter_Values_Into_DataMergeTool()
        {
            var row1InputVariabComboBox = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataMerge.LargeView.DataGrid.Row.InputCell.Row1InputVariabComboBox;
            var row1UsingComboBox = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataMerge.LargeView.DataGrid.Row.UsingCell.Row1UsingComboBox;
            var row2InputVariabComboBox = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataMerge.LargeView.DataGrid.Row2.InputCell.Row2InputVariabComboBox;
            var row2UsingComboBox = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataMerge.LargeView.DataGrid.Row2.UsingCell.Row2UsingComboBox;

            row1InputVariabComboBox.TextEdit.Text = "VarA";
            row1UsingComboBox.TextEdit.Text = "1";
            row2InputVariabComboBox.TextEdit.Text = "VarB";
            row2UsingComboBox.TextEdit.Text = "2";

            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataMerge.LargeView.DataGrid.Row3.MergeTypeCell.Row4MergeTypeComboBox, ModifierKeys.None);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataMerge.LargeView.DataGrid.Row3.MergeTypeCell.Row4MergeTypeComboBox.NewLineListItem, ModifierKeys.None);
        }

        [Given(@"I Drag Toolbox Comment Onto Switch Left Arm On DesignSurface")]
        [When(@"I Drag Toolbox Comment Onto Switch Left Arm On DesignSurface")]
        [Then(@"I Drag Toolbox Comment Onto Switch Left Arm On DesignSurface")]
        public void First_Drag_Toolbox_Comment_Onto_Switch_Left_Arm_On_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Comment";
            var switchLeftAutoConnector = new Point(250, 200);
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(switchLeftAutoConnector);
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.Comment, new Point(16, 25));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, switchLeftAutoConnector);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector2.Exists, "Second connector does not exist on design surface after drop onto autoconnector.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Comment.Exists, "Comment tool does not exist on the design surface after drag and drop from the toolbox.");
        }

        public void Resize_Assign_LargeTool()
        {
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.DoneButton.ItemIndicator, new Point(13, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(500, 562));
        }

        [Given(@"I Drag Toolbox Comment Onto Switch Right Arm On DesignSurface")]
        [When(@"I Drag Toolbox Comment Onto Switch Right Arm On DesignSurface")]
        [Then(@"I Drag Toolbox Comment Onto Switch Right Arm On DesignSurface")]
        public void Then_Drag_Toolbox_Comment_Onto_Switch_Right_Arm_On_DesignSurface()
        {
            #region Variable Declarations
            WpfEdit searchTextBox = MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox;
            WpfListItem commentToolboxItem = MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.Comment;
            WpfCustom flowchart = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart;
            WpfCustom connector3 = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector3;
            WpfCustom commentOnTheDesignSurface = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Comment;
            #endregion

            var switchRightAutoConnector = new Point(360, 200);
            flowchart.EnsureClickable(switchRightAutoConnector);
            Mouse.StartDragging(commentToolboxItem, new Point(16, 25));
            Mouse.StopDragging(flowchart, switchRightAutoConnector);
            Assert.IsTrue(DecisionOrSwitchDialog.DoneButton.Exists, "Switch case dialog done button does not exist after dragging onto switch case arm.");
        }

        [When(@"I DisplayStartNodeContextMenu")]
        public void DisplayStartNodeContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.StartNode, MouseButtons.Right, ModifierKeys.None, new Point(179, 31));
        }

        [When(@"I DoubleClick Explorer First Remote Server First Item")]
        public void DoubleClick_Explorer_First_Remote_Server_First_Item()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem, new Point(63, 11));
        }

        [When(@"I First Drag Toolbox Comment Onto Switch Left Arm On DesignSurface")]
        public void WhenIFirstDragToolboxCommentOntoSwitchLeftArmOnDesignSurface()
        {
            First_Drag_Toolbox_Comment_Onto_Switch_Left_Arm_On_DesignSurface();
        }

        [When(@"I Then Drag Toolbox Comment Onto Switch Right Arm On DesignSurface")]
        public void WhenIThenDragToolboxCommentOntoSwitchRightArmOnDesignSurface()
        {
            #region Variable Declarations
            WpfEdit searchTextBox = MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox;
            WpfListItem commentToolboxItem = MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.Comment;
            WpfCustom flowchart = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart;
            WpfCustom connector3 = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector3;
            WpfCustom commentOnTheDesignSurface = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Comment;
            #endregion

            var switchRightAutoConnector = new Point(360, 200);
            flowchart.EnsureClickable(switchRightAutoConnector);
            Mouse.StartDragging(commentToolboxItem, new Point(16, 25));
            Mouse.StopDragging(flowchart, switchRightAutoConnector);
            Assert.IsTrue(DecisionOrSwitchDialog.Exists, "DecisionSwitch Dialog did not open");
            Mouse.Click(DecisionOrSwitchDialog.DoneButton, new Point(34, 10));
            Assert.IsTrue(connector3.Exists, "Third connector does not exist on design surface after drop onto autoconnector.");
            Assert.IsTrue(commentOnTheDesignSurface.Exists, "Comment tool does not exist on the design surface after drag and drop from the toolbox.");
        }

        [When(@"I Drag Dice Onto Dice On The DesignSurface")]
        public void Drag_Dice_Onto_Dice_On_The_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(353, 479));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem, new Point(49, 10));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(353, 479));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector2.Exists, "Second connector does not exist on design surface.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ExternalWorkFlow.DoneButton.Exists, "Done button does not exist afer dragging dice service onto design surface");
        }

        [When(@"I Drag DotNet DLL Connector Onto DesignSurface")]
        public void Drag_DotNetDLLConnector_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "DotNet DLL";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(308, 127));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.ResourceTools.DotNetDLL, new Point(16, 25));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(308, 127));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Com DLL Connector Onto DesignSurface")]
        public void Drag_ComDLLConnector_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Com DLL";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(308, 127));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.ResourceTools.ComDLL, new Point(16, 25));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(308, 127));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag WCF Service Connector Onto DesignSurface")]
        public void Drag_WCFServiceConnector_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "WCF";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(308, 127));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.ResourceTools.WCF, new Point(16, 25));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(308, 127));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Explorer Localhost First Item Onto Workflow Design Surface")]
        public void Drag_Explorer_Localhost_First_Item_Onto_Workflow_Design_Surface()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.Exists, "No items to drag found in the explorer tree.");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, new Point(64, 5));
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, new Point(64, 5));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Explorer Localhost First Items First Sub Item Onto Workflow Design Surface")]
        public void Drag_Explorer_Localhost_First_Items_First_Sub_Item_Onto_Workflow_Design_Surface()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem.Exists, "No items to drag found in the explorer tree.");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem, new Point(90, 10));
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem, new Point(90, 10));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Explorer Localhost Second Items First Sub Item Onto Workflow Design Surface")]
        public void Drag_Explorer_Localhost_Second_Item_Onto_Workflow_Design_Surface()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem.Exists, "No items to drag found in the explorer tree.");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem, new Point(90, 10));
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem, new Point(90, 10));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }


        [Given(@"I Drag Explorer Remote GenericResource Onto Workflow Design Surface")]
        [When(@"I Drag Explorer Remote GenericResource Onto Workflow Design Surface")]
        [Then(@"I Drag Explorer Remote GenericResource Onto Workflow Design Surface")]
        public void Drag_Explorer_Remote_workflow1_Onto_Workflow_Design_Surface()
        {
            if (MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem.ItemEdit.Text != "GenericResource")
                Click_Explorer_Refresh_Button();
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem.ItemEdit.Text == "GenericResource", "Explorer first remote server first item is not Generic Resource.");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem, new Point(64, 5));
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem, new Point(64, 5));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SubWorkflow.Exists, "Workflow on the design surface does not exist");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [Given(@"I Drag Explorer workflow Onto Workflow Design Surface")]
        [When(@"I Drag Explorer workflow Onto Workflow Design Surface")]
        [Then(@"I Drag Explorer workflow Onto Workflow Design Surface")]
        public void Drag_Explorer_workflow_Onto_Workflow_Design_Surface()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.Exists, "Explorer first remote server does not contain any items.");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, new Point(64, 5));
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, new Point(64, 5));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.HelloWorldWorkFlow.Exists, "Workflow on the design surface does not exist");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I change Hello World input variable")]
        public void Change_HelloWorld_InputVariable()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.HelloWorldWorkFlow.ServiceDesignerLargeView.InputsDataGridTable.InputsGridRowOne.InputsGridRowOneCell.InputsAutoCompleteTextBox.InputsAutoCompleteTextBoxText.Text = "NewName";
            Keyboard.SendKeys( "{Tab}", ModifierKeys.None);
            Assert.AreEqual("[[NewName]]", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.HelloWorldWorkFlow.ServiceDesignerLargeView.InputsDataGridTable.InputsGridRowOne.InputsGridRowOneCell.InputsAutoCompleteTextBox.InputsAutoCompleteTextBoxText.Text);
        }

        [When(@"I change Hello World output variable")]
        public void Change_HelloWorld_OutputVariable()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.HelloWorldWorkFlow.ServiceDesignerLargeView.OutputsDataGridTable.OutputsGridRowOne.OutputsGridRowOneCell.OutputsAutoCompleteTextBox.OutputsAutoCompleteTextBoxText.Text = "NewMessage";
            Keyboard.SendKeys( "{Tab}", ModifierKeys.None);
            Assert.AreEqual("[[NewMessage]]", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.HelloWorldWorkFlow.ServiceDesignerLargeView.OutputsDataGridTable.OutputsGridRowOne.OutputsGridRowOneCell.OutputsAutoCompleteTextBox.OutputsAutoCompleteTextBoxText.Text);
        }

        [When(@"I open ""(.*)"" in Remote Connection Integration")]
        public void WhenIOpenInRemoteConnectionIntegration(string resourceName)
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem.Exists, "Explorer first remote server does not contain any items.");
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem, new Point(64, 5));
        }

        [When(@"I Drag Toolbox AggregateCalculate Onto DesignSurface")]
        public void Drag_Toolbox_AggregateCalculate_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Aggregate Calculate";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.AggregateCalculate, new Point(13, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox AssignObject Onto DesignSurface")]
        public void Drag_Toolbox_AssignObject_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Assign Object";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.AssignObject, new Point(13, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Base Conversion Onto DesignSurface")]
        public void Drag_Toolbox_Base_Conversion_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Base Convert";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(303, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.BaseConvert, new Point(12, 12));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(303, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Calculate Onto DesignSurface")]
        public void Drag_Toolbox_Calculate_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Calculate";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(305, 131));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.Calculate, new Point(59, -17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(305, 131));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Case Conversion Onto DesignSurface")]
        public void Drag_Toolbox_Case_Conversion_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Case Convert";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(303, 130));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.CaseConvert, new Point(19, 13));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(303, 130));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox CMD Line Onto DesignSurface")]
        public void Drag_Toolbox_CMD_Line_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "CMD Script";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(305, 122));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.ScriptingTools.CMDScript, new Point(19, 19));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(305, 122));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Comment Onto DesignSurface")]
        public void Drag_Toolbox_Comment_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Comment";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(308, 129));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.Comment, new Point(40, 15));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(308, 129));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Copy Onto DesignSurface")]
        public void Drag_Toolbox_Copy_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Copy";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(310, 129));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FileAndFTP.Copy, new Point(19, -3));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(310, 129));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Count Records Onto DesignSurface")]
        public void Drag_Toolbox_Count_Records_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Count";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 125));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.RecordsetTools.Count, new Point(13, 18));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 125));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Create Onto DesignSurface")]
        public void Drag_Toolbox_Create_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Create";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(308, 131));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FileAndFTP.Create, new Point(9, 16));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(308, 131));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Data Merge Onto DesignSurface")]
        public void Drag_Toolbox_Data_Merge_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Data Merge";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(305, 133));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.DataMerge, new Point(54, 23));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(305, 133));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Data Split Onto DesignSurface")]
        public void Drag_Toolbox_Data_Split_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Data Split";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(308, 129));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.DataSplit, new Point(3, 8));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(308, 129));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [Given(@"I Drag Toolbox Date And Time Onto DesignSurface")]
        [When(@"I Drag Toolbox Date And Time Onto DesignSurface")]
        [Then(@"I Drag Toolbox Date And Time Onto DesignSurface")]
        public void Drag_Toolbox_Date_And_Time_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Date Time";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(304, 127));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.DateTime, new Point(20, -1));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(304, 127));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox DateTime Difference Onto DesignSurface")]
        public void Drag_Toolbox_DateTime_Difference_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Date Time Diff";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 131));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.DateTimeDifference, new Point(48, 7));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 131));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Decision Onto DesignSurface")]
        public void Drag_Toolbox_Decision_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Decision";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(309, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FlowTools.Decision, new Point(16, 11));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(309, 128));
        }

        [When(@"I Drag Toolbox Delete Onto DesignSurface")]
        public void Drag_Toolbox_Delete_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Delete";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 125));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FileAndFTP.Delete, new Point(13, 9));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 125));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Delete Record Onto DesignSurface")]
        public void Drag_Toolbox_Delete_Record_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Delete";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(309, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.RecordsetTools.Delete, new Point(43, 6));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(309, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Dropbox Delete Onto DesignSurface")]
        public void Drag_Toolbox_Dropbox_Delete_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Dropbox";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 127));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.StorageDropbox.Delete, new Point(240, 550));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 127));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Dropbox Download Onto DesignSurface")]
        public void Drag_Toolbox_Dropbox_Download_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Dropbox";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 129));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.StorageDropbox.Download, new Point(16, 6));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 129));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Dropbox FileList Onto DesignSurface")]
        public void Drag_Toolbox_Dropbox_FileList_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Dropbox";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.StorageDropbox.ListContents, new Point(124, 550));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Dropbox Upload Onto DesignSurface")]
        public void Drag_Toolbox_Dropbox_Upload_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Dropbox";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(308, 126));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.StorageDropbox.Upload, new Point(66, 550));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(308, 126));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DropboxUpload.Exists, "Dropbox upload tool does not exist on design surface after dragging in from the toolbox.");
        }

        [When(@"I Drag Toolbox Exchange Send Onto DesignSurface")]
        public void Drag_Toolbox_Exchange_Send_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Exchange Send";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(308, 129));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.Email.ExchangeSend, new Point(16, -39));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(308, 129));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Find Index Onto DesignSurface")]
        public void Drag_Toolbox_Find_Index_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Find Index";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(305, 131));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.FindIndex, new Point(9, 5));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(305, 131));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Find Record Index Onto DesignSurface")]
        public void Drag_Toolbox_Find_Record_Index_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Find Records";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 130));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.RecordsetTools.FindRecords, new Point(8, 8));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 130));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox For Each Onto DesignSurface")]
        public void Drag_Toolbox_For_Each_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "ForEach";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 129));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.LoopTools.ForEach, new Point(40, 19));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 129));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Format Number Onto DesignSurface")]
        public void Drag_Toolbox_Format_Number_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Format Number";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(305, 131));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.FormatNumber, new Point(18, 11));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(305, 131));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox HTTPGETWebTool Onto DesignSurface")]
        public void Drag_HTTPGETWebTool_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "GET";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 126));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.HTTPWebMethods.GET, new Point(16, 25));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 126));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox HTTPPOSTWebTool Onto DesignSurface")]
        public void Drag_HTTPPOSTWebTool_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "POST";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.HTTPWebMethods.POST, new Point(20, 35));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox HTTPPUTWebTool Onto DesignSurface")]
        public void Drag_HTTPPUTWebTool_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "PUT";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 126));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.HTTPWebMethods.PUT, new Point(16, 25));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 126));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox HTTPDELETEWebTool Onto DesignSurface")]
        public void Drag_HTTPDELETEWebTool_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "DELETE";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 126));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.HTTPWebMethods.DELETE, new Point(16, 25));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 126));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Javascript Onto DesignSurface")]
        public void Drag_Toolbox_Javascript_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Javascript";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 130));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.ScriptingTools.JavaScript, new Point(49, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 130));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox JSON Onto DesignSurface")]
        public void Drag_Toolbox_JSON_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Create JSON";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(305, 127));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.CreateJSON, new Point(0, 10));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(305, 127));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CreateJson.Exists, "Create JSON tool on the design surface does not exist");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Length Onto DesignSurface")]
        public void Drag_Toolbox_Length_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Length";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(308, 125));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.RecordsetTools.Length, new Point(16, 6));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(308, 125));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Move Onto DesignSurface")]
        public void Drag_Toolbox_Move_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Move";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 129));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FileAndFTP.Move, new Point(32, 4));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 129));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [Given(@"I Drag Toolbox MultiAssign Onto DesignSurface")]
        [When(@"I Drag Toolbox MultiAssign Onto DesignSurface")]
        [Then(@"I Drag Toolbox MultiAssign Onto DesignSurface")]
        public void Drag_Toolbox_MultiAssign_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Assign";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.MultiAssign, new Point(13, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [Given(@"I Make Workflow Savable")]
        [When(@"I Make Workflow Savable")]
        [Then(@"I Make Workflow Savable")]
        public void Make_Workflow_Savable_By_Dragging_Start()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.StartNode.Exists, "Start Node does not Exist");
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.StartNode);
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 128));
        }

        [Given(@"I Make Workflow Savable And Then Save")]
        [When(@"I Make Workflow Savable And Then Save")]
        [Then(@"I Make Workflow Savable And Then Save")]
        public void Make_Workflow_Savable_Then_Save()
        {
            var startNode = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.StartNode;
            Assert.IsTrue(startNode.Exists, "Start Node does not Exist");
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 128));
            Mouse.StartDragging(startNode);
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 128));
            Assert.IsTrue(MainStudioWindow.SideMenuBar.SaveButton.Enabled, "Make Workflow Savable was unsucessful.");
            Keyboard.SendKeys(startNode, "S", (ModifierKeys.Control));
        }

        [When(@"I Drag Toolbox MySql Database Onto DesignSurface")]
        public void Drag_Toolbox_MySql_Database_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "MySQL";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 130));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.Database.MySQL, new Point(10, 15));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 130));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox ODBC Dtatbase Onto DesignSurface")]
        public void Drag_Toolbox_ODBC_Dtatbase_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "ODBC";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 130));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.Database.ODBC, new Point(10, 15));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 130));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Oracle Database Onto DesignSurface")]
        public void Drag_Toolbox_Oracle_Database_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Oracle";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 130));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.Database.Oracle, new Point(11, 20));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 130));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox PostgreSql Onto DesignSurface")]
        public void Drag_Toolbox_PostgreSql_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Postgre";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 130));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.Database.Postgre, new Point(10, 15));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 130));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Python Onto DesignSurface")]
        public void Drag_Toolbox_Python_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Python";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 130));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.ScriptingTools.Python, new Point(49, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 130));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox RabbitMqConsume Onto DesignSurface")]
        public void Drag_Toolbox_RabbitMqConsume_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "RabbitMq Consume";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(309, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.RabbitMQConsume, new Point(16, 11));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(309, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox RabbitMqPublish Onto DesignSurface")]
        public void Drag_Toolbox_RabbitMqPublish_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "RabbitMq Publish";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(309, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.RabbitMQPublish, new Point(16, 11));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(309, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Random Onto DesignSurface")]
        public void Drag_Toolbox_Random_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Random";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(308, 127));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.Random, new Point(9, -21));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(308, 127));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Read File Onto DesignSurface")]
        public void Drag_Toolbox_Read_File_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Read File";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(304, 125));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FileAndFTP.ReadFile, new Point(12, 15));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(304, 125));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Read Folder Onto DesignSurface")]
        public void Drag_Toolbox_Read_Folder_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Read Folder";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(305, 129));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FileAndFTP.ReadFolder, new Point(14, 3));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(305, 129));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Rename Onto DesignSurface")]
        public void Drag_Toolbox_Rename_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Rename";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(305, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FileAndFTP.Rename, new Point(6, 11));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(305, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Replace Onto DesignSurface")]
        public void Drag_Toolbox_Replace_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Replace";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 121));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.DataTools.Replace, new Point(16, 10));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 121));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Ruby Onto DesignSurface")]
        public void Drag_Toolbox_Ruby_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Ruby";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 130));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.ScriptingTools.Ruby, new Point(49, 17));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 130));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox SelectAndApply Onto DesignSurface")]
        public void Drag_Toolbox_SelectAndApply_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Select and apply";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 129));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.LoopTools.Selectandapply, new Point(40, 19));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 129));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging select and apply tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Sequence Onto DesignSurface")]
        public void Drag_Toolbox_Sequence_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Sequence";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(305, 131));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FlowTools.Sequence, new Point(18, -12));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(305, 131));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Service Picker Onto DesignSurface")]
        public void Drag_Toolbox_Service_Picker_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Service";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(304, 126));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.ResourceTools.Service, new Point(50, 5));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(304, 126));
        }

        [Given(@"I Drag Toolbox Sharepoint CopyFile Onto DesignSurface")]
        [When(@"I Drag Toolbox Sharepoint CopyFile Onto DesignSurface")]
        [Then(@"I Drag Toolbox Sharepoint CopyFile Onto DesignSurface")]
        public void Drag_Toolbox_Sharepoint_CopyFile_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Copy File";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.SharepointTools.CopyFile, new Point(10, 16));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Sharepoint Create Onto DesignSurface")]
        public void Drag_Toolbox_Sharepoint_Create_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Create List Item";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(311, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.SharepointTools.CreateListItems, new Point(10, 16));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(311, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }


        [When(@"I Drag Toolbox Sharepoint Delete List Item Onto DesignSurface")]
        public void Drag_Toolbox_Sharepoint_DeleteListItem_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Delete List Item";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 131));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.SharepointTools.DeleteListItems, new Point(16, 5));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 131));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Sharepoint Delete File Onto DesignSurface")]
        public void Drag_Toolbox_Sharepoint_DeleteFile_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Delete File";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 131));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.SharepointTools.DeleteFile, new Point(16, 5));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 131));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Sharepoint Download File Onto DesignSurface")]
        public void Drag_Toolbox_Sharepoint_Download_File_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Download";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 129));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.SharepointTools.DownloadFile, new Point(124, 593));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 129));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Sharepoint MoveFile Onto DesignSurface")]
        public void Drag_Toolbox_Sharepoint_MoveFile_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Move";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(311, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.SharepointTools.MoveFile, new Point(10, 16));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(311, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Sharepoint Read Folder Onto DesignSurface")]
        public void Drag_Toolbox_Sharepoint_Read_Folder_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Read";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(303, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.SharepointTools.ReadFolder, new Point(13, 15));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(303, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Sharepoint Read List Item Onto DesignSurface")]
        public void Drag_Toolbox_Sharepoint_Read_List_Item_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Read List Item";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(303, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.SharepointTools.ReadListItems, new Point(13, 15));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(303, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Sharepoint Update Onto DesignSurface")]
        public void Drag_Toolbox_Sharepoint_Update_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Update List Item";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(300, 127));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.SharepointTools.UpdateListItems, new Point(17, 9));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(300, 127));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Sharepoint UploadFile Onto DesignSurface")]
        public void Drag_Toolbox_Sharepoint_UploadFile_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Upload";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(311, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.SharepointTools.UploadFile, new Point(10, 16));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(311, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox SMTP Email Onto DesignSurface")]
        public void Drag_Toolbox_SMTP_Email_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "SMTP Send";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(308, 129));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.Email.SMTPSend, new Point(16, -39));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(308, 129));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Sort Record Onto DesignSurface")]
        public void Drag_Toolbox_Sort_Record_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Sort";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(300, 122));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.RecordsetTools.Sort, new Point(7, 8));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(300, 122));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox SQL Bulk Insert Onto DesignSurface")]
        public void Drag_Toolbox_SQL_Bulk_Insert_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "SQL Bulk Insert";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(304, 129));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.Database.SQLBulkInsert, new Point(10, 15));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(304, 129));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox SQL Server Tool Onto DesignSurface")]
        public void Drag_Toolbox_SQL_Server_Tool_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "SQL Server";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(304, 127));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.Database.SQLServer, new Point(10, -7));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(304, 127));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Switch Onto DesignSurface")]
        public void Drag_Toolbox_Switch_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Switch";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(303, 126));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FlowTools.Switch, new Point(22, 30));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(303, 126));
        }

        [When(@"I Drag Toolbox System Information Onto DesignSurface")]
        public void Drag_Toolbox_System_Information_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Sys Info";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(304, 129));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.SysInfo, new Point(8, 12));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(304, 129));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Unique Records Onto DesignSurface")]
        public void Drag_Toolbox_Unique_Records_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Unique";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(304, 133));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.RecordsetTools.UniqueRecords, new Point(43, 6));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(304, 133));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Unzip Onto DesignSurface")]
        public void Drag_Toolbox_Unzip_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Unzip";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FileAndFTP.UnZip, new Point(15, 15));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Web Request Onto DesignSurface")]
        public void Drag_Toolbox_Web_Request_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Web Request";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(308, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.WebRequest, new Point(14, 3));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(308, 128));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Write File Onto DesignSurface")]
        public void Drag_Toolbox_Write_File_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Write File";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 132));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FileAndFTP.WriteFile, new Point(10, 18));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 132));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox XPath Onto DesignSurface")]
        public void Drag_Toolbox_XPath_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "XPath";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(307, 123));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.UtilityTools.XPath, new Point(12, -13));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(307, 123));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [When(@"I Drag Toolbox Zip Onto DesignSurface")]
        public void Drag_Toolbox_Zip_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "Zip";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 131));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.FileAndFTP.Zip, new Point(16, 4));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 131));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Connector1.Exists, "No connectors exist on design surface after dragging tool onto start node autoconnector.");
        }

        [Given(@"I Enter ""(.*)"" Into Deploy Source Filter")]
        [When(@"I Enter ""(.*)"" Into Deploy Source Filter")]
        [Then(@"I Enter ""(.*)"" Into Deploy Source Filter")]
        public void Enter_DeployViewOnly_Into_Deploy_Source_Filter(string SearchTextboxText)
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerExplorer.SearchTextbox.Text = SearchTextboxText;
            if (SearchTextboxText.ToLower() == "localhost".ToLower()) return;
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerExplorer.ExplorerTree.SourceServerName.FirstExplorerTreeItem.Exists, "First deploy tab source explorer item does not exist after filter is applied.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerExplorer.ExplorerTree.SourceServerName.FirstExplorerTreeItem.CheckBox.Exists, "Deploy source server explorer tree first item checkbox does not exist.");
        }

        public void Filter_Deploy_Source_Explorer(string FilterText)
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerExplorer.SearchTextbox.Text = FilterText;
        }

        [When(@"I Enter Duplicate workflow name")]
        public void Enter_Duplicate_workflow_name(string name)
        {
            SaveDialogWindow.ServiceNameTextBox.Text = name;
        }

        [When(@"I Enter InputDebug value")]
        public void Enter_InputDebug_value()
        {
            Assert.IsTrue(MainStudioWindow.DebugInputDialog.TabItemsTabList.InputDataTab.InputsTable.Row1.Exists, "InputData row does not exist.");
            Assert.IsTrue(MainStudioWindow.DebugInputDialog.TabItemsTabList.InputDataTab.InputsTable.Row1.InputValueCell.InputValueComboboxl.InputValueText.Exists, "InputData row does not exist.");
            MainStudioWindow.DebugInputDialog.TabItemsTabList.InputDataTab.InputsTable.Row1.InputValueCell.InputValueComboboxl.InputValueText.Text = "100";
        }

        [When(@"I Enter LocalSchedulerAdmin Credentials Into Scheduler Tab")]
        public void Enter_LocalSchedulerAdminCredentials_Into_SchedulerTab()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SchedulerTab.WorkSurfaceContext.SchedulerView.UserNameTextBoxEdit.Text = @"Warewolf Administrators\IntegrationTester";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SchedulerTab.WorkSurfaceContext.SchedulerView.PasswordTextbox.Text = "I73573r0";            
        }

        [When(@"I Enter Public As Windows Group")]
        public void Enter_Public_As_Windows_Group()
        {
            FindWindowsGroupTextbox(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab.SecurityWindow.ResourcePermissions.Row1).Text = "Public";
        }

        [When(@"I Enter RunAsUser Username And Password")]
        public void Enter_RunAsUser_Username_And_Password()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.UsernameTextBoxEdit.Text = "testuser";
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.UsernameTextBoxEdit, "{Tab}", ModifierKeys.None);
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.PasswordTextBoxEdit, "a1cbgHEVu098QBN0jqs55wYP/bLfpGNMxw2YxtLIgKOALxPfITSBDjNERdIi/KEq", true);
        }

        [When(@"I Enter Sharepoint Server Path From OnCopyFile Tool")]
        public void Enter_Sharepoint_Server_Path_From_OnCopyFile_Tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCopyFile.SmallView.FromDirectoryComboBox.TextEdit.Text = "clocks.dat";
        }

        [When(@"I Enter Sharepoint Server Path From OnMoveFile Tool")]
        public void Enter_Sharepoint_Server_Path_From_OnMoveFile_Tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointMoveFile.SmallView.FromDirectoryComboBox.TextEdit.Text = "clocks.dat";
        }

        [When(@"I Enter Sharepoint Server Path From OnUpload Tool")]
        public void Enter_Sharepoint_Server_Path_From_OnUpload_Tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUploadFile.SmallView.LocalPathFromIntellisenseCombobox.Textbox.Text = "clocks.dat";
        }

        [When(@"I Enter Sharepoint Server Path To OnCopyFile Tool")]
        public void Enter_Sharepoint_Server_Path_To_OnCopyFile_Tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCopyFile.SmallView.PathDirectoryComboBox.TextEdit.Text = "TestFolder/clocks.dat";
        }

        [When(@"I Enter Sharepoint Server Path To OnMoveFile Tool")]
        public void Enter_Sharepoint_Server_Path_To_OnMoveFile_Tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointMoveFile.SmallView.PathDirectoryComboBox.TextEdit.Text = "TestFolder/clocks.dat";
        }

        [When(@"I Enter Sharepoint Server Path To OnUpload Tool")]
        public void Enter_Sharepoint_Server_Path_To_OnUpload_Tool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUploadFile.SmallView.ServerPathToIntellisenseCombobox.Textbox.Text = "TestFolder/clocks.dat";
        }

        [When(@"I Enter Sharepoint ServerSource ServerName")]
        public void Enter_Sharepoint_ServerSource_ServerName()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SharepointServerSourceTab.SharepointServerSourceView.SharepointView.ServerNameEdit.Text = "http://rsaklfsvrsharep/";
        }

        [When(@"I Enter Sharepoint ServerSource User Credentials")]
        public void Enter_Sharepoint_ServerSource_User_Credentials()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SharepointServerSourceTab.SharepointServerSourceView.SharepointView.UserNameTextBox.Text = "IntegrationTester";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SharepointServerSourceTab.SharepointServerSourceView.SharepointView.PasswordTextBox.Text = "I73573r0"; 
        }

        [When(@"I Enter SomeData Into Base Convert Large View Row1 Value Textbox")]
        public void Enter_SomeData_Into_BaseConvertTool_Row1ValueTextbox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.BaseConvert.LargeView.DataGrid.Row1.Cell.Listbox.ValueTextbox.Text = "SomeData";
        }

        [When(@"I Enter SomeVariable Into Calculate Large View Function Textbox")]
        public void Enter_SomeVariable_Into_CalculateTool_FXCombobox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Calculate.LargeView.FxCombobox.TextEdit.Text = "[[SomeVariable]]";
        }

        [When(@"I Enter Text Into Assign Large View Row1 Variable Textbox As SomeInvalidVariableName")]
        public void Enter_Text_Into_Assign_Large_View_Row1_Variable_Textbox_As_SomeInvalidVariableName()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text = "[[Some$Invalid%Variable]]";
            Assert.AreEqual("[[Some$Invalid%Variable]]", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text, "Multiassign small view row 1 variable textbox text does not equal \"[[Some$Invalid" +
                    "%Variable]]\".");
        }

        [Given(@"I Enter Text Into Assign Large View Row1 Variable Textbox As SomeVariable")]
        [When(@"I Enter Text Into Assign Large View Row1 Variable Textbox As SomeVariable")]
        [Then(@"I Enter Text Into Assign Large View Row1 Variable Textbox As SomeVariable")]
        public void Enter_Text_Into_Assign_Large_View_Row1_Variable_Textbox_As_SomeVariable()
        {
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox, "{Home}", ModifierKeys.Shift);
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox, "[[Some{Down}{Enter}Variable]]", ModifierKeys.None);
            Assert.AreEqual("[[SomeVariable]]", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text, "Assign large view row1 variable textbox text does not equal \"[[SomeVariable]]\" af" +
                    "ter selecting it from the intellisense using the keyboard.");
        }

        [When(@"I Enter Text Into Assign Large View Row1 Variable Textbox As SomeVariable On Unpinned Tab")]
        public void Enter_Text_Into_Assign_Large_View_Row1_Variable_Textbox_As_SomeVariable_On_Unpinned_Tab()
        {
            Keyboard.SendKeys(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox, "[[Some{Down}{Enter}Variable]]", ModifierKeys.None);
            Assert.AreEqual("[[SomeVariable]]", MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text, "Assign large view row1 variable textbox text does not equal \"[[SomeVariable]]\" on" +
                    " unpinned tab after selecting it from the intellisense using the keyboard.");
        }

        [When(@"I Enter Text Into Assign Small View Row1 Value Textbox As SomeVariable Using Click Intellisense Suggestion")]
        public void Enter_Text_Into_Assign_Small_View_Row1_Value_Textbox_As_SomeVariable_Using_Click_Intellisense_Suggestion()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text = "[[";
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.VariableCell.IntellisenseCombobox.ListItem, new Point(39, 10));
            Assert.AreEqual("[[SomeVariable]]", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text, "Multiassign small view row 1 variable textbox text does not equal \"[[SomeVariable" +
                    "]]\".");
        }

        [When(@"I Enter Text Into Assign Small View Row1 Value Textbox As SomeVariable UsingIntellisense")]
        public void Enter_Text_Into_Assign_Small_View_Row1_Value_Textbox_As_SomeVariable_UsingIntellisense()
        {
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox, "[[{Down}{Enter}", ModifierKeys.None);
            Assert.AreEqual("[[SomeVariable]]", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Textbox.Text, "Multiassign small view row 1 variable textbox text does not equal \"[[SomeVariable" +
                    "]]\".");
        }

        [When(@"I Enter Text Into Workflow Tests OutPutTable Row1 Value Textbox As CodedUITest")]
        public void Enter_Text_Into_Workflow_Tests_OutPutTable_Row1_Value_Textbox_As_CodedUITest()
        {
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestOutputsTable.Row1.Cell.IntellisenseComboBox.Textbox, "Helo User", ModifierKeys.None);
            Assert.AreEqual("Hello User", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestOutputsTable.Row1.Cell.IntellisenseComboBox.Textbox.Text, "Workflow tests output tabe row 1 value textbox text does not equal Helo User afte" +
                    "r typing that in.");
        }

        [When(@"I Enter Text Into Workflow Tests Row1 Value Textbox As CodedUITest")]
        public void Enter_Text_Into_Workflow_Tests_Row1_Value_Textbox_As_CodedUITest()
        {
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestInputsTable.Row1.Cell.IntellisenseComboBox.Textbox, "User", ModifierKeys.None);
            Assert.AreEqual("User", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestInputsTable.Row1.Cell.IntellisenseComboBox.Textbox.Text, "Workflow tests row 1 value textbox text does not equal User after typing that in." +
                    "");
        }

        [When(@"I Enter Vaiablelist Items")]
        public void Enter_Vaiablelist_Items()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.ScrollViewerPane.NameTextbox, new Point(62, 3));
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.ScrollViewerPane.NameTextbox.Text = "varableA";
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.ScrollViewerPane.NameTextbox, "{CapsLock}", ModifierKeys.None);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem2.ScrollViewerPane.NameTextbox, new Point(82, 2));
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem2.ScrollViewerPane.NameTextbox, "{CapsLock}", ModifierKeys.None);
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem2.ScrollViewerPane.NameTextbox.Text = "variableB";
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem3.ScrollViewerPane.NameTextbox, new Point(84, 2));
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem3.ScrollViewerPane.NameTextbox.Text = "VariableC";
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem3.ScrollViewerPane.NameTextbox, "{CapsLock}", ModifierKeys.None);
        }

        [When(@"I Filter variables")]
        public void Filter_variables()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.SearchTextbox.FilterText.Exists, "Variable filter textbox does not exist");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.SearchTextbox, new Point(89, 7));
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.SearchTextbox.Text = "Other";
        }

        [When(@"I Drag Explorer First Item Onto The Second Item")]
        public void Drag_Explorer_First_Item_Onto_The_Second_Item()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem.EnsureClickable(new Point(90, 11));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, new Point(94, 11));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem, new Point(90, 11));
        }

        [When(@"I Move Dice Roll To Localhost")]
        public void Move_Dice_Roll_To_Localhost()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.EnsureClickable(new Point(10, 10));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem, new Point(92, 4));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, new Point(10, 10));
        }

        [When(@"I Open AggregateCalculate Tool large view")]
        public void Open_AggregateCalculateTool_Largeview()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.AggregateCalculat, new Point(136, 13));
        }

        [When(@"I Open Assign Tool Large View")]
        public void Open_AssignTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign, new Point(145, 5));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.Exists, "Assign tool large view on the design surface does not exist");
        }

        [When(@"I Add Variables The Variable List")]
        [Given(@"I Add Variables The Variable List")]
        [Then(@"I Add Variables The Variable List")]
        public void Add_Variables(string variables)
        {
            var strings = variables.Split(',');
            if (!string.IsNullOrEmpty(strings[0]))
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.ScrollViewerPane.NameTextbox.Text = strings?[0];
            if (!string.IsNullOrEmpty(strings[1]))
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem2.ScrollViewerPane.NameTextbox.Text = strings?[1];
            if (!string.IsNullOrEmpty(strings[2]))
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem3.ScrollViewerPane.NameTextbox.Text = strings?[2];
            if (!string.IsNullOrEmpty(strings[3]))
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem4.ScrollViewerPane.NameTextbox.Text = strings?[3];
        }

        [When(@"I Add Recordset The Recordset List")]
        [Given(@"I Add Recordset The Recordset List")]
        [Then(@"I Add Recordset The Recordset List")]
        public void Add_Recordsets(string variables)
        {
            var strings = variables.Split(',');
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.RecordsetTreeItem.TreeItem1.ScrollViewerPane.NameTextbox.Text = strings?[0];
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.RecordsetTreeItem.TreeItem2.ScrollViewerPane.NameTextbox.Text = strings?[1];
        }


        [When(@"I Add Recordset Fields The Recordset List")]
        [Given(@"I Add Recordset Fields The Recordset List")]
        [Then(@"I Add Recordset Fields The Recordset List")]
        public void Add_Recordsets_Fields(string variables)
        {
            var strings = variables.Split(',');
            if (!string.IsNullOrEmpty(strings[0]))
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.RecordsetTreeItem.TreeItem2.Field1.ScrollViewerPane.NameTextbox.Text = strings?[0];
            if (!string.IsNullOrEmpty(strings[1]))
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.RecordsetTreeItem.TreeItem2.Field2.ScrollViewerPane.NameTextbox.Text = strings?[1];
            if (!string.IsNullOrEmpty(strings[0]))
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.RecordsetTreeItem.TreeItem1.Field1.ScrollViewerPane.NameTextbox.Text = strings?[0];
            if (!string.IsNullOrEmpty(strings[1]))
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.RecordsetTreeItem.TreeItem1.Field2.ScrollViewerPane.NameTextbox.Text = strings?[1];
            if (!string.IsNullOrEmpty(strings[2]))
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.RecordsetTreeItem.TreeItem1.Field3.ScrollViewerPane.NameTextbox.Text = strings?[2];
            if (!string.IsNullOrEmpty(strings[3]))
                MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.RecordsetTreeItem.TreeItem1.Field4.ScrollViewerPane.NameTextbox.Text = strings?[3];
        }

        [Given(@"I Sort Variable List")]
        [When(@"I Sort Variable List")]
        [Then(@"I Sort Variable List")]
        public void Click_Sort_Variable_List()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.Sort);
        }

        [When(@"I Open Assign Tool On Unpinned Tab Large View")]
        public void Open_Assign_Tool_On_Unpinned_Tab_Large_View()
        {
            Mouse.DoubleClick(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign, new Point(145, 5));
            Assert.IsTrue(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.Exists, "Assign tool large view on the unpinned tab design surface does not exist");
            Assert.IsTrue(MainStudioWindow.UnpinnedTab.SplitPane.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.LargeView.DataGrid.Row1.VariableCell.IntellisenseCombobox.Exists, "Assign large view row 1 variable textbox does not exist after openning large view" +
                    " with a double click on an unpinned tab.");
        }

        [When(@"I Open AssignObject Large Tool")]
        public void Open_AssignObject_Large_Tool()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.AssignObject, new Point(159, 11));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.AssignObject.DoneButton.Exists, "Done button does not exist after dragging Assign Object tool on to the workflow surface");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.AssignObject.OpenQuickVariableInput.Exists, "OpenQuickVariableInput button does not exist after dragging Assign Object tool on to the workflow surface");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.AssignObject.LargeView.DataGrid.Row1.Exists, "Row1 does not exist after dragging Assign Object tool on to the workflow surface");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.AssignObject.LargeView.OnError.OnErrorGroup.Exists, "OnErrorGroup does not exist after dragging Assign Object tool on to the workflow surface");
        }

        [When(@"I Open Base Conversion Tool Large View")]
        public void Open_BaseConvertTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.BaseConvert, new Point(160, 15));
        }

        [Given(@"I Click Base Convert Large View Done Button")]
        [When(@"I Click Base Convert Large View Done Button")]
        [Then(@"I Click Base Convert Large View Done Button")]
        public void Click_BaseConvertTool_LargeView_DoneButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.BaseConvert.DoneButton, new Point(36, 11));
        }

        [When(@"I Open Calculate Tool Large View")]
        public void Open_Calculate_Tool_Large_View()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Calculate, new Point(105, 7));
        }

        [When(@"I Open Case Conversion Tool Large View")]
        public void Open_CaseConvertTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CaseConvert, new Point(136, 13));
        }

        [When(@"I Open Context Menu On Design Surface")]
        public void Open_Context_Menu_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, MouseButtons.Right, ModifierKeys.None, new Point(304, 286));
        }

        [When(@"I Open Create JSON Large View")]
        public void Open_CreateJSON_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CreateJson, new Point(124, 9));
        }

        [When(@"I Open Copy Tool Large View")]
        public void Open_CopyTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathCopy, new Point(144, 11));
        }

        [When(@"I Open Create Tool Large View")]
        public void Open_CreateTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathCreate, new Point(118, 13));
        }

        [When(@"I Open DeleteWeb Tool Large View")]
        public void Open_DeleteWebTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete, new Point(145, 5));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.Exists, "Web delete large view does not exist on the design surface");
        }

        [When(@"I Open Delete Tool Large View")]
        public void Open_DeleteTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathDelete, new Point(118, 13));
        }

        [When(@"I Open Move Tool Large View")]
        public void Open_MoveTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathMove, new Point(125, 6));
        }

        [When(@"I Open Read File Tool Large View")]
        public void Open_ReadFileTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FileRead, new Point(120, 5));
        }

        [When(@"I Open Read Folder Tool Large View")]
        public void Open_ReadFolderTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FolderRead, new Point(138, 14));
        }

        [When(@"I Open Rename Tool Large View")]
        public void Open_RenameTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathRename, new Point(159, 11));
        }

        [Given(@"I Open Unzip Tool Large View")]
        [When(@"I Open Unzip Tool Large View")]
        [Then(@"I Open Unzip Tool Large View")]
        public void Open_UnzipTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.UnZip, new Point(102, 14));
        }

        [Given(@"I Open Write File Tool Large View")]
        [When(@"I Open Write File Tool Large View")]
        [Then(@"I Open Write File Tool Large View")]
        public void Open_WriteFileTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FileWrite, new Point(149, 13));
        }

        [Given(@"I Open Zip Tool Large View")]
        [When(@"I Open Zip Tool Large View")]
        [Then(@"I Open Zip Tool Large View")]
        public void Open_ZipTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Zip, new Point(124, 12));
        }

        [When(@"I Open Data Merge Large View")]
        public void Open_DataMerge_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataMerge, new Point(185, 9));
        }
        public void Click_DataMerge_DoneButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataMerge.DoneButton);
        }

        [When(@"I Open Data Split Large View")]
        public void Open_DataSplit_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataSplit, new Point(203, 10));
        }

        [When(@"I Open DateTime LargeView")]
        public void Open_DateTime_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DateTime, new Point(145, 7));
        }

        [When(@"I Open DateTimeDiff LargeView")]
        public void Open_DateTimeDiff_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DateTimeDifference, new Point(145, 7));
        }

        [When(@"I Open Decision Large View")]
        public void Open_Decision_LargeView()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Decision.DrawHighlight();
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Decision);
            Assert.IsTrue(DecisionOrSwitchDialog.Exists, "Decision Dialog does not exist after opening large Decision view");
        }

        [When(@"I Open ForEach Large View")]
        public void Open_ForEach_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ForEach, new Point(131, 14));
        }

        [When(@"I Open SelectAndApply Large View")]
        public void Open_SelectAndApply_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SelectAndApply, new Point(129, 10));
        }

        [When(@"I Open CountRecords Large View")]
        public void Open_CountRecords_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CountRecordset, new Point(130, 11));
        }

        [When(@"I Open DeleteRecords Large View")]
        public void Open_DeleteRecords_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DeleteRecord, new Point(133, 9));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DeleteRecord.DoneButton.Exists, "Done button does not exist after opening Delete Record large view");
        }

        [When(@"I Open Find Record Index Tool Large View")]
        public void Open_FindRecordIndexTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindRecordsIndex, new Point(172, 5));
        }

        [When(@"I Open Length Tool Large View")]
        public void Open_LengthTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Length, new Point(136, 13));
        }

        [Given(@"I Open SortRecords Large View")]
        [When(@"I Open SortRecords Large View")]
        [Then(@"I Open SortRecords Large View")]
        public void Open_SortRecords_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SortRecords, new Point(114, 13));
        }

        [Given(@"I Open UniqueRecords Large View")]
        [When(@"I Open UniqueRecords Large View")]
        [Then(@"I Open UniqueRecords Large View")]
        public void Open_UniqueRecords_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Unique, new Point(134, 10));
        }

        [When(@"I Collapse GetWebTool Large View to Small View With Double Click")]
        public void WebGetTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebGet, new Point(201, 9));
        }

        [When(@"I Collapse PostWebTool Large View to Small View With Double Click")]
        public void WebPostTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPost, new Point(201, 9));
        }

        [When(@"I Collapse PutWebTool Large View to Small View With Double Click")]
        public void WebPutTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPut, new Point(201, 9));
        }

        public void WebDeleteTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete, new Point(201, 9));
        }

        [When(@"I Select Test Source From GET Web Large View Source Combobox")]
        public void Select_GETWebTool_Source_From_SourceCombobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebGet.LargeView.SourcesComboBox, new Point(175, 9));
            Mouse.Click(MainStudioWindow.WebServerSourceComboboxListItem2, new Point(163, 17));
        }

        [When(@"I Select Test Source From POST Web Large View Source Combobox")]
        public void Select_POSTWebTool_Source_From_SourceCombobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPost.LargeView.SourcesComboBox, new Point(175, 9));
            Mouse.Click(MainStudioWindow.WebServerSourceComboboxListItem3, new Point(163, 17));
        }

        [When(@"I Select Test Source From PUT Web Large View Source Combobox")]
        public void Select_PUTWebTool_Source_From_SourceCombobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPut.LargeView.SourcesComboBox, new Point(175, 9));
            Mouse.Click(MainStudioWindow.WebServerSourceComboboxListItem4, new Point(163, 17));
        }

        [When(@"I Select Test Source From DELETE Web Large View Source Combobox")]
        public void Select_DELETEWebTool_Source_From_SourceCombobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.SourcesComboBox, new Point(175, 9));
            Mouse.Click(MainStudioWindow.WebServerSourceComboboxListItem1, new Point(163, 17));
        }

        [Given(@"I Click GET Web Large View Generate Outputs")]
        [When(@"I Click GET Web Large View Generate Outputs")]
        [Then(@"I Click GET Web Large View Generate Outputs")]
        public void Click_GETWebTool_GenerateOutputsButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebGet.LargeView.GenerateOutputsButton, new Point(7, 7));
        }

        [Given(@"I Click POST Web Large View Generate Outputs")]
        [When(@"I Click POST Web Large View Generate Outputs")]
        [Then(@"I Click POST Web Large View Generate Outputs")]
        public void Click_POSTWebTool_GenerateOutputsButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPost.LargeView.GenerateOutputsButton, new Point(7, 7));
        }

        [Given(@"I Click PUT Web Large View Generate Outputs")]
        [When(@"I Click PUT Web Large View Generate Outputs")]
        [Then(@"I Click PUT Web Large View Generate Outputs")]
        public void Click_PUTWebTool_GenerateOutputsButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPut.LargeView.GenerateOutputsButton, new Point(7, 7));
        }

        [Given(@"I Click DELETE Web Large View Generate Outputs")]
        [When(@"I Click DELETE Web Large View Generate Outputs")]
        [Then(@"I Click DELETE Web Large View Generate Outputs")]
        public void Click_DELETEWebTool_GenerateOutputsButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.GenerateOutputsButton, new Point(7, 7));
        }

        [Given(@"I Click GET Web Large View Done Button")]
        [When(@"I Click GET Web Large View Done Button")]
        [Then(@"I Click GET Web Large View Done Button")]
        public void Click_GETWebTool_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebGet.DoneButton, new Point(33, 11));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebGet.SmallView.Exists, "Web GET small view does not exist after clicking large view done button.");
        }

        [Given(@"I Click POST Web Large View Done Button")]
        [When(@"I Click POST Web Large View Done Button")]
        [Then(@"I Click POST Web Large View Done Button")]
        public void Click_POSTWebTool_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPost.DoneButton, new Point(33, 11));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPost.SmallView.Exists, "Web POST small view does not exist after clicking large view done button.");
        }

        [Given(@"I Click PUT Web Large View Done Button")]
        [When(@"I Click PUT Web Large View Done Button")]
        [Then(@"I Click PUT Web Large View Done Button")]
        public void Click_PUTWebTool_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPut.DoneButton, new Point(33, 11));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPut.SmallView.Exists, "Web PUT small view does not exist after clicking large view done button.");
        }

        [Given(@"I Click DELETE Web Large View Done Button")]
        [When(@"I Click DELETE Web Large View Done Button")]
        [Then(@"I Click DELETE Web Large View Done Button")]
        public void Click_DELETEWebTool_Done_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.DoneButton, new Point(33, 11));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.SmallView.Exists, "Web DELETE small view does not exist after clicking large view done button.");
        }

        [Given(@"I Click GET Web Large View Test Inputs Button")]
        [When(@"I Click GET Web Large View Test Inputs Button")]
        [Then(@"I Click GET Web Large View Test Inputs Button")]
        public void Click_GETWebTool_TestInputsButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebGet.LargeView.TestButton, new Point(21, 11));
        }

        [Given(@"I Click POST Web Large View Test Inputs Button")]
        [When(@"I Click POST Web Large View Test Inputs Button")]
        [Then(@"I Click POST Web Large View Test Inputs Button")]
        public void Click_POSTWebTool_TestInputsButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPost.LargeView.TestButton, new Point(21, 11));
        }

        [Given(@"I Click PUT Web Large View Test Inputs Button")]
        [When(@"I Click PUT Web Large View Test Inputs Button")]
        [Then(@"I Click PUT Web Large View Test Inputs Button")]
        public void Click_PUTWebTool_TestInputsButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPut.LargeView.TestButton, new Point(21, 11));
        }

        [Given(@"I Click DELETE Web Large View Test Inputs Button")]
        [When(@"I Click DELETE Web Large View Test Inputs Button")]
        [Then(@"I Click DELETE Web Large View Test Inputs Button")]
        public void Click_DELETEWebTool_TestInputsButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.TestButton, new Point(21, 11));
        }

        [Given(@"I Enter invalid data on the DELETE Web Large View")]
        [When(@"I Enter invalid data on the DELETE Web Large View")]
        [Then(@"I Enter invalid data on the DELETE Web Large View")]
        public void Enter_Invalid_Data_DELETEWebTool()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext
                .WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart
                .WebDelete.LargeView.Table.ItemRow1.HeaderCell.HeaderComboBox.TextEdit.Text = "stupidData";
        }

        [Given(@"I Click GET Web Tool Outputs Done Button")]
        [When(@"I Click GET Web Tool Outputs Done Button")]
        [Then(@"I Click GET Web Tool Outputs Done Button")]
        public void Click_GETWebTool_Outputs_DoneButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebGet.LargeView.DoneButton, new Point(35, 6));
        }

        [Given(@"I Click POST Web Tool Outputs Done Button")]
        [When(@"I Click POST Web Tool Outputs Done Button")]
        [Then(@"I Click POST Web Tool Outputs Done Button")]
        public void Click_POSTWebTool_Outputs_DoneButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPost.LargeView.DoneButton, new Point(35, 6));
        }

        [Given(@"I Click PUT Web Tool Outputs Done Button")]
        [When(@"I Click PUT Web Tool Outputs Done Button")]
        [Then(@"I Click PUT Web Tool Outputs Done Button")]
        public void Click_PUTWebTool_Outputs_DoneButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPut.LargeView.DoneButton, new Point(35, 6));
        }

        [Given(@"I Click DELETE Web Tool Outputs Done Button")]
        [When(@"I Click DELETE Web Tool Outputs Done Button")]
        [Then(@"I Click DELETE Web Tool Outputs Done Button")]
        public void Click_DELETEWebTool_Outputs_DoneButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.DoneButton, new Point(35, 6));
        }

        [When(@"I Double Click DotNetDLL Tool to Change View")]
        public void DotNetDLLTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll, new Point(238, 16));
        }

        [When(@"I Double Click ComDLL Tool to Change View")]
        public void ComDLLTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ComDll, new Point(238, 16));
        }

        [When(@"I Double Click WCFService Tool to Change View")]
        public void WCFServiceTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WcfService, new Point(238, 16));
        }

        [When(@"I Double Click DropboxDelete Tool to Change View")]
        public void DropboxDeleteTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DropboxDelete, new Point(174, 12));
        }

        [When(@"I Double Click DropboxListContents Tool to Change View")]
        public void DropboxListContentsTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DropboxFileList, new Point(166, 9));
        }

        [When(@"I Double Click DropboxUpload Tool to Change View")]
        public void DropboxUploadTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DropboxUpload, new Point(151, 8));
        }

        [When(@"I Double Click DropboxDownload Tool to Change View")]
        public void DropboxDownloadTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DropboxDownload, new Point(174, 14));
        }

        [When(@"I Double Click ExchangeSend Tool to Change View")]
        public void ExchangeSendTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ExchangeEmail, new Point(168, 11));
        }

        [When(@"I Open CMD Line Tool Large View")]
        public void Open_CMDLineTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ExecuteCommandLine, new Point(174, 10));
        }

        [When(@"I Open Javascript Large View")]
        public void Open_Javascript_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Javascript, new Point(115, 14));
        }

        [When(@"I Open Python Large View")]
        public void Open_Python_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Python, new Point(117, 9));
        }

        [When(@"I Open Ruby Large View")]
        public void Open_Ruby_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Ruby, new Point(116, 12));
        }

        [Given(@"I Right Click On The Folder Count")]
        [When(@"I Right Click On The Folder Count")]
        [Then(@"I Right Click On The Folder Count")]
        public void Right_Click_On_The_Folder_Count()
        {
            #region Variable Declarations
            WpfTreeItem firstItem = MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem;
            #endregion

            // Right-Click 'Warewolf.Studio.ViewModels.EnvironmentViewModel' -> 'Warewolf.Studio.ViewModels.ExplorerItemViewModel' tree item
            Mouse.Click(firstItem, MouseButtons.Right, ModifierKeys.None, new Point(211, 8));
        }

        [When(@"I Open Find Index Tool Large View")]
        public void Open_FindIndexTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindIndex, new Point(147, 11));
        }

        [When(@"I Open Json Tool Large View")]
        public void Open_Json_Tool_Large_View()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CreateJson, new Point(158, 13));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CreateJson.Exists, "JSON tool large view on the design surface does not exist");
        }

        [When(@"I Open FormatNumber Tool Large View")]
        public void Open_FormatNumberToll_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FormatNumber, new Point(145, 5));
        }

        [When(@"I Double Click MySqlDatabase Tool to Change View")]
        public void MySqlDatabaseTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MySqlDatabase, new Point(238, 15));
        }

        [When(@"I Double Click ODBCDatabase Tool to Change View")]
        public void ODBCDatabaseTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ODBCDatabaseActivCustom, new Point(145, 5));
        }

        [When(@"I Double Click OracleDatabase Tool to Change View")]
        public void OracleDatabaseTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.OracleDatabaseActCustom, new Point(145, 5));
        }

        [When(@"I Double Click PostgreSqlDatabase Tool to Change View")]
        public void PostgreSqlDatabaseTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PostgreSqlActivitCustom, new Point(173, 14));
        }

        [When(@"I Double Click SqlServerDatabase Tool to Change View")]
        public void SqlServerDatabaseTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase, new Point(145, 5));
        }

        [Given(@"I Double Click SqlBulkInsert Tool to Change View")]
        [When(@"I Double Click SqlBulkInsert Tool to Change View")]
        [Then(@"I Double Click SqlBulkInsert Tool to Change View")]
        public void SqlBulkInsertTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlBulkInsert, new Point(157, 6));
        }

        [When(@"I Open RabbitMqConsume LargeView")]
        public void Open_RabbitMqConsume_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.RabbitMQConsume, new Point(145, 7));
        }

        [When(@"I Open RabbitMqPublish LargeView")]
        public void Open_RabbitMqPublish_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.RabbitMQPublish, new Point(145, 7));
        }

        [When(@"I Open Random Large Tool")]
        public void Open_RandomTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Random, new Point(145, 7));
        }

        [When(@"I Open Replace Tool Large View")]
        public void Open_ReplaceTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Replace, new Point(159, 11));
        }

        [When(@"I Double Click Sequence Tool to Change View")]
        public void SequenceTool_ChangeView_With_DoubleClick()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence, new Point(139, 12));
        }

        [When(@"I Open Sharepoint Copy Tool Large View")]
        [Then(@"I Open Sharepoint Copy Tool Large View")]
        [Given(@"I Open Sharepoint Copy Tool Large View")]
        public void Open_SharepointCopyTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCopyFile, new Point(230, 11));
        }

        [When(@"I Open Sharepoint Create Tool Large View")]
        public void Open_SharepointCreateTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCreateListItem, new Point(195, 11));
        }

        [When(@"I Open Sharepoint Delete List Item Tool Large View")]
        public void Open_SharepointDeleteListItemTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDeleteListItem, new Point(218, 11));
        }

        [When(@"I Open Sharepoint Delete File Tool Large View")]
        public void Open_SharepointDeleteFileTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDeleteFile, new Point(218, 11));
        }

        [Given(@"I Open Sharepoint Download File Tool Large View")]
        [When(@"I Open Sharepoint Download File Tool Large View")]
        [Then(@"I Open Sharepoint Download File Tool Large View")]
        public void Open_SharepointDownloadFileTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDownloadFile, new Point(185, 9));
        }

        [Given(@"I Open Sharepoint MoveFile Tool Large View")]
        [When(@"I Open Sharepoint MoveFile Tool Large View")]
        [Then(@"I Open Sharepoint MoveFile Tool Large View")]
        public void Open_SharepointMoveFileTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointMoveFile, new Point(230, 11));
        }

        [Given(@"I Open Sharepoint Read Folder Tool Large View")]
        [When(@"I Open Sharepoint Read Folder Tool Large View")]
        [Then(@"I Open Sharepoint Read Folder Tool Large View")]
        public void Open_SharepointReadFolderTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadFolder, new Point(195, 7));
        }

        [Given(@"I Open Sharepoint Read List Item Tool Large View")]
        [When(@"I Open Sharepoint Read List Item Tool Large View")]
        [Then(@"I Open Sharepoint Read List Item Tool Large View")]
        public void Open_SharepointReadListItemTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadListItem, new Point(195, 7));
        }

        [Given(@"I Open Sharepoint Update List Item Tool Large View")]
        [When(@"I Open Sharepoint Update List Item Tool Large View")]
        [Then(@"I Open Sharepoint Update List Item Tool Large View")]
        public void Open_SharepointUpdateListItemTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUpdateListItem, new Point(230, 11));
        }

        [Given(@"I Open Sharepoint Upload Tool Large View")]
        [When(@"I Open Sharepoint Upload Tool Large View")]
        [Then(@"I Open Sharepoint Upload Tool Large View")]
        public void Open_SharepointUploadTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUploadFile, new Point(230, 11));
        }

        [Given(@"I Open SMTP Email Tool Large View")]
        [When(@"I Open SMTP Email Tool Large View")]
        [Then(@"I Open SMTP Email Tool Large View")]
        public void Open_SMTPSendTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SMTPEmail, new Point(168, 11));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SMTPEmail.LargeViewContent.Exists, "Email Tool large view does not exist on the design surface");
        }

        [Given(@"I Open System Information Tool Large View")]
        [When(@"I Open System Information Tool Large View")]
        [Then(@"I Open System Information Tool Large View")]
        public void Open_SystemInformationTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.GatherSystemInfo, new Point(145, 5));
        }

        [Given(@"I Open WebRequest LargeView")]
        [When(@"I Open WebRequest LargeView")]
        [Then(@"I Open WebRequest LargeView")]
        public void Open_WebRequestTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebRequest, new Point(126, 13));
        }

        [Given(@"I Open Xpath Tool Large View")]
        [When(@"I Open Xpath Tool Large View")]
        [Then(@"I Open Xpath Tool Large View")]
        public void Open_XpathTool_LargeView()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.XPath, new Point(113, 12));
        }

        [Given(@"I Open Assign Tool QVI View")]
        [When(@"I Open Assign Tool QVI View")]
        [Then(@"I Open Assign Tool QVI View")]
        public void Open_AssignTool_QVIView()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.OpenQuickVariableInpToggleButton.Pressed = true;
        }

        [Given(@"I Open AssignObject QVI View")]
        [When(@"I Open AssignObject QVI View")]
        [Then(@"I Open AssignObject QVI View")]
        public void Open_AssignObject_QVIView()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.AssignObject.OpenQuickVariableInput.Pressed = true;
        }

        [Given(@"I Open Base Conversion Tool QVI View")]
        [When(@"I Open Base Conversion Tool QVI View")]
        [Then(@"I Open Base Conversion Tool QVI View")]
        public void Open_BaseConvertTool_QVIView()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.BaseConvert.OpenQuickVariableInpToggleButton.Pressed = true;
        }

        [Given(@"I Open Case Conversion Tool QVI View")]
        [When(@"I Open Case Conversion Tool QVI View")]
        [Then(@"I Open Case Conversion Tool QVI View")]
        public void Open_CaseConvertTool_QVIView()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CaseConvert.OpenQuickVariableInpToggleButton.Pressed = true;
        }

        [Given(@"I Open Data Merge Tool QVI View")]
        [When(@"I Open Data Merge Tool QVI View")]
        [Then(@"I Open Data Merge Tool QVI View")]
        public void Open_DataMergeTool_QVIView()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataMerge.QVIToggleButton.Pressed = true;
        }

        [Given(@"I Open Data Split Tool QVI View")]
        [When(@"I Open Data Split Tool QVI View")]
        [Then(@"I Open Data Split Tool QVI View")]
        public void Open_DataSplitTool_QVIView()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataSplit.OpenQuickVariableInpToggleButton.Pressed = true;
        }

        [Given(@"I Open Json Tool QVI View")]
        [When(@"I Open Json Tool QVI View")]
        [Then(@"I Open Json Tool QVI View")]
        public void Open_JSONTool_QVIView()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CreateJson.OpenQuickVariableInpToggleButton.Pressed = true;
        }

        [Given(@"I Open SQL Bulk Insert Tool QVI View")]
        [When(@"I Open SQL Bulk Insert Tool QVI View")]
        [Then(@"I Open SQL Bulk Insert Tool QVI View")]
        public void Open_SQLBulkInsertTool_QVIView()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlBulkInsert.QVIToggleButton.Pressed = true;
        }

        [Given(@"I Open System Information Tool QVI View")]
        [When(@"I Open System Information Tool QVI View")]
        [Then(@"I Open System Information Tool QVI View")]
        public void Open_SystemInformationTool_QVIView()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.GatherSystemInfo.OpenQuickVariableInpToggleButton.Pressed = true;
        }

        [Given(@"I Open Xpath Tool Qvi Large View")]
        [When(@"I Open Xpath Tool Qvi Large View")]
        [Then(@"I Open Xpath Tool Qvi Large View")]
        public void Open_XpathTool_QVIView()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.XPath.OpenQuickVariableInpToggleButton.Pressed = true;
        }

//TODO: End of UIMap.WorkflowDesigner.uitest

        [When(@"I Open Explorer First Item With Double Click")]
        public void Open_Explorer_First_Item_With_Double_Click()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Left, ModifierKeys.None, new Point(40, 9));
        }

        [Given(@"I Press F6")]
        [When(@"I Press F6")]
        [Then(@"I Press F6")]
        public void Press_F6()
        {
            Keyboard.SendKeys(MainStudioWindow, "{F6}", ModifierKeys.None);
        }

        [Given(@"I Press F6 On Unpinned Tab")]
        [When(@"I Press F6 On Unpinned Tab")]
        [Given(@"I Press F6 On Unpinned Tab")]
        public void Press_F6_On_UnPinnedTab()
        {
            Keyboard.SendKeys(MainStudioWindow.UnpinnedTab, "{F6}", ModifierKeys.None);
        }

        [Given(@"I PressF11 EnterFullScreen")]
        [When(@"I PressF11 EnterFullScreen")]
        [Then(@"I PressF11 EnterFullScreen")]
        public void PressF11_EnterFullScreen()
        {
            Keyboard.SendKeys(MainStudioWindow, "{F11}", ModifierKeys.None);
        }

        [Given(@"I RabbitMqAsserts")]
        [When(@"I RabbitMqAsserts")]
        [Then(@"I RabbitMqAsserts")]
        public void RabbitMqAsserts()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.RabbitMqSourceTab.RabbitMQSourceCustom.VirtualHostTextBoxEdit.Exists, "VirtualHoast textbox does not exist after opening RabbitMq Source tab");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.RabbitMqSourceTab.RabbitMQSourceCustom.PasswordTextBoxEdit.Exists, "Password textbox does not exist after opening RabbitMq Source");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.RabbitMqSourceTab.RabbitMQSourceCustom.UserNameTextBoxEdit.Exists, "Username textbox does not exist after opening RabbitMq Source");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.RabbitMqSourceTab.RabbitMQSourceCustom.HostTextBoxEdit.Exists, "Host textbox does not exist after opening RabbitMq Source");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.RabbitMqSourceTab.RabbitMQSourceCustom.PortTextBoxEdit.Exists, "Port textbox does not exist after opening RabbitMq Source");
        }

        [Given(@"I Remove WorkflowName From Save Dialog")]
        [When(@"I Remove WorkflowName From Save Dialog")]
        [Then(@"I Remove WorkflowName From Save Dialog")]
        public void Remove_WorkflowName_From_Save_Dialog()
        {
            SaveDialogWindow.ServiceNameTextBox.Text = "";
            Assert.AreEqual("Cannot be null", SaveDialogWindow.ErrorLabel.DisplayText, "Name cannot be null validation message does not appear");
            Assert.AreEqual(false, SaveDialogWindow.SaveButton.Enabled, "Save button on the Save dialog is enabled");
        }

        [Given(@"I Rename FolderItem ToNewFolderItem")]
        [When(@"I Rename FolderItem ToNewFolderItem")]
        [Then(@"I Rename FolderItem ToNewFolderItem")]
        public void Rename_FolderItem_ToNewFolderItem(string newName)
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem, MouseButtons.Right, ModifierKeys.None, new Point(77, 12));
            Select_Rename_From_Explorer_ContextMenu();
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem.ItemEdit.Text = newName;
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem.ItemEdit, "{Enter}", ModifierKeys.None);
        }

        [Given(@"I Select Rename From Explorer Context Menu")]
        [When(@"I Select Rename From Explorer Context Menu")]
        [Then(@"I Select Rename From Explorer Context Menu")]
        private void Select_Rename_From_Explorer_ContextMenu()
        {
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Rename);
        }
        [Given(@"I Select Rename From SaveDialog Context Menu")]
        [When(@"I Select Rename From SaveDialog Context Menu")]
        [Then(@"I Select Rename From SaveDialog Context Menu")]
        private void Select_Rename_From_SaveDialog_ContextMenu()
        {
            Mouse.Click(SaveDialogWindow.SaveDialogContextMenu.RenameMenuItem);
        }

        [When(@"I Rename Folder to ""(.*)"" Using Shortcut KeyF2")]
        [Then(@"I Rename Folder to ""(.*)"" Using Shortcut KeyF2")]
        [Given(@"I Rename Folder to ""(.*)"" Using Shortcut KeyF2")]
        public void Rename_Folder_Using_Shortcut(string newName)
        {
            var firstItem = MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem;
            Mouse.Click(firstItem);
            Keyboard.SendKeys(firstItem, "{F2}");
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.ItemEdit.Text = newName;
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.ItemEdit, "{Enter}", ModifierKeys.None);
        }

        [When(@"I Rename Explorer First item")]
        [Then(@"I Rename Explorer First item")]
        [Given(@"I Rename Explorer First item")]
        public void Rename_Explorer_First_Item(string newName)
        {
            var firstItem = MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem;
            Mouse.Click(firstItem);
            Keyboard.SendKeys(firstItem, "{F2}");
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.ItemEdit.Text = newName;
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.ItemEdit, "{Enter}", ModifierKeys.None);
        }

        [When(@"I Rename Remote Folder to ""(.*)"" Using Shortcut KeyF2")]
        [Then(@"I Rename Remote Folder to ""(.*)"" Using Shortcut KeyF2")]
        [Given(@"I Rename Remote Folder to ""(.*)"" Using Shortcut KeyF2")]
        public void Rename_Remote_Folder_Using_Shortcut(string newName)
        {
            var firstItem = MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem;
            Mouse.Click(firstItem);
            Keyboard.SendKeys(firstItem, "{F2}");
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem.ItemEdit.Text = newName;
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem.ItemEdit, "{Enter}", ModifierKeys.None);
        }

        [Given(@"I Rename LocalFolder To SecondFolder")]
        [When(@"I Rename LocalFolder To SecondFolder")]
        [Then(@"I Rename LocalFolder To SecondFolder")]
        public void Rename_LocalFolder_To_SecondFolder(string newName)
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(77, 12));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Rename);
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.ItemEdit.Text = newName;
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.ItemEdit, "{Enter}", ModifierKeys.None);
        }

        [Given(@"I Rename LocalWorkflow To SecodWorkFlow")]
        [When(@"I Rename LocalWorkflow To SecodWorkFlow")]
        [Then(@"I Rename LocalWorkflow To SecodWorkFlow")]
        public void Rename_LocalWorkflow_To_SecodWorkFlow()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(69, 10));
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Rename, new Point(73, 15));
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.ItemEdit.Text = "SecondWorkflow";
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.ItemEdit, "{Enter}", ModifierKeys.None);
        }

        [Given(@"I Restore Unpinned Tab Using Context Menu")]
        [When(@"I Restore Unpinned Tab Using Context Menu")]
        [Then(@"I Restore Unpinned Tab Using Context Menu")]
        public void Restore_Unpinned_Tab_Using_Context_Menu()
        {
            Mouse.Click(MainStudioWindow.UnpinnedTab, MouseButtons.Right, ModifierKeys.None, new Point(14, 12));
            MainStudioWindow.UnpinnedTabContextMenu.TabbedDocument.Checked = true;
        }

        [Given(@"I Right Click Help Tab")]
        [When(@"I Right Click Help Tab")]
        [Then(@"I Right Click Help Tab")]
        public void Right_Click_Help_Tab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.HelpTab, MouseButtons.Right, ModifierKeys.None, new Point(64, 15));
        }

        [Given(@"I RightClick BaseConvert OnDesignSurface")]
        [When(@"I RightClick BaseConvert OnDesignSurface")]
        [Then(@"I RightClick BaseConvert OnDesignSurface")]
        public void RightClick_BaseConvert_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.BaseConvert, MouseButtons.Right, ModifierKeys.None, new Point(148, 12));
        }

        [Given(@"I RightClick Calculate OnDesignSurface")]
        [When(@"I RightClick Calculate OnDesignSurface")]
        [Then(@"I RightClick Calculate OnDesignSurface")]
        public void RightClick_Calculate_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Calculate, MouseButtons.Right, ModifierKeys.None, new Point(144, 10));
        }

        [Given(@"I RightClick STACKOVERFLOWTESTWORKFLOW OnDesignSurface")]
        [When(@"I RightClick STACKOVERFLOWTESTWORKFLOW OnDesignSurface")]
        [Then(@"I RightClick STACKOVERFLOWTESTWORKFLOW OnDesignSurface")]
        public void RightClick_StackOverFlowService_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.stackOverflowTestWF, MouseButtons.Right, ModifierKeys.None, new Point(181, 11));

        }

        public void RightClick_AssignOnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.AssignObject, MouseButtons.Right, ModifierKeys.None, new Point(156, 11));

        }

        [Given(@"I RightClick CaseConvert OnDesignSurface")]
        [When(@"I RightClick CaseConvert OnDesignSurface")]
        [Then(@"I RightClick CaseConvert OnDesignSurface")]
        public void RightClick_CaseConvert_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CaseConvert, MouseButtons.Right, ModifierKeys.None, new Point(156, 10));
        }

        [Given(@"I RightClick Comment OnDesignSurface")]
        [When(@"I RightClick Comment OnDesignSurface")]
        [Then(@"I RightClick Comment OnDesignSurface")]
        public void RightClick_Comment_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Comment, MouseButtons.Right, ModifierKeys.None, new Point(121, 10));
        }

        [Given(@"I RightClick Copy OnDesignSurface")]
        [When(@"I RightClick Copy OnDesignSurface")]
        [Then(@"I RightClick Copy OnDesignSurface")]
        public void RightClick_Copy_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathCopy, MouseButtons.Right, ModifierKeys.None, new Point(104, 10));
        }

        [Given(@"I RightClick CountRecords OnDesignSurface")]
        [When(@"I RightClick CountRecords OnDesignSurface")]
        [Then(@"I RightClick CountRecords OnDesignSurface")]
        public void RightClick_CountRecords_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CountRecordset, MouseButtons.Right, ModifierKeys.None, new Point(131, 10));
        }

        [Given(@"I RightClick CreateJSON OnDesignSurface")]
        [When(@"I RightClick CreateJSON OnDesignSurface")]
        [Then(@"I RightClick CreateJSON OnDesignSurface")]
        public void RightClick_CreateJSON_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.CreateJson, MouseButtons.Right, ModifierKeys.None, new Point(128, 9));
        }

        [Given(@"I RightClick CreateTool OnDesignSurface")]
        [When(@"I RightClick CreateTool OnDesignSurface")]
        [Then(@"I RightClick CreateTool OnDesignSurface")]
        public void RightClick_CreateTool_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathCreate, MouseButtons.Right, ModifierKeys.None, new Point(108, 14));
        }

        [Given(@"I RightClick DataMerge OnDesignSurface")]
        [When(@"I RightClick DataMerge OnDesignSurface")]
        [Then(@"I RightClick DataMerge OnDesignSurface")]
        public void RightClick_DataMerge_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataMerge, MouseButtons.Right, ModifierKeys.None, new Point(140, 7));
        }

        [Given(@"I RightClick DataSplit OnDesignSurface")]
        [When(@"I RightClick DataSplit OnDesignSurface")]
        [Then(@"I RightClick DataSplit OnDesignSurface")]
        public void RightClick_DataSplit_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DataSplit, MouseButtons.Right, ModifierKeys.None, new Point(153, 6));
        }

        [Given(@"I RightClick DateTime OnDesignSurface")]
        [When(@"I RightClick DateTime OnDesignSurface")]
        [Then(@"I RightClick DateTime OnDesignSurface")]
        public void RightClick_DateTime_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DateTime, MouseButtons.Right, ModifierKeys.None, new Point(145, 13));
        }

        [Given(@"I RightClick DateTimeDifference OnDesignSurface")]
        [When(@"I RightClick DateTimeDifference OnDesignSurface")]
        [Then(@"I RightClick DateTimeDifference OnDesignSurface")]
        public void RightClick_DateTimeDifference_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DateTimeDifference, MouseButtons.Right, ModifierKeys.None, new Point(174, 10));
        }

        [Given(@"I RightClick Decision OnDesignSurface")]
        [When(@"I RightClick Decision OnDesignSurface")]
        [Then(@"I RightClick Decision OnDesignSurface")]
        public void RightClick_Decision_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Decision, MouseButtons.Right, ModifierKeys.None, new Point(28, 22));
        }

        [Given(@"I RightClick Delete OnDesignSurface")]
        [When(@"I RightClick Delete OnDesignSurface")]
        [Then(@"I RightClick Delete OnDesignSurface")]
        public void RightClick_Delete_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathDelete, MouseButtons.Right, ModifierKeys.None, new Point(100, 10));
        }

        [Given(@"I RightClick DeleteRecord OnDesignSurface")]
        [When(@"I RightClick DeleteRecord OnDesignSurface")]
        [Then(@"I RightClick DeleteRecord OnDesignSurface")]
        public void RightClick_DeleteRecord_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DeleteRecord, MouseButtons.Right, ModifierKeys.None, new Point(116, 9));
        }

        [When(@"I RightClick DotNetDllConnector OnDesignSurface")]
        public void RightClick_DotNetDllConnector_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll, MouseButtons.Right, ModifierKeys.None, new Point(164, 10));
        }

        [Given(@"I RightClick DropboxFileOperation OnDesignSurface")]
        [When(@"I RightClick DropboxFileOperation OnDesignSurface")]
        [Then(@"I RightClick DropboxFileOperation OnDesignSurface")]
        public void RightClick_DropboxFileOperation_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DropboxDownload, MouseButtons.Right, ModifierKeys.None, new Point(181, 11));
        }

        [Given(@"I RightClick Email OnDesignSurface")]
        [When(@"I RightClick Email OnDesignSurface")]
        [Then(@"I RightClick Email OnDesignSurface")]
        public void RightClick_Email_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SMTPEmail, MouseButtons.Right, ModifierKeys.None, new Point(129, 11));
        }

        [Given(@"I RightClick ExecuteCommandLine OnDesignSurface")]
        [When(@"I RightClick ExecuteCommandLine OnDesignSurface")]
        [Then(@"I RightClick ExecuteCommandLine OnDesignSurface")]
        public void RightClick_ExecuteCommandLine_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ExecuteCommandLine, MouseButtons.Right, ModifierKeys.None, new Point(165, 13));
        }

        [Given(@"I RightClick Explorer First Remote Server First Item")]
        [When(@"I RightClick Explorer First Remote Server First Item")]
        [Then(@"I RightClick Explorer First Remote Server First Item")]
        public void RightClick_Explorer_FirstRemoteServer_FirstItem()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(107, 9));
        }

        [Given(@"I RightClick Explorer Localhost First Item")]
        [When(@"I RightClick Explorer Localhost First Item")]
        [Then(@"I RightClick Explorer Localhost First Item")]
        public void RightClick_Explorer_Localhost_FirstItem()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(77, 9));
        }

        [Given(@"I RightClick Localhost")]
        [When(@"I RightClick Localhost")]
        [Then(@"I RightClick Localhost")]
        public void RightClick_Localhost()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(77, 9));
        }

        [Given(@"I RightClick Save Dialog Localhost")]
        [When(@"I RightClick Save Dialog Localhost")]
        [Then(@"I RightClick Save Dialog Localhost")]
        public void RightClick_Save_Dialog_Localhost()
        {
            Mouse.Click(SaveDialogWindow.ExplorerView.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(77, 9));
            Assert.IsTrue(SaveDialogWindow.SaveDialogContextMenu.NewFolderMenuItem.Exists);
        }

        [Given(@"I RightClick Save Dialog Localhost First Item")]
        [When(@"I RightClick Save Dialog Localhost First Item")]
        [Then(@"I RightClick Save Dialog Localhost First Item")]
        public void RightClick_Save_Dialog_Localhost_First_Item()
        {
            Mouse.Click(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(77, 9));
        }

        [Given(@"I Rename Item using Shortcut")]
        [When(@"I Rename Item using Shortcut")]
        [Then(@"I Rename Item using Shortcut")]
        public void RenameItemUsingShortcut()
        {
            Mouse.Click(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem, new Point(77, 9));
            Keyboard.SendKeys(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem, "{F2}");
        }

        [Given(@"I Create New Folder Item using Shortcut")]
        [When(@"I Create New Folder Item using Shortcut")]
        [Then(@"I Create New Folder Item using Shortcut")]
        public void ThenICreateNewFolderItemUsingShortcut()
        {
            Mouse.Click(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem, new Point(77, 9));
            Keyboard.SendKeys(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem, "F", (ModifierKeys.Control | ModifierKeys.Shift));
        }


        [When(@"I RightClick FindIndex OnDesignSurface")]
        public void RightClick_FindIndex_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindIndex, MouseButtons.Right, ModifierKeys.None, new Point(113, 8));
        }

        [When(@"I RightClick FindRecordIndex OnDesignSurface")]
        public void RightClick_FindRecordIndex_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FindRecordsIndex, MouseButtons.Right, ModifierKeys.None, new Point(191, 11));
        }

        [When(@"I RightClick ForEach OnDesignSurface")]
        public void RightClick_ForEach_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ForEach, MouseButtons.Right, ModifierKeys.None, new Point(137, 9));
        }

        [When(@"I RightClick FormatNumber OnDesignSurface")]
        public void RightClick_FormatNumber_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FormatNumber, MouseButtons.Right, ModifierKeys.None, new Point(143, 9));
        }

        [When(@"I RightClick Length OnDesignSurface")]
        public void RightClick_Length_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Length, MouseButtons.Right, ModifierKeys.None, new Point(97, 10));
        }

        [When(@"I RightClick Move OnDesignSurface")]
        public void RightClick_Move_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathMove, MouseButtons.Right, ModifierKeys.None, new Point(98, 11));
        }

        [When(@"I RightClick MySQLConnector OnDesignSurface")]
        public void RightClick_MySQLConnector_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MySqlDatabase, MouseButtons.Right, ModifierKeys.None, new Point(202, 10));
        }

        [When(@"I RightClick New Workflow Tab")]
        public void RightClick_New_Workflow_Tab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab, MouseButtons.Right, ModifierKeys.None, new Point(63, 18));
        }

        [When(@"I RightClick Random OnDesignSurface")]
        public void RightClick_Random_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Random, MouseButtons.Right, ModifierKeys.None, new Point(107, 13));
        }

        [When(@"I RightClick ReadFile OnDesignSurface")]
        public void RightClick_ReadFile_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FileRead, MouseButtons.Right, ModifierKeys.None, new Point(99, 14));
        }

        [When(@"I RightClick ReadFolder OnDesignSurface")]
        public void RightClick_ReadFolder_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FolderRead, MouseButtons.Right, ModifierKeys.None, new Point(115, 12));
        }

        [When(@"I RightClick Rename OnDesignSurface")]
        public void RightClick_Rename_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PathRename, MouseButtons.Right, ModifierKeys.None, new Point(103, 7));
        }

        [When(@"I RightClick Replace OnDesignSurface")]
        public void RightClick_Replace_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Replace, MouseButtons.Right, ModifierKeys.None, new Point(100, 7));
        }

        [When(@"I RightClick Sequence OnDesignSurface")]
        public void RightClick_Sequence_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Sequence, MouseButtons.Right, ModifierKeys.None, new Point(119, 8));
        }

        [When(@"I RightClick SharepointCreateListItem OnDesignSurface")]
        public void RightClick_SharepointCreateListItem_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.Copy, MouseButtons.Right, ModifierKeys.None, new Point(199, 12));
        }

        [When(@"I RightClick SharepointDelete OnDesignSurface")]
        public void RightClick_SharepointDelete_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointDeleteFile, MouseButtons.Right, ModifierKeys.None, new Point(217, 8));
        }

        [When(@"I RightClick SharepointRead OnDesignSurface")]
        public void RightClick_SharepointRead_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadListItem, MouseButtons.Right, ModifierKeys.None, new Point(203, 9));
        }

        [When(@"I RightClick SharepointUpdate OnDesignSurface")]
        public void RightClick_SharepointUpdate_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUpdateListItem, MouseButtons.Right, ModifierKeys.None, new Point(210, 5));
        }

        [When(@"I RightClick SortRecords OnDesignSurface")]
        public void RightClick_SortRecords_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SortRecords, MouseButtons.Right, ModifierKeys.None, new Point(118, 8));
        }

        [When(@"I RightClick SQLConnector OnDesignSurface")]
        public void RightClick_SQLConnector_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlBulkInsert, MouseButtons.Right, ModifierKeys.None, new Point(143, 6));
        }

        [When(@"I RightClick SqlServerConnector OnDesignSurface")]
        public void RightClick_SqlServerConnector_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase, MouseButtons.Right, ModifierKeys.None, new Point(198, 8));
        }

        [When(@"I RightClick Switch OnDesignSurface")]
        public void RightClick_Switch_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Switch, MouseButtons.Right, ModifierKeys.None, new Point(46, 15));
        }

        [When(@"I RightClick Unzip OnDesignSurface")]
        public void RightClick_Unzip_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.ActionsCombobox, MouseButtons.Right, ModifierKeys.None, new Point(101, 10));
        }

        [When(@"I RightClick WebRequest OnDesignSurface")]
        public void RightClick_WebRequest_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebRequest, MouseButtons.Right, ModifierKeys.None, new Point(165, 8));
        }

        [When(@"I RightClick WriteFile OnDesignSurface")]
        public void RightClick_WriteFile_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FileWrite, MouseButtons.Right, ModifierKeys.None, new Point(96, 12));
        }

        [When(@"I RightClick XPath OnDesignSurface")]
        public void RightClick_XPath_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.XPath, MouseButtons.Right, ModifierKeys.None, new Point(99, 8));
        }

        [When(@"I RightClick Zip OnDesignSurface")]
        public void RightClick_Zip_OnDesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Zip, MouseButtons.Right, ModifierKeys.None, new Point(95, 12));
        }

        [When(@"I Search And Select DiceRoll")]
        public void Search_And_Select_DiceRoll()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.SearchTextBox, new Point(165, 9));
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.SearchTextBox.Text = "Dice Roll";
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(101, 9));
        }

        [When(@"I Search And Select HelloWolrd")]
        public void Search_And_Select_HelloWolrd()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.SearchTextBox, new Point(165, 9));
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.SearchTextBox.Text = "Hello World";
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, MouseButtons.Right, ModifierKeys.None, new Point(101, 9));
        }

        [When(@"I Select AcceptanceTestin create")]
        public void Select_AcceptanceTestin_create()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCreateListItem.SmallView.MethodList, new Point(119, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCreateListItem.SmallView.MethodList.UIAcceptanceTesting_CrListItem, new Point(114, 13));
        }

        [When(@"I Select Action")]
        public void Select_Action()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ActionMethodListBoxList.ActivitiesDesignListItem.ActivitiesExpander.ActivitiesDesignButton.ActionsComboBox, new Point(216, 7));
            Mouse.Click(MainStudioWindow.getName, new Point(137, 7));
        }

        [When(@"I Select AppData From MethodList")]
        public void Select_AppData_From_MethodList()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCreateListItem.SmallView.MethodList, new Point(174, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCreateListItem.SmallView.MethodList.UIAppdataListItem, new Point(43, 15));
        }

        [When(@"I Select AppData From MethodList From ReadTool")]
        public void Select_AppData_From_MethodList_From_ReadTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadListItem.SmallView.MethodList, new Point(174, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadListItem.SmallView.MethodList.UIAppdataListItem, new Point(43, 15));
        }

        [When(@"I Select Copy FromContextMenu")]
        public void Select_Copy_FromContextMenu()
        {
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.Copy, new Point(27, 18));
        }

        [When(@"I Select CopyAsImage FromContextMenu")]
        public void Select_CopyAsImage_FromContextMenu()
        {
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.CopyasImage, new Point(62, 22));
        }

        [When(@"I Select Cut FromContextMenu")]
        public void Select_Cut_FromContextMenu()
        {
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.Cut, new Point(53, 16));
        }

        [When(@"I Select DatabaseAndTable From BulkInsert Tool")]
        public void Select_DatabaseAndTable_From_BulkInsert_Tool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlBulkInsert.LargeViewContentCustom.DatabaseComboBox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlBulkInsert.LargeViewContentCustom.DatabaseComboBox.TestingDB);
        }

        [When(@"I Select DeleteRow FromContextMenu")]
        public void Select_DeleteRow_FromContextMenu()
        {
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.DeleteRow, new Point(74, 9));
        }

        [When(@"I Select Dev2TestingDB From DB Source Wizard Database Combobox")]
        public void Select_Dev2TestingDB_From_DB_Source_Wizard_Database_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox);
            Mouse.Click(MainStudioWindow.Dev2TestingDBCustom);
            Assert.AreEqual("Dev2TestingDB", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.UIDev2TestingDBText.DisplayText);
        }

        [When(@"I Select First Item From DotNet DLL Large View Source Combobox")]
        public void Select_First_Item_From_DotNet_DLL_Large_View_Source_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.SourcesComboBox, new Point(175, 9));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.SourcesComboBox.ListItem1, new Point(163, 17));
        }
        [When(@"I Select Classname From DotNet DLL Large View Classname Combobox")]
        public void Select_DotNet_Dll_Classname()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox, new Point(175, 9));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox.HumalClassListItem, new Point(163, 17));
        }
        public void Click_DotNet_DLL_Large_View_FirstAction_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ActionMethodListBoxList.ActivitiesDesignListItem.ActivitiesExpander.ActivitiesDesignButton.ActionsComboBox, new Point(175, 9));
        }

        public void Click_DotNet_DLL_Large_View_SecondAction_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ActionMethodListBoxList.ActivitiesDesignListItem2.ActivitiesExpander.ActivitiesDesignButton.ActionsComboBox, new Point(175, 9));
        }

        public void Click_DotNet_DLL_Large_View_ThirdAction_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ActionMethodListBoxList.ActivitiesDesignListItem.ActivitiesExpander.ActivitiesDesignButton.ActionsComboBox, new Point(175, 9));
        }

        public void Click_DotNet_DLL_Large_View_Constructor_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.CtorExpander.ActivitiesDesignButton.ConstructorsComboBox, new Point(175, 9));
        }

        public void Select_DotNet_DLL_Large_View_Constructor_With_One_Parameter_From_Constructor_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.CtorExpander.ActivitiesDesignButton.ConstructorsComboBox.CtorSystemStringListItem, new Point(175, 9));
        }
        public void I_Expand_Costructor_Tree()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.OneParamCtorExpander.Expanded = true;
        }
        public void Select_DotNet_DLL_Large_View_SetName_Action_For_FirstAction_From_Actions_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ActionMethodListBoxList.ActivitiesDesignListItem.ActivitiesExpander.ActivitiesDesignButton.ActionsComboBox.Set_NameListItem, new Point(175, 9));
        }
        public void Select_DotNet_DLL_Large_View_SetName_Action_For_SecondAction_From_Actions_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ActionMethodListBoxList.ActivitiesDesignListItem2.ActivitiesExpander.ActivitiesDesignButton.ActionsComboBox.Set_NameListItem, new Point(175, 9));
        }
        public void Select_DotNet_DLL_Large_View_SetName_Action_For_ThirdAction_From_Actions_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ActionMethodListBoxList.ActivitiesDesignListItem.ActivitiesExpander.ActivitiesDesignButton.ActionsComboBox.Set_NameListItem, new Point(175, 9));
        }
        public void Click_Refresh_Combobox_Button_On_Third_Action_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ActionMethodListBoxList.ActivitiesDesignListItem.ActivitiesExpander.ActivitiesDesignButton.ActionsComboBox.Set_NameListItem, new Point(175, 9));
        }

        public void Click_Delete_Button_On_Second_Action_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ActionMethodListBoxList.ActivitiesDesignListItem2.SetActivitiesExpander.ActivitiesDesignButton.DeleteActionButton);
        }

        [When(@"I Select Empty Constructor From DotNet DLL Large View Constructor Combobox")]
        public void Select_DotNet_Dll_Default_Costructor()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.CtorExpander.ActivitiesDesignButton.ConstructorsComboBox, new Point(175, 9));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.CtorExpander.ActivitiesDesignButton.ConstructorsComboBox.CtorListItem, new Point(163, 17));
        }
        public void Select_First_Action_From_Dotnet_Dll_LargeView_Action_Dropdown()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ActionMethodListBoxList.ActivitiesDesignListItem.ActivitiesExpander.ActivitiesDesignButton.ActionsComboBox, new Point(175, 9));
            Mouse.Click(MainStudioWindow.getName, new Point(163, 17));
        }


        [When(@"I Select GetCountries From SQL Server Large View Action Combobox")]
        public void Select_GetCountries_From_SQL_Server_Large_View_Action_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.ActionsCombobox, new Point(216, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.ActionsCombobox.GetCountriesListItem, new Point(137, 7));
            Assert.AreEqual("dbo.GetCountries", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.ActionsCombobox.SelectedItem, "GetCountries is not selected in SQL server large view action combobox.");
        }

        [When(@"I Select http From Server Source Wizard Address Protocol Dropdown")]
        public void Select_http_From_Server_Source_Wizard_Address_Protocol_Dropdown()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.NewServerSource.ProtocolCombobox.ToggleDropdown, new Point(54, 8));
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsHttp.Exists, "Http does not exist in server source wizard address protocol dropdown list.");
            Mouse.Click(MainStudioWindow.ComboboxListItemAsHttp, new Point(31, 12));
            Assert.AreEqual("http", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.NewServerSource.ProtocolCombobox.HttpSelectedItemText.DisplayText, "Server source wizard address protocol is not equal to http.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.NewServerSource.AddressComboBox.AddressEditBox.Exists, "Server source wizard address textbox does not exist");
        }

        [When(@"I Select InsertRow FromContextMenu")]
        public void Select_InsertRow_FromContextMenu()
        {
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.InsertRow, new Point(66, 19));
        }

        [When(@"I Select LocalhostConnected From Deploy Tab Destination Server Combobox")]
        public void Select_LocalhostConnected_From_Deploy_Tab_Destination_Server_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DestinationServerConectControl.Combobox.ToggleButton, new Point(230, 9));
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsNewRemoteServer.Exists, "New Remote Server... option does not exist in Destination server combobox.");
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsLocalhostConnected.Exists, "Remote Connection Integration option does not exist in Destination server combobox.");
            Mouse.Click(MainStudioWindow.ComboboxListItemAsLocalhostConnected, new Point(226, 13));
        }

        [When(@"I Select LoggingTab")]
        public void Select_LoggingTab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.LoggingTab, new Point(57, 7));
        }

        [When(@"I Select Months From AddTime Type")]
        public void Select_Months_From_AddTime_Type()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DateTime.SmallViewContentCustom.AddTimeTypeComboBox, new Point(175, 9));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DateTime.SmallViewContentCustom.AddTimeTypeComboBox.Months, new Point(163, 17));
        }

        [When(@"I Select MSSQLSERVER From DB Source Wizard Address Protocol Dropdown")]
        public void Select_MSSQLSERVER_From_DB_Source_Wizard_Address_Protocol_Dropdown()
        {
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsMicrosoftSQLServer.MicrosoftSQLServerText.Exists, "Microsoft SQL Server does not exist as an option in new DB source wizard type combobox.");
            Mouse.Click(MainStudioWindow.ComboboxListItemAsMicrosoftSQLServer.MicrosoftSQLServerText, new Point(118, 6));
        }

        [When(@"I Select Namespace")]
        public void Select_Namespace()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox, new Point(216, 7));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox.ComboboxlistItemAsSystemObject.Exists, "System.Random item does not exist in the DotNet DLL tool ClassName dropdown");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox.ComboboxlistItemAsSystemObject, new Point(137, 7));
        }

        [Given(@"I Select New Folder From SaveDialog Context Menu")]
        [When(@"I Select New Folder From SaveDialog Context Menu")]
        [Then(@"I Select New Folder From SaveDialog Context Menu")]
        public void Select_NewFolder_From_SaveDialogContextMenu()
        {
            Mouse.Click(SaveDialogWindow.SaveDialogContextMenu.NewFolderMenuItem);
        }

        [Given(@"I Select Delete From SaveDialog Context Menu")]
        [When(@"I Select Delete From SaveDialog Context Menu")]
        [Then(@"I Select Delete From SaveDialog Context Menu")]
        public void Select_Delete_From_SaveDialog_ContextMenu()
        {
            Mouse.Click(SaveDialogWindow.SaveDialogContextMenu.DeleteMenuItem);
            Assert.IsTrue(MessageBoxWindow.Exists);
            Assert.IsTrue(MessageBoxWindow.DeleteConfirmation.Exists);
        }

        [When(@"I Select NewSQLServerDatabaseSource FromSqlServerTool")]
        public void Select_NewSQLServerDatabaseSource_From_SqlServerTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.NewDbSourceButton, new Point(16, 13));
        }

        [When(@"I Select NewSharepointSource FromServer Lookup")]
        public void Select_NewSharepointSource_FromServer_Lookup()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCreateListItem.SmallView.Server, new Point(107, 13));
            Keyboard.SendKeys(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCreateListItem.SmallView.Server, "{Down}{Enter}", ModifierKeys.None);
        }

        [When(@"I Select NewWorkFlowService From ContextMenu")]
        public void Select_NewWorkFlowService_From_ContextMenu()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost, MouseButtons.Right, ModifierKeys.None, new Point(75, 10));
            Assert.IsTrue(MainStudioWindow.ExplorerEnvironmentContextMenu.NewWorkflow.Enabled, "NewWorkFlowService button is disabled.");
            Mouse.Click(MainStudioWindow.ExplorerEnvironmentContextMenu.NewWorkflow, new Point(79, 13));
        }

        [Given(@"I Select Open FromExplorerContextMenu")]
        [When(@"I Select Open FromExplorerContextMenu")]
        [Then(@"I Select Open FromExplorerContextMenu")]
        public void Select_Open_FromExplorerContextMenu()
        {
            Mouse.Click(MainStudioWindow.ExplorerContextMenu.Open);
        }

        [Then(@"Remote ""(.*)"" is open")]
        public void RemoteResourceIsOpen(string tabName)
        {
            Playback.Wait(500);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem.Exists);
            Assert.AreEqual(tabName, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.TabDescription.DisplayText);
        }

        [Then(@"Local ""(.*)"" is open")]
        public void LocalResourceIsOpen(string tabName)
        {
            Playback.Wait(1000);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.Exists, "First Item does not exist in tree.");
            Assert.AreEqual(tabName, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.TabDescription.DisplayText);
        }


        [When(@"I Select OutputIn Days")]
        public void Select_OutputIn_Days()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DateTimeDifference.SmallViewContentCustom.OutputInComboBox, new Point(119, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DateTimeDifference.SmallViewContentCustom.OutputInComboBox.Days, new Point(114, 13));
        }

        [When(@"I Select Paste FromContextMenu")]
        public void Select_Paste_FromContextMenu()
        {
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.Paste, new Point(52, 16));
        }

        [When(@"I Select PerfomanceCounterTab")]
        public void Select_PerfomanceCounterTab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.PerfomanceCounterTab, new Point(124, 14));
        }

        [Given(@"I Select RemoteConnectionIntegration From Deploy Tab Destination Server Combobox")]
        [When(@"I Select RemoteConnectionIntegration From Deploy Tab Destination Server Combobox")]
        [Then(@"I Select RemoteConnectionIntegration From Deploy Tab Destination Server Combobox")]
        public void Select_RemoteConnectionIntegration_From_Deploy_Tab_Destination_Server_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DestinationServerConectControl.Combobox.ToggleButton, new Point(230, 9));
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegration.Exists, "Remote Connection Integration option does not exist in Destination server combobox.");
            Mouse.Click(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegration, new Point(226, 13));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DestinationServerConectControl.Combobox.RemoteConnectionIntegrationText.Exists, "Selected destination server in deploy is not Remote Connection Integration.");
        }

        [Given(@"I Click Edit Deploy Destination Server Button")]
        [When(@"I Click Edit Deploy Destination Server Button")]
        [Then(@"I Click Edit Deploy Destination Server Button")]
        public void Click_Edit_Deploy_Destination_Server_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.EditDestinationButton);
        }
        public void Select_Server_Authentication_Public()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.PublicRadioButton.Selected = true;
        }

        [When(@"I Select localhost From Deploy Tab Destination Server Combobox")]
        public void Select_localhost_From_Deploy_Tab_Destination_Server_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DestinationServerConectControl.Combobox.ToggleButton, new Point(230, 9));
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsLocalhostConnected.Exists, "localhost (Connected) option does not exist in Destination server combobox.");
            Mouse.Click(MainStudioWindow.ComboboxListItemAsLocalhostConnected, new Point(226, 13));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DestinationServerConectControl.Combobox.ConnectedLocalhostText.Exists, "Selected destination server in deploy is not localhost (Connected).");
        }

        [When(@"I Select RemoteConnectionIntegration From Deploy Tab Source Server Combobox")]
        [Then(@"I Select RemoteConnectionIntegration From Deploy Tab Source Server Combobox")]
        [Given(@"I Select RemoteConnectionIntegration From Deploy Tab Source Server Combobox")]
        public void Select_RemoteConnectionIntegration_From_Deploy_Tab_Source_Server_Combobox()
        {
            WaitForControlVisible(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerConectControl.Combobox.ToggleButton);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerConectControl.Combobox.ToggleButton);
            Mouse.Click(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegration);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerConectControl.Combobox.RemoteConnectionIntegrationText.Exists, "Selected source server in deploy is not Remote Connection Integration.");
        }

        [When(@"I Select LocalhostConnected From Deploy Tab Source Server Combobox")]
        [Then(@"I Select LocalhostConnected From Deploy Tab Source Server Combobox")]
        [Given(@"I Select LocalhostConnected From Deploy Tab Source Server Combobox")]
        public void WhenISelectLocalhostConnectedFromDeployWizardTabSourceServerCombobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerConectControl.Combobox.ToggleButton);
            Mouse.Click(MainStudioWindow.ComboboxListItemAsLocalhost);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerConectControl.Combobox.RemoteConnectionIntegrationText.Exists, "Selected source server in deploy is not Remote Connection Integration.");
        }


        [When(@"I Select localhost From Deploy Tab Source Server Combobox")]
        public void Select_localhost_From_Deploy_Tab_Source_Server_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerConectControl.Combobox.ToggleButton, new Point(230, 9));
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsLocalhostConnected.Exists, "localhost (Connected) option does not exist in Destination server combobox.");
            Mouse.Click(MainStudioWindow.ComboboxListItemAsLocalhostConnected, new Point(226, 13));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerConectControl.Combobox.LocalhostText.Exists, "Selected source server in deploy is not localhost (Connected).");
        }

        [Given(@"I Select RemoteConnectionIntegration From Explorer")]
        [When(@"I Select RemoteConnectionIntegration From Explorer")]
        [Then(@"I Select RemoteConnectionIntegration From Explorer")]
        public void Select_RemoteConnectionIntegration_From_Explorer()
        {
            var toggleButton = MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.ToggleButton;
            Mouse.Click(toggleButton, new Point(136, 7));
            Playback.Wait(1000);
            Mouse.Click(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegration.Text, new Point(138, 6));
        }

        [Given(@"I Select Connected RemoteConnectionIntegration From Explorer")]
        [When(@"I Select Connected RemoteConnectionIntegration From Explorer")]
        [Then(@"I Select Connected RemoteConnectionIntegration From Explorer")]
        public void Select_ConnectedRemoteConnectionIntegration_From_Explorer()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ConnectControl.ServerComboBox.ToggleButton, new Point(136, 7));
            Playback.Wait(500);
            Mouse.Click(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegrationConnected.Text, new Point(138, 6));
        }

        [When(@"I Select RemoteConnectionIntegration \(Connected\) From Deploy Tab Source Server Combobox")]
        public void Select_ConnectedRemoteConnectionIntegration_From_Deploy_Tab_Source_Server_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerConectControl.Combobox.ToggleButton, new Point(230, 9));
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsNewRemoteServer.Exists, "New Remote Server... option does not exist in Destination server combobox.");
            Assert.IsTrue(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegrationConnected.Exists, "Remote Connection Integration option does not exist in Source server combobox.");
            Mouse.Click(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegrationConnected.Text, new Point(226, 13));
            Assert.AreEqual("Remote Connection Integration", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerConectControl.Combobox.RemoteConnectionIntegrationText.DisplayText, "Selected source server in deploy is not Remote Connection Integration.");
        }

        [When(@"I Select Round Up")]
        public void Select_Round_Up()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FormatNumber.LargeViewContentCustom.RoundingComboBox, new Point(119, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FormatNumber.LargeViewContentCustom.RoundingComboBox.RoungUP, new Point(114, 13));
        }

        [When(@"I Select RoundingType None")]
        public void Select_RoundingType_None()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FormatNumber.LargeViewContentCustom.RoundingComboBox, new Point(119, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FormatNumber.LargeViewContentCustom.RoundingComboBox.None, new Point(114, 13));
        }

        [When(@"I Select RoundingType Normal")]
        public void Select_RoundingType_Normal()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FormatNumber.LargeViewContentCustom.RoundingComboBox, new Point(119, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.FormatNumber.LargeViewContentCustom.RoundingComboBox.Normal, new Point(114, 13));
        }

        [When(@"I Select RSAKLFSVRGENDEV From Server Source Wizard Dropdownlist")]
        public void Select_RSAKLFSVRGENDEV_From_Server_Source_Wizard_Dropdownlist()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.ServerComboBox.RSAKLFSVRGENDEV, new Point(97, 17));
            Assert.AreEqual("RSAKLFSVRGENDEV", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.ServerComboBox.Textbox.Text, "RSAKLFSVRGENDEV is not selected as the server in the DB source wizard.");
        }

        [When(@"I Select SaveAsImage FromContextMenu")]
        public void Select_SaveAsImage_FromContextMenu()
        {
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.SaveasImage, new Point(38, 15));
        }

        [When(@"I Select SecurityTab")]
        public void Select_SecurityTab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SettingsTab.WorksurfaceContext.SettingsView.TabList.SecurityTab, new Point(102, 10));
        }

        [When(@"I Select SetAsStartNode FromContextMenu")]
        public void Select_SetAsStartNode_FromContextMenu()
        {
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.SetasStartNode, new Point(67, 16));
        }

        [When(@"I Select SharepointTestServer")]
        public void Select_SharepointTestServer()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCreateListItem.SmallView.Server, new Point(98, 12));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCreateListItem.SmallView.Server.SharepointTestServer, new Point(67, 13));
        }

        [When(@"I Select SharepointTestServer From SharepointRead Tool")]
        public void Select_SharepointTestServer_From_SharepointRead_Tool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadListItem.SmallView.Server, new Point(98, 12));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadListItem.SmallView.Server.SharepointTestServer, new Point(67, 13));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointReadListItem.SmallView.EditSourceButton.Enabled, "edit sharepoint source is disabled after selecting a source");
        }

        [When(@"I Select SharepointTestServer From SharepointUpdateListItem Tool")]
        public void Select_SharepointTestServer_From_SharepointUpdateListItem_Tool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUpdateListItem.SmallView.Server, new Point(98, 12));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUpdateListItem.SmallView.Server.SharepointTestServer, new Point(67, 13));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointUpdateListItem.SmallView.EditSourceButton.Enabled, "edit sharepoint source is disabled after selecting a source");
        }

        [When(@"I Select ShowLargeView FromContextMenu")]
        public void Select_ShowLargeView_FromContextMenu()
        {
            Mouse.Click(MainStudioWindow.DesignSurfaceContextMenu.ShowLargeView, new Point(43, 15));
        }

        [When(@"I Select Source From DotnetTool")]
        public void Select_Source_From_DotnetTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.SourcesComboBox, new Point(119, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.SourcesComboBox.DotNetSource, new Point(114, 13));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox.Enabled, "ClassNameComboBox is not Enabled after selecting a source");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox, new Point(119, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox.AssemblyLocationGACCListItem, new Point(114, 13));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ActionMethodListBoxList.ActivitiesDesignListItem.ActivitiesExpander.ActivitiesDesignButton.ActionsComboBox, new Point(119, 7));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.GenerateOutputsButton.Enabled, "GenerateOutputsButton is not Enabled after selecting an Action");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.LargeDataGridTable.Row1.Enabled, "InputsDataGridTable is not Enabled after selecting an Action");
            Mouse.Click(MainStudioWindow.SideMenuBar.SaveButton, new Point(10, 5));
        }

        [When(@"I Select SystemObject From DotNet DLL Large View Namespace Combobox")]
        public void Select_SystemObject_From_DotNet_DLL_Large_View_Namespace_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox, new Point(216, 7));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox.ComboboxlistItemAsSystemObject.Exists, "System.Random item does not exist in the DotNet DLL tool ClassName dropdown");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox.ComboboxlistItemAsSystemObject, new Point(137, 7));
            Assert.AreEqual("{\"AssemblyLocation\":\"C:\\\\Windows\\\\Microsoft.NET\\\\Framework64\\\\v4.0.30319\\\\mscorli" +
                            "b.dll\",\"AssemblyName\":\"mscorlib.dll\",\"FullName\":\"System.Object\",\"MethodName\":nul" +
                            "l}", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox.SelectedItem, "System.Object is not selected in DotNet DLL tool large view namespace combobox.");
        }

        [When(@"I Select SystemRandom From DotNet DLL Large View Namespace Combobox")]
        public void Select_SystemRandom_From_DotNet_DLL_Large_View_Namespace_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox, new Point(216, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox.SystemRandomListItem, new Point(137, 7));
            Assert.AreEqual("{\"AssemblyLocation\":\"C:\\\\Windows\\\\Microsoft.NET\\\\Framework64\\\\v4.0.30319\\\\mscorli" +
                            "b.dll\",\"AssemblyName\":\"mscorlib.dll\",\"FullName\":\"System.Random\",\"MethodName\":nul" +
                            "l}", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ClassNameComboBox.SelectedItem, "System.Random is not selected in DotNet DLL tool large view namespace combobox.");
        }

        [When(@"I Select ToString From DotNet DLL Large View Action Combobox")]
        public void Select_ToString_From_DotNet_DLL_Large_View_Action_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ActionMethodListBoxList.ActivitiesDesignListItem.ActivitiesExpander.ActivitiesDesignButton.ActionsComboBox, new Point(216, 7));
        }

        [When(@"I Select TSTCIREMOTE From Server Source Wizard Dropdownlist")]
        public void Select_TSTCIREMOTE_From_Server_Source_Wizard_Dropdownlist()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.NewServerSource.AddressComboBox.TSTCIREMOTE, new Point(70, 19));
            Assert.AreEqual("TST-CI-REMOTE", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.NewServerSource.AddressComboBox.AddressEditBox.Text, "Server source address textbox text does not equal TST-CI-REMOTE");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.NewServerSource.TestConnectionButton.Exists, "Server source wizard does not contain a test connection button");
        }

        [When(@"I Select UITestingDBSource From SQL Server Large View Source Combobox")]
        public void Select_UITestingDBSource_From_SQL_Server_Large_View_Source_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.SourcesCombobox, new Point(216, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.SourcesCombobox.UITestingDBSourceListItem, new Point(137, 7));
            Assert.AreEqual("UITestingDBSource", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.SourcesCombobox.SelectedItem, "SQL Server large view source combobox selected item is not equal to UITestingDBSource.");
        }

        [When(@"I Select UITestingSource From Web Server Large View Source Combobox")]
        public void Select_UITestingSource_From_Web_Server_Large_View_Source_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.SourcesComboBox, new Point(216, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.SourcesComboBox.UITesting, new Point(137, 7));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.EditSourceButton.Enabled, "Delete Web large view source combobox EDIT button is disabled.");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.GenerateOutputsButton.Enabled, "Delete Web large view source combobox GenerateOutput button is disabled.");
        }

        [When(@"I Select User From RunTestAs")]
        public void Select_User_From_RunTestAs()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.UserRadioButton.Selected = true;
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.UsernameTextBoxEdit.Exists, "Username textbox does not exist after clicking RunAsUser radio button");
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.PasswordTextBoxEdit.Exists, "Password textbox does not exist after clicking RunAsUser radio button");
            Assert.IsTrue(MainStudioWindow.SideMenuBar.SaveButton.Enabled, "Save Ribbon Menu buton is disabled after changing test");
        }

        [When(@"I Select Zip Compression")]
        public void Select_Zip_Compression()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Zip.LargeViewContentCustom.SelectedCompressComboBox, new Point(119, 7));
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Zip.LargeViewContentCustom.SelectedCompressComboBox.NormalDefault, new Point(114, 13));
        }

        [When(@"I Type 0 Into SQL Server Large View Inputs Row1 Data Textbox")]
        public void Type_0_Into_SQL_Server_Large_View_Inputs_Row1_Data_Textbox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.InputsTable.Row1.DataCell.DataCombobox.DataTextbox.Text = "0";
            Assert.AreEqual("0", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.InputsTable.Row1.DataCell.DataCombobox.DataTextbox.Text, "SQL Server large view inputs row 1 data textbox text is not equal to S");
        }

        [When(@"I Type 0 Into SQL Server Large View Test Inputs Row1 Test Data Textbox")]
        public void Type_0_Into_SQL_Server_Large_View_Test_Inputs_Row1_Test_Data_Textbox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.TestInputsTable.Row1.TestDataCell.TestDataComboBox.TestDataTextbox.Text = "0";
            Assert.AreEqual("0", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.TestInputsTable.Row1.TestDataCell.TestDataComboBox.TestDataTextbox.Text, "SQL Server large view test inputs row 1 test data textbox text is not equal to S");
        }

        [When(@"I Type rsaklfsvrgen into DB Source Wizard Server Textbox")]
        public void Type_rsaklfsvrgen_into_DB_Source_Wizard_Server_Textbox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.ServerComboBox.Textbox.Text = "rsaklfsvrgen";
        }

        [Given(@"RSAKLFSVRGENDEV appears as an option in the DB source wizard server combobox")]
        [Then(@"RSAKLFSVRGENDEV appears as an option in the DB source wizard server combobox")]
        public void Assert_RSAKLFSVRGENDEV_appears_as_an_option_in_the_DB_source_wizard_server_combobox()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.ServerComboBox.RSAKLFSVRGENDEV.Exists, "RSAKLFSVRGENDEV does not exist as an option in DB source wizard server combobox.");
        }

        [When(@"I Type RSAKLFSVRGENDEV into DB Source Wizard Server Textbox")]
        public void Type_RSAKLFSVRGENDEV_into_DB_Source_Wizard_Server_Textbox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.ServerComboBox.Textbox.Text = "RSAKLFSVRGENDEV";
        }

        [When(@"I Type The Testing Site into Web POST Source Wizard Address Textbox")]
        public void Type_The_Testing_Site_into_Web_POST_Source_Wizard_Address_Textbox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.WorkSurfaceContext.AddressTextbox.Text = "http://rsaklfsvrtfsbld:9810/api/products/Post";
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.WorkSurfaceContext.TestConnectionButton.Enabled, "New web source wizard test connection button is not enabled after entering a valid web post address.");
        }

        [When(@"I Type The Testing Site into Web DELETE Source Wizard Address Textbox")]
        public void Type_The_Testing_Site_into_Web_DELETE_Source_Wizard_Address_Textbox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.WorkSurfaceContext.AddressTextbox.Text = "http://rsaklfsvrtfsbld:9810/api/products/Delete";
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.WorkSurfaceContext.TestConnectionButton.Enabled, "New web source wizard test connection button is not enabled after entering a valid web delete address.");
        }

        [When(@"I Type The Testing Site into Web PUT Source Wizard Address Textbox")]
        public void Type_The_Testing_Site_into_Web_PUT_Source_Wizard_Address_Textbox()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.WorkSurfaceContext.AddressTextbox.Text = "http://rsaklfsvrtfsbld:9810/api/products/Put";
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.WorkSurfaceContext.TestConnectionButton.Enabled, "New web source wizard test connection button is not enabled after entering a valid web put address.");
        }

        [When(@"I Click Decision Large View Match Type Combobox")]
        public void Click_Decision_Large_View_Match_Type_Combobox()
        {
            Mouse.Click(DecisionOrSwitchDialog.LargeView.Table.Row1.MatchTypeCell.MatchTypeCombobox, new Point(5, 5));
        }

        [When(@"I Make Workflow Savable")]
        public void Make_Workflow_Savable()
        {
            Drag_Toolbox_Comment_Onto_DesignSurface();
        }

        public void Move_Assign_Message_Tool_On_The_Design_Surface()
        {
            var multiAssign1 = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign;
            multiAssign1.EnsureClickable(new Point(90, 7));
            Mouse.StartDragging(multiAssign1, new Point(94, 11));
            Mouse.StopDragging(multiAssign1, 70, 3);
        }

        [When(@"I Drag Explorer First Sub Item Onto Second Sub Item")]
        public void Drag_Explorer_First_Sub_Item_Onto_Second_Sub_Item()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.SecondSubItem.EnsureClickable(new Point(90, 7));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem, new Point(94, 11));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.SecondSubItem, new Point(90, 7));
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.Spinner);
        }
        [When(@"I Drag Explorer First Item Onto Second Sub Item")]
        public void Drag_Explorer_First_Item_Onto_Second_Item()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem.EnsureClickable(new Point(90, 7));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem, new Point(94, 11));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem, new Point(90, 7));
        }

        [Given(@"I Double Click Localhost Server")]
        [When(@"I Double Click Localhost Server")]
        [Then(@"I Double Click Localhost Server")]
        public void DoubleClick_Localhost_Server()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost);
        }

        [When(@"I Click Web Browser Error Messagebox OK Button")]
        public void Click_Web_Browser_Error_Messagebox_OK_Button()
        {
            Mouse.Click(WebBrowserErrorWindow.Pane.OKButton, new Point(30, 8));
        }


        [Then(@"I Click Create Test From Debug")]
        [Given(@"I Click Create Test From Debug")]
        [When(@"I Click Create Test From Debug")]
        public void Click_Create_Test_From_Debug()
        {
            int CreateTestButtonEnabledTimeout = 60000;
            WaitForControlEnabled(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.CreateTestFromDebugButton, CreateTestButtonEnabledTimeout);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.CreateTestFromDebugButton.Enabled, "Debug Output New Test button not enabled after waiting for " + CreateTestButtonEnabledTimeout + "ms.");
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.CreateTestFromDebugButton, new Point(5, 5));
        }

        public void Save_Button_IsEnabled()
        {
            MainStudioWindow.SideMenuBar.SaveButton.EnsureClickable();
        }

        [Then(@"Test Tab Is Open")]
        [Given(@"Test Tab Is Open")]
        [When(@"Test Tab Is Open")]
        public void Test_Tab_Is_Open()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.Exists, "Test tab does not exist.");
        }

        [Then(@"Hello World Workflow Tab Is Open")]
        [Given(@"Hello World Workflow Tab Is Open")]
        public void Hello_World_Workflow_Tab_Is_Open()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.UIHelloWorldText.Exists, "Hello World workflow tab does not exist.");
        }

        public void Click_MockRadioButton_On_AssignValue_TestStep()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.DecisionTreeItem.DecisionAssert.SmallDataGridTable.ColumnHeadersPrHeader.MockOrAssert.MockRadioButton, new Point(5, 5));
        }

        public void Try_Click_Create_New_Tests()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.TestsListboxList.CreateTest.CreateTestButton, new Point(158, 10));
        }

        public void Click_Delete_On_AssignValue_TestStep()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.OutputMessageStep.OutputStepHeader.Delete.DrawHighlight();
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.OutputMessageStep.OutputStepHeader.Delete.Enabled);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.OutputMessageStep.OutputStepHeader.Delete);
        }

        [When(@"I Click AssigName From DesignSurface")]
        public void Click_AssigName_From_DesignSurface()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.UserControl_1Custom.ScrollViewerPane.ActivityBuilderCustom.WorkflowItemPresenteCustom.FlowchartCustom.DsfMultiAssignActiviCustom);
        }

        public int Expand_Comment_Tool_Size()
        {
            var defaultHeight = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Comment.LargeViewContentCustom.Height;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Comment.LargeViewContentCustom.EnsureClickable(new Point(226, 335));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Comment.LargeViewContentCustom.ItemResizer);
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Comment.LargeViewContentCustom, new Point(0, 350));
            var newHeight = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Comment.LargeViewContentCustom.Height;

            Assert.IsTrue(newHeight > defaultHeight, "Comment tool height has not changed after dragging the resize indicator downward.");
            return newHeight;
        }

        public void Click_MockRadioButton_On_Decision_TestStep()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.DecisionTreeItem.DecisionAssert.SmallDataGridTable.ColumnHeadersPrHeader.MockOrAssert.MockRadioButton, new Point(5, 5));
        }

        public void Click_MockRadioButton_On_Assign_TestStep()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.AssignToNameTreeItem.AssignAssert.SmallDataGridTable.ColumnHeadersPrHeader.MockOrAssert.MockRadioButton, new Point(5, 5));
        }
        public void Click_MockRadioButton_On_TestStep()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.StepTestDataTreeTree.SetOutputTreeItem.OutputMessageAssert.SmallDataGridTable.ColumnHeadersPrHeader.MockOrAssert.MockRadioButton, new Point(5, 5));
        }

        [When(@"I Expand Debug Output Recordset")]
        public void Expand_Debug_Output_Recordset()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.DebugOutputTree.Step1.RecordsetGroup.Expanded = true;
        }

        [Then(@"The GetCountries Recordset Is Visible in Debug Output")]
        public void ThenTheDebugOutputShowsGetCountriesRecordset()
        {
            Assert.AreEqual("[[dbo_GetCountries(204).CountryID]]", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.DebugOutputTree.Step1.RecordsetGroup.RecordsetName.DisplayText, "Wrong recordset name in debug output for new DB connector.");
            Assert.AreEqual("155", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.DebugOutput.DebugOutputTree.Step1.RecordsetGroup.RecordsetValue.DisplayText, "Wrong recordset value in debug output for new DB connector.");
        }
        [Given(@"I have Hello World workflow on the Explorer")]
        [When(@"I have Hello World workflow on the Explorer")]
        [Then(@"I have Hello World workflow on the Explorer")]
        public void GivenIHaveHelloWorldWorkflowOnTheExplorer()
        {
            Filter_Explorer("Hello World");
        }

        [Given(@"I Click The Create ""(.*)""th test Button")]
        [When(@"I Click The Create ""(.*)""th test Button")]
        [Then(@"I Click The Create ""(.*)""th test Button")]
        public void GivenIClickTheCreateThTestButton(int testIntance)
        {
            Click_Create_New_Tests(true, testIntance);
        }

        [Then(@"Message box window appears")]
        [When(@"Message box window appears")]
        [Given(@"Message box window appears")]
        public void ThenMessageBoxWindowAppears()
        {
            Assert.IsTrue(MessageBoxWindow.Exists);
        }

        [Then(@"Test tab is open")]
        [Given(@"Test tab is open")]
        [When(@"Test tab is open")]
        public void ThenTestTabIsOpen()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.Exists);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.Exists);
        }

        [Then(@"I click Run ""(.*)""th test expecting ""(.*)""")]
        [When(@"I click Run ""(.*)""th test expecting ""(.*)""")]
        [Given(@"I click Run ""(.*)""th test expecting ""(.*)""")]
        public void ThenIClickRunThTestExpecting(int testInstance, string status)
        {
            var statusEnum = GetStatus(status);
            Click_Run_Test_Button(statusEnum, testInstance);
        }
        private TestResultEnum GetStatus(string status)
        {
            if (status == "Pending")
                return TestResultEnum.Pending;
            else if (status == "Invalid")
                return TestResultEnum.Invalid;
            else if (status == "Fail")
                return TestResultEnum.Fail;
            else
                return TestResultEnum.Pass;
        }

        [When(@"I Click Test Tab")]
        [Then(@"I Click Test Tab")]
        [Given(@"I Click Test Tab")]
        public void WhenIClickTestTab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab);
        }

        [Then(@"The Test step in now ""(.*)""")]
        [When(@"The Test step in now ""(.*)""")]
        [Given(@"The Test step in now ""(.*)""")]
        public void ThenTheTestStepInNow(string status)
        {
            Assert.AreEqual(TestResultEnum.Invalid, GetStatus(status));
        }

        [Then(@"I Click Run all tests button")]
        [When(@"I Click Run all tests button")]
        [Given(@"I Click Run all tests button")]
        public void ThenIClickRunAllTestsButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.RunAllButton);
        }

        [Then(@"I Click workflow tab")]
        [Given(@"I Click workflow tab")]
        [When(@"I Click workflow tab")]
        public void ThenIClickWorkflowWizardTab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Exists);
        }

        [Then(@"I Enter ""(.*)"" in the Assign message tool")]
        [When(@"I Enter ""(.*)"" in the Assign message tool")]
        [Given(@"I Enter ""(.*)"" in the Assign message tool")]
        public void ThenIEnterInTheAssignMessageTool(string message)
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MultiAssign.SmallView.DataGrid.Row1.ValueCell.IntellisenseCombobox.Textbox.Text = message;
        }


        /// <summary>
        /// Create_New_Folder_Using_Shortcut - Use 'Create_New_Folder_Using_ShortcutParams' to pass parameters into this method.
        /// </summary>
        public void Create_New_Folder_Using_Shortcut()
        {
            #region Variable Declarations
            WpfTreeItem localhost = MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost;
            #endregion

            Mouse.Click(localhost, new Point(74, 8));

            Keyboard.SendKeys(localhost, "F", (ModifierKeys.Control | ModifierKeys.Shift));
        }

        [Given(@"I Create New Workflow using shortcut")]
        [When(@"I Create New Workflow using shortcut")]
        [Then(@"I Create New Workflow using shortcut")]
        public void Create_New_Workflow_In_LocalHost_With_Shortcut()
        {
            #region Variable Declarations
            WpfTreeItem localhost = MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost;
            #endregion

            Mouse.Click(localhost, new Point(74, 8));

            Keyboard.SendKeys(localhost, "W", (ModifierKeys.Control));
        }

        public void Create_New_Workflow_In_Explorer_First_Item_With_Shortcut()
        {
            #region Variable Declarations
            WpfTreeItem firstItem = MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem;
            #endregion

            Mouse.Click(firstItem, new Point(74, 8));

            Keyboard.SendKeys(firstItem, "W", (ModifierKeys.Control));
        }

        public void Create_New_Workflow_Using_Shortcut()
        {
            #region Variable Declarations
            WpfTreeItem localhost = MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost;
            #endregion

            Mouse.Click(localhost, new Point(74, 8));

            Keyboard.SendKeys(localhost, "W", (ModifierKeys.Control));
        }

        public void Open_Deploy_Using_Shortcut()
        {
            Keyboard.SendKeys("D", (ModifierKeys.Control));
        }

        [Given(@"I Save Workflow Using Shortcut")]
        [When(@"I Save Workflow Using Shortcut")]
        [Then(@"I Save Workflow Using Shortcut")]
        public void Save_Workflow_Using_Shortcut()
        {
            #region Variable Declarations
            WpfCustom flowchart = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart;
            #endregion

            Mouse.Click(flowchart, new Point(74, 8));

            Keyboard.SendKeys(flowchart, "S", (ModifierKeys.Control));
        }

        [Given(@"I am connected on a remote server")]
        [When(@"I am connected on a remote server")]
        [Then(@"I am connected on a remote server")]
        public void GivenIAmConnectedOnARemoteServer()
        {
            Select_RemoteConnectionIntegration_From_Explorer();
            Click_Explorer_RemoteServer_Connect_Button();
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.Spinner);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.Exists);
        }

        [Then(@"Remote Server Refreshes")]
        public void ThenRemoteServerRefreshes()
        {
            Assert.IsTrue(ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.Spinner));
            Assert.IsTrue(ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.Spinner));
            Point point;
            Assert.IsFalse(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.Spinner.TryGetClickablePoint(out point));
        }

        [Then(@"Filtered Resourse Is Checked For Deploy")]
        public void ThenFilteredResourseIsCheckedForDeploy()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerExplorer.ExplorerTree.LocalHost.Item1.CheckBox.Checked);
        }

        [Then(@"Deploy Button Is Enabled")]
        public void ThenDeployButtonIsEnabled()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DeployButton.Enabled, "Deploy button is not enabled");
        }

        [Then(@"Deploy Version Conflict Window Shows")]
        public void ThenDeployVersionConflictWindowShows()
        {
            Assert.IsTrue(MessageBoxWindow.Exists);
            Assert.IsTrue(MessageBoxWindow.DeployVersionConflicText.Exists);
        }

        [Then(@"Deploy is Successfully")]
        [When(@"Deploy is Successfully")]
        [Given(@"Deploy is Successfully")]
        public void ThenDeployIsSuccessfully()
        {
            Assert.IsTrue(MessageBoxWindow.Exists);
            Assert.IsTrue(MessageBoxWindow.ResourcesDeployedSucText.Exists);
        }

        [Then(@"The deploy validation message is ""(.*)""")]
        [When(@"The deploy validation message is ""(.*)""")]
        [Given(@"The deploy validation message is ""(.*)""")]
        public void ThenTheDeployValidationMessageIs(string message)
        {
            Assert.AreEqual(message, MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DeployButtonMessageText.DisplayText);
        }

        [Given(@"I Click Save Ribbon Button to Open Save Dialog")]
        [When(@"I Click Save Ribbon Button to Open Save Dialog")]
        [Then(@"I Click Save Ribbon Button to Open Save Dialog")]
        public void Click_Save_Ribbon_Button_to_Open_Save_Dialog()
        {
            Mouse.Click(MainStudioWindow.SideMenuBar.SaveButton);
            Assert.IsTrue(SaveDialogWindow.Exists, "Save dialog does not exist after clicking save ribbon button.");
        }

        [Given(@"I Click MessageBox Cancel")]
        [When(@"I Click MessageBox Cancel")]
        [Then(@"I Click MessageBox Cancel")]
        public void ThenIClickMessageBoxCancel()
        {
            Mouse.Click(MessageBoxWindow.CancelButton);
        }

        [Given(@"Deploy Window Is Still Open")]
        [When(@"Deploy Window Is Still Open")]
        [Then(@"Deploy Window Is Still Open")]
        public void ThenDeployWindowIsStillOpen()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.Exists);
        }

        [Then(@"Destination Deploy Information Clears")]
        public void ThenDestinationDeployInformationClears()
        {
            Assert.IsFalse(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DeployButton.Enabled);
            Assert.IsFalse(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.ShowDependenciesButton.Enabled);
        }

        public void Click_SelectAllDependencies_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.ShowDependenciesButton);
        }

        [Given(@"Filter Textbox is cleared")]
        [When(@"Filter Textbox is cleared")]
        [Then(@"Filter Textbox is cleared")]
        public void ThenFilterTextboxIsCleared()
        {
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.Spinner);
            Assert.IsTrue(string.IsNullOrEmpty(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.SearchTextBox.Text), "Explorer Filter Textbox text is not blank after clicking the clear button.");
        }

        [Given(@"Filter Textbox has ""(.*)""")]
        [When(@"Filter Textbox has ""(.*)""")]
        [Then(@"Filter Textbox has ""(.*)""")]
        public void ThenFilterTextboxHas(string filterText)
        {
            Assert.AreEqual(filterText, MainStudioWindow.DockManager.SplitPaneLeft.Explorer.SearchTextBox.Text);
        }

        [Given(@"Unit Tests Url Exists")]
        [When(@"Unit Tests Url Exists")]
        [Then(@"Unit Tests Url Exists")]
        public void UnitTestUrlExists()
        {
            #region Variable Declarations
            WpfHyperlink unitTestsUrlWorkflowUrlHyperlink = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.TopScrollViewerPane.UnitTestsUrlWorkflowUrlText.UnitTestsUrlWorkflowUrlHyperlink;
            #endregion

            // Verify that the 'Exists' property of 'http://rsaklfsanele:3142/secure/Unit Tests/Unsaved...' link equals 'True'
            Assert.IsTrue(unitTestsUrlWorkflowUrlHyperlink.Exists, "UnitTestsUrlWorkflowUrl does not exist");
        }

        [Given(@"I Rename Save Dialog Explorer First Item To ""(.*)""")]
        [When(@"I Rename Save Dialog Explorer First Item To ""(.*)""")]
        [Then(@"I Rename Save Dialog Explorer First Item To ""(.*)""")]
        public void Rename_Folder_From_Save_Dialog(string filterText)
        {
            #region Variable Declarations            
            WpfEdit uIItemEdit = SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.UIItemEdit;
            #endregion
            uIItemEdit.Text = filterText;
        }

        [Given(@"I Name New Folder as ""(.*)""")]
        [When(@"I Name New Folder as ""(.*)""")]
        [Then(@"I Name New Folder as ""(.*)""")]
        public void Name_New_Folder_From_Save_Dialog(string name)
        {
            #region Variable Declarations
            WpfEdit newFolderEdit = this.SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.UIItemEdit;
            #endregion

            // Type 'NewFolder' in text box
            newFolderEdit.Text = name;

            // Type '{Enter}' in text box
            Keyboard.SendKeys(newFolderEdit, "{Right}{Enter}", ModifierKeys.None);
        }

        [Given(@"I Hit Escape Key On The Keyboard")]
        [When(@"I Hit Escape Key On The Keyboard")]
        [Then(@"I Hit Escape Key On The Keyboard")]
        public void ThenIHitEscapeKeyOnTheKeyboard()
        {
            #region Variable Declarations
            WpfEdit newFolderEdit = this.SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.UIItemEdit;
            #endregion

            // Type '{Enter}' in text box
            Keyboard.SendKeys(newFolderEdit, "{Escape}", ModifierKeys.None);
        }

        [Given(@"I Hit Escape Key On The Keyboard on Activity Default Window")]
        [When(@"I Hit Escape Key On The Keyboard on Activity Default Window")]
        [Then(@"I Hit Escape Key On The Keyboard on Activity Default Window")]
        public void WhenIHitEscapeKeyOnTheKeyboardOnActivityDefaultWindow()
        {
            Keyboard.SendKeys(DecisionOrSwitchDialog, "{Escape}", ModifierKeys.None);
        }

        [Given(@"I Dont Name The Created Folder")]
        [When(@"I Dont Name The Created Folder")]
        [Then(@"I Dont Name The Created Folder")]
        public void ThenIDontNameTheCreatedFolder()
        {
            WpfEdit newFolderEdit = this.SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.UIItemEdit;
            WpfButton saveButton = this.SaveDialogWindow.SaveButton;

            Keyboard.SendKeys(newFolderEdit, "{Right}{Enter}", ModifierKeys.None);
            // Click 'Save' button
            Mouse.Click(saveButton, new Point(22, 16));
        }

        [Given(@"I Enter New Folder Name as ""(.*)""")]
        [When(@"I Enter New Folder Name as ""(.*)""")]
        [Then(@"I Enter New Folder Name as ""(.*)""")]
        public void EnterNewFolderNameAs(string name)
        {
            #region Variable Declarations
            WpfEdit newFolderEdit = this.SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.UIItemEdit;
            WpfButton saveButton = this.SaveDialogWindow.SaveButton;
            #endregion

            // Type 'NewFolder' in text box
            newFolderEdit.Text = name;

            // Type '{Enter}' in text box
            Keyboard.SendKeys(newFolderEdit, "{Right}{Enter}", ModifierKeys.None);
        }

        [Given(@"I Enter New Sub Folder Name as ""(.*)""")]
        [When(@"I Enter New Sub Folder Name as ""(.*)""")]
        [Then(@"I Enter New Sub Folder Name as ""(.*)""")]
        public void ThenIEnterNewSubFolderNameAs(string name)
        {
            #region Variable Declarations
            WpfEdit newFolderEdit = this.SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.FirstSubItem.UIItemEdit;
            WpfEdit namedFolderExit = this.SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.FirstSubItem.UIItemEdit;
            WpfButton saveButton = this.SaveDialogWindow.SaveButton;
            #endregion

            // Type 'NewFolder' in text box
            newFolderEdit.Text = name;

            // Type '{Enter}' in text box
            Keyboard.SendKeys(namedFolderExit, "{Right}{Enter}", ModifierKeys.None);
        }


        [Given(@"I Name New Sub Folder as ""(.*)""")]
        [When(@"I Name New Sub Folder as ""(.*)""")]
        [Then(@"I Name New Sub Folder as ""(.*)""")]
        public void I_Name_New_Sub_Folder_As(string name)
        {
            #region Variable Declarations
            WpfEdit newFolderEdit = this.SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.FirstSubItem.UIItemEdit;
            WpfEdit namedFolderExit = this.SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.FirstSubItem.UIItemEdit;
            WpfButton saveButton = this.SaveDialogWindow.SaveButton;
            #endregion

            // Type 'NewFolder' in text box
            Keyboard.SendKeys(newFolderEdit, "{Back}", ModifierKeys.None);

            newFolderEdit.Text = name;

            // Type '{Enter}' in text box
            Keyboard.SendKeys(namedFolderExit, "{Right}{Enter}", ModifierKeys.None);
        }

        [Given(@"Explorer Contain Item ""(.*)""")]
        [When(@"Explorer Contain Item ""(.*)""")]
        [Then(@"Explorer Contain Item ""(.*)""")]
        public void ExplorerContainItem(string itemName)
        {
            Assert.AreEqual(itemName, MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.ItemEdit.Text);
        }

        [Given(@"Save Dialog Explorer Contain Item ""(.*)""")]
        [When(@"Save Dialog Explorer Contain Item ""(.*)""")]
        [Then(@"Save Dialog Explorer Contain Item ""(.*)""")]
        public void ThenSaveDialogExplorerContainItem(string itemName)
        {
            Assert.IsTrue(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.UIItemEdit.Text.Contains(itemName));
        }

        [Given(@"Explorer Does Not Contain Item ""(.*)""")]
        [When(@"Explorer Does Not Contain Item ""(.*)""")]
        [Then(@"Explorer Does Not Contain Item ""(.*)""")]
        public void ExplorerDoesNotContainItem(string p0)
        {
            Assert.IsFalse(ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem));
        }

        [Given(@"Explorer Contain Sub Item ""(.*)""")]
        [When(@"Explorer Contain Sub Item ""(.*)""")]
        [Then(@"Explorer Contain Sub Item ""(.*)""")]
        public void ExplorerContainSubFolder(string itemName)
        {
            Assert.AreEqual(itemName, MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem.ItemEdit.Text);
        }

        [Given(@"Explorer Items appear on the Explorer Tree")]
        [When(@"Explorer Items appear on the Explorer Tree")]
        [Then(@"Explorer Items appear on the Explorer Tree")]
        public void ExplorerItemsAppearOnTheExplorerTree()
        {
            Assert.IsTrue(ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem));
            Assert.IsTrue(ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem));
        }

        [Given(@"Explorer Items appear on the Save Dialog Explorer Tree")]
        [When(@"Explorer Items appear on the Save Dialog Explorer Tree")]
        [Then(@"Explorer Items appear on the Save Dialog Explorer Tree")]
        public void ExplorerItemsAppearOnTheSaveDialogExplorerTree()
        {
            Assert.IsTrue(ControlExistsNow(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem));
            Assert.IsTrue(ControlExistsNow(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.SecondItem));
        }

        [Given(@"Resource Did not Open")]
        [When(@"Resource Did not Open")]
        [Then(@"Resource Did not Open")]
        public void ResourceDidNotOpen()
        {
            WaitForControlVisible(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab);
            Assert.IsFalse(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.TabDescription.DisplayText.Contains("Hello World"));
        }

        [Given(@"""(.*)"" is child of ""(.*)""")]
        [When(@"""(.*)"" is child of ""(.*)""")]
        [Then(@"""(.*)"" is child of ""(.*)""")]
        public void FolderIsChildOfParentFolder(string child, string parent)
        {
            Assert.IsTrue(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.UIItemEdit.Text.Contains(parent));
            Assert.AreEqual(child, SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.FirstSubItem.UIItemEdit.Text);
        }

        [Given(@"""(.*)"" is child of localhost")]
        [When(@"""(.*)"" is child of localhost")]
        [Then(@"""(.*)"" is child of localhost")]
        public void ResourceIsChildOfLocalhost(string child)
        {
            Assert.IsTrue(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.Exists);
            Assert.IsTrue(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.UIItemEdit.Text.Contains(child));
        }

        [Given(@"I Move resource to localhost")]
        [When(@"I Move resource to localhost")]
        [Then(@"I Move resource to localhost")]
        public void MoveResourceToLocalhost()
        {
            SaveDialogWindow.ExplorerView.ExplorerTree.localhost.EnsureClickable(new Point(90, 11));
            Mouse.StartDragging(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.FirstSubItem, new Point(94, 11));
            Mouse.StopDragging(SaveDialogWindow.ExplorerView.ExplorerTree.localhost, new Point(90, 11));
        }

        [Given(@"I Move FolderToMove into FolderToRename")]
        [When(@"I Move FolderToMove into FolderToRename")]
        [Then(@"I Move FolderToMove into FolderToRename")]
        public void MoveFolderToMoveIntoFolderToRename()
        {
            SaveDialogWindow.ExplorerView.ExplorerTree.localhost.ThirdItem.EnsureClickable(new Point(90, 11));
            Mouse.StartDragging(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.SecondItem, new Point(94, 11));
            Mouse.StopDragging(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.ThirdItem, new Point(90, 11));
        }

        [Given(@"I Move FolderToRename into localhost")]
        [When(@"I Move FolderToRename into localhost")]
        [Then(@"I Move FolderToRename into localhost")]
        public void MoveFolderToRenameIntoLocalhost()
        {
            SaveDialogWindow.ExplorerView.ExplorerTree.localhost.EnsureClickable(new Point(90, 11));
            Mouse.StartDragging(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem, new Point(94, 11));
            Mouse.StopDragging(SaveDialogWindow.ExplorerView.ExplorerTree.localhost, new Point(90, 11));
        }

        [Then(@"""(.*)"" Resource Exists In Windows Directory ""(.*)""")]
        public void ResourceExistsInWindowsDirectory(string serviceName, string path)
        {
            Filter_Explorer(serviceName);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem.FirstSubItem.Exists);
        }


        [Given(@"Context Menu Has Two Items")]
        [When(@"Context Menu Has Two Items")]
        [Then(@"Context Menu Has Two Items")]
        public void ThenContextMenuHasTwoItems()
        {
            Assert.IsTrue(SaveDialogWindow.SaveDialogContextMenu.RenameMenuItem.Exists);
            Assert.IsTrue(SaveDialogWindow.SaveDialogContextMenu.UINewFolderMenuItem.Exists);
            Point point;
            Assert.IsFalse(SaveDialogWindow.SaveDialogContextMenu.SourcesMenuItem.TryGetClickablePoint(out point));
            Assert.IsFalse(SaveDialogWindow.SaveDialogContextMenu.DeleteMenuItem.TryGetClickablePoint(out point));
        }

        [Given(@"Folder Is Removed From Explorer")]
        [When(@"Folder Is Removed From Explorer")]
        [Then(@"Folder Is Removed From Explorer")]
        public void ThenFolderIsRemovedFromExplorer()
        {
            Assert.IsFalse(ControlExistsNow(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem));
        }
        [Given(@"I Filter Variable List ""(.*)""")]
        [When(@"I Filter Variable List ""(.*)""")]
        [Then(@"I Filter Variable List ""(.*)""")]
        public void Filter_VariableList(string text)
        {
            #region Variable Declarations
            WpfEdit searchText = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.SearchTextbox;
            #endregion

            searchText.Text = text;
        }

        [Given(@"I Click Clear Variable List Filter")]
        [When(@"I Click Clear Variable List Filter")]
        [Then(@"I Click Clear Variable List Filter")]
        public void Click_Clear_Variable_List_Filter()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.SearchTextbox.ClearSearchButton);
        }

        public void Set_Input_Output_Variables()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem1.InputCheckbox.Checked = true;
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.ContentPane.ContentDockManager.SplitPaneRight.Variables.DatalistView.VariableTree.VariableTreeItem.TreeItem2.OutputCheckbox.Checked = true;
        }

        public void Resize_Decision_LargeTool()
        {
            #region Variable Declarations
            WpfWindow uIActivityDefaultWindoWindow = this.DecisionOrSwitchDialog;
            #endregion

            Mouse.StartDragging(uIActivityDefaultWindoWindow, new Point(396, 387));
            Mouse.StopDragging(uIActivityDefaultWindoWindow, new Point(0, 450));
        }

        [Given(@"Destination Remote Server Is Connected")]
        [Then(@"Destination Remote Server Is Connected")]
        public void ThenDestinationRemoteServerIsConnected()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.DestinationServerConectControl.Combobox.ConnectedRemoteConnectionText.Exists, "Remote Server is Disconnected");
        }

        public void Enter_Text_Into_Exchange_Tab()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ExchangeSourceTab.SendTestModelsCustom.AutoDiscoverUrlTxtBox.Text = "https://outlook.office365.com/EWS/Exchange.asmx";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ExchangeSourceTab.SendTestModelsCustom.UserNameTextBox.Text = "Nkosinathi.Sangweni@TheUnlimited.co.za";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ExchangeSourceTab.SendTestModelsCustom.PasswordTextBox.Text = "Password123";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ExchangeSourceTab.SendTestModelsCustom.ToTextBox.Text = "dev2warewolf@gmail.com";
        }

        [When(@"I Click ExchangeSource TestConnection Button")]
        public void Click_ExchangeSource_TestConnection_Button()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ExchangeSourceTab.SendTestModelsCustom.TestConnectionButton, new Point(58, 16));
            WaitForSpinner(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ExchangeSourceTab.SendTestModelsCustom.Spinner);
        }


        [Then(@"There is an error")]
        public void TheArdonerhasAnError()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Adornert_numbernText.Exists, "The tool is valid and did not throw any errors on Test Clicked.");
        }

        [Given(@"I DoubleClick Explorer Localhost First Item")]
        [When(@"I DoubleClick Explorer Localhost First Item")]
        [Then(@"I DoubleClick Explorer Localhost First Item")]
        public void DoubleClick_Explorer_Localhost_First_Item()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem);
        }

        [Given(@"I DoubleClick Explorer Localhost Second Item")]
        [When(@"I DoubleClick Explorer Localhost Second Item")]
        [Then(@"I DoubleClick Explorer Localhost Second Item")]
        public void DoubleClick_Explorer_Localhost_Second_Item()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.SecondItem);
        }

        [Given(@"I RightClick Ardoner Hyperlink")]
        [When(@"I RightClick Ardoner Hyperlink")]
        [Then(@"I RightClick Ardoner Hyperlink")]
        public void RightClick_Adorner_Control()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Adornert_numbernText.NumbernHyperlink, MouseButtons.Right, ModifierKeys.None, new Point(88, 12));
        }

        [Given(@"Filtered Item Exists")]
        [When(@"Filtered Item Exists")]
        [Then(@"Filtered Item Exists")]
        public void FilteredItemExists()
        {
            Assert.IsTrue(SaveDialogWindow.ExplorerView.ExplorerTree.localhost.FirstItem.Exists);
        }

        [Given(@"I drag a ""(.*)"" tool")]
        [When(@"I drag a ""(.*)"" tool")]
        [Then(@"I drag a ""(.*)"" tool")]
        public void WhenIDragATool(string tool)
        {
            Drag_Toolbox_Sharepoint_CopyFile_Onto_DesignSurface();
        }

        /// <summary>
        /// Click_Output_Step
        /// </summary>
        public void Click_Output_Step()
        {
            #region Variable Declarations
            WpfCustom uIDsfMultiAssignActiviCustom = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.UserControl_1Custom.ScrollViewerPane.ActivityBuilderCustom.WorkflowItemPresenteCustom.UIFlowchartCustom.UIDsfMultiAssignActiviCustom;
            #endregion

            // Click 'DsfMultiAssignActivity' custom control
            Mouse.Click(uIDsfMultiAssignActiviCustom, new Point(77, 8));
        }

        /// <summary>
        /// Click_Output_Step
        /// </summary>
        public void Click_Decision_Step()
        {
            #region Variable Declarations
            var uIDsfDescisionActiviCustom = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.TestsTab.WorkSurfaceContext.ServiceTestView.UserControl_1Custom.ScrollViewerPane.ActivityBuilderCustom.WorkflowItemPresenteCustom.FlowchartCustom.DsfDecisioActiviCustom.DisplayNameEdit;
            #endregion

            // Click 'DsfMultiAssignActivity' custom control
            uIDsfDescisionActiviCustom.DrawHighlight();
            Mouse.Click(uIDsfDescisionActiviCustom, new Point(77, 8));
        }

        [Then(@"I Enter Text Into Database Server Tab")]
        [Given(@"I Enter Text Into Database Server Tab")]
        [Then(@"I Enter Text Into Database Server Tab")]
        public void Enter_Text_Into_DatabaseServer_Tab()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.ServerComboBox.Textbox.Text = "RSAKLFSVRGENDEV";
        }

        [When(@"I Enter RunAsUser(Root) Username And Password on Database source")]
        [Given(@"I Enter RunAsUser(Root) Username And Password on Database source")]
        [Then(@"I Enter RunAsUser(Root) Username And Password on Database source")]
        public void IEnterRunAsUserRootOnDatabaseSource()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.UserNameTextBox.Text = "root";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.PasswordTextBox.Text = "admin";
        }

        [When(@"I Enter RunAsUser(PostGres) Username And Password on Database source")]
        [Given(@"I Enter RunAsUser(PostGres) Username And Password on Database source")]
        [Then(@"I Enter RunAsUser(PostGres) Username And Password on Database source")]
        public void IEnterRunAsUserPostGresOnDatabaseSource()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.UserNameTextBox.Text = "postgres";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.PasswordTextBox.Text = "test123";
        }

        [When(@"I Select mysql From DB Source Wizard Database Combobox")]
        [Given(@"I Select mysql From DB Source Wizard Database Combobox")]
        [Then(@"I Select mysql From DB Source Wizard Database Combobox")]
        public void Select_mysql_From_DB_Source_Wizard_Database_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.ToggleButton);
            Mouse.Click(MainStudioWindow.ComboboxListItemAsmysqlDB);
            Assert.AreEqual("mysql", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.UIMysqlText.DisplayText);
        }

        [When(@"I Select postgres From DB Source Wizard Database Combobox")]
        [Given(@"I Select postgres From DB Source Wizard Database Combobox")]
        [Then(@"I Select postgres From DB Source Wizard Database Combobox")]
        public void Select_postgres_From_DB_Source_Wizard_Database_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.ToggleButton);
            Mouse.Click(MainStudioWindow.ComboboxListItemAspostgresDB);
            Assert.AreEqual("postgres", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.UIPostgresText.DisplayText);
        }

        [When(@"I Select HR From DB Source Wizard Database Combobox")]
        [Given(@"I Select HR From DB Source Wizard Database Combobox")]
        [Then(@"I Select HR From DB Source Wizard Database Combobox")]
        public void Select_HR_From_DB_Source_Wizard_Database_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.ToggleButton);
            Mouse.Click(MainStudioWindow.ComboboxListItemAsHRDB);
            Assert.AreEqual("HR", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.UIHRText.DisplayText);
        }

        [When(@"I Select ExcelFiles From DB Source Wizard Database Combobox")]
        [Given(@"I Select ExcelFiles From DB Source Wizard Database Combobox")]
        [Then(@"I Select ExcelFiles From DB Source Wizard Database Combobox")]
        public void Select_ExcelFiles_From_DB_Source_Wizard_Database_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.ToggleButton);
            Mouse.Click(MainStudioWindow.ComboboxListItemAsExcelFilesDB);
            Assert.AreEqual("Excel Files", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.UIExcelFilesText.DisplayText);
        }

        [When(@"I Select MSAccess From DB Source Wizard Database Combobox")]
        [Given(@"I Select MSAccess From DB Source Wizard Database Combobox")]
        [Then(@"I Select MSAccess From DB Source Wizard Database Combobox")]
        public void Select_MSAccess_From_DB_Source_Wizard_Database_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.ToggleButton);
            Mouse.Click(MainStudioWindow.ComboboxListItemAsMSAccessDB);
            Assert.AreEqual("MS Access Database", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.MSAccessDatabaseText.DisplayText);
        }

        [When(@"I Select TestDB From DB Source Wizard Database Combobox")]
        [Given(@"I Select TestDB From DB Source Wizard Database Combobox")]
        [Then(@"I Select TestDB From DB Source Wizard Database Combobox")]
        public void Select_TestDB_From_DB_Source_Wizard_Database_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.ToggleButton);
            Mouse.Click(MainStudioWindow.ComboboxListItemAsTestDB);
            Assert.AreEqual("TestDB", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.TestDBText.DisplayText);
        }

        [When(@"I Select test From DB Source Wizard Database Combobox")]
        [Given(@"I Select test From DB Source Wizard Database Combobox")]
        [Then(@"I Select test From DB Source Wizard Database Combobox")]
        public void Select_test_From_DB_Source_Wizard_Database_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.ToggleButton);
            Mouse.Click(MainStudioWindow.ComboboxListItemAstest);
            Assert.AreEqual("test", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.testText.DisplayText);
        }

        [When(@"I Select master From DB Source Wizard Database Combobox")]
        [Given(@"I Select master From DB Source Wizard Database Combobox")]
        [Then(@"I Select master From DB Source Wizard Database Combobox")]
        public void Select_master_From_DB_Source_Wizard_Database_Combobox()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.ToggleButton);
            Mouse.Click(MainStudioWindow.ComboboxListItemAsmaster);
            Assert.AreEqual("master", MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DBSourceTab.WorkSurfaceContext.ManageDatabaseSourceControl.DatabaseComboxBox.masterText.DisplayText);
        }

        public void Click_AssemblyDirectoryButton_On_DotnetPluginSourceTab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DotNetPluginSourceTab.WorkSurfaceContext.AssemblyDirectoryButton);
        }

        public void Select_DLLAssemblyFile_From_ChooseDLLWindow(string fileName)
        {
            ChooseDLLWindow.FilterTextBox.Text = fileName.Replace(@"C:\", "");
            Mouse.Click(ChooseDLLWindow.DLLDataTree.CDrive, new Point(11, 14));
            Mouse.Click(ChooseDLLWindow.DLLDataTree.CDrive.FirstItem, new Point(69, 34));
            Assert.AreEqual(fileName, ChooseDLLWindow.FilesTextBox.Text);
            Mouse.Click(ChooseDLLWindow.SelectButton);
        }

        public void Click_ConfigFileDirectoryButton_On_DotnetPluginSourceTab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DotNetPluginSourceTab.WorkSurfaceContext.ConfigFileDirectoryButton);
            Assert.IsTrue(SelectFilesWindow.Exists, "Select Files Window did not open after clicking Assembly Directory Button");
        }

        public void Enter_ConfigFile_In_SelectFilesWindow()
        {
            Mouse.Click(SelectFilesWindow.DrivesDataTree.CTreeItem.swapfile);
            Mouse.Click(SelectFilesWindow.SelectButton);
        }

        public void Select_GACAssemblyFile_From_ChooseDLLWindow(string filter)
        {
            ChooseDLLWindow.FilterTextBox.Text = filter;
            ChooseDLLWindow.DLLDataTree.GAC.DataTreeItem.DrawHighlight();
            Mouse.Click(ChooseDLLWindow.DLLDataTree.GAC.DataTreeItem, new Point(122, 6));
            Assert.IsFalse(string.IsNullOrEmpty(ChooseDLLWindow.FilesTextBox.Text), "Files Textbox is empty.");
            ChooseDLLWindow.SelectButton.DrawHighlight();
            Mouse.Click(ChooseDLLWindow.SelectButton);
        }

        public void Select_AssemblyFile_From_COMPluginDataTree(string filter)
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.COMPlugInSourceTab.WorkSurfaceContext.SearchTextBox.Text = filter;
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.COMPlugInSourceTab.WorkSurfaceContext.DataTree.Nodes[1]);
        }

        public void Click_COMDLLPluginTool_LargeView_NewSourceButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ComDll.LargeView.NewSourceButton);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.COMPlugInSourceTab.Exists, "New COM Plugin source wizard tab does not exist after clicking new source from COM DLL Tool");
        }

        public void Click_DotNetPluginTool_LargeView_NewSourceButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.NewSourceButton);
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DotNetPluginSourceTab.Exists, "New DotNet Plugin source wizard tab does not exist after clicking new source from DotNet DLL Tool");
        }

        public void Select_Attachments_From_SelectFilesWindow()
        {
            Mouse.DoubleClick(SelectFilesWindow.DrivesDataTree.CTreeItem.AttachmentsForEmailFolder);
            SelectFilesWindow.DrivesDataTree.CTreeItem.AttachmentsForEmailFolder.attachment1.CheckBox.Checked = true;
            SelectFilesWindow.DrivesDataTree.CTreeItem.AttachmentsForEmailFolder.attachment2.CheckBox.Checked = true;
            Assert.IsNotNull(SelectFilesWindow.FileNameTextBox.Text, "Files Name is empty even after selecting Files..");
            Mouse.Click(SelectFilesWindow.SelectButton);
        }

        public void Click_SelectFilesButton_On_SMTPEmailTool_LargeView()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SMTPEmail.LargeViewContent.SelectFilesButton);
            Assert.IsTrue(SelectFilesWindow.Exists);
        }

        [When(@"I Run All Hello World Tests")]
        public void WhenIRunAllHelloWorldTests()
        {
            Filter_Explorer("Hello World");
            Click_RunAllTests_On_FirstLocalhostItem_From_ExplorerContextMenu();
        }

        [When(@"I Drag Start Node")]
        public void Drag_Start_Node()
        {
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.StartNode, new Point(186, 30));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.StartNode, 1400, 968);
        }

        [Given(@"First remote Item should be ""(.*)""")]
        [When(@"First remote Item should be ""(.*)""")]
        [Then(@"First remote Item should be ""(.*)""")]
        public void FirstRemoteItemShouldBe(string resource)
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.FirstRemoteServer.FirstItem.ItemEdit.Text == resource);
        }
        [Given(@"I change Server Authentication type")]
        [When(@"I change Server Authentication type")]
        [Then(@"I change Server Authentication type")]
        public void ChangeServerAuthenticationType()
        {
            var publicRadioButton = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.PublicRadioButton;
            var windowsRadioButton = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.WindowsRadioButton;
            if (publicRadioButton.Selected)
            {
                windowsRadioButton.Selected = true;
                Click_Server_Source_Wizard_Test_Connection_Button();
                Click_Save_Ribbon_Button_With_No_Save_Dialog();
                Playback.Wait(1000);
                Click_Close_Server_Source_Wizard_Tab_Button();
                Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerConectControl.Combobox.ToggleButton);
                Mouse.Click(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegration);                
                Click_Deploy_Tab_Source_Server_Edit_Button();
                Assert.IsTrue(windowsRadioButton.Selected);
            }
            else
            {
                publicRadioButton.Selected = true;
                Click_Server_Source_Wizard_Test_Connection_Button();
                Click_Save_Ribbon_Button_With_No_Save_Dialog();
                Playback.Wait(1000);
                Click_Close_Server_Source_Wizard_Tab_Button();
                Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerConectControl.Combobox.ToggleButton);
                Mouse.Click(MainStudioWindow.ComboboxListItemAsRemoteConnectionIntegration);
                Click_Deploy_Tab_Source_Server_Edit_Button();
                Assert.IsTrue(publicRadioButton.Selected);
            }
        }

        [Given(@"I change Server Authentication From Deploy And Validate Changes From Explorer")]
        [When(@"I change Server Authentication From Deploy And Validate Changes From Explorer")]
        [Then(@"I change Server Authentication From Deploy And Validate Changes From Explorer")]
        public void ChangeServerAuthenticationFromDeployAndValidateChangesFromExplorer()
        {
            var windowsRadioButton = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.WindowsRadioButton;
            var publicRadioButton = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.PublicRadioButton;

            if (publicRadioButton.Selected)
            {
                windowsRadioButton.Selected = true;
                Click_Server_Source_Wizard_Test_Connection_Button();
                Click_Save_Ribbon_Button_With_No_Save_Dialog();
                Playback.Wait(1000);
                Click_Close_Server_Source_Wizard_Tab_Button();
                Select_RemoteConnectionIntegration_From_Explorer();
                Click_Explorer_RemoteServer_Edit_Button();
                Playback.Wait(1000);
                Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.WindowsRadioButton.Selected, "Windows Radio Button not selected.");
                Click_Deploy_Ribbon_Button();
                Select_RemoteConnectionIntegration_From_Deploy_Tab_Source_Server_Combobox();
                Click_Deploy_Tab_Source_Server_Edit_Button();
                Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.WindowsRadioButton.Selected, "Windows Radio Button not selected.");
            }
            else
            {
                publicRadioButton.Selected = true;
                Click_Server_Source_Wizard_Test_Connection_Button();
                Click_Save_Ribbon_Button_With_No_Save_Dialog();
                Playback.Wait(1000);
                Click_Close_Server_Source_Wizard_Tab_Button();
                Select_RemoteConnectionIntegration_From_Explorer();
                Click_Explorer_RemoteServer_Edit_Button();
                Playback.Wait(1000);
                Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.PublicRadioButton.Selected, "Public Radio Button not selected.");
                Click_Deploy_Ribbon_Button();
                Select_RemoteConnectionIntegration_From_Deploy_Tab_Source_Server_Combobox();
                Click_Deploy_Tab_Source_Server_Edit_Button();
                Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.PublicRadioButton.Selected, "Public Radio Button not selected.");
            }
        }

        [Given(@"I set AuthenticationType to Public")]
        [When(@"I set AuthenticationType to Public")]
        [Then(@"I set AuthenticationType to Public")]
        public void ChangeServerAuthenticationTypeToPublic()
        {
            Click_Explorer_RemoteServer_Edit_Button();
            var publicRadioButton = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.PublicRadioButton;
            if (!publicRadioButton.Selected)
            {
                publicRadioButton.Selected = true;
                Click_Server_Source_Wizard_Test_Connection_Button();
                Click_Save_Ribbon_Button_With_No_Save_Dialog();
                Click_Close_Server_Source_Wizard_Tab_Button();
            }
            else
            {
                Click_Close_Server_Source_Wizard_Tab_Button();
            }
        }

        [Given(@"I validate the Resource tree is loaded")]
        [When(@"I validate the Resource tree is loaded")]
        [Then(@"I validate the Resource tree is loaded")]
        public void WhenIValidateTheResourceTreeIsLoaded()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DeployTab.WorkSurfaceContext.DockManager.DeployView.SourceServerExplorer.Exists);
        }

        [When(@"I Drag POSTWebTool Onto DesignSurface")]
        public void Drag_POSTWebTool_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "POST";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 128));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.HTTPWebMethods.POST, new Point(20, 35));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 128));
            Click_Clear_Toolbox_Filter_Clear_Button();
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPost.LargeView.Exists, "Web Post Request large view does not exist on the design surface.");
        }

        [Given(@"I Open Switch Tool Large View")]
        [When(@"I Open Switch Tool Large View")]
        [Then(@"I Open Switch Tool Large View")]
        public void Open_Switch_Tool_Large_View()
        {
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.Switch, new Point(39, 35));
            Assert.IsTrue(DecisionOrSwitchDialog.Enabled, "Switch dialog does not exist after opening switch large view");
        }
        [When(@"I Drag PUTWebTool Onto DesignSurface")]
        public void Drag_PUTWebTool_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "PUT";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 126));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.HTTPWebMethods.PUT, new Point(16, 25));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 126));
            Click_Clear_Toolbox_Filter_Clear_Button();
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPut.LargeView.Exists, "Put Web connector large view does not exist on the design surface after drag and drop from toolbox.");
        }

        [When(@"I Drag DELETEWebTool Onto DesignSurface")]
        public void Drag_DELETEWebTool_Onto_DesignSurface()
        {
            MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.SearchTextBox.Text = "DELETE";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.EnsureClickable(new Point(306, 126));
            Mouse.StartDragging(MainStudioWindow.DockManager.SplitPaneLeft.ToolBox.ToolListBox.HTTPWebMethods.DELETE, new Point(16, 25));
            Mouse.StopDragging(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart, new Point(306, 126));
            Click_Clear_Toolbox_Filter_Clear_Button();
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.Exists, "Delete Web Tool large view does not exist on the design surface after drag and drop from toolbox.");
        }

        [When(@"I Collapse GetWebTool Large View to Small View With Double Click")]
        public void Collapse_GETWebTool_LargeView_To_SmallView()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebGet.LargeView.Exists, "Cannot collapse Web GET tool to small view, large view does not exist.");
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebGet, new Point(201, 9));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebGet.SmallView.Exists, "Web GET small view does not exist after collapsing the large view with a double click.");
        }

        [When(@"I Collapse PostWebTool Large View to Small View With Double Click")]
        public void Collapse_POSTWebTool_LargeView_To_SmallView()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPost.LargeView.Exists, "Cannot collapse Web POST tool to small view, large view does not exist.");
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPost, new Point(201, 9));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPost.SmallView.Exists, "Web POST small view does not exist after collapsing the large view with a double click.");
        }

        [When(@"I Collapse PutWebTool Large View to Small View With Double Click")]
        public void Collapse_PUTWebTool_LargeView_To_SmallView()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPut.LargeView.Exists, "Cannot collapse Web PUT tool to small view, large view does not exist.");
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPut, new Point(201, 9));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebPut.SmallView.Exists, "Web PUT small view does not exist after collapsing the large view with a double click.");
        }

        [When(@"I Collapse DeleteWebTool Large View to Small View With Double Click")]
        public void Collapse_DELETEWebTool_LargeView_To_SmallView()
        {
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.Exists, "Cannot collapse Web DELETE tool to small view, large view does not exist.");
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete, new Point(201, 9));
            Assert.IsTrue(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.SmallView.Exists, "Web DELETE small view does not exist after collapsing the large view with a double click.");
        }
        
        public void Click_UserButton_On_ServerSourceTab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.UserRadioButton);
        }

        public void Enter_TextIntoAddress_On_ServerSourceTab()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.NewServerSource.AddressComboBox.AddressEditBox.Text = "RSAKLFSVRGENDEV";
        }

        public void Enter_RunAsUser_On_ServerSourceTab()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.UsernameTextBox.Text = "IntegrationTester";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.ServerSourceTab.WorkSurfaceContext.PasswordTextBox.Text = "I73573r0";
        }

        public void Click_UserButton_On_WebServiceSourceTab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.WorkSurfaceContext.UserRadioButton);
        }

        public void Click_AnonymousButton_On_WebServiceSourceTab()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.WorkSurfaceContext.AnonymousRadioButton);
        }

        public void Enter_TextIntoAddress_On_WebServiceSourceTab()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.WorkSurfaceContext.AddressTextbox.Text = "http://RSAKLFSVRTFSBLD:9810";
        }

        public void Enter_RunAsUser_On_WebServiceSourceTab()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.WorkSurfaceContext.UserNameTextBox.Text = "IntegrationTester";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.WorkSurfaceContext.PasswordTextBox.Text = "I73573r0";
        }

        public void Enter_DefaultQuery_On_WebServiceSourceTab()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WebSourceTab.WorkSurfaceContext.DefaultQueryTextBox.Text = "";
        }

        public void Enter_Text_On_RabbitMQSourceTab()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.RabbitMqSourceTab.RabbitMQSourceCustom.HostTextBoxEdit.Text = "rsaklfsvrgendev";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.RabbitMqSourceTab.RabbitMQSourceCustom.PortTextBoxEdit.Text = "5672";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.RabbitMqSourceTab.RabbitMQSourceCustom.UserNameTextBoxEdit.Text = "test";
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.RabbitMqSourceTab.RabbitMQSourceCustom.PasswordTextBoxEdit.Text = "test";
        }

        public void Click_RabbitMQSource_TestConnectionButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.RabbitMqSourceTab.RabbitMQSourceCustom.TestConnectionButton);
        }

        public void Enter_TextIntoAddress_In_SharepointServiceSourceTab()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SharepointServerSourceTab.SharepointServerSourceView.SharepointView.ServerNameEdit.Text = "http://rsaklfsvrsharep";
        }

        public void Enter_TextIntoAddress_On_WCFServiceTab()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WCFServiceSourceTab.WorkSurfaceContext.WCFEndpointURLEdit.Text = "test";
        }

        public void Click_WCFServiceSource_TestConnectionButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WCFServiceSourceTab.WorkSurfaceContext.TestConnectionButton);
        }

        public void Enter_TextIntoOAuthKey_On_OAuthSourceTab()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.OAuthSourceWizardTab.WorkSurfaceContext.OAuthKeyTextBox.Text = "test";
        }

        public void Click_OAuthSource_AuthoriseButton()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.OAuthSourceWizardTab.WorkSurfaceContext.AuthoriseButton);
        }

        public void Click_SelectFilesButton_On_ExchangeEmailTool_LargeView()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ExchangeEmail.LargeViewContent.SelectFilesButton);
            Assert.IsTrue(SelectFilesWindow.Exists);
        }

        public void CreateAttachmentsForTest(string filepath)
        {
            var fileStream = File.Create(filepath);
            fileStream.Close();
        }

        public void CreateFolderForAttachments(string folderName)
        {
            Directory.CreateDirectory(folderName);
        }

        public void RemoveTestFiles(string filePath1, string filePath2, string folderName)
        {
            if (File.Exists(filePath1))
            {
                File.Delete(filePath1);
                File.Delete(filePath2);
                Directory.Delete(folderName);
                Assert.IsFalse(Directory.Exists(folderName));
            }
        }

        public void I_Expand_First_Action_Tree()
        {
            MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.ActionMethodListBoxList.ActivitiesDesignListItem.SetActivitiesExpander.Expanded = true;
        }

        public void Create_Scheduler_Using_Shortcut()
        {
            var scheduleNewTaskListItem = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.SchedulerTab.WorkSurfaceContext.SchedulerView.SchedulesList;
            Mouse.Click(scheduleNewTaskListItem, new Point(151, 13));
            Keyboard.SendKeys(scheduleNewTaskListItem, "N", ModifierKeys.Control);
        }

        public void Select_Source_From_ExplorerContextMenu(String sourceName)
        {
            Filter_Explorer(sourceName);
            Mouse.DoubleClick(MainStudioWindow.DockManager.SplitPaneLeft.Explorer.ExplorerTree.localhost.FirstItem);
        }
        public void Change_Dll_And_Save(string newDll)
        {
            AssemblyComboBox assembly = MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.DotNetPluginSourceTab.WorkSurfaceContext.AssemblyComboBox;            
            assembly.TextEdit.Text = newDll;
            Keyboard.SendKeys(assembly.TextEdit, "S", (ModifierKeys.Control));
        }

        public void Select_Source_From_MySQLTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MySqlDatabase.LargeView.SourcesComboBox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MySqlDatabase.LargeView.SourcesComboBox.MySQLSourceFromToolListItem);
        }

        public void Select_Source_From_ODBCTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ODBCDatabaseActivCustom.LargeView.SourcesComboBox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ODBCDatabaseActivCustom.LargeView.SourcesComboBox.ODBCSourceFromToolListItem);
        }

        public void Select_Source_From_OracleTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.OracleDatabaseActCustom.LargeView.SourcesComboBox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.OracleDatabaseActCustom.LargeView.SourcesComboBox.OracleSourceFromToolListItem);
        }

        public void Select_Source_From_PostgreTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PostgreSqlActivitCustom.LargeView.SourcesComboBox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PostgreSqlActivitCustom.LargeView.SourcesComboBox.PostgreSQLSourceFromListItem);
        }

        public void Select_Source_From_SQLServerTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.SourcesCombobox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.SourcesCombobox.SQLServerSourceFromTListItem);
        }

        public void Select_Source_From_ComDLLTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ComDll.LargeView.SourcesCombobox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ComDll.LargeView.SourcesCombobox.COMPluginSourceToEditListItem);
        }

        public void Select_Source_From_DotNetDLLTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.SourcesComboBox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.SourcesComboBox.DotNetPluginSource);
        }

        public void Select_New_Source_From_DotNetDLLTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.SourcesComboBox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.SourcesComboBox.NewDotNetPluginSource);
        }

        public void Select_Source_From_EmailTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SMTPEmail.LargeViewContent.SourceComboBox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SMTPEmail.LargeViewContent.SourceComboBox.UIHostlocalhostUserNamListItem);
        }

        public void Select_Source_From_RabbitMQConsumeTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.RabbitMQConsume.LargeViewContentCustom.SourceComboBox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.RabbitMQConsume.LargeViewContentCustom.SourceComboBox.RabbitMQSourceFromToolListItem);
        }

        public void Select_Source_From_SharepointCopyFileTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCopyFile.LargeView.Server);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCopyFile.LargeView.Server.SharepointSourceFromToolListItem);
        }

        public void Select_Source_From_DELETEWebTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.SourcesComboBox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.SourcesComboBox.WebServiceSourceFromToolListItem);
        }

        public void Select_Source_From_ExchangeSendTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ExchangeEmail.LargeViewContent.SourcesComboBox);
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ExchangeEmail.LargeViewContent.SourcesComboBox.Dev2CommonInterfacesListItem);
        }

        public void Click_EditSourceButton_On_MySQLTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.MySqlDatabase.LargeView.EditSourceButton);
        }

        public void Click_EditSourceButton_On_ODBCTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ODBCDatabaseActivCustom.LargeView.EdistSourceButton);
        }

        public void Click_EditSourceButton_On_OracleTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.OracleDatabaseActCustom.LargeView.EditSourceButton);
        }

        public void Click_EditSourceButton_On_PostgreSQLTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.PostgreSqlActivitCustom.LargeView.EditSourceButton);
        }

        public void Click_EditSourceButton_On_SQLServerTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SqlServerDatabase.LargeView.EditSourceButton);
        }
        public void Click_EditSourceButton_On_ComDLLTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ComDll.LargeView.EditSourceButton);
        }
        public void Click_EditSourceButton_On_DotNetDLLTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.DotNetDll.LargeView.EditSourceButton);
        }

        public void Click_EditSourceButton_On_EmailTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SMTPEmail.LargeViewContent.EditSourceButton);
        }

        public void Click_EditSourceButton_On_RabbitMQConsumeTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.RabbitMQConsume.LargeViewContentCustom.EditSourceButton);
        }

        public void Click_EditSourceButton_On_RabbitMQPublishTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.RabbitMQPublish.LargeViewContentCustom.EditSourceButton);
        }

        public void Click_EditSourceButton_On_SharepointCopyFileTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.SharepointCopyFile.LargeView.EditSourceButton);
        }

        public void Click_EditSourceButton_On_DELETEWebTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.WebDelete.LargeView.EditSourceButton);
        }

        public void Click_EditSourceButton_On_ExchangeSendTool()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ExchangeEmail.LargeViewContent.ItemButton);
        }
        public void Click_EditSourceButton_On_ExchangeSendToolSmallView()
        {
            Mouse.Click(MainStudioWindow.DockManager.SplitPaneMiddle.TabManSplitPane.TabMan.WorkflowTab.WorkSurfaceContext.WorkflowDesignerView.DesignerView.ScrollViewerPane.ActivityTypeDesigner.WorkflowItemPresenter.Flowchart.ExchangeEmail.SmallViewContent.ItemButton);
        }
    }
}
