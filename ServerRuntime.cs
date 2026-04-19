using System;
using System.Diagnostics;
using System.IO;

class ServerRuntime
{
    readonly ServerEntry server;

    // то, что пришло из config.json
    readonly string engineRelativePath; // "dedicated\\xrEngine.exe"
    readonly string fsGameRelativePath; // "..\\fsgame_dedicated.ltx"

    readonly string binariesDir;

    Process process;
    DateTime startTime;
    DateTime lastOutputTime;
    DateTime startAttemptTime;
    bool isStarting;
    bool isRestarting;
    bool hasEverProducedOutput;

    bool stopRequested;              // мы сами нажали "Остановить"
    bool crashRestartPending;        // чтобы не планировать рестарт 100 раз

    TimeSpan lastCpuTime;            // для детекта "повис и не жрёт CPU"
    DateTime lastCpuSampleTime;

    public DateTime LastExitTime { get; private set; } = DateTime.MinValue;


    static readonly TimeSpan MaxStartupTime = TimeSpan.FromSeconds(80);

    public bool StopRequested => stopRequested;

    public bool CrashRestartPending
    {
        get => crashRestartPending;
        set => crashRestartPending = value;
    }

    /// <summary>
    /// Проверка "живой ли процесс": если CPU не изменялся N минут, считаем завис.
    /// Этот метод должен вызываться из TickServers().
    /// </summary>
    public bool IsHungByCpu(TimeSpan idleThreshold)
    {
        if (!IsRunning) return false;
        if (StartupGracePeriodActive) return false;

        // первый замер
        if (lastCpuSampleTime == DateTime.MinValue)
        {
            lastCpuSampleTime = DateTime.Now;
            lastCpuTime = process.TotalProcessorTime;
            return false;
        }

        // обновляем раз в 10 секунд (у тебя Tick раз в 10с)
        var now = DateTime.Now;
        var cpuNow = process.TotalProcessorTime;

        if (cpuNow != lastCpuTime)
        {
            // процесс хоть как-то двигается
            lastCpuTime = cpuNow;
            lastCpuSampleTime = now;
            return false;
        }

        // CPU не двигается
        return (now - lastCpuSampleTime) >= idleThreshold;
    }




    public ServerRuntime(ServerEntry server, string enginePath, string fsGamePath)
    {
        this.server = server;

        engineRelativePath = enginePath;
        fsGameRelativePath = fsGamePath;

        // GUI лежит в binaries → cwd = binaries
        binariesDir = AppContext.BaseDirectory;
    }

    public bool IsRunning =>
        process != null && !process.HasExited;

    public TimeSpan Uptime =>
        IsRunning ? DateTime.Now - startTime : TimeSpan.Zero;

    public bool LifetimeExpired =>
        server.LifetimeHours > 0 &&
        Uptime.TotalMinutes >= server.LifetimeHours;

    public bool StartupGracePeriodActive =>
        isStarting &&
        (DateTime.Now - startAttemptTime) < TimeSpan.FromSeconds(80);

    public bool StartupFinished =>
    isStarting &&
    (DateTime.Now - startAttemptTime) >= MaxStartupTime;


    public bool HasEverProducedOutput => hasEverProducedOutput;

    public bool IsHung =>
        IsRunning &&
        hasEverProducedOutput &&  
        !StartupGracePeriodActive &&
        (DateTime.Now - lastOutputTime) > TimeSpan.FromSeconds(80);

    public void MarkStarted()
    {
        isStarting = false;
    }

    public void Start()
    {
        string pswArg = string.IsNullOrWhiteSpace(server.Password)
    ? ""
    : $"/psw={server.Password}";

        if (IsRunning)
            return;

        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = engineRelativePath,

                Arguments =
     "-dedicated -i -ignore_session_id -silent_error_mode " +
     $"-fsltx {fsGameRelativePath} " +
     $"-start server({server.Location}/{server.Mode}/hname={server.Name}/maxplayers={server.MaxPlayers}{pswArg}" +
     $"/portsv={server.PortSv}/portgs={server.PortGs}) " +
     $"client(localhost/portcl={server.PortCl})",

                WorkingDirectory = binariesDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            },
            EnableRaisingEvents = true

        };

        process.Exited += (_, __) =>
        {
            LastExitTime = DateTime.Now;
        };


        // ⬇️ СТАРТОВЫЕ ФЛАГИ
        isStarting = true;
        startAttemptTime = DateTime.Now;
        lastOutputTime = DateTime.Now;
        hasEverProducedOutput = false;

        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                lastOutputTime = DateTime.Now;
                hasEverProducedOutput = true;
                isStarting = false; // 🔥 КЛЮЧЕВО
            }
        };

        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
                lastOutputTime = DateTime.Now;
        };

        stopRequested = false;
        crashRestartPending = false;

        lastCpuTime = TimeSpan.Zero;
        lastCpuSampleTime = DateTime.Now;


        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        startTime = DateTime.Now;

        try
        {
            if (server.AffinityCores > 0)
                process.ProcessorAffinity = (IntPtr)server.AffinityCores;
        }
        catch { }
    }



    public void Stop()
    {
        stopRequested = true;
        crashRestartPending = false;

        if (!IsRunning)
            return;

        try
        {
            process.Kill(true);
            process.WaitForExit(5000);
        }
        catch { }
    }

    public void Restart()
    {
        Stop();
        Start();
    }

}
