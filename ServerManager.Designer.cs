namespace NetOnlineDedicatedGUI
{
    partial class ServerManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerManager));
            panel1 = new Panel();
            ServerManagerStatus = new StatusStrip();
            ServerManagerStrip = new MenuStrip();
            серверToolStripMenuItem = new ToolStripMenuItem();
            запуститьToolStripMenuItem = new ToolStripMenuItem();
            остановитьToolStripMenuItem = new ToolStripMenuItem();
            перезапускToolStripMenuItem = new ToolStripMenuItem();
            редактироватьНастройкиToolStripMenuItem = new ToolStripMenuItem();
            выполнитьToolStripMenuItem = new ToolStripMenuItem();
            сохранитьКонфигурациюToolStripMenuItem = new ToolStripMenuItem();
            загрузитьКонфигурациюToolStripMenuItem = new ToolStripMenuItem();
            сохранитьИгровойМирToolStripMenuItem = new ToolStripMenuItem();
            загрузитьИгровойМирToolStripMenuItem = new ToolStripMenuItem();
            настройкиToolStripMenuItem = new ToolStripMenuItem();
            тикрейтToolStripMenuItem = new ToolStripMenuItem();
            пингToolStripMenuItem = new ToolStripMenuItem();
            плагиныToolStripMenuItem = new ToolStripMenuItem();
            adManagerToolStripMenuItem = new ToolStripMenuItem();
            autoKickToolStripMenuItem = new ToolStripMenuItem();
            rCONToolStripMenuItem = new ToolStripMenuItem();
            чатToolStripMenuItem = new ToolStripMenuItem();
            банлистToolStripMenuItem = new ToolStripMenuItem();
            serverGrid = new DataGridView();
            panel1.SuspendLayout();
            ServerManagerStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)serverGrid).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(ServerManagerStatus);
            panel1.Controls.Add(ServerManagerStrip);
            panel1.Controls.Add(serverGrid);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(784, 441);
            panel1.TabIndex = 0;
            // 
            // ServerManagerStatus
            // 
            ServerManagerStatus.AutoSize = false;
            ServerManagerStatus.BackColor = Color.White;
            ServerManagerStatus.Location = new Point(0, 419);
            ServerManagerStatus.Name = "ServerManagerStatus";
            ServerManagerStatus.Size = new Size(784, 22);
            ServerManagerStatus.TabIndex = 2;
            ServerManagerStatus.Text = "statusStrip1";
            ServerManagerStatus.ItemClicked += statusStrip1_ItemClicked;
            // 
            // ServerManagerStrip
            // 
            ServerManagerStrip.AutoSize = false;
            ServerManagerStrip.BackColor = Color.White;
            ServerManagerStrip.Items.AddRange(new ToolStripItem[] { серверToolStripMenuItem, выполнитьToolStripMenuItem, настройкиToolStripMenuItem, плагиныToolStripMenuItem, rCONToolStripMenuItem });
            ServerManagerStrip.Location = new Point(0, 0);
            ServerManagerStrip.Name = "ServerManagerStrip";
            ServerManagerStrip.Padding = new Padding(0);
            ServerManagerStrip.Size = new Size(784, 22);
            ServerManagerStrip.TabIndex = 3;
            ServerManagerStrip.Text = "menuStrip1";
            ServerManagerStrip.ItemClicked += ServerManagerStrip_ItemClicked;
            // 
            // серверToolStripMenuItem
            // 
            серверToolStripMenuItem.AutoSize = false;
            серверToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { запуститьToolStripMenuItem, остановитьToolStripMenuItem, перезапускToolStripMenuItem, редактироватьНастройкиToolStripMenuItem });
            серверToolStripMenuItem.Name = "серверToolStripMenuItem";
            серверToolStripMenuItem.Padding = new Padding(0);
            серверToolStripMenuItem.Size = new Size(51, 20);
            серверToolStripMenuItem.Text = "Сервер";
            // 
            // запуститьToolStripMenuItem
            // 
            запуститьToolStripMenuItem.Name = "запуститьToolStripMenuItem";
            запуститьToolStripMenuItem.Size = new Size(215, 22);
            запуститьToolStripMenuItem.Text = "Запустить";
            // 
            // остановитьToolStripMenuItem
            // 
            остановитьToolStripMenuItem.Name = "остановитьToolStripMenuItem";
            остановитьToolStripMenuItem.Size = new Size(215, 22);
            остановитьToolStripMenuItem.Text = "Остановить";
            // 
            // перезапускToolStripMenuItem
            // 
            перезапускToolStripMenuItem.Name = "перезапускToolStripMenuItem";
            перезапускToolStripMenuItem.Size = new Size(215, 22);
            перезапускToolStripMenuItem.Text = "Перезапуск";
            // 
            // редактироватьНастройкиToolStripMenuItem
            // 
            редактироватьНастройкиToolStripMenuItem.Name = "редактироватьНастройкиToolStripMenuItem";
            редактироватьНастройкиToolStripMenuItem.Size = new Size(215, 22);
            редактироватьНастройкиToolStripMenuItem.Text = "Редактировать настройки";
            // 
            // выполнитьToolStripMenuItem
            // 
            выполнитьToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { сохранитьКонфигурациюToolStripMenuItem, загрузитьКонфигурациюToolStripMenuItem, сохранитьИгровойМирToolStripMenuItem, загрузитьИгровойМирToolStripMenuItem });
            выполнитьToolStripMenuItem.Name = "выполнитьToolStripMenuItem";
            выполнитьToolStripMenuItem.Padding = new Padding(0);
            выполнитьToolStripMenuItem.Size = new Size(73, 22);
            выполнитьToolStripMenuItem.Text = "Выполнить";
            // 
            // сохранитьКонфигурациюToolStripMenuItem
            // 
            сохранитьКонфигурациюToolStripMenuItem.Name = "сохранитьКонфигурациюToolStripMenuItem";
            сохранитьКонфигурациюToolStripMenuItem.Size = new Size(219, 22);
            сохранитьКонфигурациюToolStripMenuItem.Text = "Сохранить конфигурацию";
            // 
            // загрузитьКонфигурациюToolStripMenuItem
            // 
            загрузитьКонфигурациюToolStripMenuItem.Name = "загрузитьКонфигурациюToolStripMenuItem";
            загрузитьКонфигурациюToolStripMenuItem.Size = new Size(219, 22);
            загрузитьКонфигурациюToolStripMenuItem.Text = "Загрузить конфигурацию";
            // 
            // сохранитьИгровойМирToolStripMenuItem
            // 
            сохранитьИгровойМирToolStripMenuItem.Name = "сохранитьИгровойМирToolStripMenuItem";
            сохранитьИгровойМирToolStripMenuItem.Size = new Size(219, 22);
            сохранитьИгровойМирToolStripMenuItem.Text = "Сохранить игровой мир";
            // 
            // загрузитьИгровойМирToolStripMenuItem
            // 
            загрузитьИгровойМирToolStripMenuItem.Name = "загрузитьИгровойМирToolStripMenuItem";
            загрузитьИгровойМирToolStripMenuItem.Size = new Size(219, 22);
            загрузитьИгровойМирToolStripMenuItem.Text = "Загрузить игровой мир";
            // 
            // настройкиToolStripMenuItem
            // 
            настройкиToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { тикрейтToolStripMenuItem, пингToolStripMenuItem });
            настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            настройкиToolStripMenuItem.Size = new Size(79, 22);
            настройкиToolStripMenuItem.Text = "Настройки";
            // 
            // тикрейтToolStripMenuItem
            // 
            тикрейтToolStripMenuItem.Name = "тикрейтToolStripMenuItem";
            тикрейтToolStripMenuItem.Size = new Size(124, 22);
            тикрейтToolStripMenuItem.Text = "Тик-рейт";
            // 
            // пингToolStripMenuItem
            // 
            пингToolStripMenuItem.Name = "пингToolStripMenuItem";
            пингToolStripMenuItem.Size = new Size(124, 22);
            пингToolStripMenuItem.Text = "Пинг";
            // 
            // плагиныToolStripMenuItem
            // 
            плагиныToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { adManagerToolStripMenuItem, autoKickToolStripMenuItem });
            плагиныToolStripMenuItem.Name = "плагиныToolStripMenuItem";
            плагиныToolStripMenuItem.Size = new Size(69, 22);
            плагиныToolStripMenuItem.Text = "Плагины";
            // 
            // adManagerToolStripMenuItem
            // 
            adManagerToolStripMenuItem.Name = "adManagerToolStripMenuItem";
            adManagerToolStripMenuItem.Size = new Size(136, 22);
            adManagerToolStripMenuItem.Text = "AdManager";
            // 
            // autoKickToolStripMenuItem
            // 
            autoKickToolStripMenuItem.Name = "autoKickToolStripMenuItem";
            autoKickToolStripMenuItem.Size = new Size(136, 22);
            autoKickToolStripMenuItem.Text = "Auto Kick";
            // 
            // rCONToolStripMenuItem
            // 
            rCONToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { чатToolStripMenuItem, банлистToolStripMenuItem });
            rCONToolStripMenuItem.Name = "rCONToolStripMenuItem";
            rCONToolStripMenuItem.Size = new Size(52, 22);
            rCONToolStripMenuItem.Text = "RCON";
            // 
            // чатToolStripMenuItem
            // 
            чатToolStripMenuItem.Name = "чатToolStripMenuItem";
            чатToolStripMenuItem.Size = new Size(124, 22);
            чатToolStripMenuItem.Text = "Чат";
            // 
            // банлистToolStripMenuItem
            // 
            банлистToolStripMenuItem.Name = "банлистToolStripMenuItem";
            банлистToolStripMenuItem.Size = new Size(124, 22);
            банлистToolStripMenuItem.Text = "Бан-лист";
            // 
            // serverGrid
            // 
            serverGrid.AllowUserToAddRows = false;
            serverGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            serverGrid.Location = new Point(0, 25);
            serverGrid.MultiSelect = false;
            serverGrid.Name = "serverGrid";
            serverGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            serverGrid.Size = new Size(784, 391);
            serverGrid.TabIndex = 0;
            serverGrid.CellContentClick += serverGrid_CellContentClick;
            // 
            // ServerManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 441);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ServerManager";
            Text = "ServerManager";
            Load += ServerManager_Load;
            panel1.ResumeLayout(false);
            ServerManagerStrip.ResumeLayout(false);
            ServerManagerStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)serverGrid).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private DataGridView serverGrid;
        private StatusStrip ServerManagerStatus;
        private MenuStrip ServerManagerStrip;
        private ToolStripMenuItem серверToolStripMenuItem;
        private ToolStripMenuItem выполнитьToolStripMenuItem;
        private ToolStripMenuItem настройкиToolStripMenuItem;
        private ToolStripMenuItem плагиныToolStripMenuItem;
        private ToolStripMenuItem rCONToolStripMenuItem;
        private ToolStripMenuItem запуститьToolStripMenuItem;
        private ToolStripMenuItem остановитьToolStripMenuItem;
        private ToolStripMenuItem перезапускToolStripMenuItem;
        private ToolStripMenuItem редактироватьНастройкиToolStripMenuItem;
        private ToolStripMenuItem сохранитьКонфигурациюToolStripMenuItem;
        private ToolStripMenuItem загрузитьКонфигурациюToolStripMenuItem;
        private ToolStripMenuItem сохранитьИгровойМирToolStripMenuItem;
        private ToolStripMenuItem загрузитьИгровойМирToolStripMenuItem;
        private ToolStripMenuItem тикрейтToolStripMenuItem;
        private ToolStripMenuItem пингToolStripMenuItem;
        private ToolStripMenuItem adManagerToolStripMenuItem;
        private ToolStripMenuItem autoKickToolStripMenuItem;
        private ToolStripMenuItem чатToolStripMenuItem;
        private ToolStripMenuItem банлистToolStripMenuItem;
    }
}