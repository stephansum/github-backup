using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Octokit;
using System;
using System.Reflection;

namespace GithubBackup
{
    class Program
    {
        static int Main(string[] args)
        {
            int result = -1;

            try
            {
                SetupIocContainer();
                var githubBackupCmdWrapper = _container.Resolve<BackupCommand>();
                result = githubBackupCmdWrapper.Command.Execute(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        private static IContainer _container;

        public static void SetupIocContainer()
        {
            var builder = new ContainerBuilder();

            // Registering types within the assembly:
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsSelf();

            // Registering types of 3rd party assemblies
            builder.RegisterType<Credentials>().AsSelf();

            _container = builder.Build();
        }
    }
}
