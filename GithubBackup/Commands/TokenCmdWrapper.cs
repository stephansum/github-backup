using McMaster.Extensions.CommandLineUtils;
using Octokit;
using System;
using System.IO;

namespace GithubBackup
{
    public class TokenCmdWrapper
    {

        public CommandLineApplication ParentCommand { get; set; }
        public CommandLineApplication Command { get; set; }

        public Func<Credentials, string, BackupService> BackupServiceFactory { get; set; }
        public Func<string, AuthenticationType, Credentials> CredentialsFactory { get; set; }

        public TokenCmdWrapper(
            CommandLineApplication parentCommand,
            Func<Credentials, string, BackupService> backupServiceFactory,
            Func<string, AuthenticationType, Credentials> credentialsFactory)
        {
            ParentCommand = parentCommand;

            BackupServiceFactory = backupServiceFactory;
            CredentialsFactory = credentialsFactory;

            Command = ParentCommand.Command("token-based", (tokenBasedCmd) =>
            {
                tokenBasedCmd.Description = "Creates a github backup. Authentification is done via tokens.";

                var tokenArgument = tokenBasedCmd.Argument("Token", "Your git token with sufficient rights.").IsRequired();
                var destinationArgument = tokenBasedCmd.Argument("Destination", "The destination folder for the backup.")
                                            .Accepts(v => v.ExistingDirectory());

                tokenBasedCmd.OnExecute(() =>
                {
                    var credentials = CredentialsFactory(tokenArgument.Value, AuthenticationType.Oauth);

                    var currentFolder = Directory.GetCurrentDirectory();
                    var destinationFolder = string.IsNullOrWhiteSpace(destinationArgument.Value) ? currentFolder : destinationArgument.Value;
                    var backupService = BackupServiceFactory(credentials, destinationFolder);

                    backupService.GetUserData();
                    var repos = backupService.GetRepos();

                    foreach (var repo in repos)
                    {
                        Console.WriteLine(repo.FullName);
                    }

                    backupService.CloneRepos(repos);
                });
            });
        }

    }
}
