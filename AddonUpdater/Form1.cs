using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AddonUpdater.Controls;
using AddonUpdater.Models;
using Newtonsoft.Json;
using AddonUpdater.Controlers;
using AddonUpdater.Utils;

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

        private async Task GetOnlineAsync()
        {
            try
            {
                string result = await HttpClientProvider.Instance.GetStringAsync("https://api.sirus.su/api/server/status");
                result = result.Replace("\n", "").Trim();
                var json = JsonConvert.DeserializeObject<List<Realms>>(result);
                if (json == null) return;

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
            catch (Exception ex)
            {
                Logger.LogWarning("GetOnline failed", ex);
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
                if (!progressBar1.Visible)
                {
                    progressBar1.Visible = true;
                    labelInfo.Visible = true;
                }
                activity = "обновления";
                List<GitHub> NeedUpdate = DownloadAddonGitHub.GitHubs.FindAll(find => find.NeedUpdate);
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
                    if (!openFormAddons)
                    {
                        progressBar1.Visible = false;
                        labelInfo.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("DownloadAddonAuto failed", ex);
                progressBar1.Value = 0;
                labelInfo.Text = "Ошибка подключения";
            }
            ButtonOn();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                await Form1_LoadAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError("Form1_Load failed", ex);
            }
        }

        private async Task Form1_LoadAsync()
        {
            await AddonUpdaterSetting.GetSettingsTask();
            LabelVersion.Text = "v." + Application.ProductVersion;

            PathCheck();
            _ = GetOnlineAsync();
            await downloadAddonGitHub.AupdatecheckAsync();
            SetNotificationsAddons();
            timerGithub.Start();
            timerLocal.Start();
            timerUpdate.Start();
            timerKill.Start();
            timerSirus.Start();

            VisibleOn();
            _ = WTFAsync();
            OpenChildForm(new AddonFormControl(this, true), buttonAddons);
            await AutoUpdateAsync();
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
        private readonly SemaphoreSlim _checkSemaphore = new(1, 1);

        private async void TimerGithub_Tick(object sender, EventArgs e)
        {
            if (activity != null) return;
            if (!await _checkSemaphore.WaitAsync(0)) return;
            try
            {
                await downloadAddonGitHub.AupdatecheckAsync();
            }
            catch (Exception ex) { Logger.LogError("TimerGithub_Tick failed", ex); }
            finally { _checkSemaphore.Release(); }
        }

        private async void TimerLocal_Tick(object sender, EventArgs e)
        {
            if (activity != null) return;
            if (!await _checkSemaphore.WaitAsync(0)) return;
            try
            {
                await downloadAddonGitHub.AupdatecheckLocalAsync();
            }
            catch (Exception ex) { Logger.LogError("TimerLocal_Tick failed", ex); }
            finally { _checkSemaphore.Release(); }
        }

        private async void TimerSirus_Tick(object sender, EventArgs e)
        {
            try
            {
                titleOnlinePanel.Controls.Clear();
                await GetOnlineAsync();
            }
            catch (Exception ex) { Logger.LogError("TimerSirus_Tick failed", ex); }
        }


        private async void TimerUpdate_Tick(object sender, EventArgs e)
        {
            try
            {
                if (activity != null) return;

                SetNotificationsAddons();
                if (DownloadAddonGitHub.GitHubs.Count > 0)
                {
                    if (DownloadAddonGitHub.UpdateInfo)
                        OpenForm();

                    await AutoUpdateAsync();
                }
            }
            catch (Exception ex) { Logger.LogError("TimerUpdate_Tick failed", ex); }
        }

        private async Task AutoUpdateAsync()
        {
            if (AddonUpdaterSettingApp.SettingsApp.AutoUpdateBool != true) return;
            if (DownloadAddonGitHub.GitHubs.FindAll(addon => addon.NeedUpdate).Count == 0) return;

            if (Directory.Exists(AddonUpdaterSettingApp.SettingsApp.PathWow))
            {
                activity = "Cкачивания";
                try
                {
                    await DownloadAddonAuto();
                }
                finally
                {
                    activity = null;
                }
                OpenForm();
            }
            else
            {
                labelNeedUpdateMyAddon.Visible = false;
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

        private static async Task WTFAsync()
        {
            try
            {
                if (Directory.Exists("BackupWTF") && Directory.GetFiles("BackupWTF").Length > 15)
                {
                    await BackupWTF.DeleteFileBackupTask(15);
                }

                string settings = AddonUpdaterSettingApp.SettingsApp.BackupWTF;

                if (string.IsNullOrEmpty(settings))
                {
                    AddonUpdaterSettingApp.SettingsApp.BackupWTF = "Никогда";
                    AddonUpdaterSettingApp.Save();
                    return;
                }

                switch (settings)
                {
                    case "При запуске программы":
                        await BackupWTF.CreateFileBackupTask();
                        break;
                    case "Раз в день":
                        await BackupIfDueAsync(1, useMonth: false);
                        break;
                    case "Раз в два дня":
                        await BackupIfDueAsync(2, useMonth: false);
                        break;
                    case "Раз в три дня":
                        await BackupIfDueAsync(3, useMonth: false);
                        break;
                    case "Раз в неделю":
                        await BackupIfDueAsync(7, useMonth: false);
                        break;
                    case "Раз в месяц":
                        await BackupIfDueAsync(1, useMonth: true);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("WTFAsync failed", ex);
            }
        }

        private static async Task BackupIfDueAsync(int threshold, bool useMonth)
        {
            DateTime last = BackupWTF.GetLastBackupWtf();
            if (last == new DateTime()) return;

            int diff = useMonth
                ? last.Month - DateTime.Now.Month
                : last.Day - DateTime.Now.Day;

            if (diff == threshold)
                await BackupWTF.CreateFileBackupTask();
        }
    }

}
