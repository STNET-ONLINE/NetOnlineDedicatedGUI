using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Collections.Generic;

namespace NetOnlineDedicatedGUI
{
    public partial class ServerManager : Form
    {

        BindingList<ServerEntry> servers;
        Dictionary<ServerEntry, ServerRuntime> runtimes = new();

        string enginePath;
        string fsGamePath;
        bool useSandboxie = false;
        string sandboxieStartPath;
        string sandboxieBoxName;
        public ServerManager()
        {
            InitializeComponent();
            LoadConfig();
            InitServerGrid();

            var timer = new System.Windows.Forms.Timer
            {
                Interval = 10000 // 10 секунд
            };

            timer.Tick += (_, __) => TickServers();
            timer.Start();
        }

        void ServerGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (serverGrid.Columns[e.ColumnIndex].DataPropertyName != "Status")
                return;

            if (e.Value == null)
                return;

            var status = e.Value.ToString();
            var cell = serverGrid.Rows[e.RowIndex].Cells[e.ColumnIndex];

            switch (status)
            {
                case "Running":
                    cell.Style.ForeColor = Color.LimeGreen;
                    break;

                case "Stopped":
                    cell.Style.ForeColor = Color.Gray;
                    break;

                case "Crashed":
                    cell.Style.ForeColor = Color.Red;
                    break;
                case "Restart (Lifetime)":
                    cell.Style.ForeColor = Color.Orange;
                    break;
                case "Not Responding":
                    cell.Style.ForeColor = Color.DarkRed;
                    break;

                case "Restart (Hang)":
                    cell.Style.ForeColor = Color.OrangeRed;
                    break;
                case "Starting":
                    cell.Style.ForeColor = Color.DodgerBlue;
                    break;
                case "Restart (Crash)":
                    cell.Style.ForeColor = Color.Orange;
                    break;



                default:
                    cell.Style.ForeColor = Color.Black;
                    break;
            }
        }


        // ===== ЗАГРУЗКА КОНФИГА =====
        void LoadConfig()
        {
            if (!File.Exists("config.json"))
            {
                MessageBox.Show("Конфигурация не найдена.", "Ошибка");
                Close();
                return;
            }

            var json = File.ReadAllText("config.json");
            var config = JsonSerializer.Deserialize<AppConfig>(json);

            enginePath = config.EnginePath;
            fsGamePath = config.FsGamePath;
            useSandboxie = config.UseSandboxie;

            sandboxieStartPath = string.IsNullOrWhiteSpace(config.SandboxieStartPath)
                ? @"C:\Program Files\Sandboxie-Plus\Start.exe"
                : config.SandboxieStartPath;

            sandboxieBoxName = string.IsNullOrWhiteSpace(config.SandboxieBoxName)
                ? "DefaultBox"
                : config.SandboxieBoxName;

            servers = new BindingList<ServerEntry>(config.Servers.ToList());
        }


        // ===== ИНИЦИАЛИЗАЦИЯ ТАБЛИЦЫ =====
        void SaveConfig()
        {
            var config = new AppConfig
            {
                EnginePath = enginePath,
                FsGamePath = fsGamePath,
                Servers = servers
            };

            var json = JsonSerializer.Serialize(
                config,
                new JsonSerializerOptions { WriteIndented = true }
            );

            File.WriteAllText("config.json", json);


        }

        void ServerGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var column = serverGrid.Columns[e.ColumnIndex];
            if (column.DataPropertyName != "AffinityCores")
                return;

            var server = serverGrid.Rows[e.RowIndex].DataBoundItem as ServerEntry;
            if (server == null)
                return;

            ShowCpuAffinityEditor(server);
            serverGrid.Refresh();
        }

        void ShowCpuAffinityEditor(ServerEntry server)
        {
            var form = new Form
            {
                Text = $"CPU Affinity — {server.Name}",
                Size = new Size(220, 300),
                StartPosition = FormStartPosition.CenterParent
            };

            var list = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                CheckOnClick = true
            };

            int cpuCount = Environment.ProcessorCount;

            for (int i = 0; i < cpuCount; i++)
            {
                list.Items.Add($"CPU {i}", (server.AffinityCores & (1 << i)) != 0);
            }

            var btnOk = new Button
            {
                Text = "OK",
                Dock = DockStyle.Bottom
            };

            btnOk.Click += (_, __) =>
            {
                int mask = 0;
                for (int i = 0; i < list.Items.Count; i++)
                    if (list.GetItemChecked(i))
                        mask |= 1 << i;

                server.AffinityCores = mask;
                SaveConfig();
                form.Close();

            };

            form.Controls.Add(list);
            form.Controls.Add(btnOk);
            form.ShowDialog(this);

        }


        void InitServerGrid()
        {
            serverGrid.CellDoubleClick += ServerGrid_CellDoubleClick;
            serverGrid.AutoGenerateColumns = false;
            serverGrid.DataSource = servers;
            serverGrid.ContextMenuStrip = CreateServerContextMenu();


            serverGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "STATUS",
                DataPropertyName = "Status",
                Width = 70
            });

            serverGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "NAME",
                DataPropertyName = "Name",
                Width = 160
            });

            serverGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "GAMEMODE",
                DataPropertyName = "Mode",
                Width = 55
            });

            serverGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "LOCATION",
                DataPropertyName = "Location",
                Width = 120
            });

            serverGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "PLAYERS",
                DataPropertyName = "MaxPlayers",
                Width = 50
            });

            serverGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "CPU",
                DataPropertyName = "AffinityCores",
                Width = 50
            });

            serverGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "LIFE-TIME (Minutes)",
                DataPropertyName = "LifetimeHours",
                Width = 50

            });

            serverGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "PASSWORD",
                DataPropertyName = "Password",
                Width = 80
            });

            serverGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "SERVER PORT",
                DataPropertyName = "PortSv",
                Width = 50
            });

            serverGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "GAMESPY PORT",
                DataPropertyName = "PortGs",
                Width = 50
            });

            serverGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "CLIENT PORT",
                DataPropertyName = "PortCl",
                Width = 50
            });

            serverGrid.MouseDown += ServerGrid_MouseDown;
            serverGrid.CellFormatting += ServerGrid_CellFormatting;

        }
        void ServerGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hit = serverGrid.HitTest(e.X, e.Y);
                if (hit.RowIndex >= 0)
                {
                    serverGrid.ClearSelection();
                    serverGrid.Rows[hit.RowIndex].Selected = true;
                    serverGrid.CurrentCell = serverGrid.Rows[hit.RowIndex].Cells[0];
                }
            }
        }
        ContextMenuStrip CreateServerContextMenu()
        {
            var menu = new ContextMenuStrip();

            var miStart = new ToolStripMenuItem("Запустить");
            var miStop = new ToolStripMenuItem("Остановить");
            var miRestart = new ToolStripMenuItem("Перезапустить");
            var miEdit = new ToolStripMenuItem("Редактировать");

            miStart.Click += (s, e) => OnStartServer();
            miStop.Click += (s, e) => OnStopServer();
            miRestart.Click += (s, e) => OnRestartServer();
            miEdit.Click += (s, e) => OnEditServer();

            menu.Items.AddRange(new ToolStripItem[]
            {
                miStart,
                miStop,
                new ToolStripSeparator(),
                miRestart,
                new ToolStripSeparator(),
                miEdit
            });

            return menu;
        }

        // ===== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ =====
        ServerEntry GetSelectedServer()
        {
            return serverGrid.CurrentRow?.DataBoundItem as ServerEntry;
        }

        void OnStartServer()
        {
            var server = GetSelectedServer();
            if (server == null) return;

            if (!runtimes.TryGetValue(server, out var runtime))
            {
                runtime = new ServerRuntime(server, enginePath, fsGamePath, useSandboxie, sandboxieStartPath, sandboxieBoxName);

                runtimes.Add(server, runtime);
            }

            runtime.Start();
            server.Status = "Starting";
            serverGrid.Refresh();
        }


        void OnStopServer()
        {
            var server = GetSelectedServer();
            if (server == null) return;

            if (runtimes.TryGetValue(server, out var runtime))
            {
                runtime.Stop();
                server.Status = "Stopped";
                serverGrid.Refresh();
            }
        }

        void OnRestartServer()
        {
            var server = GetSelectedServer();
            if (server == null) return;

            if (runtimes.TryGetValue(server, out var runtime))
            {
                runtime.Restart();
                server.Status = "Restarting";
                serverGrid.Refresh();
            }
        }


        void OnEditServer()
        {
            var server = GetSelectedServer();
            if (server == null) return;

            MessageBox.Show($"Редактирование сервера: {server.Name}");
        }

        void TickServers()
        {
            foreach (var pair in runtimes.ToList())
            {
                var server = pair.Key;
                var runtime = pair.Value;

                // Для обычного режима ничего не делает.
                // Для Sandboxie ищет реальный xrEngine.exe,
                // сбрасывает Starting и применяет affinity.
                runtime.TickSandboxieRuntime();

                // 1) STARTING (grace)
                if (runtime.StartupGracePeriodActive)
                {
                    server.Status = "Starting";
                    continue;
                }

                // 2) STARTING (долгий старт без stdout) — твой текущий механизм
                // Если у тебя есть runtime.StartupFinished — оставь эту часть:
                if (runtime.IsRunning && !runtime.HasEverProducedOutput && !runtime.StartupFinished)
                {
                    server.Status = "Starting";
                    continue;
                }

                // 3) HANG (по CPU-тишине 2 минуты)
                // Настройка порога: 2 минуты как ты просил
                if (runtime.IsHungByCpu(TimeSpan.FromSeconds(80)))
                {
                    server.Status = "Not Responding";

                    // защита от повторного планирования
                    if (!runtime.CrashRestartPending)
                    {
                        runtime.CrashRestartPending = true;

                        Task.Delay(TimeSpan.FromSeconds(80))
                            .ContinueWith(_ =>
                            {
                                // если за это время его остановили вручную — не рестартим
                                if (!runtime.StopRequested)
                                {
                                    runtime.Restart();
                                    server.Status = "Restart (Hang)";
                                }

                                BeginInvoke((Action)(() => serverGrid.Refresh()));
                                runtime.CrashRestartPending = false;
                            });
                    }

                    continue;
                }

                // 4) LIFETIME EXPIRED
                if (runtime.IsRunning && runtime.LifetimeExpired)
                {
                    server.Status = "Restart (Lifetime)";

                    if (!runtime.CrashRestartPending)
                    {
                        runtime.CrashRestartPending = true;

                        Task.Run(() =>
                        {
                            if (!runtime.StopRequested)
                            {
                                runtime.Restart();
                            }

                            BeginInvoke((Action)(() => serverGrid.Refresh()));
                            runtime.CrashRestartPending = false;
                        });
                    }

                    continue;
                }

                // 4) RUNNING
                if (runtime.IsRunning)
                {
                    server.Status = "Running";
                    continue;
                }

                // 5) EXITED: решаем — это crash или manual stop
                if (!runtime.IsRunning)
                {
                    if (runtime.StopRequested)
                    {
                        server.Status = "Stopped";
                        continue;
                    }

                    // Если он был Running/Starting и умер — считаем crash
                    server.Status = "Crashed";

                    // планируем автоперезапуск через 2 минуты (как ты хотел)
                    if (!runtime.CrashRestartPending)
                    {
                        runtime.CrashRestartPending = true;

                        Task.Delay(TimeSpan.FromSeconds(80))
                            .ContinueWith(_ =>
                            {
                                if (!runtime.StopRequested)
                                {
                                    runtime.Start();
                                    server.Status = "Restart (Crash)";
                                }

                                BeginInvoke((Action)(() => serverGrid.Refresh()));
                                runtime.CrashRestartPending = false;
                            });
                    }

                    continue;
                }
            }

            serverGrid.Refresh();
        }





        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void ServerManagerStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void serverGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void ServerManager_Load(object sender, EventArgs e)
        {

        }
    }
}
