import { expect, test } from "@playwright/test";
import { mkdtemp, mkdir, rm, writeFile } from "node:fs/promises";
import { tmpdir } from "node:os";
import path from "node:path";
import {
  scanDiagnosticEvidence,
} from "../support/diagnostic-evidence.mjs";

test("AI-BROWSER-008 safe diagnostic evidence passes without retaining content", async () => {
  const root = await mkdtemp(path.join(tmpdir(), "hotjoes-safe-diagnostics-"));

  try {
    await mkdir(path.join(root, "nested"));
    await writeFile(
      path.join(root, "nested", "summary.txt"),
      "Synthetic browser failure evidence.",
      "utf8",
    );

    await expect(scanDiagnosticEvidence({
      root,
      forbiddenValues: ["controlled-secret", "browser-token"],
    })).resolves.toEqual({ safe: true, unsafeFiles: [] });
  } finally {
    await rm(root, { recursive: true, force: true });
  }
});

test("AI-BROWSER-008 sensitive diagnostic content is rejected without echoing it", async () => {
  const root = await mkdtemp(path.join(tmpdir(), "hotjoes-unsafe-diagnostics-"));
  const secret = "controlled-secret-value";
  const credential = "ephemeral-browser-credential";

  try {
    await mkdir(path.join(root, "nested"));
    await writeFile(
      path.join(root, "nested", "trace.txt"),
      `Failure contained ${secret} and ${credential}.`,
      "utf8",
    );

    const result = await scanDiagnosticEvidence({
      root,
      forbiddenValues: [secret, credential, ""],
    });

    expect(result).toEqual({
      safe: false,
      unsafeFiles: ["nested/trace.txt"],
    });
    expect(JSON.stringify(result)).not.toContain(secret);
    expect(JSON.stringify(result)).not.toContain(credential);
  } finally {
    await rm(root, { recursive: true, force: true });
  }
});

test("AI-BROWSER-008 binary diagnostic content is included in leakage detection", async () => {
  const root = await mkdtemp(path.join(tmpdir(), "hotjoes-binary-diagnostics-"));
  const token = "synthetic-access-token";

  try {
    await writeFile(
      path.join(root, "trace.bin"),
      Buffer.concat([Buffer.from([0, 1, 2]), Buffer.from(token, "utf8")]),
    );

    await expect(scanDiagnosticEvidence({
      root,
      forbiddenValues: [token],
    })).resolves.toEqual({
      safe: false,
      unsafeFiles: ["trace.bin"],
    });
  } finally {
    await rm(root, { recursive: true, force: true });
  }
});
