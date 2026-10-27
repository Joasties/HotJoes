import path from "node:path";
import {
  createBrowserRuntimeLifecycle,
  createDockerComposeRunner,
} from "./browser-runtime-lifecycle.mjs";

const allocation = {
  runId: required("HOTJOES_BROWSER_RUN_ID"),
  composeProject: required("HOTJOES_COMPOSE_PROJECT"),
  postgresPassword: required("HOTJOES_POSTGRES_PASSWORD"),
  edgePort: required("HOTJOES_EDGE_PORT"),
  baseUrl: required("HOTJOES_BROWSER_BASE_URL"),
};
const repository = path.resolve(import.meta.dirname, "../../..");
const environment = {
  ...process.env,
  HOTJOES_BROWSER_RUN_ID: allocation.runId,
  HOTJOES_COMPOSE_PROJECT: allocation.composeProject,
  HOTJOES_POSTGRES_PASSWORD: allocation.postgresPassword,
  HOTJOES_EDGE_PORT: allocation.edgePort,
  HOTJOES_BROWSER_BASE_URL: allocation.baseUrl,
};
const lifecycle = createBrowserRuntimeLifecycle({
  allocation,
  runCompose: createDockerComposeRunner({
    repository,
    environment,
    project: allocation.composeProject,
  }),
  reportCleanupFailure: (report: { readonly runId: string }) => {
    console.error(`Browser runtime cleanup failed for run ${report.runId}.`);
  },
});

export default async function globalSetup(): Promise<() => Promise<void>> {
  if (process.env.HOTJOES_BROWSER_EXTERNAL_RUNTIME === "true") {
    return async () => undefined;
  }

  await lifecycle.provision();
  return lifecycle.cleanup;
}

function required(name: string): string {
  const value = process.env[name];
  if (!value) throw new Error(`Required browser-harness setting '${name}' is missing.`);
  return value;
}
