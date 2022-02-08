using McMaster.NETCore.Plugins;
using PluginContracts;
using System.Diagnostics;
/*
var pluginPath = @"C:\Users\Zachary Londono\source\repos\AppWithPlugin\TimeStampLibrary\bin\Debug\net6.0\TimestampedPlugin.dll";//args[0];
var loader = PluginLoader.CreateFromAssemblyFile(pluginPath,
sharedTypes: new[] { typeof(IPlugin) },
config => config.EnableHotReload = true);

loader.Reloaded += ShowPluginInfo;

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, __) => cts.Cancel();

// Show info on first load
InvokePlugin(loader);

await Task.Delay(-1, cts.Token);

static void ShowPluginInfo(object sender, PluginReloadedEventArgs eventArgs) {
Console.ForegroundColor = ConsoleColor.Blue;
Console.Write("HotReloadApp: ");
Console.ResetColor();
Console.WriteLine("plugin was reloaded");
InvokePlugin(eventArgs.Loader);
}

static void InvokePlugin(PluginLoader loader) {
foreach (var pluginType in loader
.LoadDefaultAssembly()
.GetTypes()
.Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract)) {
// This assumes the implementation of IPlugin has a parameterless constructor
var plugin = Activator.CreateInstance(pluginType) as IPlugin;

Console.WriteLine("----------------");
Console.WriteLine($"Created plugin instance '{plugin?.GetName()}':");
plugin?.Print();
Console.WriteLine("\n----------------");
}

}*/


string source = @"C:\Users\Zachary Londono\source\repos\AppWithPlugin\plugins\";

PluginService pluginService = new();

Console.WriteLine("Adding source");
pluginService.AddSource(source);

pluginService.PluginReloadEvent += (string pluginName, PluginReloadedEventArgs e) => {
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.Write("HotReloadApp: ");
    Console.ResetColor();
    Console.WriteLine($"Plugin '{pluginName}' was reloaded");
};

string[] plugins = pluginService.LoadPlugins();

foreach (string plugin in plugins) {
    Console.WriteLine($"Plugin loaded: {plugin}");
}

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, __) => cts.Cancel();

await Task.Delay(-1, cts.Token);

public class PluginService {

    /// <summary>
    /// Stores directories that hold plugins
    /// </summary>
    private ISet<string> _sources;

    private readonly IDictionary<string, PluginLoader> _plugins;

    /// <summary>
    /// Event is invoked whenever any of the plugins are reloaded
    /// </summary>
    public event PluginReloadHandler? PluginReloadEvent;
    public delegate void PluginReloadHandler(string pluginName, PluginReloadedEventArgs e);

    public PluginService() {
        _sources = new HashSet<string>();
        _plugins = new Dictionary<string, PluginLoader>();
    }

    public void AddSource(string source) {
        // Check if the source is already in the source list
        if (_sources.Contains(source)) return;
        // Check if the source is a directory
        if (!string.IsNullOrEmpty(Path.GetFileName(source)) || !Directory.Exists(source)) return;

        Console.WriteLine($"Adding new plugin source: {source}");
        _sources.Add(source);
    }

    public string[] LoadPlugins() {

        foreach (string source in _sources) {

            IEnumerable<string> plugins = Directory.EnumerateDirectories(source);

            foreach (string pluginDirectory in plugins) {

                Console.WriteLine($"Trying to load plugin {pluginDirectory}");

                string pluginName = Path.GetFileName(pluginDirectory); ;

                // Check if file is already loaded
                if (_plugins.ContainsKey(pluginName)) continue;

                // The plug in should be a file with the same name as it's parent directory
                string file = Path.Combine(pluginDirectory, $"{pluginName}.dll");

                if (!File.Exists(file)) continue;

                // Get plugin loader from file
                var loader = PluginLoader.CreateFromAssemblyFile(file,
                    sharedTypes: new[] { typeof(IPlugin) },
                    config => config.EnableHotReload = true);

                _plugins.Add(pluginName, loader);

                loader.Reloaded += (object sender, PluginReloadedEventArgs eventArgs) => {
                    PluginReloadEvent?.Invoke(pluginName, eventArgs);
                };

            }

        }

        return _plugins.Keys.ToArray();

    }

}