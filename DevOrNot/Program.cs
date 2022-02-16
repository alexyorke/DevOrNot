using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevOrNot
{
    class Program
    {
        static void Main(string[] args)
        {
            const double defaultConsensusThreshold = 0.80;
            if (args.Length != 1 && args.Length != 2)
            {
                Console.WriteLine("DevOrNot: find out if your package.json dependencies should be devDependencies or vice-versa.");
                Console.WriteLine("Usage: DevOrNot.exe /path/to/package.json [consensus threshold between 0 and 1]");
                return;
            }

            var filePath = args[0];
            var consensusThreshold = defaultConsensusThreshold;

            if (args.Length == 2)
            {
                if (double.TryParse(args[1], out var consensusThresholdParsed))
                {
                    consensusThreshold = consensusThresholdParsed;
                }
                else if (!string.IsNullOrWhiteSpace(args[1]))
                {
                    Console.WriteLine("Error: could not parse consensus threshold percent. It must be between 0 and 1.");
                    return;
                }
            }
            
            var packageJson = (JObject)JsonConvert.DeserializeObject(File.ReadAllText(filePath));

            if (packageJson == null) throw new InvalidDataException("The JSON file is not in the correct format.");

            var deps = System.Text.Encoding.Default.GetString(Properties.Resources.deps);
            var packageDatabase = new PackageRepository();
            var parsedPackages = packageDatabase.GetAll(deps);

            var (mistakenDependenciesAsDev, mistakenDevDependenciesAsDep) = DependencyParser.ParseDependencies(parsedPackages, consensusThreshold, packageJson);

            foreach (var mistakenDependency in mistakenDependenciesAsDev)
            {
                Console.WriteLine(
                    $"\"{mistakenDependency.package}\" is installed as a dependency, but it is usually installed as a devDependency (consensus: {Math.Round(mistakenDependency.consensus * 100, 1)}%)");
            }

            foreach (var mistakenDependency in mistakenDevDependenciesAsDep)
            {
                Console.WriteLine(
                    $"\"{mistakenDependency.package}\" is installed as a devDependency, but it is usually installed as a dependency (consensus: {Math.Round(mistakenDependency.consensus * 100, 1)}%)");
            }

            if (!mistakenDependenciesAsDev.Any() && !mistakenDevDependenciesAsDep.Any())
            {
                Console.WriteLine("No violations found.");
            }
        }
    }
}
