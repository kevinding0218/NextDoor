namespace NextDoor.Core.Logging
{
    public class SerilogOptions
    {
        public bool ConsoleEnabled { get; set; }
        public bool FileEnabled { get; set; }
        public string Level { get; set; }
    }
}