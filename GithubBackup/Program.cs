using Octokit;
using System;

namespace GithubBackup
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = PromptForCredentials();
            DisplayUserData(client);

            ListAllRepositories(client);

            Console.ReadLine();
        }

        private static async void ListAllRepositories(GitHubClient client)
        {
            var repos = await client.Repository.GetAllForCurrent();
            foreach (var repo in repos)
            {
                Console.WriteLine($"{repo.Name}");
                Console.WriteLine($"{repo.CloneUrl}");
            }
        }

        private static GitHubClient PromptForCredentials()
        {
            Console.WriteLine("Username?");
            var username = Console.ReadLine();

            Console.WriteLine("Password?");
            var password = Console.ReadLine();

            var client = new GitHubClient(new ProductHeaderValue(username));
            var basicAuth = new Credentials(username, password);
            client.Credentials = basicAuth;

            return client;
        }

        private static async void DisplayUserData(GitHubClient client)
        {
            var user = await client.User.Current();

            Console.WriteLine($"Hello {user.Name} (Id: {user.Id}),");
        }
    }
}
