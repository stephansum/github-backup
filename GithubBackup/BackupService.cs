using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GithubBackup
{
    public class BackupService
    {

        private string Username { get; set; }
        private string Password { get; set; }
        private string Destination { get; set; }

        public BackupService()
        {
            var client = PromptForCredentials();
            DisplayUserData(client);

            var repos = GetAllRepositoriesAsync(client);
            PromptForDestinationFolder();
            BackupAllRepositories(repos, client, Destination);
        }

        private IReadOnlyList<Repository> GetAllRepositoriesAsync(GitHubClient client)
        {
            var task = client.Repository.GetAllForCurrent();
            task.Wait();
            var repos = task.Result;

            foreach (var repo in repos)
            {
                Console.WriteLine($"{repo.Name}");
                Console.WriteLine($"{repo.CloneUrl}");
            }
            return repos;
        }

        private void BackupAllRepositories(IReadOnlyList<Repository> repos, GitHubClient client, string destinationRootFolder)
        {
            var cloneOptions = new LibGit2Sharp.CloneOptions();

            cloneOptions.CredentialsProvider = (url, user, cred)
                => new LibGit2Sharp.UsernamePasswordCredentials { Username = Username, Password = Password };

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

        private GitHubClient PromptForCredentials()
        {
            Console.WriteLine("Username?");
            Username = Console.ReadLine();

            Console.WriteLine("Password?");
            Password = Console.ReadLine();

            var client = new GitHubClient(new ProductHeaderValue(Username));
            var basicAuth = new Credentials(Username, Password);
            client.Credentials = basicAuth;

            return client;
        }

        private void PromptForDestinationFolder()
        {
            Console.WriteLine("Destination?");
            Destination = Console.ReadLine();
        }

        private async void DisplayUserData(GitHubClient client)
        {
            var user = await client.User.Current();

            Console.WriteLine($"Hello {user.Name} (Id: {user.Id}),");
        }
    }
}
