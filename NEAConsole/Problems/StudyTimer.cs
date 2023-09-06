namespace NEAConsole.Problems;
public class StudyTimer
{
    public bool Enabled { get; set; }
    public DateTime LastRevised { get; set; }
    public TimeSpan StudyLength { get; set; }
    public TimeSpan BreakLength { get; set; }

    public StudyTimer() : this(DateTime.Now, TimeSpan.FromMinutes(25), TimeSpan.FromMinutes(5), true) { }
    public StudyTimer(DateTime lastRevised, TimeSpan studyLength, TimeSpan breakLength, bool enabled)
    {
        LastRevised = lastRevised;
        StudyLength = studyLength;
        BreakLength = breakLength;
        Enabled = enabled;
    }
}