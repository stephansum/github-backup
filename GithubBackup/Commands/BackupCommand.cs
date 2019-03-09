using McMaster.Extensions.CommandLineUtils;
using System;

namespace GithubBackup
{
    public class BackupCommand
    {
        public CommandLineApplication Command { get; set; }

        public BackupCommand(
            Func<CommandLineApplication, CredentialSubCommand> credentialCmdWrapperFactory,
            Func<CommandLineApplication, TokenSubCommand> tokenCmdWrapperFactory)
        {
            Command = new CommandLineApplication();
            Command.ThrowOnUnexpectedArgument = true;
            Command.Name = "github-backup";
            Command.Description = "Creates a local backup of all repositories of a given github user.";
            Command.HelpOption(true);

            Command.OnExecute(() =>
            {
                Command.ShowHelp();
                Console.WriteLine();
                Console.WriteLine("Please specify the authentication mode via the appropriate subcommand.");
                Console.WriteLine();
                return 1;
            });

            credentialCmdWrapperFactory(Command);
            tokenCmdWrapperFactory(Command);
        }

    }
}
