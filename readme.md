github-backup
================

github-backup lets you create a local backup of all your github repositories.

All repositories of the given github user will be concurrently cloned to the provided destination folder.

If you dont provide a destination folder, the backup will be created in your current folder.

You can choose between credential- or token-based authentication. 

</br>

![github-backup screenshot](screenshot.png?raw=true "github-backup screenshot")

## Usage

##### creating a backup with credential-based authentification:

```bash
$ github-backup credential-based <username> <password> 
```

</br>

##### creating a backup with token-based authentification:
```bash
$ github-backup token-based <token>
```

</br>

##### creating a backup with credential-based authentification and custom backup destination:
```bash
$ github-backup credential-based <username> <password> <destination>
```

</br>

##### creating a backup with token-based authentification and custom backup destination:
```bash
$ github-backup token-based <token> <destination>
```

</br>

## Building

So far I tested the application only on win-x64 systems, but it might also work on different platforms.

```bash
$ dotnet publish -r win-x64 -c release /p:TrimUnusedDependencies=true
```

</br>

## Used 3rd party libraries


[Octokit.NET](https://github.com/octokit/octokit.net)

[McMaster's CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils)

[LitGit2Sharp](https://github.com/libgit2/libgit2sharp)

[Autofac](https://github.com/autofac/Autofac)

[ShellProgressBar](https://github.com/Mpdreamz/shellprogressbar)
