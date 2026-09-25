using LibGit2Sharp;
using Rivet.Core;
using Rivet.Core.Registry;
using System.Text.Json;

PackageRegistry packageRegistry = new();

JsonSerializerOptions jsonOptions = new()
{
    WriteIndented = true,
    IncludeFields = true
};

if (args.Length == 0)
{
    Console.WriteLine("Rivet package manager");
    Console.WriteLine("Run 'rivet --help' for available commands.");
    return;
}

string command = args[0].ToLower();

if (command == "init")
{
    Init();
    return;
}

if (command == "add")
{
    await Add();
    return;
}

if (command == "install")
{
    await Install();
    return;
}

if (command == "--version" || command == "-v")
{
    Console.WriteLine("Rivet 0.1.0");
    return;
}

if (command == "--help" || command == "-h")
{
    PrintHelp();
    return;
}

Console.WriteLine($"Unknown command '{args[0]}'.");
Console.WriteLine("Run 'rivet --help' for available commands.");


void Init()
{
    if (File.Exists("rivet.json"))
    {
        Console.WriteLine("This directory is already a rivet project.");
        return;
    }

    RivetConfig config = new();

    Directory.CreateDirectory("external");

    SaveConfig(config);

    Console.WriteLine("Initialised rivet project.");
    Console.WriteLine("Created rivet.json");
    Console.WriteLine("Created external/");
}


async Task Add()
{
    if (!File.Exists("rivet.json"))
    {
        Console.WriteLine("This directory is not a rivet project.");
        Console.WriteLine("Run 'rivet init' first.");
        return;
    }

    if (args.Length < 2)
    {
        Console.WriteLine("No package specified.");
        Console.WriteLine("Usage: rivet add <package>");
        return;
    }

    RivetConfig? config = LoadConfig();

    if (config == null)
    {
        return;
    }

    Directory.CreateDirectory("external");

    for (int i = 1; i < args.Length; i++)
    {
        string packageName = args[i];

        Console.WriteLine($"Searching for '{packageName}'...");

        Package? package;

        try
        {
            package = await packageRegistry.FindPackageAsync(packageName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to search registry: {ex.Message}");
            continue;
        }

        if (package == null)
        {
            Console.WriteLine($"Package '{packageName}' was not found.");
            continue;
        }

        if (config.Packages.Any(x =>
            x.Name.Equals(package.Name, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine($"Package '{package.Name}' is already added.");
            continue;
        }

        string packagePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "external",
            package.Name
        );

        if (Directory.Exists(packagePath))
        {
            Console.WriteLine(
                $"Directory 'external/{package.Name}' already exists."
            );

            continue;
        }

        Console.WriteLine($"Found {package.Name}");
        Console.WriteLine($"Repository: {package.Repository}");

        bool downloaded = DownloadRepository(
            package.Repository,
            packagePath,
            package.Version
        );

        if (!downloaded)
        {
            continue;
        }

        config.Packages.Add(new RivetDependency
        {
            Name = package.Name,
            Version = package.Version
        });

        SaveConfig(config);

        Console.WriteLine($"Added '{package.Name}' successfully.");
    }
}

async Task Install()
{
    if (!File.Exists("rivet.json"))
    {
        Console.WriteLine("This directory is not a rivet project.");
        Console.WriteLine("Run 'rivet init' first.");
        return;
    }

    RivetConfig? config = LoadConfig();

    if (config == null)
    {
        return;
    }

    if (config.Packages.Count == 0)
    {
        Console.WriteLine("No packages to install.");
        return;
    }

    Directory.CreateDirectory("external");

    Console.WriteLine($"Installing {config.Packages.Count} package(s)...");

    foreach (RivetDependency dependency in config.Packages)
    {
        Console.WriteLine();
        Console.WriteLine($"Installing '{dependency.Name}' {dependency.Version}...");

        Package? package;

        try
        {
            package = await packageRegistry.FindPackageAsync(dependency.Name);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to search registry: {ex.Message}");
            continue;
        }

        if (package == null)
        {
            Console.WriteLine(
                $"Package '{dependency.Name}' was not found in the registry."
            );
            continue;
        }

        string packagePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "external",
            package.Name
        );

        if (Directory.Exists(packagePath))
        {
            Console.WriteLine($"'{package.Name}' is already installed.");
            continue;
        }

        Console.WriteLine($"Found {package.Name}");
        Console.WriteLine($"Version: {dependency.Version}");
        Console.WriteLine($"Repository: {package.Repository}");

        bool downloaded = DownloadRepository(
            package.Repository,
            packagePath,
            package.Version
        );

        if (!downloaded)
        {
            continue;
        }

        Console.WriteLine(
            $"Installed '{package.Name}' {dependency.Version} successfully."
        );
    }

    Console.WriteLine();
    Console.WriteLine("Installation complete.");
}

RivetConfig? LoadConfig()
{
    try
    {
        string json = File.ReadAllText("rivet.json");

        RivetConfig? config =
            JsonSerializer.Deserialize<RivetConfig>(json, jsonOptions);

        if (config == null)
        {
            Console.WriteLine("Failed to read rivet.json.");
            return null;
        }

        return config;
    }
    catch (JsonException ex)
    {
        Console.WriteLine("rivet.json contains invalid JSON.");
        Console.WriteLine(ex.Message);
        return null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to read rivet.json: {ex.Message}");
        return null;
    }
}


void SaveConfig(RivetConfig config)
{
    string json = JsonSerializer.Serialize(config, jsonOptions);

    File.WriteAllText("rivet.json", json);
}


bool DownloadRepository(string repoUrl, string path, string version)
{
    try
    {
        Console.WriteLine($"Cloning repository...");

        Repository.Clone(repoUrl, path);

        using Repository repo = new Repository(path);

        Tag? tag = repo.Tags.FirstOrDefault(x =>
            x.FriendlyName.Equals(version, StringComparison.OrdinalIgnoreCase));

        if (tag == null)
        {
            tag = repo.Tags.FirstOrDefault(x =>
                x.FriendlyName.Equals($"v{version}", StringComparison.OrdinalIgnoreCase));
        }

        if (tag == null)
        {
            Console.WriteLine($"Version '{version}' was not found.");

            repo.Dispose();
            Directory.Delete(path, true);

            return false;
        }

        Commit commit = tag.Target.Peel<Commit>();

        Commands.Checkout(repo, commit);

        Console.WriteLine($"Installed version {version}.");

        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to clone repository: {ex.Message}");

        if (Directory.Exists(path))
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch
            {
            }
        }

        return false;
    }
}


void PrintHelp()
{
    Console.WriteLine("Rivet - C++ Package Manager");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  rivet <command>");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  init                  Initialise a Rivet project");
    Console.WriteLine("  add <package...>      Add one or more packages");
    Console.WriteLine("  install               Install packages from rivet.json");
    Console.WriteLine("  -v, --version         Show Rivet version");
    Console.WriteLine("  -h, --help            Show this help message");
}