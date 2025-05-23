﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using AddonUpdater.Controls;
using System.Security.Policy;
using AddonUpdater.Models;
using Newtonsoft.Json;
using System.Timers;
using Newtonsoft.Json.Linq;
using AddonUpdater.Properties;
using AddonUpdater.Controlers;

namespace AddonUpdater
{

    public partial class FormMainMenu : Form
    {
        private Control activeform;
        private Button currentButton = new();
        public static string activity = null;

        private AddonFormControl formControl = null;

        bool openFormAddons = false;
        DownloadAddonGitHub downloadAddonGitHub = new();

        Color standardButton = Color.FromArgb(37, 35, 47);
        Color activeButton = Color.FromArgb(123, 119, 159);

        public FormMainMenu()
        {
            AddonUpdaterSettingApp.ReadAppSetting();
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.Selectable, false);
            UpdateStyles();
        }

        private async void GetOnline()
        {
            string result;
            using HttpClient httpClient = new();
            try
            {
                using Stream stream = await httpClient.GetStreamAsync("https://api.sirus.su/api/server/status");
                using StreamReader streamReader = new(stream);
                result = streamReader.ReadToEnd().Replace("\n", "").Trim();
                List<Realms> json = JsonConvert.DeserializeObject<List<Realms>>(result);

                for (int i = 0; i < json.Count; i++)
                {
                    OnlineControl onlineControl = new(json[i].IsOnline, json[i].Name, json[i].Online, this)
                    {
                        Location = new Point(i * 200, 0),
                        Name = json[i].Name,
                        Width = 200
                    };

                    titleOnlinePanel.Controls.Add(onlineControl);
                }
            }
            catch (Exception)
            {
            }
        }

        #region Click
        private void ButtonAddons_Click(object sender, EventArgs e)
        {
            ActiveControl = null;
            if (activity != null)
                MessageBox.Show($"Дождитесь окончания {activity}");
            else
            {
                OpenChildForm(new AddonFormControl(this, true), sender);
                openFormAddons = true;
            }
        }


        private void ButtonSettings_Click(object sender, EventArgs e)
        {
            ActiveControl = null;
            if (activity != null)
                MessageBox.Show($"Дождитесь окончания {activity}");
            else
            {
                if (progressBar1.Visible == true)
                {
                    progressBar1.Visible = false;
                    labelInfo.Visible = false;
                }
                OpenChildForm(new SettingsControl(this), sender);
                openFormAddons = false;

            }
        }

        private void ButtonAllAddons_Click(object sender, EventArgs e)
        {
            ActiveControl = null;
            if (activity != null)
                MessageBox.Show($"Дождитесь окончания {activity}");
            else
            {
                OpenChildForm(new AddonFormControl(this, false), sender);
                openFormAddons = true;
            }
        }

        private void ButtonAbout_Click(object sender, EventArgs e)
        {
            ActiveControl = null;
            if (activity != null)
                MessageBox.Show($"Дождитесь окончания {activity}");
            else
            {
                if (progressBar1.Visible == true)
                {
                    progressBar1.Visible = false;
                    labelInfo.Visible = false;
                }
                OpenChildForm(new AboutFormControl(), sender);
                openFormAddons = false;
            }
        }

        private void ButtonClose_Click(object sender, EventArgs e)
        {
            ActiveControl = null;
            if (activity != null)
                MessageBox.Show($"Дождитесь окончания {activity}");
            else
            {
                notifyIcon1.Visible = false;
                Application.Exit();
            }
        }

        private void ButtonModifications_Click(object sender, EventArgs e)
        {
            ActiveControl = null;
            if (activity != null)
                MessageBox.Show($"Дождитесь окончания {activity}");
            else
            {
                if (progressBar1.Visible == true)
                {
                    progressBar1.Visible = false;
                    labelInfo.Visible = false;
                }
                OpenChildForm(new ModificationsControl(this), sender);
                openFormAddons = false;
            }
        }

        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (activity != null)
                MessageBox.Show($"Дождитесь окончания {activity}");
            else
            {
                notifyIcon1.Visible = false;
                Application.Exit();
            }
        }

        private void NotifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (WindowState == FormWindowState.Normal)
                {
                    WindowState = FormWindowState.Minimized;
                    ShowInTaskbar = false;
                }
                else
                {
                    ShowInTaskbar = true;
                    WindowState = FormWindowState.Normal;
                }
            }
        }

        private void ButtonResize_Click(object sender, EventArgs e)
        {
            ActiveControl = null;
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            HideFromAltTab(this.Handle);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;

        }

        private void LabelVersion_Click(object sender, EventArgs e)
        {
            if (AddonUpdaterSetting.Setting.News != null)
            {
                Process.Start(new ProcessStartInfo(AddonUpdaterSetting.Setting.News) { UseShellExecute = true });
            }
        }

        private void ButtonClose_MouseMove(object sender, MouseEventArgs e)
        {
            buttonClose.BackColor = Color.FromArgb(191, 48, 48);
        }

        private void ButtonClose_MouseLeave(object sender, EventArgs e)
        {
            buttonClose.BackColor = Color.FromArgb(44, 42, 63);

        }

        #endregion

        public async Task DownloadAddonAuto()
        {
            ButtonOff();
            try
            {
                if (progressBar1.Visible == false)
                {
                    progressBar1.Visible = true;
                    labelInfo.Visible = true;
                }
                activity = "обновления";
                //  DownloadAddonGitHub.NeedUpdate.Clear();
                List<GitHub> NeedUpdate = DownloadAddonGitHub.GitHubs.FindAll(find => find.NeedUpdate == true);
                if (NeedUpdate.Count > 0)
                {
                    progressBar1.Value = 0;
                    progressBar1.Maximum = NeedUpdate.Count;
                    for (int i = 0; i < NeedUpdate.Count; i++)
                    {
                        labelInfo.Text = NeedUpdate[i].Name;
                        await downloadAddonGitHub.DownloadAddonGitHubTask(NeedUpdate[i].Name, NeedUpdate[i].GithubLink, NeedUpdate[i].Branches);
                        progressBar1.Value++;
                    }
                    labelInfo.Text = "Распаковка Аддонов";
                    await downloadAddonGitHub.GetAddonAsync(NeedUpdate);
                    labelInfo.Text = "Обновление";
                    progressBar1.Value = 0;
                    progressBar1.Maximum = 2;
                    progressBar1.Value++;
                    await downloadAddonGitHub.AupdatecheckAsync();
                    SetNotificationsAddons();
                    progressBar1.Value++;
                    progressBar1.Value = 0;
                    labelInfo.Text = "";
                    activity = null;
                    if (openFormAddons == false)
                    {
                        progressBar1.Visible = false;
                        labelInfo.Visible = false;
                    }
                }
            }
            catch
            {
                progressBar1.Value = 0;
                labelInfo.Text = "Ошибка подключения";
            }
            ButtonOn();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await AddonUpdaterSetting.GetSettingsTask();
            LabelVersion.Text = "v." + Application.ProductVersion;
            
            PathCheck();
            GetOnline();
            await downloadAddonGitHub.AupdatecheckAsync();
            SetNotificationsAddons();
            timerGithub.Start();
            timerLocal.Start();
            timerUpdate.Start();
            timerKill.Start();
            timerSirus.Start();

            VisibleOn();
            WTF();
            OpenChildForm(new AddonFormControl(this, true), buttonAddons);
            AutoUpdate();
            openFormAddons = true;
            labelNeedUpdateMyAddon.Parent = buttonAddons;
            labelNeedUpdateMyAddon.Location = new Point(0, 12);
        }

        #region MoveForm
        private void MoveForm(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                _ = SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void PanelTitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            MoveForm(e);
        }

        private void LabelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            MoveForm(e);
        }

        private void LabelTitleName_MouseDown(object sender, MouseEventArgs e)
        {
            MoveForm(e);
        }

        public void PanelTitleMove_MouseDown(object sender, MouseEventArgs e)
        {
            MoveForm(e);
        }

        private void TitleOnlinePanel_MouseDown(object sender, MouseEventArgs e)
        {
            MoveForm(e);
        }

        private void PanelTitleRight_MouseDown(object sender, MouseEventArgs e)
        {
            MoveForm(e);
        }

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]

        private static extern bool ReleaseCapture();
        #endregion

        #region OpenChildForm

        private void OpenChildForm(Control childForm, object btnSender)
        {
            activeform?.Dispose();
            panelDesktopPane.Controls.Clear();
            ActivateButton(btnSender);
            activeform = childForm;
            OffPanel();
            childForm.Dock = DockStyle.Fill;
            panelDesktopPane.Controls.Add(childForm);
            panelDesktopPane.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            OnPanel();
            if (childForm.GetType().ToString() == "AddonUpdater.Controls.AddonFormControl")
            {
                formControl = new AddonFormControl();
                formControl = (AddonFormControl)childForm;
            }
            else
            {
                formControl = null;
            }
        }

        private void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {
                if (currentButton != (Button)btnSender)
                {
                    DisableButton();
                    currentButton = (Button)btnSender;
                    currentButton.BackColor = activeButton;
                    currentButton.ForeColor = Color.White;
                    if (currentButton.Name == "buttonAddons")
                        labelNeedUpdateMyAddon.BackColor = activeButton;
                }
            }
        }

        private void DisableButton()
        {
            foreach (Control previousBtn in panelMenu.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = standardButton;
                    previousBtn.ForeColor = Color.Gainsboro;
                    if (currentButton.Name == "buttonAddons")
                        labelNeedUpdateMyAddon.BackColor = standardButton;
                }
            }
        }

        #endregion 

        private void OpenForm()
        {
            if (DownloadAddonGitHub.UpdateInfo)
            {
                formControl?.UpdatePanelAddonsView();

            }
        }

        private void LabelVersion_MouseHover(object sender, EventArgs e)
        {
            ToolTip.Show("Нажмите для просмотра списка нововведения", LabelVersion);
        }

        #region  Timer
        private void TimerKill_Tick(object sender, EventArgs e)
        {
            string proc = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(proc);
            if (processes.Length > 1)
            {
                foreach (Process process in processes)
                {
                    if (process.Id != Environment.ProcessId)
                        process.Kill();
                }
                ShowInTaskbar = true;
                WindowState = FormWindowState.Normal;

            }
        }
        bool timer = false;
        private async void TimerGithub_Tick(object sender, EventArgs e)
        {
            if (activity == null)
            {
                timer = true;
                await downloadAddonGitHub.AupdatecheckAsync();
                timer = false;
            }
        }

        private async void TimerLocal_Tick(object sender, EventArgs e)
        {
            if (activity == null && timer == false)
            {
                await downloadAddonGitHub.AupdatecheckLocalAsync();
            }
        }

        private void TimerSirus_Tick(object sender, EventArgs e)
        {
            titleOnlinePanel.Controls.Clear();
            GetOnline();
        }


        private void TimerUpdate_Tick(object sender, EventArgs e)
        {
            if (activity == null)
            {
                SetNotificationsAddons();
                if (DownloadAddonGitHub.GitHubs.Count > 0)
                {
                    if (DownloadAddonGitHub.UpdateInfo)
                    {
                        OpenForm();

                    }
                    AutoUpdate();

                }
            }
        }

        private async void AutoUpdate()
        {
            if (AddonUpdaterSettingApp.SettingsApp.AutoUpdateBool == true && DownloadAddonGitHub.GitHubs.FindAll(addon => addon.NeedUpdate == true).Count > 0)
            {
                if (Directory.Exists(AddonUpdaterSettingApp.SettingsApp.PathWow))
                {
                    activity = "Cкачивания";
                    await DownloadAddonAuto();
                    activity = null;
                    OpenForm();
                }
                else
                {
                    labelNeedUpdateMyAddon.Visible = false;
                }
            }
        }

        #endregion

        #region Set/Off/on
        public void SetNotificationsAddons()
        {

            int count = DownloadAddonGitHub.GitHubs.FindAll(addon => addon.NeedUpdate == true).Count;
            if (count > 0)
            {
                labelNeedUpdateMyAddon.Visible = true;
                labelNeedUpdateMyAddon.Text = count.ToString();
            }
            else
            {
                labelNeedUpdateMyAddon.Visible = false;
            }
        }

        public static void PathCheck()
        {

            System.Collections.IList list = AddonUpdaterSettingApp.SettingsApp.PathsWow;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null)
                {
                    string path = list[i].ToString();
                    if (!Directory.Exists(path))
                    {
                        AddonUpdaterSettingApp.SettingsApp.PathsWow.Remove(path);
                        i--;
                    }
                }
                else
                {
                    AddonUpdaterSettingApp.SettingsApp.PathsWow.Remove(null);
                    i--;
                }
            }

            if (!AddonUpdaterSettingApp.SettingsApp.PathsWow.Contains(AddonUpdaterSettingApp.SettingsApp.PathWow))
            {
                if (AddonUpdaterSettingApp.SettingsApp.PathsWow.Count >= 1)
                {
                    AddonUpdaterSettingApp.SettingsApp.PathWow = AddonUpdaterSettingApp.SettingsApp.PathsWow[0];
                }
                else
                {
                    AddonUpdaterSettingApp.SettingsApp.PathWow = null;
                }
            }
            AddonUpdaterSettingApp.Save();

        }

        private void OffPanel()
        {
            activeform.Visible = false;
            panelDesktopPane.Visible = false;
        }

        private void OnPanel()
        {
            activeform.Visible = true;
            panelDesktopPane.Visible = true;
        }

        public void ButtonOff()
        {
            buttonAbout.Enabled = false;
            buttonAddons.Enabled = false;
            buttonAllAddons.Enabled = false;
            buttonSettings.Enabled = false;
            buttonClose.Enabled = false;
            buttonModifications.Enabled = false;
        }

        public void ButtonOn()
        {
            buttonAbout.Enabled = true;
            buttonAddons.Enabled = true;
            buttonAllAddons.Enabled = true;
            buttonSettings.Enabled = true;
            buttonClose.Enabled = true;
            buttonModifications.Enabled = true;
        }

        private void VisibleOn()
        {
            buttonAbout.Visible = true;
            buttonAddons.Visible = true;
            buttonSettings.Visible = true;
            buttonModifications.Visible = true;
            buttonAllAddons.Visible = true;
            labelTitleName.Visible = true;
            LabelVersion.Visible = true;
            buttonClose.Visible = true;
            buttonResize.Visible = true;
            progressBar1.Visible = true;
            labelInfo.Visible = true;
            titleOnlinePanel.Visible = true;

        }

        #endregion

        #region Убрать из atl+tab
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr window, int index, int value);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr window, int index);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;

        public static void HideFromAltTab(IntPtr Handle)
        {
            _ = SetWindowLong(Handle, GWL_EXSTYLE, GetWindowLong(Handle,
                GWL_EXSTYLE) | WS_EX_TOOLWINDOW);
        }
        #endregion

        private static async void WTF()
        {

            if (Directory.Exists("BackupWTF"))
            {
                if (Directory.GetFiles("BackupWTF").Length > 15)
                {
                    await BackupWTF.DeleteFileBackupTask(15);
                }
            }

            string settings = AddonUpdaterSettingApp.SettingsApp.BackupWTF;

            if (settings == "")
            {
                AddonUpdaterSettingApp.SettingsApp.BackupWTF = "Никогда";
                AddonUpdaterSettingApp.Save();
            }
            else if (settings == "При запуске программы")
            {
                await BackupWTF.CreateFileBackupTask();
            }
            else if (settings == "Раз в день")
            {
                DateTime last = BackupWTF.GetLastBackupWtf();

                if (last != new DateTime())
                {
                    if (last.Day - DateTime.Now.Day == 1)
                    {
                        await BackupWTF.CreateFileBackupTask();
                    }
                }
            }
            else if (settings == "Раз в два дня")
            {
                DateTime last = BackupWTF.GetLastBackupWtf();

                if (last != new DateTime())
                {
                    if (last.Day - DateTime.Now.Day == 2)
                    {
                        await BackupWTF.CreateFileBackupTask();
                    }
                }
            }
            else if (settings == "Раз в три дня")
            {
                DateTime last = BackupWTF.GetLastBackupWtf();

                if (last != new DateTime())
                {
                    if (last.Day - DateTime.Now.Day == 3)
                    {
                        await BackupWTF.CreateFileBackupTask();
                    }
                }
            }
            else if (settings == "Раз в неделю")
            {
                DateTime last = BackupWTF.GetLastBackupWtf();

                if (last != new DateTime())
                {
                    if (last.Day - DateTime.Now.Day == 7)
                    {
                        await BackupWTF.CreateFileBackupTask();
                    }
                }
            }
            else if (settings == "Раз в месяц")
            {
                DateTime last = BackupWTF.GetLastBackupWtf();

                if (last != new DateTime())
                {
                    if (last.Month - DateTime.Now.Month == 1)
                    {
                        await BackupWTF.CreateFileBackupTask();
                    }
                }
            }

        }
    }

}
