using McMaster.Extensions.CommandLineUtils;
using Octokit;
using System;

namespace GithubBackup
{
    public class CredentialCmdWrapper
    {

        public CommandLineApplication ParentCommand { get; set; }
        public CommandLineApplication Command { get; set; }
        public Func<Credentials, string, BackupService> BackupServiceFactory { get; set; }
        public Func<string, string, AuthenticationType, Credentials> CredentialsFactory { get; set; }


        public CredentialCmdWrapper(
            CommandLineApplication parentCommand, 
            Func<Credentials, string, BackupService> backupServiceFactory,
            Func<string, string, AuthenticationType, Credentials> credentialsFactory)
        {
            ParentCommand = parentCommand;

            BackupServiceFactory = backupServiceFactory;
            CredentialsFactory = credentialsFactory;


            Command = ParentCommand.Command("credential-based", (credentialBasedCmd) =>
            {
                credentialBasedCmd.Description = "Creates a github backup. Authentification is done via user credentials.";
                var userNameArgument = credentialBasedCmd.Argument("Username", "Your git username or mail.").IsRequired();
                var passwordArgument = credentialBasedCmd.Argument("Password", "Your git password.").IsRequired();
                var destinationArgument = credentialBasedCmd.Argument("Destination", "The destination folder for the backup.")
                                            .Accepts(v => v.ExistingDirectory());

                credentialBasedCmd.OnExecute(() =>
                {
                    var credentials = CredentialsFactory(userNameArgument.Value, passwordArgument.Value, AuthenticationType.Basic);
                    var backupService = BackupServiceFactory(credentials, destinationArgument.Value);
                });
            });
        }

    }
}
