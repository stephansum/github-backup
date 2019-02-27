using McMaster.Extensions.CommandLineUtils;
using System;

namespace GithubBackup
{
    public class GithubBackupCmdWrapper
    {
        public CommandLineApplication Command { get; set; }

        public GithubBackupCmdWrapper(
            Func<CommandLineApplication, CredentialCmdWrapper> credentialCmdWrapperFactory,
            Func<CommandLineApplication, TokenCmdWrapper> tokenCmdWrapperFactory)
        {
            Command = new CommandLineApplication();
            Command.Name = "github-backup";
            Command.Description = "Clones all git repositories of a user.";
            Command.HelpOption(true);

            Command.OnExecute(() =>
            {
                Console.WriteLine("Specify a subcommand");
                Command.ShowHelp();
                return 1;
            });

            credentialCmdWrapperFactory(Command);
            tokenCmdWrapperFactory(Command);
        }

    }
}
