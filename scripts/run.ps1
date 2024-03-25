param (
    $command
)

if (-not $command)  {
    $command = "local"
}

$ProjectRoot = "${PSScriptRoot}/.."

switch ($command) {
    "setup" {
        dotnet ef database update --project ${ProjectRoot} --context TokengramDbContext
    }
    "local" {
        docker compose -f ${ProjectRoot}/docker-compose-local.yml up
    }
    default {
        throw "Unknown command: $command"
    }
}
