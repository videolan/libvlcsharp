using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LibVLCSharp.Tests
{
    [TestFixture]
    public class LibVLCAPICoverage
    {
        const string LibVLCSymURL = "https://raw.githubusercontent.com/videolan/vlc/master/lib/libvlc.sym";

        static readonly string[] AllowedNonLibVLCExports =
        {
            "libvlc_unity_get_texture",
            "libvlc_unity_media_player_new",
            "libvlc_unity_media_player_release"
        };

        [Test]
        public async Task CheckLibVLCCoverage()
        {
            var repoRoot = FindRepositoryRoot();
            var libVLCSharpPath = Path.Combine(repoRoot, "src", "LibVLCSharp");

            var exportedSymbols = await ReadExportedSymbols();
            var dllImports = ReadDllImports(libVLCSharpPath);

            var allowedExtraImports = new HashSet<string>(AllowedNonLibVLCExports);
            var extraDllImports = dllImports
                .Where(symbol => !allowedExtraImports.Contains(symbol))
                .Except(exportedSymbols)
                .OrderBy(symbol => symbol)
                .ToArray();
            var missingDllImports = exportedSymbols
                .Except(dllImports)
                .OrderBy(symbol => symbol)
                .ToArray();

            Debug.WriteLine($"{exportedSymbols.Count} LibVLC symbols exported");
            Debug.WriteLine($"{dllImports.Count} LibVLCSharp DllImport entry points found");
            Debug.WriteLine($"{missingDllImports.Length} missing LibVLCSharp DllImport entry points");
            Debug.WriteLine($"{extraDllImports.Length} extra LibVLCSharp DllImport entry points");

            Assert.Zero(
                missingDllImports.Length,
                "Missing LibVLCSharp DllImport entry points:" + Environment.NewLine + string.Join(Environment.NewLine, missingDllImports));
            Assert.Zero(
                extraDllImports.Length,
                "Extra LibVLCSharp DllImport entry points:" + Environment.NewLine + string.Join(Environment.NewLine, extraDllImports));
        }

        static string FindRepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "src", "LibVLCSharp", "LibVLCSharp.csproj")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }

            Assert.Fail("Could not find repository root containing src/LibVLCSharp/LibVLCSharp.csproj");
            return string.Empty;
        }

        static async Task<HashSet<string>> ReadExportedSymbols()
        {
            using (var httpClient = new HttpClient())
            {
                var symbols = await httpClient.GetStringAsync(LibVLCSymURL);
                return symbols
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Trim())
                    .Where(line => IsCheckedSymbol(line))
                    .Where(line => !IsExcludedSymbol(line))
                    .ToHashSet();
            }
        }

        static HashSet<string> ReadDllImports(string sourcePath)
        {
            var symbols = new HashSet<string>();

            foreach (var file in Directory.EnumerateFiles(sourcePath, "*.cs", SearchOption.AllDirectories))
            {
                var source = File.ReadAllText(file);
                foreach (Match match in Regex.Matches(source, @"EntryPoint\s*=\s*""(libvlc_[^""]+)"""))
                {
                    var symbol = match.Groups[1].Value;
                    if (IsCheckedSymbol(symbol) && !IsExcludedSymbol(symbol))
                    {
                        symbols.Add(symbol);
                    }
                }
            }

            return symbols;
        }

        static bool IsCheckedSymbol(string symbol)
        {
            return symbol.StartsWith("libvlc_", StringComparison.Ordinal);
        }

        static bool IsExcludedSymbol(string symbol)
        {
            return symbol.StartsWith("libvlc_media_list_player", StringComparison.Ordinal);
        }
    }
}
