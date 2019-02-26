using McMaster.Extensions.CommandLineUtils;
using System;

namespace GithubBackup
{
    class Program
    {
        // https://github.com/iamarcel/dotnet-core-neat-console-starter
        // https://gist.github.com/iamarcel/8047384bfbe9941e52817cf14a79dc34
        // https://gist.github.com/iamarcel/9bdc3f40d95c13f80d259b7eb2bbcabb

        // dotnet .\GithubBackup.dll token-based  --help

        static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            app.Name = "github-backup";
            app.Description = "Clones all git repositories of a user.";
            app.HelpOption(true);

            app.Command("credential-based", (credentialBasedCmd) =>
            {

                var userNameArgument = credentialBasedCmd.Argument("Username", "Your git username or mail.").IsRequired();
                var passwordArgument = credentialBasedCmd.Argument("Password", "Your git password.").IsRequired();
                var destinationArgument = credentialBasedCmd.Argument("Destination", "The destination folder for the backup.");

                credentialBasedCmd.OnExecute(() =>
                {
                    Console.WriteLine($"Loggin in using {userNameArgument.Value} / {passwordArgument.Value}");
                    Console.WriteLine("Creating a credential based backup.");
                });
            });

            app.Command("token-based", (tokenBasedCmd) =>
            {
                var tokenArgument = tokenBasedCmd.Argument("Token", "Your git token with sufficient rights.").IsRequired();
                var destinationArgument = tokenBasedCmd.Argument("Destination", "The destination folder for the backup.");

                tokenBasedCmd.OnExecute(() =>
                {
                    Console.WriteLine($"Logging in using token {tokenArgument.Value}");
                    Console.WriteLine("Creating a token based backup.");
                });
            });

            app.OnExecute(() =>
            {
                Console.WriteLine("Specify a subcommand");
                app.ShowHelp();
                return 1;
            });


            //new BackupService();

            //Console.ReadLine();

            return app.Execute(args);
        }
    }
}
