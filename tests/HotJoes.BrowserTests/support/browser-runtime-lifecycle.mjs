import { createHash, randomUUID } from "node:crypto";
import { execFileSync } from "node:child_process";
import net from "node:net";

export async function allocateBrowserRuntime({
  environment = process.env,
  createRunId = defaultRunId,
  reservePort = availablePort,
} = {}) {
  const runId = environment.HOTJOES_BROWSER_RUN_ID ?? createRunId();
  const edgePort = environment.HOTJOES_EDGE_PORT ?? String(await reservePort());

  return {
    runId,
    composeProject:
      environment.HOTJOES_COMPOSE_PROJECT
      ?? `hotjoes-browser-${runId.slice(0, 16)}`,
    postgresPassword:
      environment.HOTJOES_POSTGRES_PASSWORD
      ?? `browser-${runId}`,
    edgePort,
    baseUrl:
      environment.HOTJOES_BROWSER_BASE_URL
      ?? `http://127.0.0.1:${edgePort}`,
  };
}

export function createBrowserTestIdentity({
  runId,
  titlePath,
  workerIndex,
  repeatEachIndex,
  retryIndex = 0,
}) {
  const test = createHash("sha256")
    .update(JSON.stringify([
      runId,
      titlePath,
      workerIndex,
      repeatEachIndex,
      retryIndex,
    ]), "utf8")
    .digest("hex")
    .slice(0, 12);
  const run = runId
    .toLowerCase()
    .replaceAll(/[^a-z0-9]+/g, "-")
    .replaceAll(/^-|-$/g, "")
    .slice(0, 12);

  return `${run || "run"}-${workerIndex}-${repeatEachIndex}-${retryIndex}-${test}`;
}

export function createBrowserTestIdentityRegistry() {
  const reserved = new Set();

  return {
    reserve(identity) {
      if (reserved.has(identity)) {
        throw new Error("Browser test identity collision detected.");
      }

      reserved.add(identity);
    },
  };
}

export function createDockerComposeRunner({
  repository,
  environment,
  project,
}) {
  return async (arguments_) => {
    execFileSync("docker", ["compose", "-p", project, ...arguments_], {
      cwd: repository,
      env: environment,
      stdio: "inherit",
    });
  };
}

export function createBrowserRuntimeLifecycle({
  allocation,
  runCompose,
  reportCleanupFailure,
}) {
  let cleanupAttempted = false;

  async function cleanup() {
    if (cleanupAttempted) return;
    cleanupAttempted = true;

    try {
      await runCompose(["down", "--volumes", "--remove-orphans"]);
    } catch {
      reportCleanupFailure({
        runId: allocation.runId,
        outcome: "cleanupFailed",
      });
    }
  }

  return {
    async provision() {
      await runCompose(["down", "--volumes", "--remove-orphans"]);

      try {
        await runCompose(["up", "--build", "--wait", "--detach"]);
      } catch (error) {
        await cleanup();
        throw error;
      }
    },
    cleanup,
  };
}

function defaultRunId() {
  return randomUUID().replaceAll("-", "");
}

function availablePort() {
  return new Promise((resolve, reject) => {
    const server = net.createServer();
    server.unref();
    server.once("error", reject);
    server.listen(0, "127.0.0.1", () => {
      const address = server.address();
      if (typeof address === "string" || address === null) {
        server.close();
        reject(new Error("Could not allocate an isolated browser-test port."));
        return;
      }

      const port = address.port;
      server.close((error) => error ? reject(error) : resolve(port));
    });
  });
}
