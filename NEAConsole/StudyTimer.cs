namespace NEAConsole;

public class StudyTimer(TimeSpan timeSinceLastBreak, TimeSpan studyLength, TimeSpan breakLength, bool enabled)
{
    public bool Enabled { get; set; } = enabled;
    public TimeSpan TimeSinceLastBreak { get; set; } = timeSinceLastBreak;
    public TimeSpan StudyLength { get; set; } = studyLength;
    public TimeSpan BreakLength { get; set; } = breakLength;
    public bool TimeForBreak => Enabled && TimeSinceLastBreak > StudyLength;
    public async Task UseBreak()
    {
        Console.WriteLine($"You've been studying for over {StudyLength.Minutes} minutes! Do you want to take a break?");
        if (Menu.Affirm())
        {
            CancellationTokenSource cts = new();
            Console.WriteLine($"Take a rest for the next {BreakLength.Minutes} minutes.");
            var timeRemaining = BreakLength;
            var second = TimeSpan.FromSeconds(1);

            var affirmation = Task.Run(() =>
            {
                try { InputMethods.Wait("Press any key to skip the break.", cts.Token); }
                catch (KeyNotFoundException) { }
                finally { cts.Cancel(); }
            });
            Exam.WriteTimer(timeRemaining);
            var examTimer = Task.Delay(timeRemaining, cts.Token);
            var timeRemainingWriter = new Timer((_) => {
                timeRemaining -= second;
                Exam.WriteTimer(timeRemaining);
            }, null, second, second);

            await Task.WhenAny(affirmation, examTimer);
            cts.Cancel();

            timeRemainingWriter.Dispose();
            await affirmation;
            affirmation.Dispose();
            examTimer.Dispose();
        }
        TimeSinceLastBreak = TimeSpan.Zero;
        Console.Clear();
    }

    public StudyTimer() : this(TimeSpan.Zero, TimeSpan.FromMinutes(25), TimeSpan.FromMinutes(5), true) { }
}