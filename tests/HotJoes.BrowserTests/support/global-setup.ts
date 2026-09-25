import { execFileSync } from "node:child_process";
import path from "node:path";

const project = process.env.HOTJOES_COMPOSE_PROJECT ?? "hotjoes-browser-tests";
const password = process.env.HOTJOES_POSTGRES_PASSWORD ?? "browser-tests-only";
const edgePort = process.env.HOTJOES_EDGE_PORT ?? "18080";
const repository = path.resolve(import.meta.dirname, "../../..");
const environment = {
  ...process.env,
  HOTJOES_COMPOSE_PROJECT: project,
  HOTJOES_POSTGRES_PASSWORD: password,
  HOTJOES_EDGE_PORT: edgePort
};

export default async function globalSetup(): Promise<() => Promise<void>> {
  if (process.env.HOTJOES_BROWSER_EXTERNAL_RUNTIME === "true")
    return async () => undefined;

  compose(["down", "--volumes", "--remove-orphans"]);
  compose(["up", "--build", "--wait", "--detach"]);

  return async () => {
    compose(["down", "--volumes", "--remove-orphans"]);
  };
}

function compose(arguments_: readonly string[]): void {
  execFileSync("docker", ["compose", "-p", project, ...arguments_], {
    cwd: repository,
    env: environment,
    stdio: "inherit"
  });
}
