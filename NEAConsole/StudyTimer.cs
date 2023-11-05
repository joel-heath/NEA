namespace NEAConsole;

public class StudyTimer
{
    public bool Enabled { get; set; }
    public TimeSpan TimeSinceLastBreak { get; set; }
    public TimeSpan StudyLength { get; set; }
    public TimeSpan BreakLength { get; set; }
    public bool TimeForBreak => Enabled && TimeSinceLastBreak > StudyLength;
    public void UseBreak()
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
            var timer = Task.Run(() => Exam.WriteTimer(timeRemaining));

            while (!cts.Token.IsCancellationRequested && (timeRemaining.Seconds > 0 || timeRemaining > TimeSpan.Zero)) // do our actual calculation for whether time is up based on datetimes, more accurately
            {
                Thread.Sleep(second);
                timeRemaining -= second;
                timer = Task.Run(() => Exam.WriteTimer(timeRemaining));
            }

            cts.Cancel();

            while (!affirmation.IsCompletedSuccessfully || !timer.IsCompletedSuccessfully) { }
            affirmation.Dispose();
            timer.Dispose();
        }
        TimeSinceLastBreak = TimeSpan.Zero;
        Console.Clear();
    }

    public StudyTimer() : this(TimeSpan.Zero, TimeSpan.FromMinutes(25), TimeSpan.FromMinutes(5), true) { }
    public StudyTimer(TimeSpan timeSinceLastBreak, TimeSpan studyLength, TimeSpan breakLength, bool enabled)
    {
        TimeSinceLastBreak = timeSinceLastBreak;
        StudyLength = studyLength;
        BreakLength = breakLength;
        Enabled = enabled;
    }
}