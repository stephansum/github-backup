﻿using McMaster.Extensions.CommandLineUtils;
using Octokit;
using System;
using System.IO;

namespace GithubBackup
{
    public class CredentialSubCommand
    {

        public CommandLineApplication ParentCommand { get; set; }
        public CommandLineApplication Command { get; set; }

        // a custom delegate s required because autofac cant handle Func<T1, T1, T2> delegates when we deal with multiple parameters of the same Type (T1)
        // in order for the reflection magic to work, the parameter names of the delegate (e.g. "login", "password" ..) must literally match the naming of the parameters in the constructor of the type that is being created (e.g. public Credentials (string login, ..))
        public delegate Credentials CredentialsFactoryDelegate(string login, string password, AuthenticationType authenticationType);
        public CredentialsFactoryDelegate CredentialsFactory { get; set; }
        public Func<Credentials, string, BackupService> BackupServiceFactory { get; set; }


        public CredentialSubCommand(
            CommandLineApplication parentCommand, 
            Func<Credentials, string, BackupService> backupServiceFactory,
            CredentialsFactoryDelegate credentialsFactory)
        {
            ParentCommand = parentCommand;

            BackupServiceFactory = backupServiceFactory;
            CredentialsFactory = credentialsFactory;


            Command = ParentCommand.Command("credential-based", (credentialBasedCmd) =>
            {
                credentialBasedCmd.Description = "Using a credential-based authentication.";
                credentialBasedCmd.ThrowOnUnexpectedArgument = true;

                var userNameArgument = credentialBasedCmd.Argument("Username", "Github login (either username or email)").IsRequired();
                var passwordArgument = credentialBasedCmd.Argument("Password", "Github password.").IsRequired();
                var destinationArgument = credentialBasedCmd.Argument("Destination", "The destination folder for the backup.")
                                            .Accepts(v => v.ExistingDirectory());

                credentialBasedCmd.OnExecute(() =>
                {
                    var credentials = CredentialsFactory(userNameArgument.Value, passwordArgument.Value, AuthenticationType.Basic);

                    var currentFolder = Directory.GetCurrentDirectory();
                    var destinationFolder = string.IsNullOrWhiteSpace(destinationArgument.Value) ? currentFolder : destinationArgument.Value;

                    var backupService = BackupServiceFactory(credentials, destinationFolder);

                    var user = backupService.GetUserData();

                    Console.WriteLine($"Hello {user.Name}!");
                    Console.WriteLine();

                    backupService.CreateBackup();
                });
            });
        }

    }
}