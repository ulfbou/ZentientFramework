namespace Zentient.Configuration
{
    public class ConfigurationChangeEventArgs
    {
        public string Key { get; set; }
        public object NewValue { get; set; }
    }
}