using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace DevOrNot.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMistakenDevDependenciesAsDependencies()
        {
            var packageDatabase = System.Text.Encoding.Default.GetString(Properties.Resources.deps);
            var repo = new PackageRepository();
            string SAMPLE_PACKAGE_JSON_1 = "{\n  \"homepage\": \"zz\",\n  \"name\": \"z\",\n  \"version\": \"0.1.0\",\n  \"private\": true,\n  \"dependencies\": {\n    \"@emotion/react\": \"^11.7.1\",\n    \"@emotion/styled\": \"^11.6.0\",\n    \"@mui/icons-material\": \"^5.4.2\",\n    \n    \"react-dom\": \"^17.0.2\",\n    \"react-scripts\": \"5.0.0\",\n    \"web-vitals\": \"^2.1.4\"\n  },\n  \"scripts\": {\n    \"predeploy\": \"npm run build\",\n    \"deploy\": \"gh-pages -d build\",\n    \"start\": \"react-scripts start\",\n    \"build\": \"react-scripts build\",\n  },\n  \"eslintConfig\": {\n    \"extends\": [\n      \"react-app\",\n      \"react-app/jest\"\n    ]\n  },\n  \"browserslist\": {\n    \"production\": [\n      \">0.2%\",\n      \"not dead\",\n      \"not op_mini all\"\n    ],\n    \"development\": [\n      \"last 1 chrome version\",\n      \"last 1 firefox version\",\n      \"last 1 safari version\"\n    ]\n  },\n  \"devDependencies\": {\n\t  \"react\": \"^17.0.2\",\n    \"gh-pages\": \"^3.2.3\"\n  }\n}\n";
            var packageJson = (JObject)JsonConvert.DeserializeObject(SAMPLE_PACKAGE_JSON_1);
            (IEnumerable<PackageRow> mistakenDependenciesAsDev, IEnumerable<PackageRow> mistakenDevDependenciesAsDep) = DependencyParser.ParseDependencies(repo.GetAll(packageDatabase), 0.9, packageJson);

            Assert.IsTrue(mistakenDevDependenciesAsDep.Count() == 1);
            Assert.IsTrue(mistakenDevDependenciesAsDep.First().package.Equals("react"));
            Assert.IsFalse(mistakenDependenciesAsDev.Any());
        }
    }
}
