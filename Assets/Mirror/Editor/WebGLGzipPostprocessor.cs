using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.IO.Compression;

public class WebGLGzipPostprocessor
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.WebGL) return;

        var dir = Path.GetDirectoryName(pathToBuiltProject);
        var files = Directory.GetFiles(dir, "*.unityweb");
        foreach (var file in files)
        {
            var gzPath = Path.ChangeExtension(file, ".gz");
            using (var original = File.OpenRead(file))
            using (var gz = File.Create(gzPath))
            using (var compressor = new GZipStream(gz, CompressionLevel.Optimal))
            {
                original.CopyTo(compressor);
            }
            File.Delete(file);
        }
    }
}
