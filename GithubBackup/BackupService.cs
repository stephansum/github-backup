using Octokit;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GithubBackup
{
    public class BackupService
    {

        public string Destination { get; set; }
        public Credentials Credentials { get; set; }

        public BackupService(Credentials credentials, string destination)
        {
            var backupFolderName = $"Github Backup [{DateTime.Now.ToString("yyyy-MM-dd HH-mm", CultureInfo.InvariantCulture)}]";
            Destination = Path.Combine(destination, backupFolderName);
            Credentials = credentials;
        }

        public void CreateBackup()
        {
            var user = GetUserData();

            Console.WriteLine($"Hello {user.Name}!");
            Console.WriteLine();

            var repos = GetRepos();

            Console.WriteLine($"Total repositories found: {repos.Count}");
            Console.WriteLine($"Backup destination folder: {Destination}");

            Console.WriteLine($"Starting to clone all repositories");

            var exceptions = CloneRepos(repos);

            if (exceptions.Any())
            {
                Console.WriteLine($"Backup finished with {exceptions.Count} errors:");

                foreach (var repoName in exceptions.Keys)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Error while cloning {repoName}:");
                    Console.WriteLine(exceptions[repoName]);
                }
            }
            else
                Console.WriteLine("Backup finished successfully");
        }

        private IReadOnlyList<Repository> GetRepos()
        {
            var client = CreateGithubClient();
            var task = client.Repository.GetAllForCurrent();
            task.Wait();
            var repos = task.Result;
            return repos;
        }

        private  Dictionary<string, Exception> CloneRepos(IReadOnlyList<Repository> repos)
        {
            var exceptions = new Dictionary<string, Exception>();

            var rootProgressBarOptions = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Magenta,
                CollapseWhenFinished = false,
                EnableTaskBarProgress = true,
            };

            var rootProgressBar = new ProgressBar(repos.Count, "Overall", rootProgressBarOptions);

            Parallel.ForEach(repos, (repo) =>
            {
                var repoDestination = Path.Combine(Destination, repo.FullName);

                ChildProgressBar progressBar = null;

                var cloneOptions = new LibGit2Sharp.CloneOptions();
                cloneOptions.OnTransferProgress = (progress) =>
                {
                    if (progressBar == null)
                        progressBar = rootProgressBar.Spawn(progress.TotalObjects, repo.Name, new ProgressBarOptions {
                            CollapseWhenFinished = true,  ForegroundColorDone = ConsoleColor.Green, ForegroundColor = ConsoleColor.Yellow });

                    progressBar.Tick(progress.ReceivedObjects);
                    return true;
                };

                cloneOptions.RepositoryOperationCompleted = (context) =>
                {
                    progressBar?.Dispose();
                };

                if (Credentials.AuthenticationType == AuthenticationType.Basic)
                {
                    cloneOptions.CredentialsProvider = (url, user, cred)
                    => new LibGit2Sharp.UsernamePasswordCredentials { Username = Credentials.Login, Password = Credentials.Password };
                }
                else if (Credentials.AuthenticationType == AuthenticationType.Oauth)
                {
                    cloneOptions.CredentialsProvider = (url, user, cred)
                        => new LibGit2Sharp.UsernamePasswordCredentials { Username = Credentials.GetToken(), Password = string.Empty };
                }

                try
                {
                    LibGit2Sharp.Repository.Clone(repo.CloneUrl, repoDestination, cloneOptions);
                }
                catch (Exception ex)
                {
                    exceptions[repo.FullName] = ex;
                }

                rootProgressBar.Tick();
            });

            rootProgressBar.Dispose();

            return exceptions;
        }

        private User GetUserData()
        {
            var client = CreateGithubClient();

            User user = null;
            try
            {
                var userTask = client.User.Current();
                userTask.Wait();
                user = userTask.Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return user;
        }


        private GitHubClient CreateGithubClient()
        {
            var client = new GitHubClient(new ProductHeaderValue(Credentials.Login));
            client.Credentials = Credentials;
            return client;
        }


    }
}
