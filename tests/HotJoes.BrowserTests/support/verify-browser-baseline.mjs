import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const projectRoot = path.resolve(
  path.dirname(fileURLToPath(import.meta.url)),
  "..",
);

const packageJson = readJson("package.json");
const packageLock = readJson("package-lock.json");
const matrix = readJson("browser-matrix.json");
const browserRegistry = readJson("node_modules/playwright-core/browsers.json");

const declaredVersion = requiredString(
  packageJson.devDependencies?.["@playwright/test"],
  "package.json @playwright/test version",
);
const lockedVersion = requiredString(
  packageLock.packages?.["node_modules/@playwright/test"]?.version,
  "package-lock.json @playwright/test version",
);
const coreVersion = requiredString(
  packageLock.packages?.["node_modules/playwright-core"]?.version,
  "package-lock.json playwright-core version",
);
const matrixVersion = requiredString(
  matrix.playwrightVersion,
  "browser-matrix.json Playwright version",
);

if (!/^\d+\.\d+\.\d+$/.test(declaredVersion)) {
  fail("@playwright/test must use an exact semantic version.");
}

for (const [source, version] of [
  ["package lock", lockedVersion],
  ["Playwright core", coreVersion],
  ["browser matrix", matrixVersion],
]) {
  if (version !== declaredVersion) {
    fail(`${source} version ${version} does not match ${declaredVersion}.`);
  }
}

const chromium = browserRegistry.browsers?.find(
  (browser) => browser.name === "chromium",
);
const installedRevision = requiredString(
  chromium?.revision,
  "Playwright Chromium revision",
);
if (installedRevision !== matrix.chromiumRevision) {
  fail(
    `Chromium revision ${installedRevision} does not match controlled `
      + `revision ${matrix.chromiumRevision}.`,
  );
}

if (!Array.isArray(matrix.contexts) || matrix.contexts.length === 0) {
  fail("The controlled browser matrix must declare at least one context.");
}

const names = new Set();
for (const context of matrix.contexts) {
  const name = requiredString(context.name, "browser context name");
  if (names.has(name)) fail(`Duplicate browser context '${name}'.`);
  names.add(name);
  if (context.browser !== "chromium") {
    fail(`Unsupported browser '${context.browser}' in context '${name}'.`);
  }
  if (context.evidenceKind !== "emulated") {
    fail(`Context '${name}' must describe its evidence as emulated.`);
  }
  if (!Number.isInteger(context.viewport?.width)
      || !Number.isInteger(context.viewport?.height)
      || context.viewport.width <= 0
      || context.viewport.height <= 0) {
    fail(`Context '${name}' has an invalid viewport.`);
  }
}

function readJson(relativePath) {
  return JSON.parse(fs.readFileSync(path.join(projectRoot, relativePath), "utf8"));
}

function requiredString(value, description) {
  if (typeof value !== "string" || value.trim().length === 0) {
    fail(`Missing ${description}.`);
  }
  return value;
}

function fail(message) {
  throw new Error(`Browser execution baseline verification failed: ${message}`);
}
