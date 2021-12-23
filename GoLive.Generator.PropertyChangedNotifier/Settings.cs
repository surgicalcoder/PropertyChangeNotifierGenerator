using System;

namespace GoLive.Generator.PropertyChangedNotifier
{
    public class Settings
    {
        public string BaseTypeName { get; set; }

        public string LogFile { get; set; }
        /*public String OutputFile { get; set; }
        public List<String> OutputFiles { get; set; }*/
        
    }
    
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input[0].ToString().ToUpper() + input.Substring(1);
            }
        }
    }
}