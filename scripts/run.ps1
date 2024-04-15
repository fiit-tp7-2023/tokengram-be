param (
    $command
)

if (-not $command)  {
    $command = "local"
}

$Context = "TokengramDbContext"
$BackendContainer = "backend"
$ProjectRoot = "${PSScriptRoot}/.."
$ContainerProjectRoot = "/app"
$LocalDockerCompose = "docker-compose.local.yml"

switch ($command) {
    "db" {
        docker compose -f ${ProjectRoot}/${LocalDockerCompose} exec ${BackendContainer} bash -c "dotnet ef database drop --project ${ContainerProjectRoot} --context ${Context} --force; dotnet ef database update --project ${ContainerProjectRoot} --context ${Context}"
    }
    "db:setup" {
        docker compose -f ${ProjectRoot}/${LocalDockerCompose} exec ${BackendContainer} bash -c "dotnet ef database update --project ${ContainerProjectRoot} --context ${Context}"
    }
    "db:reset" {
        docker compose -f ${ProjectRoot}/${LocalDockerCompose} exec ${BackendContainer} bash -c "dotnet ef database drop --project ${ContainerProjectRoot} --context ${Context} --force"
    }
    "local" {
        docker compose -f ${ProjectRoot}/${LocalDockerCompose} down; docker compose -f ${ProjectRoot}/${LocalDockerCompose} up --build
    }
    "local:up" {
        docker compose -f ${ProjectRoot}/${LocalDockerCompose} up --build
    }
    "local:down" {
        docker compose -f ${ProjectRoot}/${LocalDockerCompose} down
    }
    default {
        throw "Unknown command: $command"
    }
}
