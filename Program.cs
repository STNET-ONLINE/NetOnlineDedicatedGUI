using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;

namespace NetOnlineDedicatedGUI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (File.Exists("config.json"))
            {
                // Конфиг есть → сразу основное окно
                Application.Run(new ServerManager());
            }
            else
            {
                // Конфига нет → мастер первоначальной настройки
                Application.Run(new SetupWizard());
            }
        }
    }
}


class ServerEntry
{
    public string Name { get; set; }            // hname
    public string Mode { get; set; }            // lb / fmp / tdm / dm / cp / br
    public string Location { get; set; }        // lobby / map name

    public int MaxPlayers { get; set; }
    public int PortSv { get; set; }
    public int PortGs { get; set; }
    public int PortCl { get; set; }
    public string Password { get; set; }

    public int AffinityCores { get; set; }      // кол-во ядер (пока)
    public int LifetimeHours { get; set; } // 0 = бессрочно

    public string Status { get; set; } = "Stopped";

}

class AppConfig
{
    public string EnginePath { get; set; }
    public string FsGamePath { get; set; }
    public BindingList<ServerEntry> Servers { get; set; }
    public bool UseSandboxie { get; set; }

    public string SandboxieStartPath { get; set; } =
    @"C:\Program Files\Sandboxie-Plus\Start.exe";

    public string SandboxieBoxName { get; set; } = "DefaultBox";
}

