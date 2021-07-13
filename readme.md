github-backup
================

github-backup lets you create a local backup of all your github repositories.

All repositories of the given github user will be concurrently cloned to the provided destination folder.

If you dont provide a destination folder, the backup will be created in your current folder.

</br>

![github-backup screenshot](screenshot.png?raw=true "github-backup screenshot")

</br>

## Usage

```bash
$ github-backup token-based <token> [<destination>]
```

You can create a personal access token here: https://github.com/settings/tokens/new

</br>

## Building

```bash
$ dotnet publish -r win10-x64 -c release
```

So far I tested the application only for win10-x64 systems, but it might also work on other platforms.


</br>

## Used 3rd party libraries


[Octokit.NET](https://github.com/octokit/octokit.net)

[McMaster's CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils)

[LitGit2Sharp](https://github.com/libgit2/libgit2sharp)

[Autofac](https://github.com/autofac/Autofac)

[ShellProgressBar](https://github.com/Mpdreamz/shellprogressbar)
