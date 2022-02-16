using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DevOrNot
{
    public class PackageRepository
    {
        public IEnumerable<PackageRow> GetAll(string data)
        {
            foreach (var row in data.Split(
                new string[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            ).Skip(1))
            {
                var packageRow = new PackageRow();
                var rowSplit = row.Split('\t');
                packageRow.package = rowSplit[0];
                packageRow.dependency = Convert.ToInt64(rowSplit[1]);
                packageRow.devDependency = Convert.ToInt64(rowSplit[2]);
                packageRow.dependencyPercent = Convert.ToDouble(rowSplit[3]);
                packageRow.devDependencyPercent = Convert.ToDouble(rowSplit[4]);
                packageRow.consensus = Convert.ToDouble(rowSplit[5]);
                packageRow.totalInstalled = Convert.ToInt64(rowSplit[6]);
                yield return packageRow;
            }
        }
    }
}