using Octokit;
using System;
using System.Collections.Generic;
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
            Destination = destination;
            Credentials = credentials;
        }

        public IReadOnlyList<Repository> GetRepos()
        {
            var client = CreateGithubClient(Credentials);
            var task = client.Repository.GetAllForCurrent();
            task.Wait();
            var repos = task.Result;
            return repos;
        }

        public void CloneRepos(IReadOnlyList<Repository> repos)
        {
            var cloneOptions = new LibGit2Sharp.CloneOptions();

            cloneOptions.CredentialsProvider = (url, user, cred)
                => new LibGit2Sharp.UsernamePasswordCredentials { Username = Credentials.Login, Password = Credentials.Password };

            Console.WriteLine($"Starting to clone all repos to {Destination}");

            Parallel.ForEach(repos, (repo) =>
            {
                var repoDestination = Path.Combine(Destination, repo.Name);
                Console.WriteLine($"Starting to clone {repo.Name}");
                LibGit2Sharp.Repository.Clone(repo.CloneUrl, repoDestination, cloneOptions);
                Console.WriteLine($"Finished to clone {repo.Name}");
            });

            Console.WriteLine($"Finished cloning all {repos.Count} repos.");
        }

        public void CloneRepos(IReadOnlyList<Repository> repos, string token, string destinationRootFolder)
        {
            var cloneOptions = new LibGit2Sharp.CloneOptions();

            cloneOptions.CredentialsProvider = (url, user, cred)
                => new LibGit2Sharp.UsernamePasswordCredentials { Username = token, Password = string.Empty };

            Console.WriteLine($"Starting to clone all repos to {destinationRootFolder}");

            Parallel.ForEach(repos, (repo) =>
            {
                var destination = Path.Combine(destinationRootFolder, repo.Name);
                Console.WriteLine($"Starting to clone {repo.Name}");
                LibGit2Sharp.Repository.Clone(repo.CloneUrl, destination, cloneOptions);
                Console.WriteLine($"Finished to clone {repo.Name}");
            });

            Console.WriteLine($"Finished cloning all {repos.Count} repos.");
        }

        public void DisplayUserData()
        {
            var client = CreateGithubClient(Credentials);
            var userTask = client.User.Current();
            userTask.Wait();
            var user = userTask.Result;

            Console.WriteLine($"Hello {user.Name} (Id: {user.Id}),");
        }


        private GitHubClient CreateGithubClient(Credentials credentials)
        {
            var client = new GitHubClient(new ProductHeaderValue(credentials.Login));
            client.Credentials = credentials;
            return client;
        }


    }
}
