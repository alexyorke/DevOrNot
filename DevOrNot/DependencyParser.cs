using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DevOrNot
{
    public static class DependencyParser
    {
        public static (IEnumerable<PackageRow> mistakenDependenciesAsDev, IEnumerable<PackageRow> mistakenDevDependenciesAsDep) ParseDependencies(
            IEnumerable<PackageRow> packageDatabase, double consensusThreshold, JObject packageJson)
        {
            var packages = packageDatabase.Where(p => p.consensus > consensusThreshold).ToList();

            // parse dependencies
            ICollection<string> dependencies = new List<string>();
            ICollection<string> devDependencies = new List<string>();
            if (packageJson.TryGetValue("dependencies", out var dependenciesAndVersions))
            {
                dependencies = dependenciesAndVersions.Select(d => ((JProperty)d).Name).ToList();
            }

            if (packageJson.TryGetValue("devDependencies", out var devDependenciesAndVersions))
            {
                devDependencies = devDependenciesAndVersions.Select(d => ((JProperty)d).Name).ToList();
            }

            var usuallyDependencies = packages.Where(p => p.dependencyPercent > 0.50);
            var usuallyDevDependencies = packages.Where(p => p.devDependencyPercent > 0.50);

            // check if a package is installed as a dependency (but is usually a dev dependency) and vice-versa
            var mistakenDependenciesAsDev = usuallyDevDependencies.Where(p => dependencies.Contains(p.package));
            var mistakenDevDependenciesAsDep = usuallyDependencies.Where(p => devDependencies.Contains(p.package));
            return (mistakenDependenciesAsDev, mistakenDevDependenciesAsDep);
        }
    }
}