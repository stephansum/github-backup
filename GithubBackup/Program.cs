using Autofac;
using Octokit;
using System.Reflection;

namespace GithubBackup
{
    class Program
    {
        // https://github.com/natemcmaster/CommandLineUtils
        // https://github.com/iamarcel/dotnet-core-neat-console-starter
        // https://gist.github.com/iamarcel/8047384bfbe9941e52817cf14a79dc34
        // https://gist.github.com/iamarcel/9bdc3f40d95c13f80d259b7eb2bbcabb

        // dotnet .\GithubBackup.dll token-based  --help

        static int Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // Registering types within the assembly:
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsSelf();
            //builder.RegisterType<BackupService>().AsSelf();
            //builder.RegisterType<GithubBackupCmdWrapper>().AsSelf();
            //builder.RegisterType<CredentialCmdWrapper>().AsSelf();
            //builder.RegisterType<TokenCmdWrapper>().AsSelf();

            // Registering types of 3rd party assemblies
            builder.RegisterType<Credentials>().AsSelf();

            var container = builder.Build();
            var githubBackupCmdWrapper = container.Resolve<GithubBackupCmdWrapper>();
            return githubBackupCmdWrapper.Command.Execute(args);
        }
    }
}
