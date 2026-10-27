import path from "node:path";
import { spawnSync } from "node:child_process";
import fs from "node:fs";
import { fileURLToPath } from "node:url";
import { allocateBrowserRuntime } from "./browser-runtime-lifecycle.mjs";

const directory = path.dirname(fileURLToPath(import.meta.url));
const matrix = JSON.parse(fs.readFileSync(
  path.resolve(directory, "../browser-matrix.json"),
  "utf8",
));
const mode = process.argv[2];
const suppliedArguments = process.argv.slice(3);

if (mode !== "test" && mode !== "demo") {
  console.error("Expected browser execution mode 'test' or 'demo'.");
  process.exit(2);
}

const allocation = await allocateBrowserRuntime();
const hasProject = suppliedArguments.some((argument) => argument.startsWith("--project"));
const matrixProjects = matrix.contexts.map(
  (context) => `--project=${context.name}`,
);
const arguments_ = mode === "test" && !hasProject
  ? [...matrixProjects, ...suppliedArguments]
  : suppliedArguments;
const cli = path.resolve(directory, "../node_modules/@playwright/test/cli.js");

const result = spawnSync(process.execPath, [cli, "test", ...arguments_], {
  stdio: "inherit",
  env: {
    ...process.env,
    HOTJOES_BROWSER_RUN_ID: allocation.runId,
    HOTJOES_COMPOSE_PROJECT: allocation.composeProject,
    HOTJOES_POSTGRES_PASSWORD: allocation.postgresPassword,
    HOTJOES_EDGE_PORT: allocation.edgePort,
    HOTJOES_BROWSER_BASE_URL: allocation.baseUrl,
    HOTJOES_BROWSER_DEMONSTRATION: mode === "demo" ? "true" : "false",
  },
});

process.exit(result.status ?? 1);
