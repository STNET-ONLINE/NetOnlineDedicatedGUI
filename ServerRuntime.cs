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

    readonly bool useSandboxie;
    readonly string sandboxieStartPath;
    readonly string sandboxieBoxName;
    Process sandboxedEngineProcess;
    public bool UseSandboxie => useSandboxie;

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


    static readonly TimeSpan MaxStartupTime = TimeSpan.FromSeconds(60);
    static readonly TimeSpan MinSandboxieStartingTime = TimeSpan.FromSeconds(5);

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
        if (!IsRunning)
            return false;

        if (StartupGracePeriodActive)
            return false;

        Process targetProcess = useSandboxie
            ? FindRealEngineProcess()
            : process;

        if (targetProcess == null)
            return false;

        try
        {
            if (targetProcess.HasExited)
                return false;

            if (lastCpuSampleTime == DateTime.MinValue)
            {
                lastCpuSampleTime = DateTime.Now;
                lastCpuTime = targetProcess.TotalProcessorTime;
                return false;
            }

            var now = DateTime.Now;
            var cpuNow = targetProcess.TotalProcessorTime;

            if (cpuNow != lastCpuTime)
            {
                lastCpuTime = cpuNow;
                lastCpuSampleTime = now;
                return false;
            }

            return (now - lastCpuSampleTime) >= idleThreshold;
        }
        catch
        {
            return false;
        }
    }



    public ServerRuntime(ServerEntry server, string enginePath, string fsGamePath, bool useSandboxie, string sandboxieStartPath, string sandboxieBoxName)
    {
        this.server = server;

        engineRelativePath = enginePath;
        fsGameRelativePath = fsGamePath;

        // GUI лежит в binaries → cwd = binaries
        binariesDir = AppContext.BaseDirectory;

        this.useSandboxie = useSandboxie;

        this.sandboxieStartPath = string.IsNullOrWhiteSpace(sandboxieStartPath)
            ? @"C:\Program Files\Sandboxie-Plus\Start.exe"
            : sandboxieStartPath;

        this.sandboxieBoxName = string.IsNullOrWhiteSpace(sandboxieBoxName)
            ? "DefaultBox"
            : sandboxieBoxName;
    }

    public bool IsRunning
    {
        get
        {
            if (!useSandboxie)
            {
                try
                {
                    return process != null && !process.HasExited;
                }
                catch
                {
                    return false;
                }
            }

            // Если нажали Stop, не ищем xrEngine.exe заново.
            // Иначе можно случайно снова прицепиться к уже останавливаемому
            // или чужому процессу.
            if (stopRequested)
            {
                try
                {
                    if (sandboxedEngineProcess != null && !sandboxedEngineProcess.HasExited)
                        return true;
                }
                catch
                {
                }

                try
                {
                    if (process != null && !process.HasExited)
                        return true;
                }
                catch
                {
                }

                return false;
            }

            Process realEngine = FindRealEngineProcess();

            if (realEngine != null)
            {
                sandboxedEngineProcess = realEngine;
                return true;
            }

            try
            {
                return process != null && !process.HasExited;
            }
            catch
            {
                return false;
            }
        }
    }

    public TimeSpan Uptime =>
        IsRunning ? DateTime.Now - startTime : TimeSpan.Zero;

    public bool LifetimeExpired =>
        server.LifetimeHours > 0 &&
        Uptime.TotalMinutes >= server.LifetimeHours;

    public bool StartupGracePeriodActive
    {
        get
        {
            if (!isStarting)
                return false;

            return (DateTime.Now - startAttemptTime) < MaxStartupTime;
        }
    }

    public bool StartupFinished =>
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


    //Логика для Sandboxie
    private static string QuoteIfNeeded(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        if (value.StartsWith("\"") && value.EndsWith("\""))
            return value;

        if (value.Contains(" "))
            return "\"" + value + "\"";

        return value;
    }
    private string BuildEngineArguments()
    {
        string pswArg = string.IsNullOrWhiteSpace(server.Password)
            ? ""
            : $"/psw={server.Password}";

        return
            "-dedicated -i -ignore_session_id -silent_error_mode " +
            $"-fsltx {QuoteIfNeeded(fsGameRelativePath)} " +
            $"-start server({server.Location}/{server.Mode}/hname={server.Name}/maxplayers={server.MaxPlayers}{pswArg}" +
            $"/portsv={server.PortSv}/portgs={server.PortGs}) " +
            $"client(localhost/portcl={server.PortCl})";
    }

    private string BuildSandboxieArguments(string enginePath, string engineArgs)
    {
        return
            $"/box:{sandboxieBoxName} " +
            "/wait " +
           // "/silent " +
           // "/nosbiectrl " +
            $"{QuoteIfNeeded(enginePath)} " +
            engineArgs;
    }
    private void TryApplyAffinity(Process targetProcess)
    {
        try
        {
            if (server.AffinityCores > 0 && targetProcess != null && !targetProcess.HasExited)
                targetProcess.ProcessorAffinity = (IntPtr)server.AffinityCores;
        }
        catch
        {
        }
    }
    private Process FindRealEngineProcess()
    {
        try
        {
            string engineExeName = Path.GetFileNameWithoutExtension(engineRelativePath);

            var processes = Process.GetProcessesByName(engineExeName);

            foreach (var p in processes)
            {
                try
                {
                    if (p.HasExited)
                        continue;

                    // Грубая, но рабочая проверка:
                    // ищем процесс, который был запущен после startAttemptTime.
                    if (p.StartTime >= startAttemptTime.AddSeconds(-3))
                        return p;
                }
                catch
                {
                }
            }
        }
        catch
        {
        }

        return null;
    }

    public void TickSandboxieRuntime()
    {
        if (!useSandboxie)
            return;

        Process realEngine = FindRealEngineProcess();

        if (realEngine != null)
        {
            sandboxedEngineProcess = realEngine;

            hasEverProducedOutput = true;
            lastOutputTime = DateTime.Now;

            TryApplyAffinity(realEngine);

            // Пока идёт стартовый grace-период — держим Starting.
            // После него TickServers() сам поставит Running.
            if ((DateTime.Now - startAttemptTime) < MaxStartupTime)
                isStarting = true;
            else
                isStarting = false;

            return;
        }

        // Если xrEngine ещё не найден, но grace-период идёт —
        // считаем, что запуск всё ещё происходит.
        if ((DateTime.Now - startAttemptTime) < MaxStartupTime)
        {
            isStarting = true;
            return;
        }

        isStarting = false;
    }
    //
    public void Start()
    {
        if (IsRunning)
            return;

        string engineArgs = BuildEngineArguments();

        string fileName;
        string arguments;

        bool sandboxed = useSandboxie;

        if (sandboxed)
        {
            fileName = sandboxieStartPath;

            string engineExeForSandboxie = Path.GetFullPath(Path.Combine(binariesDir, engineRelativePath));
            arguments = BuildSandboxieArguments(engineExeForSandboxie, engineArgs);
        }
        else
        {
            fileName = engineRelativePath;
            arguments = engineArgs;
        }

        process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = binariesDir,

                UseShellExecute = false,

                // В обычном режиме stdout полезен.
                // В Sandboxie stdout от xrEngine обычно через Start.exe нормально не живёт.
                RedirectStandardOutput = !sandboxed,
                RedirectStandardError = !sandboxed,

                CreateNoWindow = true
            },
            EnableRaisingEvents = true
        };

        process.Exited += (_, __) =>
        {
            LastExitTime = DateTime.Now;
        };

        // Стартовые флаги
        isStarting = true;
        startAttemptTime = DateTime.Now;
        lastOutputTime = DateTime.Now;
        hasEverProducedOutput = false;

        if (!sandboxed)
        {
            process.OutputDataReceived += (_, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    lastOutputTime = DateTime.Now;
                    hasEverProducedOutput = true;
                    isStarting = false;
                }
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    lastOutputTime = DateTime.Now;
            };
        }
        else
        {
            // В Sandboxie stdout может быть недоступен.
            // Не ждём вывода как признака успешного старта.
            hasEverProducedOutput = true;
        }

        stopRequested = false;
        crashRestartPending = false;

        lastCpuTime = TimeSpan.Zero;
        lastCpuSampleTime = DateTime.Now;

        process.Start();

        if (!sandboxed)
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        startTime = DateTime.Now;

        if (!sandboxed)
        {
            TryApplyAffinity(process);
        }
        else
        {
            // Реальный xrEngine появится не всегда мгновенно.
            // Лучше применить affinity чуть позже из TickServers().
            isStarting = true;
        }
    }



    public void Stop()
    {
        stopRequested = true;
        crashRestartPending = false;
        isStarting = false;

        try
        {
            if (useSandboxie)
            {
                Process realEngine = null;

                try
                {
                    realEngine = sandboxedEngineProcess;

                    if (realEngine == null || realEngine.HasExited)
                        realEngine = FindRealEngineProcess();
                }
                catch
                {
                    realEngine = null;
                }

                try
                {
                    if (realEngine != null && !realEngine.HasExited)
                    {
                        realEngine.Kill();
                        realEngine.WaitForExit(5000);
                    }
                }
                catch
                {
                }

                try
                {
                    if (process != null && !process.HasExited)
                    {
                        process.Kill();
                        process.WaitForExit(5000);
                    }
                }
                catch
                {
                }

                sandboxedEngineProcess = null;
                process = null;
                LastExitTime = DateTime.Now;

                return;
            }

            if (process != null && !process.HasExited)
            {
                process.Kill();
                process.WaitForExit(5000);
            }

            process = null;
            LastExitTime = DateTime.Now;
        }
        catch
        {
        }
    }

    public void Restart()
    {
        Stop();
        Start();
    }

}
