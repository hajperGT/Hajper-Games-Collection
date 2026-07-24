using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text.Json;
using System.Windows;

namespace HGC.Services
{
    public static class HgcBackupService
    {
        private const string BackupExtension = ".hgcbak";

        public static string GetBackupsFolder()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "HGC",
                "Backups");

            Directory.CreateDirectory(folder);
            return folder;
        }

        public static string GetDefaultBackupFileName()
        {
            return $"HGC_Backup_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}{BackupExtension}";
        }

        public static void CreateBackup(Window owner, string sourceDataFolder, string language)
        {
            if (!Directory.Exists(sourceDataFolder))
            {
                HgcMessageBox.Show(
                     owner,
                     language == "pl"
                         ? "Nie znaleziono folderu danych programu."
                         : "Program data folder was not found.",
                     "HGC",
                     HgcMessageBoxType.Warning,
                     language);
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Utwórz kopię zapasową",
                Filter = "HGC backup (*.hgcbak)|*.hgcbak",
                FileName = GetDefaultBackupFileName(),
                InitialDirectory = GetBackupsFolder(),
                AddExtension = true,
                DefaultExt = BackupExtension
            };

            if (dialog.ShowDialog() != true)
                return;

            string tempFolder = Path.Combine(Path.GetTempPath(), "HGC_Backup_" + Guid.NewGuid());
            Directory.CreateDirectory(tempFolder);

            try
            {
                string dataFolder = Path.Combine(tempFolder, "Data");
                Directory.CreateDirectory(dataFolder);

                CopyFileIfExists(
                    Path.Combine(sourceDataFolder, "Collection.hgc"),
                    Path.Combine(dataFolder, "Collection.hgc"));

                CopyFileIfExists(
                    Path.Combine(sourceDataFolder, "Settings.hgc"),
                    Path.Combine(dataFolder, "Settings.hgc"));

                string mediaFolder = Path.Combine(
                     Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                     "HGC");

                CopyDirectoryIfExists(
                    Path.Combine(mediaFolder, "Photos"),
                    Path.Combine(dataFolder, "Photos"));

                CopyDirectoryIfExists(
                    Path.Combine(mediaFolder, "Backgrounds"),
                    Path.Combine(dataFolder, "Backgrounds"));

                var manifest = new HgcBackupManifest
                {
                    CreatedAt = DateTime.Now,
                    AppName = "HGC",
                    BackupVersion = 1
                };

                string manifestJson = JsonSerializer.Serialize(manifest, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(Path.Combine(tempFolder, "manifest.json"), manifestJson);

                if (File.Exists(dialog.FileName))
                    File.Delete(dialog.FileName);

                ZipFile.CreateFromDirectory(
                    tempFolder,
                    dialog.FileName,
                    CompressionLevel.Optimal,
                    includeBaseDirectory: false);

                HgcMessageBox.Show(
                     owner,
                     language == "pl"
                         ? "Kopia zapasowa została utworzona."
                         : "Backup has been created successfully.",
                     "HGC",
                     HgcMessageBoxType.Information,
                     language);

            }
            catch (Exception ex)
            {
                HgcMessageBox.Show(
                 owner,
                 language == "pl"
                     ? "Nie udało się utworzyć kopii zapasowej.\n\n" + ex.Message
                     : "Failed to create the backup.\n\n" + ex.Message,
                 "HGC",
                 HgcMessageBoxType.Error,
                 language);
            }
            finally
            {
                TryDeleteDirectory(tempFolder);
            }
        }
        private static void CopyFileIfExists(string sourceFile, string targetFile)
        {
            if (!File.Exists(sourceFile))
                return;

            Directory.CreateDirectory(Path.GetDirectoryName(targetFile)!);
            File.Copy(sourceFile, targetFile, true);
        }

        private static void CopyDirectoryIfExists(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(sourceDir))
                return;

            CopyDirectory(sourceDir, targetDir);
        }
        public static bool RestoreBackup(Window owner, string targetDataFolder, string language)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Przywróć kopię zapasową",
                Filter = "HGC backup (*.hgcbak)|*.hgcbak",
                InitialDirectory = GetBackupsFolder()
            };

            if (dialog.ShowDialog() != true)
                return false;

            bool confirm = HgcMessageBox.ShowYesNo(
                  owner,
                  language == "pl"
                      ? "Przywrócenie kopii zapasowej zastąpi aktualne dane programu.\n\nKontynuować?"
                      : "Restoring the backup will replace the current program data.\n\nContinue?",
                  "HGC",
                  HgcMessageBoxType.Warning,
                  language);

            if (!confirm)
                return false;

            string tempFolder = Path.Combine(Path.GetTempPath(), "HGC_Restore_" + Guid.NewGuid());
            Directory.CreateDirectory(tempFolder);

            try
            {
                ZipFile.ExtractToDirectory(dialog.FileName, tempFolder);

                string restoredDataFolder = Path.Combine(tempFolder, "Data");

                if (!Directory.Exists(restoredDataFolder))
                    throw new InvalidDataException("Nieprawidłowy plik kopii zapasowej.");

                Directory.CreateDirectory(targetDataFolder);

                foreach (string file in Directory.GetFiles(targetDataFolder))
                {
                    File.Delete(file);
                }

                foreach (string directory in Directory.GetDirectories(targetDataFolder))
                {
                    string folderName = Path.GetFileName(directory);

                    if (folderName.Equals("Backups", StringComparison.OrdinalIgnoreCase))
                        continue;

                    Directory.Delete(directory, true);
                }

                CopyFileIfExists(
                     Path.Combine(restoredDataFolder, "Collection.hgc"),
                     Path.Combine(targetDataFolder, "Collection.hgc"));

                CopyFileIfExists(
                    Path.Combine(restoredDataFolder, "Settings.hgc"),
                    Path.Combine(targetDataFolder, "Settings.hgc"));

                string mediaFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "HGC");

                Directory.CreateDirectory(mediaFolder);

                CopyDirectoryIfExists(
                    Path.Combine(restoredDataFolder, "Photos"),
                    Path.Combine(mediaFolder, "Photos"));

                CopyDirectoryIfExists(
                    Path.Combine(restoredDataFolder, "Backgrounds"),
                    Path.Combine(mediaFolder, "Backgrounds"));

                HgcMessageBox.Show(
                    owner,
                    language == "pl"
                        ? "Kopia zapasowa została przywrócona.\n\nAplikacja zostanie teraz ponownie uruchomiona."
                        : "The backup has been restored.\n\nThe application will now restart.",
                    "HGC",
                    HgcMessageBoxType.Information,
                    language);

                return true;
            }
            catch (Exception ex)
            {
                HgcMessageBox.Show(
                    owner,
                    language == "pl"
                        ? "Nie udało się przywrócić kopii zapasowej.\n\n" + ex.Message
                        : "Failed to restore the backup.\n\n" + ex.Message,
                    "HGC",
                    HgcMessageBoxType.Error,
                    language);

                return false;
            }
            finally
            {
                TryDeleteDirectory(tempFolder);
            }
        }

        public static void OpenBackupsFolder()
        {
            string folder = GetBackupsFolder();

            Process.Start(new ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true
            });
        }

        private static void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string targetFile = Path.Combine(targetDir, Path.GetFileName(file));
                File.Copy(file, targetFile, true);
            }

            foreach (string directory in Directory.GetDirectories(sourceDir))
            {
                string folderName = Path.GetFileName(directory);

                if (folderName.Equals("Backups", StringComparison.OrdinalIgnoreCase))
                    continue;

                string targetSubDir = Path.Combine(targetDir, folderName);
                CopyDirectory(directory, targetSubDir);
            }
        }

        private static void TryDeleteDirectory(string folder)
        {
            try
            {
                if (Directory.Exists(folder))
                    Directory.Delete(folder, true);
            }
            catch
            {
                // celowo puste
            }
        }

        private class HgcBackupManifest
        {
            public string AppName { get; set; } = "";
            public int BackupVersion { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}