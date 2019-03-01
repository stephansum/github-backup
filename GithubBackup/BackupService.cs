using Octokit;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace GithubBackup
{
    public class BackupService
    {

        public string Destination { get; set; }
        public Credentials Credentials { get; set; }

        public BackupService(Credentials credentials, string destination)
        {
            var backupFolderName = $"Backup [{DateTime.Now.ToString("yyyy-MM-dd HH-mm", CultureInfo.InvariantCulture)}]";
            Destination = Path.Combine(destination, backupFolderName);
            Credentials = credentials;
        }

        public void CreateBackup()
        {
            var repos = GetRepos();
            CloneRepos(repos);
        }

        private IReadOnlyList<Repository> GetRepos()
        {
            var client = CreateGithubClient(Credentials);
            var task = client.Repository.GetAllForCurrent();
            task.Wait();
            var repos = task.Result;
            return repos;
        }

        private Dictionary<string, Exception> _exceptions = new Dictionary<string, Exception>();

        public void CloneRepos(IReadOnlyList<Repository> repos)
        {
            Console.WriteLine($"Starting to clone all repos to {Destination}");

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
                    _exceptions[repo.FullName] = ex;
                }

                rootProgressBar.Tick();
            });

            rootProgressBar.Dispose();

            Console.WriteLine($"Finished cloning all {repos.Count} repos.");

            foreach (var repoName in _exceptions.Keys)
            {
                Console.WriteLine();
                Console.WriteLine($"Error while cloning {repoName}:");
                Console.WriteLine(_exceptions[repoName]);
            }
        }

        public User GetUserData()
        {
            var client = CreateGithubClient(Credentials);
            var userTask = client.User.Current();
            userTask.Wait();
            var user = userTask.Result;
            return user;
        }


        private GitHubClient CreateGithubClient(Credentials credentials)
        {
            var client = new GitHubClient(new ProductHeaderValue(credentials.Login));
            client.Credentials = credentials;
            return client;
        }


    }
}
