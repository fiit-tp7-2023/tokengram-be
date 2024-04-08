param (
    $command
)

if (-not $command)  {
    $command = "local"
}

$ProjectRoot = "${PSScriptRoot}/.."

switch ($command) {
    "db" {
        dotnet ef database drop --project ${ProjectRoot} --context TokengramDbContext --force; dotnet ef database update --project ${ProjectRoot} --context TokengramDbContext
    }
    "db:setup" {
        dotnet ef database update --project ${ProjectRoot} --context TokengramDbContext
    }
    "db:reset" {
        dotnet ef database drop --project ${ProjectRoot} --context TokengramDbContext --force
    }
    "local" {
        docker compose -f ${ProjectRoot}/docker-compose-local.yml down; docker compose -f ${ProjectRoot}/docker-compose-local.yml up --build
    }
    "local:up" {
        docker compose -f ${ProjectRoot}/docker-compose-local.yml up --build
    }
    "local:down" {
        docker compose -f ${ProjectRoot}/docker-compose-local.yml down
    }
    default {
        throw "Unknown command: $command"
    }
}
