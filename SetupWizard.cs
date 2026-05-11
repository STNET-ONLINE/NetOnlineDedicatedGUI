using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace NetOnlineDedicatedGUI
{
    public partial class SetupWizard : Form
    {

        string MakeRelativeToBase(string absolutePath)
        {
            var baseDir = AppContext.BaseDirectory;

            var baseUri = new Uri(baseDir.EndsWith("\\") ? baseDir : baseDir + "\\");
            var fileUri = new Uri(absolutePath);

            return Uri.UnescapeDataString(
                baseUri.MakeRelativeUri(fileUri)
                       .ToString()
                       .Replace('/', '\\')
            );
        }

        BindingList<ServerEntry> servers = new BindingList<ServerEntry>();
        // ===== ПОЛЯ КЛАССА =====
        WizardStep currentStep = WizardStep.Welcome;
        string enginePath;
        string fsGamePath;


        CheckBox chkSandboxie;
        bool useSandboxie;

        // ===== ENUM ШАГОВ =====
        enum WizardStep
        {
            Welcome = 0,
            Paths = 1,
            Servers = 2,
            Finish = 3
        }

        // ===== КОНСТРУКТОР =====
        public SetupWizard()
        {

            InitializeComponent();
            LoadStep(); // ОБЯЗАТЕЛЬНО

        }

        // ===== КНОПКИ =====

        bool ValidateCurrentStep()
        {
            if (currentStep == WizardStep.Servers)
            {
                if (servers.Count == 0)
                {
                    MessageBox.Show(
                        "Добавьте хотя бы один сервер.",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return false;
                }

                foreach (var s in servers)
                {
                    if (string.IsNullOrWhiteSpace(s.Name))
                    {
                        MessageBox.Show("У одного из серверов не задано название.", "Ошибка");
                        return false;
                    }

                    if (s.MaxPlayers <= 0 || s.MaxPlayers > 128)
                    {
                        MessageBox.Show($"Некорректное количество игроков у сервера {s.Name}.", "Ошибка");
                        return false;
                    }

                    if (s.PortSv <= 0 || s.PortGs <= 0 || s.PortCl <= 0)
                    {
                        MessageBox.Show($"Некорректные порты у сервера {s.Name}.", "Ошибка");
                        return false;
                    }
                }
            }

            return true;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            // Валидируем текущий шаг (если нужно)
            if (!ValidateCurrentStep())
                return;

            // Если это Finish — сохраняем и выходим
            if (currentStep == WizardStep.Finish)
            {
                SaveConfig();
                Close();
                return;
            }

            currentStep++;
            LoadStep();
        }




        private void btnBack_Click(object sender, EventArgs e)
        {
            currentStep--;
            LoadStep();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        // ===== ЗАГРУЗКА ШАГОВ =====
        void LoadStep()
        {
            panelContent.Controls.Clear();

            btnBack.Enabled = currentStep != WizardStep.Welcome;

            switch (currentStep)
            {
                case WizardStep.Welcome:
                    LoadWelcomeStep();
                    btnNext.Text = "Далее";
                    break;

                case WizardStep.Paths:
                    // ПОКА ПУСТО
                    LoadPathsStep();
                    btnNext.Text = "Далее";
                    break;

                case WizardStep.Servers:
                    LoadServersStep();
                    btnNext.Text = "Далее";
                    break;

                case WizardStep.Finish:
                    LoadFinishStep();
                    btnNext.Text = "Готово";
                    break;

            }
        }

        void LoadPathsStep()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 4,
                Padding = new Padding(10)
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // Заголовок
            var title = new Label
            {
                Text = "Пути к файлам сервера",
                Font = new Font(Font.FontFamily, 10f, FontStyle.Bold),
                Dock = DockStyle.Fill
            };
            layout.SetColumnSpan(title, 3);
            layout.Controls.Add(title, 0, 0);

            // xrEngine.exe
            layout.Controls.Add(
                new Label { Text = "xrEngine.exe:", TextAlign = ContentAlignment.MiddleLeft },
                0, 1
            );

            var txtEngine = new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Text = enginePath ?? ""
            };
            layout.Controls.Add(txtEngine, 1, 1);

            var btnBrowseEngine = new Button { Text = "Обзор..." };
            btnBrowseEngine.Click += (s, e) =>
            {
                using (var dlg = new OpenFileDialog())
                {
                    dlg.Filter = "xrEngine.exe|xrEngine.exe|Exe files (*.exe)|*.exe";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        enginePath = dlg.FileName;
                        txtEngine.Text = enginePath;
                    }
                }
            };
            layout.Controls.Add(btnBrowseEngine, 2, 1);

            var chkSandboxieLocal = new CheckBox
            {
                Text = "Запускать сервер в Sandboxie (песочнице)",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Checked = useSandboxie
            };

            chkSandboxieLocal.CheckedChanged += (s, e) =>
            {
                useSandboxie = chkSandboxieLocal.Checked;
            };

            layout.SetColumnSpan(chkSandboxieLocal, 3);
            layout.Controls.Add(chkSandboxieLocal, 0, 3);

            chkSandboxie = chkSandboxieLocal;

            // fsgame_dedicated.ltx
            layout.Controls.Add(
                new Label { Text = "fsgame_dedicated.ltx:", TextAlign = ContentAlignment.MiddleLeft },
                0, 2
            );

            var txtFsGame = new TextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Text = fsGamePath ?? ""
            };
            layout.Controls.Add(txtFsGame, 1, 2);

            var btnBrowseFsGame = new Button { Text = "Обзор..." };
            btnBrowseFsGame.Click += (s, e) =>
            {
                using (var dlg = new OpenFileDialog())
                {
                    dlg.Filter = "fsgame_dedicated.ltx|fsgame_dedicated.ltx|LTX files (*.ltx)|*.ltx";
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        fsGamePath = dlg.FileName;
                        txtFsGame.Text = fsGamePath;
                    }
                }
            };
            layout.Controls.Add(btnBrowseFsGame, 2, 2);
            panelContent.Controls.Add(layout);
        }



        // ===== ПЕРВЫЙ ЭКРАН =====
        void LoadWelcomeStep()
            {
                var label = new Label
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font(Font.FontFamily, 11f),
                    Text =
                        "NET Online Dedicated Server Manager\n\n" +
                        "Этот мастер поможет выполнить первоначальную настройку сервера."
                };

                panelContent.Controls.Add(label);
            }

            void LoadServersStep()
            {
                var panel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(10)
                };

                var title = new Label
                {
                    Text = "Создайте серверы",
                    Font = new Font(Font.FontFamily, 10f, FontStyle.Bold),
                    Dock = DockStyle.Top,
                    Height = 30
                };

                var contextMenu = new ContextMenuStrip();

                var miAdd = new ToolStripMenuItem("Добавить сервер");
                var miRemove = new ToolStripMenuItem("Удалить сервер");

                contextMenu.Items.AddRange(new ToolStripItem[]
                {
             miAdd,
             miRemove
                });

                var grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    AutoGenerateColumns = false,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    ContextMenuStrip = contextMenu   // ← ВАЖНО
                };

                miAdd.Click += (s, e) =>
                {
                    servers.Add(new ServerEntry
                    {
                        Name = $"NET_Server_{servers.Count}",
                        Mode = "lb",
                        Location = "lobby",
                        Password = "",
                        MaxPlayers = 32,
                        AffinityCores = 1,
                        PortSv = 5500 + servers.Count * 10,
                        PortGs = 5501 + servers.Count * 10,
                        PortCl = 5502 + servers.Count * 10
                    });
                };

                miRemove.Click += (s, e) =>
                {
                    if (grid.CurrentRow == null)
                        return;

                    var item = grid.CurrentRow.DataBoundItem as ServerEntry;
                    if (item != null)
                        servers.Remove(item);
                };


                grid.MouseDown += (s, e) =>
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        var hit = grid.HitTest(e.X, e.Y);
                        if (hit.RowIndex >= 0)
                        {
                            grid.ClearSelection();
                            grid.Rows[hit.RowIndex].Selected = true;
                        }
                    }
                };



                // === КОЛОНКИ ===
                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Название",
                    DataPropertyName = "Name",
                    Width = 160
                });

                grid.Columns.Add(new DataGridViewComboBoxColumn
                {
                    HeaderText = "Режим",
                    DataPropertyName = "Mode",
                    DataSource = new[] { "lb", "fmp", "tdm", "dm", "cp", "br" },
                    Width = 60
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Локация",
                    DataPropertyName = "Location",
                    Width = 120
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Игроки",
                    DataPropertyName = "MaxPlayers",
                    Width = 70
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "CPU",
                    DataPropertyName = "AffinityCores",
                    Width = 50
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Life Time (минуты)",
                    DataPropertyName = "LifetimeHours",
                    Width = 80
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "Пароль",
                    DataPropertyName = "Password",
                    Width = 80
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "port sv",
                    DataPropertyName = "PortSv",
                    Width = 70
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "port gs",
                    DataPropertyName = "PortGs",
                    Width = 70
                });

                grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "port cl",
                    DataPropertyName = "PortCl",
                    Width = 70
                });

                // === КНОПКИ ===
                var btnAdd = new Button { Text = "Добавить сервер" };
                var btnRemove = new Button { Text = "Удалить" };

                btnAdd.Click += (s, e) =>
                {
                    servers.Add(new ServerEntry
                    {
                        Name = $"NET_Server_{servers.Count + 1}",
                        Mode = "lb",
                        Location = "lobby",
                        Password = "",
                        MaxPlayers = 32,
                        AffinityCores = 1,
                        PortSv = 5500 + servers.Count * 10,
                        PortGs = 5501 + servers.Count * 10,
                        PortCl = 5502 + servers.Count * 10
                    });

                    grid.DataSource = servers;

                };

                btnRemove.Click += (s, e) =>
                {
                    if (grid.SelectedRows.Count > 0)
                    {
                        servers.RemoveAt(grid.SelectedRows[0].Index);
                        grid.DataSource = servers;

                    }
                };

                var buttons = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 40
                };

                buttons.Controls.Add(btnAdd);
                buttons.Controls.Add(btnRemove);

                grid.DataSource = servers;

                panel.Controls.Add(grid);
                panel.Controls.Add(buttons);
                panel.Controls.Add(title);

                panelContent.Controls.Add(panel);
            }

            void LoadFinishStep()
            {
                var panel = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(20)
                };

                // Берем актуальное состояние галочки
                bool sandboxMode = useSandboxie;

                var label = new Label
                {
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    Font = new Font(Font.FontFamily, 10f),
                    Text =
                    "Первоначальная настройка завершена.\n\n" +
                    $"• Путь к xrEngine.exe: {(string.IsNullOrEmpty(enginePath) ? "не задан" : "OK")}\n" +
                    $"• Путь к fsgame_dedicated.ltx: {(string.IsNullOrEmpty(fsGamePath) ? "не задан" : "OK")}\n" +
                    $"• Создано серверов: {servers.Count}\n\n" +
                    "Нажмите «Готово», чтобы сохранить настройки."
                };

                panel.Controls.Add(label);
                panelContent.Controls.Add(panel);
            }

            void SaveConfig()
            {
                if (string.IsNullOrEmpty(enginePath) || string.IsNullOrEmpty(fsGamePath))
                {
                    MessageBox.Show(
                        "Не заданы пути к xrEngine.exe или fsgame_dedicated.ltx",
                        "Ошибка",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                bool sandboxieChecked = useSandboxie;

                var config = new AppConfig
                {
                    EnginePath = MakeRelativeToBase(enginePath),
                    FsGamePath = MakeRelativeToBase(fsGamePath),
                    UseSandboxie = sandboxieChecked, // Используем её здесь

                    Servers = servers
                };

                var json = System.Text.Json.JsonSerializer.Serialize(
                    config,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    }
                );

                File.WriteAllText("config.json", json);
            }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
