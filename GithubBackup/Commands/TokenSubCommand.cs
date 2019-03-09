using McMaster.Extensions.CommandLineUtils;
using Octokit;
using System;
using System.IO;

namespace GithubBackup
{
    public class TokenSubCommand
    {

        public CommandLineApplication ParentCommand { get; set; }
        public CommandLineApplication Command { get; set; }

        public Func<Credentials, string, BackupService> BackupServiceFactory { get; set; }
        public Func<string, Credentials> CredentialsFactory { get; set; }

        public TokenSubCommand(
            CommandLineApplication parentCommand,
            Func<Credentials, string, BackupService> backupServiceFactory,
            Func<string, Credentials> credentialsFactory)
        {
            ParentCommand = parentCommand;

            BackupServiceFactory = backupServiceFactory;
            CredentialsFactory = credentialsFactory;

            Command = ParentCommand.Command("token-based", (tokenBasedCmd) =>
            {
                tokenBasedCmd.Description = "Using a token-based authentication.";
                tokenBasedCmd.ThrowOnUnexpectedArgument = true;

                var tokenArgument = tokenBasedCmd.Argument("Token", "A valid github token.").IsRequired();
                var destinationArgument = tokenBasedCmd.Argument("Destination", "The destination folder for the backup.")
                                            .Accepts(v => v.ExistingDirectory());

                tokenBasedCmd.OnExecute(() =>
                {
                    var credentials = CredentialsFactory(tokenArgument.Value);

                    var currentFolder = Directory.GetCurrentDirectory();
                    var destinationFolder = string.IsNullOrWhiteSpace(destinationArgument.Value) ? currentFolder : destinationArgument.Value;

                    var backupService = BackupServiceFactory(credentials, destinationFolder);

                    backupService.CreateBackup();
                });
            });
        }

    }
}
