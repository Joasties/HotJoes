import { readFile, readdir } from "node:fs/promises";
import path from "node:path";

const sensitiveEnvironmentName =
  /(?:PASSWORD|PASSCODE|TOKEN|SECRET|CREDENTIAL|CONNECTION_STRING|API_KEY)/i;

export async function scanDiagnosticEvidence({ root, forbiddenValues }) {
  const needles = [...new Set(forbiddenValues)]
    .filter((value) => typeof value === "string" && value.length > 0)
    .map((value) => Buffer.from(value, "utf8"));
  const files = await diagnosticFiles(root);
  const unsafeFiles = [];

  for (const file of files) {
    const content = await readFile(file);
    if (needles.some((needle) => content.includes(needle))) {
      unsafeFiles.push(path.relative(root, file).split(path.sep).join("/"));
    }
  }

  unsafeFiles.sort((left, right) => left.localeCompare(right, "en"));
  return {
    safe: unsafeFiles.length === 0,
    unsafeFiles,
  };
}

export function forbiddenDiagnosticValues(environment = process.env) {
  return Object.entries(environment)
    .filter(([name, value]) => sensitiveEnvironmentName.test(name) && value)
    .map(([, value]) => value);
}

async function diagnosticFiles(root, current = root) {
  let entries;
  try {
    entries = await readdir(current, { withFileTypes: true });
  } catch (error) {
    if (error?.code === "ENOENT") return [];
    throw error;
  }

  const files = [];
  for (const entry of entries) {
    const entryPath = path.join(current, entry.name);
    if (entry.isDirectory()) {
      files.push(...await diagnosticFiles(root, entryPath));
    } else if (entry.isFile()) {
      files.push(entryPath);
    }
  }

  return files;
}
