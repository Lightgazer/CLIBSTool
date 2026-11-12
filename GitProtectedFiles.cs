using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace CLIBSTool;

public static class GitProtectedFiles
{
    public static string[] GetFiles(string workingDirectory)
    {
        // A .git file also marks a worktree or submodule checkout.
        var directory = new DirectoryInfo(workingDirectory);
        while (directory != null &&
               !Directory.Exists(Path.Combine(directory.FullName, ".git")) &&
               !File.Exists(Path.Combine(directory.FullName, ".git")))
            directory = directory.Parent;

        if (directory == null)
            return [];

        var startInfo = new ProcessStartInfo("git")
        {
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };
        foreach (var argument in new[]
                 {
                     "ls-tree", "-r", "--name-only", "-z", "HEAD", "--",
                     "Script", "Pictures", "DATA3", "pDATA3", "cDATA3", "lDATA3"
                 })
            startInfo.ArgumentList.Add(argument);

        using var process = new Process { StartInfo = startInfo };
        try
        {
            process.Start();
        }
        catch (Win32Exception exception) when (exception.NativeErrorCode is 2 or 3)
        {
            Console.WriteLine("Backup: Git is not installed; using protect.txt only.");
            return [];
        }

        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        process.WaitForExit();
        var paths = output.GetAwaiter().GetResult();
        var errorMessage = error.GetAwaiter().GetResult();
        if (process.ExitCode != 0)
            throw new IOException("Cannot read Git protected files. Unpack cancelled before removing folders. " +
                                  errorMessage.Trim());

        return paths.Split('\0', StringSplitOptions.RemoveEmptyEntries)
            .Select(TrackedFiles.NormalizePath).ToArray();
    }
}
